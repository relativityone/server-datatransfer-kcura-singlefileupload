using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;
using System;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Resources.EventHandlers
{
    [kCura.EventHandler.CustomAttributes.RunTarget(kCura.EventHandler.Helper.RunTargets.Instance)]
    [kCura.EventHandler.CustomAttributes.RunOnce(false)]
    [kCura.EventHandler.CustomAttributes.Description("Single File Upload Post Install Event Handler")]
    [System.Runtime.InteropServices.Guid("D94A421D-E7C8-433D-B325-E98998C846BA")]
    public class SingleFileUploadPostInstallEventHandler : PostInstallEventHandler
    {
        ITelemetryManager TelemetryRepository
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
        ITelemetryManager _telemetryRepository;
        IDocumentManager DocumentRepository
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
        IDocumentManager _repository;

        public override Response Execute()
        {
            var response = new Response();
            CacheContextScope disposableContext = null;
            try
            {
                RepositoryHelper.ConfigureRepository(Helper);
                disposableContext = RepositoryHelper.InitializeRepository(this.Helper.GetActiveCaseID());
                DocumentRepository.SetCreateInstanceSettings();
                executeTelemetryAsync().Wait();
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

        private async Task executeTelemetryAsync()
        {
            if (!await ToggleManager.Instance.GetChangeFileNameAsync())
            {
                await ToggleManager.Instance.SetChangeFileNameAsync(true);
            }
            if (await ToggleManager.Instance.GetCheckSFUFieldsAsync())
            {
                await ToggleManager.Instance.SetCheckSFUFieldsAsync(false);
            }
        }
    }
}
