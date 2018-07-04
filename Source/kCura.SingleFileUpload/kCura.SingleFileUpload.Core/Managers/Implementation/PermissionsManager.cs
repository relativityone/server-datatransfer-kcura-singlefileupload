﻿using kCura.Relativity.Client;
using kCura.SingleFileUpload.Core.SQL;
using Relativity.API;
using Services = Relativity.Services;
using Relativity.Services.Permission;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Relativity.Services;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class PermissionsManager : BaseManager, IPermissionsManager
    {
        private static Lazy<IPermissionsManager> _instance = new Lazy<IPermissionsManager>(() => new PermissionsManager());
        public static IPermissionsManager Instance => _instance.Value;

        private PermissionsManager()
        {
        }

		public async Task<bool> CurrentUserHasPermissionToObjectType(int workspaceId, string objectTypeGuid, string permissionName)
		{
			var artifactTypeId = this.GetArtifactTypeFromObjectGuid(workspaceId, objectTypeGuid);
			bool task = await Permission_ReadSelectedSingleAsync(workspaceId, artifactTypeId.Key, permissionName);
			return task;
		}

		public async Task<bool> Permission_CreateSingleAsync(string permissionName, int artifactTypeId)
        {
            bool success = false;

            using (IPermissionManager proxy = _Repository.CreateProxy<IPermissionManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    // Create a new permission to be added to the above RDO.
                    Permission newPermission = new Permission();
                    newPermission.Name = permissionName;
                    newPermission.ArtifactType.ID = artifactTypeId;
                    newPermission.PermissionType = PermissionType.Custom;

                    // Create the new permission.
                    int permissionId = await proxy.CreateSingleAsync(WorkspaceID, newPermission);
                    success = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return success;
        }
        public async Task<bool> Permission_ReadSelectedSingleAsync(int workspaceId, int artifactTypeId, string permissionName)
        {
            bool selected = false;
            using (IPermissionManager proxy = _Repository.CreateProxy<IPermissionManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    List<PermissionRef> _permmision = new List<PermissionRef>();
                    PermissionRef pref = new PermissionRef();
                    pref.Name = permissionName;
                    pref.ArtifactType.ID = artifactTypeId;
                    _permmision.Add(pref);

                    List<PermissionValue> permissionValues = await proxy.GetPermissionSelectedAsync(workspaceId, _permmision);

                    foreach (var permission in permissionValues)
                    {
                        if (permission.Name == permissionName)
                        {
                            selected = permission.Selected;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return selected;
        }

        public bool Permission_Exist(string permissionName)
        {
            bool exist = false;
            using (IPermissionManager proxy = _Repository.CreateProxy<IPermissionManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    Services.Query query = new Services.Query();
                    Services.Condition queryCondition = new Services.TextCondition(PermissionFieldNames.Name, Services.TextConditionEnum.EqualTo, permissionName);

                    string queryString = queryCondition.ToQueryString();
                    query.Condition = queryString;

                    PermissionQueryResultSet queryResultSet = proxy.QueryAsync(WorkspaceID, query).Result;

                    exist = (queryResultSet.Results.Count > 0) ? true : false;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return exist;
        }

        public bool IsUserAdministrator(int workspaceID, int userID)
        {
            return (bool)_Repository.MasterDBContext.ExecuteSqlStatementAsScalar(Queries.IsUserAdministrator, new SqlParameter[]{
                  SqlHelper.CreateSqlParameter("@workspaceArtifactID", workspaceID),
                    SqlHelper.CreateSqlParameter("@userArtifactID", userID)
            });
        }

		private KeyValuePair<int, string> GetArtifactTypeFromObjectGuid(int workspaceId, string objectGuid)
		{
			KeyValuePair<int, string> fieldData = default(KeyValuePair<int, string>);
			System.Data.Common.DbDataReader reader = _Repository.CaseDBContext.ExecuteSqlStatementAsDbDataReader(Queries.GetObjectTypeByGuid, new[] { SqlHelper.CreateSqlParameter("@artifactGuid", objectGuid) });

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