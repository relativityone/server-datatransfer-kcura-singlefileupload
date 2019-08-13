using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;
using System;
using System.Data;

namespace kCura.SingleFileUpload.Core.Factories
{
	public class ImportApiFactory : IImportApiFactory
	{
		private static IImportAPI _importAPIInstance;
		private static IImportBulkArtifactJob _importBulkArtifactJobInstance;
		private static readonly Lazy<IImportApiFactory> ImportApiFactoryLazy = new Lazy<IImportApiFactory>(() => new ImportApiFactory());

		public static IImportApiFactory Instance => ImportApiFactoryLazy.Value;

		private ImportApiFactory()
		{

		}

		/// <summary>
		/// Use this specifically to inject a testing instance... DO NOT USE DURING RUNTIME.
		/// </summary>
		/// <param name="importApi">Singleton ImportAPI for host execution</param>
		public static void SetUpSingleton(IImportAPI importApi, IImportBulkArtifactJob importBulkArtifactJobInstance)
		{
			_importAPIInstance = importApi;
			_importBulkArtifactJobInstance = importBulkArtifactJobInstance;
		}

		public IImportAPI GetImportAPI(ImportSettings settings)
		{
			IImportAPI importApi;
			if (_importAPIInstance == null)
			{
				string username = settings.RelativityUsername;
				string token = settings.RelativityPassword;
				string webServiceUrl = settings.WebServiceURL;
				importApi = new ExtendedImportAPI(username, token, webServiceUrl);
			}
			else
			{
				importApi = _importAPIInstance;
			}

			return importApi;
		}

		public IImportBulkArtifactJob GetImportApiBulkArtifactJob(
			IImportAPI importApi,
			int workspaceID,
			IImportNotifier.OnCompleteEventHandler importJobOnComplete,
			ImportBulkArtifactJob.OnErrorEventHandler importJobOnError,
			IImportNotifier.OnFatalExceptionEventHandler importJobOnFatal,
			int folderId,
			DocumentIdentifierField identityField,
			IDataReader documentsDataReader)
		{
			IImportBulkArtifactJob importBulkArtifactJob;
			if (_importBulkArtifactJobInstance == null)
			{
				importBulkArtifactJob = ProvisionLegitImportBulkArtifactJob(importApi, workspaceID, importJobOnComplete, importJobOnError, importJobOnFatal, folderId, identityField, documentsDataReader);
			}
			else
			{
				importBulkArtifactJob = _importBulkArtifactJobInstance;

			}

			return importBulkArtifactJob;
		}

		private ImportBulkArtifactJob ProvisionLegitImportBulkArtifactJob(IImportAPI importApi,
			int workspaceID,
			IImportNotifier.OnCompleteEventHandler importJobOnComplete,
			ImportBulkArtifactJob.OnErrorEventHandler importJobOnError,
			IImportNotifier.OnFatalExceptionEventHandler importJobOnFatal,
			int folderId,
			DocumentIdentifierField identityField,
			IDataReader documentsDataReader)
		{
			ImportBulkArtifactJob importJob = importApi.NewNativeDocumentImportJob();
			importJob.Settings.CaseArtifactId = workspaceID;
			importJob.Settings.ExtractedTextFieldContainsFilePath = false;
			importJob.Settings.DisableExtractedTextEncodingCheck = true;
			importJob.Settings.DisableExtractedTextFileLocationValidation = true;
			importJob.Settings.DisableNativeLocationValidation = true;
			importJob.Settings.DisableNativeValidation = false;
			importJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			importJob.OnComplete += importJobOnComplete;
			importJob.OnError += importJobOnError;
			importJob.OnFatalException += importJobOnFatal;
			importJob.Settings.DisableUserSecurityCheck = false;


			if (folderId != 0)
			{
				importJob.Settings.DestinationFolderArtifactID = folderId;
			}

			importJob.Settings.SelectedIdentifierFieldName = identityField.Name;
			importJob.Settings.NativeFilePathSourceFieldName = "Native File";
			importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			importJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			// Specify the ArtifactID of the document identifier field, such as a control number.
			importJob.Settings.IdentityFieldId = identityField.ArtifactId;
			importJob.SourceData.SourceData = documentsDataReader;

			return importJob;
		}
	}
}

