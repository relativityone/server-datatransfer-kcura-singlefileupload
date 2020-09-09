using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
	public interface IPermissionsManager
	{
		Task<bool> Permission_CreateSingleAsync(string permissionName, int artifactTypeId);

		Task<bool> Permission_ReadSelectedSingleAsync(int workspaceId, int artifactTypeId, string permissionName);

		bool IsUserAdministrator(int workspaceID, int userID);

		Task<bool> Permission_ExistAsync(string permissionName);

		Task<bool> CurrentUserHasPermissionToObjectTypeAsync(int workspaceId, string objectTypeGuid, string permissionName);

	}
}
