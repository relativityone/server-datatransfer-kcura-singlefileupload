using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;
using System;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Resources.EventHandlers
{
    [EventHandler.CustomAttributes.RunTarget(kCura.EventHandler.Helper.RunTargets.Instance)]
    [EventHandler.CustomAttributes.RunOnce(false)]
    [EventHandler.CustomAttributes.Description("Single File Upload Post Install Event Handler")]
    [System.Runtime.InteropServices.Guid("D94A421D-E7C8-433D-B325-E98998C846BA")]
    public class SingleFileUploadPostInstallEventHandler : PostInstallEventHandler
    {

		private IDocumentManager _repository;

		private ITelemetryManager _telemetryRepository;

		private ITelemetryManager TelemetryRepository
        {
            get
            {
                if (_telemetryRepository == null)
                {
                    _telemetryRepository = new TelemetryManager();
                }
                return _telemetryRepository;
            }
        }

		private IDocumentManager DocumentRepository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = new DocumentManager();
                }
                return _repository;
            }
        }

        public override Response Execute()
        {
            var response = new Response();
            CacheContextScope disposableContext = null;
            try
            {
                RepositoryHelper.ConfigureRepository(Helper);
                disposableContext = RepositoryHelper.InitializeRepository(this.Helper.GetActiveCaseID());
                DocumentRepository.SetCreateInstanceSettings();
                ExecuteTelemetryAsync().Wait();
                response.Success = true;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                response.Exception = e;
            }
            finally
            {
                disposableContext?.Dispose();
            }
            return response;
        }

        private async Task ExecuteTelemetryAsync()
        {
            if (await ToggleManager.Instance.GetChangeFileNameAsync())
            {
                await ToggleManager.Instance.SetChangeFileNameAsync(false);
            }
            if (await ToggleManager.Instance.GetCheckSFUFieldsAsync())
            {
                await ToggleManager.Instance.SetCheckSFUFieldsAsync(false);
            }
            if (!await ToggleManager.Instance.GetCheckUploadMassiveAsync())
            {
                await ToggleManager.Instance.SetCheckUploadMassiveAsync(true);
            }
            await ToggleManager.Instance.SetValidateSFUCustomPermissionsAsync(false);
			await InstanceSettingManager.Instance.CreateMaxFilesInstanceSettingAsync();
		}
    }

}
