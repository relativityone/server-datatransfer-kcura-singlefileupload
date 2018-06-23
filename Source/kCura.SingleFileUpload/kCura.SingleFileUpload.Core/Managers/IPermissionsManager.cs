using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IPermissionsManager
    {
        Task<bool> Permission_CreateSingleAsync(string permissionName, int artifactTypeId);
        Task<bool> Permission_ReadSelectedSingleAsync(string permissionName);
        bool IsUserAdministrator(int workspaceID, int userID);
        bool Permission_Exist(string permissionName);
    }
}
