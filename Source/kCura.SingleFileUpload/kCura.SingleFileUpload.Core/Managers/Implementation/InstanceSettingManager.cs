using kCura.SingleFileUpload.Core.Helpers;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.InstanceSetting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public class InstanceSettingManager : BaseManager, IInstanceSettingManager
	{
        private const int _MIN_FILES = 20;
		private const int _MAX_FILES = 100;

		private static readonly Lazy<IInstanceSettingManager> _instance = new Lazy<IInstanceSettingManager>(() => new InstanceSettingManager());

		public static IInstanceSettingManager Instance => _instance.Value;

		private InstanceSettingManager()
		{
		}

		public async Task<int> GetMaxFilesInstanceSettingAsync()
		{
			int result = _MIN_FILES;
			try
			{
				InstanceSetting instanceSetting = await GetMaxFilesToUploadInstanceSettingAsync().ConfigureAwait(false);
				if (instanceSetting != null)
				{
					int.TryParse(instanceSetting.Value, out result);

					if (result < 1)
					{
						result = 1;
					}
					else if (result > _MAX_FILES)
					{
						result = _MAX_FILES;
					}
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
			return result;
		}

		public async Task CreateMaxFilesInstanceSettingAsync()
		{
			InstanceSetting existingInstanceSetting = await GetMaxFilesToUploadInstanceSettingAsync().ConfigureAwait(false);

			if (existingInstanceSetting == null)
			{
				var instanceSetting = new InstanceSetting()
				{
					Name = Constants.MAX_FILES_TO_UPLOAD_INSTANCE_SETTING_NAME,
					Section = Constants.INSTANCE_SETTING_SECTION,
					ValueType = global::Relativity.Services.InstanceSetting.ValueType.Int32,
					Value = Constants.MAX_FILES_TO_UPLOAD_INSTANCE_SETTING_DEFAULT_VALUE,
					InitialValue = Constants.MAX_FILES_TO_UPLOAD_INSTANCE_SETTING_DEFAULT_VALUE,
					Description = Constants.MAX_FILES_TO_UPLOAD_INSTANCE_SETTING_DESCRIPTION
				};
				await CreateInstanceSettingAsync(instanceSetting).ConfigureAwait(false);
			}
		}
		
		private async Task<InstanceSetting> GetMaxFilesToUploadInstanceSettingAsync()
		{
			using (var instanceSettingManager = _Repository.CreateProxy<global::Relativity.Services.InstanceSetting.IInstanceSettingManager>(ExecutionIdentity.System))
			{
                string condition = $"'Name' IN ['{Constants.MAX_FILES_TO_UPLOAD_INSTANCE_SETTING_NAME}']";

				var query = new Query
				{
					Condition = condition
				};

                InstanceSettingQueryResultSet queryResultSet = await instanceSettingManager.QueryAsync(query).ConfigureAwait(false);

				if (queryResultSet.Success)
                {
                    return queryResultSet.Results?.FirstOrDefault()?.Artifact;
                }
				else
				{
					throw new ApplicationException($"An error occured when querying for the instance setting. Error message: {queryResultSet.Message}");
				}
			}
		}

		private async Task CreateInstanceSettingAsync(InstanceSetting instanceSetting)
		{
			using (var instanceSettingProxy = _Repository.CreateProxy<global::Relativity.Services.InstanceSetting.IInstanceSettingManager>(ExecutionIdentity.System))
			{
				await instanceSettingProxy.CreateSingleAsync(instanceSetting).ConfigureAwait(false);
			}
		}

	}
}
