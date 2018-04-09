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
    [kCura.EventHandler.CustomAttributes.RunOnce(true)]
    [kCura.EventHandler.CustomAttributes.Description("Single File Upload Post Install Event Handler")]
    [System.Runtime.InteropServices.Guid("DD7D97E1-8FF7-4E47-A3EF-FB78BC473A9D")]
    public class SingleFileUploadPostInstallEventHandler : PostInstallEventHandler
    {
        IDocumentManager Repository
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

        public override Response Execute()
        {
            Response response = new Response();
            RepositoryHelper.ConfigureRepository(this.Helper);
            using (CacheContextScope d = RepositoryHelper.InitializeRepository(this.Helper.GetActiveCaseID()))
            {
                try
                {
                    executeAsync().Wait();
                    response.Success = true;
                }
                catch (Exception e)
                {
                    response.Success = false;
                    response.Message = e.Message;
                    response.Exception = e;
                }
            }


            return response;
        }

        private async Task executeAsync()
        {
            await TelemetryRepository.CreateMetricsAsync();
            await ToggleManager.Instance.SetChangeFileNameAsync(true);
            await ToggleManager.Instance.SetCheckSFUFieldsAsync(true);
            Repository.SetCreateInstanceSettings();
        }
    }
}
