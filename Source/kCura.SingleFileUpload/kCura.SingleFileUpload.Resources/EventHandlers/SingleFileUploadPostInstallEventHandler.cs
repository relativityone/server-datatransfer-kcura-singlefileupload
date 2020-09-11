using kCura.EventHandler;
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
		public override Response Execute()
		{
			var response = new Response();

			try
			{
				RepositoryHelper.ConfigureRepository(Helper);
				using (CacheContextScope disposableContext = RepositoryHelper.InitializeRepository(Helper.GetActiveCaseID()))
				{
					DocumentManager.Instance.SetCreateInstanceSettings();

					ExecuteTelemetryAsync().GetAwaiter().GetResult();
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

		private static async Task ExecuteTelemetryAsync()
		{
			if (await ToggleManager.Instance.GetChangeFileNameAsync().ConfigureAwait(false))
			{
				await ToggleManager.Instance.SetChangeFileNameAsync(false).ConfigureAwait(false);
			}

			if (await ToggleManager.Instance.GetCheckSFUFieldsAsync().ConfigureAwait(false))
			{
				await ToggleManager.Instance.SetCheckSFUFieldsAsync(false).ConfigureAwait(false);
			}

			if (!await ToggleManager.Instance.GetCheckUploadMassiveAsync().ConfigureAwait(false))
			{
				await ToggleManager.Instance.SetCheckUploadMassiveAsync(true).ConfigureAwait(false);
			}

			await ToggleManager.Instance.SetValidateSFUCustomPermissionsAsync(false).ConfigureAwait(false);

			await InstanceSettingManager.Instance.CreateMaxFilesInstanceSettingAsync().ConfigureAwait(false);
		}
	}

}
