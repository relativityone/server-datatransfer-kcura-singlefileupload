using System.Collections.Generic;
using System.Threading.Tasks;
using Relativity.API;

namespace kCura.SingleFileUpload.Core.Managers
{
	public interface IInstanceSettingManager
	{
		Task<int> GetMaxFilesInstanceSettingAsync();
		Task CreateMaxFilesInstanceSettingAsync();
		Task<IEnumerable<string>> GetRestrictedExtensionsAsync(IHelper helper);
	}
}
