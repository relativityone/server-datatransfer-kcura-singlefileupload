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
		private static readonly Lazy<IPermissionsManager> _instance = new Lazy<IPermissionsManager>(() => new PermissionsManager());

		public static IPermissionsManager Instance => _instance.Value;

		private PermissionsManager()
		{
		}

		public async Task<bool> CurrentUserHasPermissionToObjectTypeAsync(int workspaceId, string objectTypeGuid, string permissionName)
		{
			KeyValuePair<int, string> artifactTypeId = GetArtifactTypeFromObjectGuid(objectTypeGuid);
			bool task = await Permission_ReadSelectedSingleAsync(workspaceId, artifactTypeId.Key, permissionName).ConfigureAwait(false);
			return task;
		}

		public async Task<bool> Permission_CreateSingleAsync(string permissionName, int artifactTypeId)
		{
			using (IPermissionManager proxy = _Repository.CreateProxy<IPermissionManager>(ExecutionIdentity.CurrentUser))
			{
				// Create a new permission to be added to the above RDO.
				Permission newPermission = new Permission
				{
					Name = permissionName,
					ArtifactType = {ID = artifactTypeId},
					PermissionType = PermissionType.Custom
				};

				// Create the new permission.
				await ExecuteWithServiceRetriesAsync(() => proxy.CreateSingleAsync(WorkspaceID, newPermission)).ConfigureAwait(false);

				return true;
			}
		}

		public async Task<bool> Permission_ReadSelectedSingleAsync(int workspaceId, int artifactTypeId, string permissionName)
		{
			bool selected = false;
			using (IPermissionManager proxy = _Repository.CreateProxy<IPermissionManager>(ExecutionIdentity.CurrentUser))
			{
				List<PermissionRef> permmision = new List<PermissionRef>();
				PermissionRef pref = new PermissionRef();
				pref.Name = permissionName;
				pref.ArtifactType.ID = artifactTypeId;
				permmision.Add(pref);

				List<PermissionValue> permissionValues = await proxy.GetPermissionSelectedAsync(workspaceId, permmision).ConfigureAwait(false);

				foreach (var permission in permissionValues)
				{
					if (permission.Name == permissionName)
					{
						selected = permission.Selected;
						break;
					}
				}
			}

			return selected;
		}

		public async Task<bool> Permission_ExistAsync(string permissionName)
		{
			bool exist = false;
			using (IPermissionManager proxy = _Repository.CreateProxy<IPermissionManager>(ExecutionIdentity.CurrentUser))
			{
				Services.Query query = new Services.Query();
				Services.Condition queryCondition = new Services.TextCondition(PermissionFieldNames.Name, Services.TextConditionEnum.EqualTo, permissionName);

				string queryString = queryCondition.ToQueryString();
				query.Condition = queryString;

				PermissionQueryResultSet queryResultSet = await ExecuteWithServiceRetriesAsync(() => proxy.QueryAsync(WorkspaceID, query)).ConfigureAwait(false);

				exist = (queryResultSet.Results.Count > 0) ? true : false;
			}

			return exist;
		}

		public bool IsUserAdministrator(int workspaceID, int userID)
		{
			return (bool)_Repository.MasterDBContext.ExecuteSqlStatementAsScalar(Queries.IsUserAdministrator,
				new SqlParameter[]
				{
					SqlHelper.CreateSqlParameter("@workspaceArtifactID", workspaceID),
					SqlHelper.CreateSqlParameter("@userArtifactID", userID)
				}
			);
		}

		private KeyValuePair<int, string> GetArtifactTypeFromObjectGuid(string objectGuid)
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
