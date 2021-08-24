using kCura.SingleFileUpload.Core.Toggles;
using Relativity.Toggles;
using System;
using System.Threading.Tasks;
using Relativity.SingleFileUpload.Core.Toggles;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class ToggleManager: BaseManager , IToggleManager
    {
        private static Lazy<IToggleManager> _instance = new Lazy<IToggleManager>(() => new ToggleManager());
        public static IToggleManager Instance => _instance.Value;

        private ToggleManager()
        {
        }

        public async Task<bool> GetChangeFileNameAsync()
        {
            bool result = await ToggleProvider.Current.IsEnabledAsync<ChangeFileName>().ConfigureAwait(false);
            return result;
        }

        public Task<bool> GetCheckSFUFieldsAsync()
        {
            return ToggleProvider.Current.IsEnabledAsync<CheckSFUFields>();
        }

        public Task<bool> GetValidateSFUCustomPermissionsAsync()
        {
	        return ToggleProvider.Current.IsEnabledAsync<ValidateSFUCustomPermissions>();
        }

        public Task<bool> GetCheckUploadMassiveAsync()
        {
            return ToggleProvider.Current.IsEnabledAsync<UploadMassiveDocuments>();
        }
    }
}
