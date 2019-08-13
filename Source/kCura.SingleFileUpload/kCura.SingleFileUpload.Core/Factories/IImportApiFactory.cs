using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;

namespace kCura.SingleFileUpload.Core.Factories
{
	public interface IImportApiFactory
	{
		IExtendedImportAPI GetImportAPI(ImportSettings settings);
	}
}
