using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;

namespace kCura.SingleFileUpload.Core.Factories
{
	public class ImportApiFactory : IImportApiFactory
	{

		public IExtendedImportAPI GetImportAPI(ImportSettings settings)
		{
			string username = settings.RelativityUsername;
			string token = settings.RelativityPassword;
			string webServiceUrl = settings.WebServiceURL;

			return new ExtendedImportAPI(username, token, webServiceUrl);
		}
	}
}

