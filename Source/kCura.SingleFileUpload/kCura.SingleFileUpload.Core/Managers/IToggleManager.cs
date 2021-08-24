using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IToggleManager
    {
        Task<bool> GetChangeFileNameAsync();

        Task<bool> GetCheckSFUFieldsAsync();

        Task<bool> GetValidateSFUCustomPermissionsAsync();

        Task<bool> GetCheckUploadMassiveAsync();
    }
}
