using Relativity.SingleFileUpload.Core.Toggles;
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

        public async Task<bool> GetChangeFileNameAsync()
        {
            bool result = await ToggleProvider.Current.IsEnabledAsync<ChangeFileName>();
            return result;
        }

        public async Task SetChangeFileNameAsync(bool enabled)
        {
            await ToggleProvider.Current.SetAsync<ChangeFileName>(enabled);
        }

        public async Task<bool> GetCheckSFUFieldsAsync()
        {
            bool result = await ToggleProvider.Current.IsEnabledAsync<CheckSFUFields>();
            return result;
        }

        public async Task SetCheckSFUFieldsAsync(bool enabled)
        {
            await ToggleProvider.Current.SetAsync<CheckSFUFields>(enabled);
        }

        public async Task<bool> GetValidateSFUCustomPermissionsAsync()
        {
            bool result = await ToggleProvider.Current.IsEnabledAsync<ValidateSFUCustomPermissions>();
            return result;
        }

        public async Task SetValidateSFUCustomPermissionsAsync(bool enabled)
        {
            await ToggleProvider.Current.SetAsync<ValidateSFUCustomPermissions>(enabled);
        }

    }
}
