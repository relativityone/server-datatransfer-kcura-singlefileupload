using kCura.SingleFileUpload.Core.Toggles;
using Relativity.Toggles;
using System;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class ToggleManager: BaseManager , IToggleManager
    {
        private static Lazy<IToggleManager> _instance = new Lazy<IToggleManager>(() => new ToggleManager());
        public static IToggleManager Instance => _instance.Value;

        private ToggleManager()
        {
        }
        
        public Task<bool> GetCheckSFUFieldsAsync()
        {
            return ToggleProvider.Current.IsEnabledAsync<CheckSFUFields>();
        }
        
        public Task<bool> GetCheckUploadMassiveAsync()
        {
            return ToggleProvider.Current.IsEnabledAsync<UploadMassiveDocuments>();
        }
    }
}
