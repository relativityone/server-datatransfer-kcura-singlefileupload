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
		public static IInstanceSettingManager Instance => _instance.Value;

		private InstanceSettingManager()
		{
		}
		public async Task<int> GetMaxFIlesInstanceSettingAsync()
		{
			try
			{
				var condition = $"'Name' IN ['{Constants.MAX_FILES_TO_UPLOAD}']";
				var resultList = await GetInstanceSettingsByCondition(condition);
				int.TryParse(resultList.FirstOrDefault().Value, out int result);
				if (result < 1)
				{
					result = 1;
				}
				if (result > 100)
				{
					result = 100;
				}
				return result;
			}
			catch (Exception ex)
			{
				LogError(ex);
				return 20;
			}
		}
		public async Task CreateMaxFIlesInstanceSettingAsync()
		{
			var instanceSetting = new InstanceSetting()
			{
				Name = Constants.MAX_FILES_TO_UPLOAD,
				Section = "SFU",
				ValueType = global::Relativity.Services.InstanceSetting.ValueType.Int32,
				Value = "20",
				InitialValue = "20",
				Description = "Determines the maximum value of files to upload using Simple File Upload. Maximum value should be 100."
			};
			await CreateInstanceSettingAsync(instanceSetting);
		}

		private async Task<IEnumerable<InstanceSetting>> GetInstanceSettingsByCondition(string condition)
		{
			using (var instanceSettingProxy = _Repository.CreateProxy<global::Relativity.Services.InstanceSetting.IInstanceSettingManager>(ExecutionIdentity.System))
			{
				var query = new Query
				{
					Condition = condition
				};
				var instanceSettingQueryResultSet = await instanceSettingProxy.QueryAsync(query);
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
			try
			{
				using (var instanceSettingProxy = _Repository.CreateProxy<global::Relativity.Services.InstanceSetting.IInstanceSettingManager>(ExecutionIdentity.System))
				{
					await instanceSettingProxy.CreateSingleAsync(instanceSetting);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

	}
}
