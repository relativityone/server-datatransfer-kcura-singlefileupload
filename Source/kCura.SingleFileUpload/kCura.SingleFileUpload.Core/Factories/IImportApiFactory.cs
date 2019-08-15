using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;

namespace kCura.SingleFileUpload.Core.Factories
{
	public interface IImportApiFactory
	{
		IImportAPI GetImportAPI(ImportSettings settings);

		IImportBulkArtifactJob GetImportApiBulkArtifactJob(
			IImportAPI importApi,
			IImportNotifier.OnCompleteEventHandler importJobOnComplete,
			ImportBulkArtifactJob.OnErrorEventHandler importJobOnError,
			IImportNotifier.OnFatalExceptionEventHandler importJobOnFatal,
			ImportJobSettings importJobSettings);
	}
}
