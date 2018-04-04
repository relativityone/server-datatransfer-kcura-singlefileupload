using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IToggleManager
    {
        Task<bool> GetChangeFileNameAsync();
        Task SetChangeFileNameAsync(bool enabled);

        Task<bool> GetCheckSFUFieldsAsync();
        Task SetCheckSFUFieldsAsync(bool enabled);
    }
}
