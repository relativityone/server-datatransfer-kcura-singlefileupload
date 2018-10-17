using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;
using System;

namespace kCura.SingleFileUpload.Resources.EventHandlers
{
    [EventHandler.CustomAttributes.Description("Single File Upload Permission Post Install Event Handler")]
    [System.Runtime.InteropServices.Guid("C4C61DB8-4151-45A8-9885-C0A4A8A5C69C")]
    [EventHandler.CustomAttributes.RunOnce(false)]
    public class SingleFileUploadPermissionPostInstallEventHandler : PostInstallEventHandler
    {
        public override Response Execute()
        {
            var response = new Response();
            CacheContextScope disposableContext = null;
            try
            {
                RepositoryHelper.ConfigureRepository(Helper);
                disposableContext = RepositoryHelper.InitializeRepository(this.Helper.GetActiveCaseID());
                if (!PermissionsManager.Instance.Permission_Exist(Core.Helpers.Constants.ADD_DOCUMENT_CUSTOM_PERMISSION))
                {
                    PermissionsManager.Instance.Permission_CreateSingleAsync(Core.Helpers.Constants.ADD_DOCUMENT_CUSTOM_PERMISSION, 10);
                }
				DocumentRepository.RemovePageInteractionEvenHandlerFromDocumentObject();
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

	}
}
