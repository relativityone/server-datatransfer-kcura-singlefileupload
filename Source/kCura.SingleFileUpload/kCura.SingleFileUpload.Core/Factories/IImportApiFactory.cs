using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;
using System.Data;

namespace kCura.SingleFileUpload.Core.Factories
{
	public interface IImportApiFactory
	{
		IImportAPI GetImportAPI(ImportSettings settings);

		IImportBulkArtifactJob GetImportApiBulkArtifactJob(
			IImportAPI importApi,
			int workspaceID,
			IImportNotifier.OnCompleteEventHandler importJobOnComplete,
			ImportBulkArtifactJob.OnErrorEventHandler importJobOnError,
			IImportNotifier.OnFatalExceptionEventHandler importJobOnFatal,
			int folderId,
			DocumentIdentifierField identityField,
			IDataReader documentsDataReader);
	}
}
