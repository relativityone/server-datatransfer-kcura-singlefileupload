using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Relativity.API;
using Relativity.Services.Exceptions;
using Relativity.Services.Permission;
using kCura.Relativity.Client;
using Relativity.Services;
using System.Data.SqlClient;
using kCura.SingleFileUpload.Core.SQL;
using System.Threading;

namespace kCura.SingleFileUpload.Core.Helpers
{
    public class PermissionHelper
    {
        private IHelper _helper;

        public PermissionHelper(IHelper helper)
        {
            _helper = helper;

        }

        /// <summary>
        /// Create a custom permission in a workspace.  
        /// Call this from a Post-install event handler.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="objectTypeId"></param>
        /// <param name="permissionName"></param>
        public void CreateCustomPermissionOnObjectType(int workspaceId, string objectTypeGuid, string permissionName)
        {
            var objectTypeId = this.GetArtifactTypeFromObjectGuid(workspaceId, objectTypeGuid);
            Task<bool> task = Permission_CreateSinglePermissionAsync(workspaceId, objectTypeId.Key, permissionName);
            task.Wait();
            if (!task.Result)
            {
                throw new System.Exception($"Create permission '{permissionName}' failed.");
            }
        }

        /// <summary>
        /// Check to see if the current user has an object type permission
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="artifactTypeId"></param>
        /// <param name="permissionName"></param>
        /// <returns>True if user has permission, false if they don't</returns>
        public async Task<bool> CurrentUserHasPermissionToObjectType(int workspaceId, string objectTypeGuid, string permissionName)
        {
            var artifactTypeId = this.GetArtifactTypeFromObjectGuid(workspaceId, objectTypeGuid);
            bool task = await Permission_UserHasItAsync(workspaceId, artifactTypeId.Key, permissionName);
            return task;
        }

        private async Task<bool> Permission_CreateSinglePermissionAsync(int workspaceId, int objectTypeId, string permission)
        {
            bool success = false;

            using (IPermissionManager proxy = _helper.GetServicesManager().CreateProxy<IPermissionManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    IRSAPIClient client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);
                    client.APIOptions.WorkspaceID = workspaceId; //this.SampleWorkspace_ID;

                    // Create a new permission to be added to the above RDO.
                    Permission newPermission = new Permission();
                    newPermission.Name = string.Format(permission);
                    newPermission.ArtifactType.ID = objectTypeId;
                    newPermission.PermissionType = PermissionType.Custom;

                    // Create the new permission.
                    int permissionID = await proxy.CreateSingleAsync(workspaceId, newPermission);

                    success = true;
                }
                catch (Exception)
                {
                    success = false;
                }
            }

            return success;
        }

        private async Task<bool> Permission_UserHasItAsync(int workspaceId, int artifactTypeId, string permissionName)
        {
            bool success = false;

            using (IPermissionManager proxy = _helper.GetServicesManager().CreateProxy<IPermissionManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    ArtifactTypeIdentifier ati = new ArtifactTypeIdentifier(artifactTypeId);
                    PermissionRef permission = new PermissionRef
                    {
                        Name = permissionName,
                        ArtifactType = ati
                    };
                    List<PermissionRef> plist = new List<PermissionRef> { permission };


                    CancellationTokenSource cts = new CancellationTokenSource(5000);
                    var tList = Task.Run(() => proxy.GetPermissionSelectedAsync(workspaceId, plist), cts.Token); ;
                    List<PermissionValue> pvsList = await tList;

                    if (pvsList.Count == 1)
                    {
                        PermissionValue pv = pvsList[0];
                        success = pv.Selected;
                    }

                }
                catch (ServiceException)
                {
                    //Exceptions are returned as an ServiceException
                    success = false;
                }
            }
            return success;
        }

        public bool IsSytemAdminUser(int userID)
        {
            var result = _helper.GetDBContext(-1).ExecuteSqlStatementAsScalar(Queries.GetIsSystemAdminUser, new SqlParameter[]
                {
                    new SqlParameter("@UserId", userID),
                });

            return int.Parse(result.ToString()) > 0;
        }

        private KeyValuePair<int, string> GetArtifactTypeFromObjectGuid(int workspaceId, string objectGuid)
        {
            KeyValuePair<int, string> fieldData = default(KeyValuePair<int, string>);
            System.Data.Common.DbDataReader reader = _helper.GetDBContext(workspaceId).ExecuteSqlStatementAsDbDataReader(Queries.GetObjectTypeByGuid, new[] { SqlHelper.CreateSqlParameter("@artifactGuid", objectGuid) });

            if (reader.HasRows)
            {
                reader.Read();
                fieldData = new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1));
            }

            reader.Close();
            return fieldData;
        }

    }
}
