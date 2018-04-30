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

        public override Response Execute()
        {
            Response response = new Response();
            RepositoryHelper.ConfigureRepository(this.Helper);
            using (CacheContextScope d = RepositoryHelper.InitializeRepository(this.Helper.GetActiveCaseID()))
            {
                try
                {
                    Repository.SetCreateInstanceSettings();
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
