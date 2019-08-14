using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;
using System;

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
			IImportNotifier.OnCompleteEventHandler importJobOnComplete,
			ImportBulkArtifactJob.OnErrorEventHandler importJobOnError,
			IImportNotifier.OnFatalExceptionEventHandler importJobOnFatal,
			ImportJobSettings importJobSettings)
		{
			IImportBulkArtifactJob importBulkArtifactJob;
			if (_importBulkArtifactJobInstance == null)
			{
				importBulkArtifactJob = ProvisionLegitImportBulkArtifactJob(importApi,
					importJobOnComplete, importJobOnError, importJobOnFatal, importJobSettings);
			}
			else
			{
				importBulkArtifactJob = _importBulkArtifactJobInstance;

			}

			return importBulkArtifactJob;
		}

		private ImportBulkArtifactJob ProvisionLegitImportBulkArtifactJob(IImportAPI importApi,
			IImportNotifier.OnCompleteEventHandler importJobOnComplete,
			ImportBulkArtifactJob.OnErrorEventHandler importJobOnError,
			IImportNotifier.OnFatalExceptionEventHandler importJobOnFatal,
			ImportJobSettings importJobSettings)
		{
			ImportBulkArtifactJob importJob = importApi.NewNativeDocumentImportJob();
			importJob.Settings.CaseArtifactId = importJobSettings.WorkspaceID;
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


			if (importJobSettings.FolderId != 0)
			{
				importJob.Settings.DestinationFolderArtifactID = importJobSettings.FolderId;
			}

			importJob.Settings.SelectedIdentifierFieldName = importJobSettings.IdentityField.Name;
			importJob.Settings.NativeFilePathSourceFieldName = "Native File";
			importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			importJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			// Specify the ArtifactID of the document identifier field, such as a control number.
			importJob.Settings.IdentityFieldId = importJobSettings.IdentityField.ArtifactId;
			importJob.SourceData.SourceData = importJobSettings.DocumentsDataReader;

			return importJob;
		}
	}
}

