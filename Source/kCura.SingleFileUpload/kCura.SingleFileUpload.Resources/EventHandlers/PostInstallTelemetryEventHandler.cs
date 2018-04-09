using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;
using System;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Resources.EventHandlers
{
    [kCura.EventHandler.CustomAttributes.RunTarget(kCura.EventHandler.Helper.RunTargets.Workspace)]
    [kCura.EventHandler.CustomAttributes.RunOnce(false)]
    [kCura.EventHandler.CustomAttributes.Description("Single File Upload Post Install Event Handler")]
    [System.Runtime.InteropServices.Guid("D94A421D-E7C8-433D-B325-E98998C846BA")]
    public class SingleFileUploadPostInstallTelemetryEventHandler : PostInstallEventHandler
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

        public override Response Execute()
        {
            Response response = new Response();
            RepositoryHelper.ConfigureRepository(this.Helper);
            using (CacheContextScope d = RepositoryHelper.InitializeRepository(this.Helper.GetActiveCaseID()))
            {
                try
                {
                    TelemetryRepository.CreateMetricsAsync().Wait();
                    ToggleManager.Instance.SetChangeFileNameAsync(true).Wait();
                    ToggleManager.Instance.SetCheckSFUFieldsAsync(true).Wait();
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
    }
}
