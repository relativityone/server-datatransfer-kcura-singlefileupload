using System;
using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;


namespace kCura.SingleFileUpload.Resources.EventHandlers
{
	[EventHandler.CustomAttributes.Description("Single File Upload Permission Post Install Event Handler")]
	[System.Runtime.InteropServices.Guid("C4C61DB8-4151-45A8-9885-C0A4A8A5C69C")]
	[EventHandler.CustomAttributes.RunOnce(false)]
	public class SingleFileUploadPermissionPostInstallEventHandler : PostInstallEventHandler
	{
		private const int _DOCUMENT_ARTIFACT_TYPE_ID = 10;

		public override Response Execute()
		{
			var response = new Response();

			try
			{
				RepositoryHelper.ConfigureRepository(Helper);

				using (CacheContextScope disposableContext = RepositoryHelper.InitializeRepository(Helper.GetActiveCaseID()))
				{
					if (!PermissionsManager.Instance.Permission_ExistAsync(Core.Helpers.Constants.ADD_DOCUMENT_CUSTOM_PERMISSION).GetAwaiter().GetResult())
					{
						PermissionsManager.Instance.Permission_CreateSingleAsync(Core.Helpers.Constants.ADD_DOCUMENT_CUSTOM_PERMISSION, _DOCUMENT_ARTIFACT_TYPE_ID).GetAwaiter().GetResult();
					}

					try
					{
						DocumentManager.Instance.RemovePageInteractionEvenHandlerFromDocumentObject();
					}
					catch (Exception ex)
					{
						Helper.GetLoggerFactory().GetLogger().ForContext<SingleFileUploadPermissionPostInstallEventHandler>()
							.LogWarning(ex, $"An error occured while removing {nameof(DocumentPageInteractionEventHandler)}: {{0}}", ex.Message);
					}
				}

				response.Success = true;
			}
			catch (Exception e)
			{
				response.Success = false;
				response.Message = e.Message;
				response.Exception = e;
			}

			return response;
		}
	}
}
