using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IToggleManager
    {
        Task<bool> GetCheckUploadMassiveAsync();
    }
}
