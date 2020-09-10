using kCura.SingleFileUpload.Core.Helpers;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.InstanceSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public class InstanceSettingManager : BaseManager, IInstanceSettingManager
	{
		private static Lazy<IInstanceSettingManager> _instance = new Lazy<IInstanceSettingManager>(() => new InstanceSettingManager());
		private const int _MAX_FILES = 100;
		private const int _MIN_FILES = 20;
		public static IInstanceSettingManager Instance => _instance.Value;

		private InstanceSettingManager()
		{
		}

		public async Task<int> GetMaxFilesInstanceSettingAsync()
		{
			int result = _MIN_FILES;
			try
			{
				string condition = $"'Name' IN ['{Constants.INSTANCE_SETTING_NAME}']";
				IEnumerable<InstanceSetting> resultList = await GetInstanceSettingsByConditionAsync(condition).ConfigureAwait(false);
				if (resultList.Any())
				{
					int.TryParse(resultList.FirstOrDefault().Value, out result);
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
			bool existInstanceSetting = await ExistMaxFilesInstanceSettingAsync().ConfigureAwait(false);
			if (!existInstanceSetting)
			{
				var instanceSetting = new InstanceSetting()
				{
					Name = Constants.INSTANCE_SETTING_NAME,
					Section = Constants.INSTANCE_SETTING_SECTION,
					ValueType = global::Relativity.Services.InstanceSetting.ValueType.Int32,
					Value = Constants.INSTANCE_SETTING_VALUE,
					InitialValue = Constants.INSTANCE_SETTING_VALUE,
					Description = Constants.INSTANCE_SETTING_DESCRIPTION
				};
				await CreateInstanceSettingAsync(instanceSetting).ConfigureAwait(false);
			}
		}

		private async Task<bool> ExistMaxFilesInstanceSettingAsync()
		{
			bool result = false;
			try
			{
				string condition = $"'Name' IN ['{Constants.INSTANCE_SETTING_NAME}']";
				IEnumerable<InstanceSetting> resultList = await GetInstanceSettingsByConditionAsync(condition).ConfigureAwait(false);
				result = resultList.Any();
			}
			catch (Exception ex)
			{
				LogError(ex);

				// GetMaxFilesInstanceSettingAsync will return default value in case of exception or instance setting absence.
				// Thanks to that we can return true in case of error.
				// This will prevent errors coming from attempt to create the instance setting twice.
				result = true;
			}
			return result;
		}

		private async Task<IEnumerable<InstanceSetting>> GetInstanceSettingsByConditionAsync(string condition)
		{
			using (var instanceSettingProxy = _Repository.CreateProxy<global::Relativity.Services.InstanceSetting.IInstanceSettingManager>(ExecutionIdentity.System))
			{
				var query = new Query
				{
					Condition = condition
				};
				InstanceSettingQueryResultSet instanceSettingQueryResultSet = await instanceSettingProxy.QueryAsync(query).ConfigureAwait(false);
				if (instanceSettingQueryResultSet.Success && instanceSettingQueryResultSet.Results.Any())
				{
					var list = new List<InstanceSetting>(instanceSettingQueryResultSet.Results.Count);
					foreach (var instance in instanceSettingQueryResultSet.Results)
					{
						list.Add(instance.Artifact);
					}
					return list;
				}
				else
				{
					throw new ApplicationException($"An error occured when querying for the instance setting. ErrorMessage = {instanceSettingQueryResultSet.Message}");
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
