using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
	public interface IInstanceSettingManager
	{
		Task<int> GetMaxFilesInstanceSettingAsync();
		Task CreateMaxFilesInstanceSettingAsync();
	}
}
