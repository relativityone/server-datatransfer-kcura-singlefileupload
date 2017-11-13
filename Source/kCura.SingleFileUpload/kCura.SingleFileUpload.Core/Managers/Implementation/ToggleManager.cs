using System;
using System.Threading.Tasks;
using Relativity.Toggles;
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
            bool result = await ToggleProvider.Current.IsEnabledAsync<ChangeFileName>();
            return result;
        }

        public async Task SetChangeFileNameAsync(bool enabled)
        {
            await ToggleProvider.Current.SetAsync<ChangeFileName>(enabled);
        }
    }
}
