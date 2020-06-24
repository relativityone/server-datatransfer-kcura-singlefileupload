using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Factories;
using kCura.SingleFileUpload.Core.Helpers;
using kCura.SingleFileUpload.Core.SQL;
using Newtonsoft.Json.Linq;
using NSerio.Relativity;
using Relativity.API;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client = kCura.Relativity.Client;
using DataExchange = Relativity.DataExchange;
using DTOs = kCura.Relativity.Client.DTOs;
using Services = Relativity.Services;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public class DocumentManager : BaseManager, IDocumentManager
	{

		private const int _FILED_ARTIFACT_TYPE = 14;
		private readonly int _timeOutValue = 300;
		private readonly int[] _INCLUDE_PERMISSIONS = new int[] { 1, 2, 3, 4, 5, 6 };

		private static readonly Lazy<IDocumentManager> _INSTANCE = new Lazy<IDocumentManager>(() => new DocumentManager());

		public static IDocumentManager Instance => _INSTANCE.Value;


		private DocumentManager()
		{

		}
		public async Task<Response> SaveSingleDocument(ExportedMetadata documentInfo, int folderID, string webApiUrl, int workspaceID, int userID)
		{
			Tuple<string, string> importResult = await ImportDocumentAsync(documentInfo, webApiUrl, workspaceID, folderID);
			if (string.IsNullOrEmpty(importResult.Item1))
			{
				await CreateMetricsAsync(documentInfo, Constants.BUCKET_DOCUMENTSUPLOADED);
				return new Response() { Result = Path.GetFileNameWithoutExtension(documentInfo.FileName), Success = true };
			}
			return new Response() { Result = importResult.Item1, Success = false };
		}
		public async Task ReplaceSingleDocument(ExportedMetadata documentInfo, DocumentExtraInfo documentExtraInfo)
		{
			if (!documentExtraInfo.AvoidControlNumber || !documentExtraInfo.FromDocumentViewer)
			{
				DTOs.Document replacedDocument = new DTOs.Document(documentExtraInfo.DocID);
				if (!documentExtraInfo.AvoidControlNumber)
				{
					replacedDocument.TextIdentifier = Path.GetFileNameWithoutExtension(documentInfo.FileName);
				}
				if (!documentExtraInfo.FromDocumentViewer)
				{
					replacedDocument.ParentArtifact = new DTOs.Artifact(DefineFolder(documentExtraInfo.FolderID));
					await ChangeFolderAsync(documentExtraInfo.FolderID, documentExtraInfo.DocID);
				}
			}

			UpdateNative(documentInfo, documentExtraInfo.DocID);
			Tuple<string, string> importResult = await ImportDocumentAsync(
				documentInfo, documentExtraInfo.WebApiUrl, documentExtraInfo.WorkspaceID, documentExtraInfo.FolderID, documentExtraInfo.DocID);
			await CreateMetricsAsync(documentInfo, Constants.BUCKET_DOCUMENTSREPLACED);
			UpdateDocumentLastModificationFields(documentExtraInfo.DocID, documentExtraInfo.UserID, false);
		}
		public bool ValidateDocImages(int docArtifactId)
		{
			bool result = false;

			DTOs.Document document = new DTOs.Document(docArtifactId);
			document.Fields.Add(new DTOs.FieldValue(DTOs.DocumentFieldNames.HasImages));
			DTOs.ResultSet<DTOs.Document> docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
			if (docResults.Success)
			{
				DTOs.Document documentArtifact = docResults.Results.FirstOrDefault().Artifact;
				result = documentArtifact.HasImages.Name.Equals("Yes");
			}

			return result;
		}
		public bool ValidateDocNative(int docArtifactId)
		{
			bool result = false;

			DTOs.Document document = new DTOs.Document(docArtifactId);
			document.Fields.Add(new DTOs.FieldValue(DTOs.DocumentFieldNames.HasNative));
			DTOs.ResultSet<DTOs.Document> docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
			if (docResults.Success)
			{
				DTOs.Document documentArtifact = docResults.Results.FirstOrDefault().Artifact;
				result = documentArtifact.HasNative.Value;
			}

			return result;
		}
		public string GetDocumentControlNumber(int docArtifactId)
		{
			string result = null;

			DTOs.Document document = new DTOs.Document(docArtifactId);
			document.Fields.Add(new DTOs.FieldValue(new Guid(Helpers.Constants.CONTROL_NUMBER_FIELD)));
			DTOs.ResultSet<DTOs.Document> docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
			if (docResults.Success)
			{
				DTOs.Document documentArtifact = docResults.Results.FirstOrDefault().Artifact;
				result = documentArtifact.Fields[0].Value.ToString();
			}

			return result;
		}

		public bool ValidateHasRedactions(int docArtifactId)
		{
			string query = Queries.DocumentHasRedactions;
			return _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(query, new[]
				 {
					SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
				 }
			) > 0;
		}
		public void UpdateDocumentLastModificationFields(int docArtifactId, int userID, bool isNew)
		{
			_Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.UpdateDocumentLastModificationFields,
				new[]
				{
					SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
					SqlHelper.CreateSqlParameter("@UserID", userID),
					SqlHelper.CreateSqlParameter("@New", isNew),
				 }
			);
		}
		public void DeleteRedactions(int docArtifactId)
		{
			_Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.DeleteDocumentRedactions,
				 new SqlParameter[]
				 {
					SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
				 }
			);
		}
		public void DeleteExistingImages(int dArtifactId)
		{
			_Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.DeleteDocumentImages,
				new SqlParameter[]
				{
					SqlHelper.CreateSqlParameter("@DocumentID", dArtifactId),
				}
			);
		}
		public void InsertImage(FileInformation image)
		{
			_Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.InsertImageInFileTable,
				new SqlParameter[]
				{
					SqlHelper.CreateSqlParameter("@DocumentID", image.DocumentArtifactID),
					SqlHelper.CreateSqlParameter("@FileName", image.FileName),
					SqlHelper.CreateSqlParameter("@Order", image.Order),
					SqlHelper.CreateSqlParameter("@DocIdentifier", image.DocumentIdentifier),
					SqlHelper.CreateSqlParameter("@Location", image.FileLocation),
					SqlHelper.CreateSqlParameter("@Size", image.FileSize),
					SqlHelper.CreateSqlParameter("@Type", image.FileType)
				}
			);
		}
		public void UpdateHasImages(int dArtifactId)
		{
			_Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.UpdateHasImages,
				new SqlParameter[]
				{
					SqlHelper.CreateSqlParameter("@DocumentID", dArtifactId),
					SqlHelper.CreateSqlParameter("@HasImagesFieldGuid", Helpers.Constants.DOCUMENT_HAS_IMAGES_FIELD),
					SqlHelper.CreateSqlParameter("@HasImagesCodeYesGuid", Helpers.Constants.DOCUMENT_HAS_IMAGES_YES_CHOICE)
				}
			);
		}
		public FileInformation GetFileByArtifactId(int docArtifactId)
		{
			FileInformation fInformation = null;

			DataTable dt = _Repository.CaseDBContext.ExecuteSqlStatementAsDataTable(
				Queries.GetFileInfoByDocumentArtifactID, new[] { new SqlParameter("@documentArtifactId", docArtifactId) });
			DbDataReader reader = dt.CreateDataReader();

			if (reader.HasRows)
			{
				reader.Read();
				fInformation = new FileInformation()
				{
					FileID = reader.GetInt32(reader.GetOrdinal("FileID")),
					DocumentArtifactID = reader.GetInt32(reader.GetOrdinal("DocumentArtifactID")),
					FileName = reader.GetString(reader.GetOrdinal("Filename")),
					FileLocation = reader.GetString(reader.GetOrdinal("Location"))
				};
			}

			reader.Close();

			return fInformation;
		}

		private object SqlParameter(string v, int docArtifactId)
		{
			throw new NotImplementedException();
		}

		public int GetDocByName(string docName)
		{
			DTOs.Query<DTOs.Document> qDocs = new DTOs.Query<DTOs.Document>
			{
				Condition = new Client.TextCondition(DTOs.DocumentFieldNames.TextIdentifier, Client.TextConditionEnum.EqualTo, docName),
				Fields = DTOs.FieldValue.NoFields
			};
			return _Repository.RSAPIClient.Repositories.Document.Query(qDocs, 1).Results.FirstOrDefault()?.Artifact?.ArtifactID ?? -1;
		}
		public void SetCreateInstanceSettings()
		{
			Repository.Instance.MasterDBContext.ExecuteNonQuerySQLStatement(Queries.InsertInstanceSettings);
			int isResult = Repository.Instance.MasterDBContext.ExecuteNonQuerySQLStatement(Queries.InsertFieldsInstanceSetting);

			// Read Default fields configuration from instance setting
			if (isResult > 0)
			{
				CreateWorkspaceFieldSettings();
			}
		}
		public void RemovePageInteractionEvenHandlerFromDocumentObject()
		{
			_Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.RemovePageInteractionEvenHandlerFromDocumentObject);
		}
		public async Task<bool> ValidateFileTypes(string extension)
		{
			QueryResult results;
			using (IObjectManager objectManager = _Repository.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{
				QueryRequest query = new QueryRequest
				{
					ObjectType = new ObjectTypeRef() { ArtifactTypeID = (int)Client.ArtifactType.InstanceSetting },
					Fields = new[] { new FieldRef() { Name = "Value" } },
					IncludeIDWindow = false,
					Condition = "'Name' IN ['RestrictedNativeFileTypes']"
				};
				results = await objectManager.QueryAsync(-1, query, 1, 10);
			}
			var restricted = results.Objects[0].FieldValues[0].Value.ToString().Split(';').Select(x => x.ToLower()).ToList();
			restricted.AddRange(new[] { "dll", "exe", "js" });
			return !restricted.Contains(extension.Replace(".", ""));
		}

		public async Task<bool> IsDataGridEnabled(int workspaceID)
		{
			QueryResult results;
			using (IObjectManager objectManager = _Repository.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{
				QueryRequest query = new QueryRequest
				{
					ObjectType = new ObjectTypeRef() { ArtifactTypeID = (int)Client.ArtifactType.Case },
					Fields = new[] { new FieldRef() { Name = DTOs.WorkspaceFieldNames.EnableDataGrid } },
					IncludeIDWindow = false,
					Condition = $"'ArtifactID' IN [{workspaceID}]"
				};
				results = await objectManager.QueryAsync(-1, query, 1, 10);
			}
			var isDataGrid = (bool)results.Objects[0].FieldValues[0].Value;
			return isDataGrid;
		}

		public async Task<DataTable> GetDocumentDataTableAsync(string identifierName)
		{
			DataTable documentsDataTable = new DataTable();

			documentsDataTable.Columns.Add(identifierName, typeof(string));
			documentsDataTable.Columns.Add("Extracted Text", typeof(string));


			if (await ToggleManager.Instance.GetCheckSFUFieldsAsync())
			{
				object wsResult = Repository.Instance.CaseDBContext.ExecuteSqlStatementAsScalar(Queries.GetFieldsWorspaceSetting);
				JObject wsFields = null;
				if (wsResult != null)
				{
					wsFields = JObject.Parse(wsResult.ToString());
				}
				else
				{
					CreateWorkspaceFieldSettings();
					wsResult = Repository.Instance.CaseDBContext.ExecuteSqlStatementAsScalar(Queries.GetFieldsWorspaceSetting);

					if (wsResult != null)
					{
						wsFields = JObject.Parse(wsResult.ToString());
					}
					else
					{
						throw new MissingFieldException("Fields Settings does not exist.");
					}
				}

				foreach (var wsItem in wsFields)
				{
					documentsDataTable.Columns.Add(wsItem.Value.ToString(), typeof(string));
				}
			}
			else
			{
				documentsDataTable.Columns.Add("Document Extension", typeof(string));
				documentsDataTable.Columns.Add("File Extension", typeof(string));
				documentsDataTable.Columns.Add("FileExtension", typeof(string));
				documentsDataTable.Columns.Add("File Name", typeof(string));
				documentsDataTable.Columns.Add("FileName", typeof(string));
				documentsDataTable.Columns.Add("File Size");
				documentsDataTable.Columns.Add("FileSize");
			}

			documentsDataTable.Columns.Add("Native File", typeof(string));
			return documentsDataTable;
		}
		public void WriteFile(byte[] file, FileInformation document)
		{
			File.WriteAllBytes(document.FileLocation, file);
		}
		public string GetRepositoryLocation()
		{
			string location = _Repository.MasterDBContext.ExecuteSqlStatementAsScalar(Queries.GetRepoLocationByCaseID,
				new[] { SqlHelper.CreateSqlParameter("AID", _Repository.WorkspaceID) }).ToString();
			return !location.EndsWith("\\") ? string.Concat(location, "\\") : location;
		}
		public async Task CreateMetricsAsync(ExportedMetadata documentInfo, string bucket)
		{
			if (!string.IsNullOrEmpty(bucket))
			{
				await TelemetryManager.Instance.LogCountAsync(bucket, 1L);
				await TelemetryManager.Instance.LogCountAsync(Helpers.Constants.BUCKET_TOTALSIZEDOCUMENTUPLOADED, documentInfo.Native.LongLength);
				//Create File tipe metric
				await TelemetryManager.Instance.CreateMetricAsync(string.Concat(Helpers.Constants.BUCKET_FILETYPE,
					Path.GetExtension(documentInfo.FileName)),
					$"Number of {Path.GetExtension(documentInfo.FileName).Remove(0, 1)} uploaded");

				await TelemetryManager.Instance.LogCountAsync(string.Concat(Helpers.Constants.BUCKET_FILETYPE, Path.GetExtension(documentInfo.FileName)), 1L);
			}
		}

		public string InstanceFile(byte[] fileBytes, string fileExt, string baseRepo = null)
		{
			string guidFileName = Guid.NewGuid() + (fileExt ?? String.Empty);

			return InstanceFile(guidFileName, fileBytes, baseRepo);
		}

		private string InstanceFile(string fileName, byte[] fileBytes, string baseRepo = null)
		{
			string folder = Path.Combine(baseRepo ?? Path.GetTempPath(), $"RV_{Guid.NewGuid()}");
			Directory.CreateDirectory(folder);

			string tmpFileName = Path.Combine(folder, fileName);

			File.WriteAllBytes(tmpFileName, fileBytes);

			return tmpFileName;
		}

		public DataExchange.Io.IFileTypeInfo GetNativeTypeByFilename(string fileName)
		{
			try
			{
				return DataExchange.Io.FileTypeIdentifierService.Instance.Identify(fileName);
			}
			catch (Exception ex)
			{
				if (!ex.Message.Contains("cannot be identified because an Outside In error '0' occurred."))
				{
					throw;
				}
				return DataExchange.Io.FileTypeIdentifierService.Instance.Identify(fileName);
			}

		}
		private void CreateWorkspaceFieldSettings()
		{

			string fieldNames = Repository.Instance.MasterDBContext.ExecuteSqlStatementAsScalar(Queries.GetFieldsInstanceSetting).ToString();

			if (!string.IsNullOrEmpty(fieldNames))
			{
				JObject fields = JObject.Parse(fieldNames);
				JObject wsFields = new JObject();
				StringBuilder builder = new StringBuilder();

				foreach (var fItem in fields)
				{
					builder.Append(string.Format(Queries.GetFieldItem, fItem.Key, fItem.Value["value"].ToString()));
					builder.Append("\nUNION\n");
					wsFields.Add(fItem.Key, fItem.Value["default"].ToString());
				}

				string wsValue = builder.ToString();
				wsValue = wsValue.Substring(0, wsValue.LastIndexOf("UNION"));
				DataTable tblFields = Repository.Instance.CaseDBContext.ExecuteSqlStatementAsDataTable(wsValue);

				if (tblFields != null && tblFields.Rows.Count > 0)
				{
					foreach (DataRow row in tblFields.Rows)
					{
						wsFields[row[0].ToString()] = row[1].ToString();
					}
				}

				Repository.Instance.CaseDBContext.ExecuteNonQuerySQLStatement(string.Format(Queries.InsertFieldsWorspaceSetting, wsFields.ToString()));
			}
		}
		private void ForceTapiSettings()
		{
			SetWinEDDSSetting(Constants.TAPI_FORCE_WEB_UPLOAD);
			SetWinEDDSSetting(Constants.TAPI_FORCE_HTTP_CLIENT);
			SetWinEDDSSetting(Constants.TAPI_FORCE_BCP_HTTP_CLIENT);
		}
		private void SetWinEDDSSetting(string key, string value = "True")
		{
			if (WinEDDS.Config.ConfigSettings.Contains(key))
			{
				WinEDDS.Config.ConfigSettings[key] = value;
			}
			else
			{
				WinEDDS.Config.ConfigSettings.Add(key, value);
			}
		}
		private async Task<Tuple<string, string>> ImportDocumentAsync(ExportedMetadata documentInfo, string webApiUrl, int workspaceID, int folderId = 0, int? documentId = null)
		{
			try
			{
				ForceTapiSettings();

				ImportSettings settings = new ImportSettings()
				{
					RelativityPassword = GetBearerToken(),
					RelativityUsername = "XxX_BearerTokenCredentials_XxX",
					WebServiceURL = webApiUrl.Replace("/Relativity", "/RelativityWebAPI")
				};
				IImportAPI iapi = ImportApiFactory.Instance.GetImportAPI(settings);

				DocumentIdentifierField identityField = await GetDocumentIdentifierAsync();

				DataTable dtDocument = await GetDocumentDataTableAsync(identityField.Name);

				// upper case extension and remove period
				string extension = Path.GetExtension(documentInfo.FileName).ToUpper().Remove(0, 1),
					fileName = Path.GetFileNameWithoutExtension(documentInfo.FileName),
					fullFileName = documentInfo.FileName;

				decimal fileSize = decimal.Parse(documentInfo.Native.LongLength.ToString());

				// Add file to load
				if (await ToggleManager.Instance.GetCheckSFUFieldsAsync())
				{

					dtDocument.Rows.Add(
					!string.IsNullOrEmpty(documentInfo.ControlNumber) ? documentInfo.ControlNumber :
						(documentId.HasValue ? GetDocumentControlNumber(documentId.Value) : Path.GetFileNameWithoutExtension(documentInfo.FileName)),
					documentInfo.ExtractedText,
					extension,
					fullFileName,
					fileSize,
					documentInfo.TempFileLocation);
				}
				else
				{
					dtDocument.Rows.Add(
					!string.IsNullOrEmpty(documentInfo.ControlNumber) ? documentInfo.ControlNumber :
						(documentId.HasValue ? GetDocumentControlNumber(documentId.Value) : Path.GetFileNameWithoutExtension(documentInfo.FileName)),
					documentInfo.ExtractedText,
					extension,
					extension,
					extension,
					fullFileName,
					fileName,
					fileSize,
					fileSize,
					documentInfo.TempFileLocation);
				}
				ImportJobSettings importJobSettings = new ImportJobSettings()
				{
					WorkspaceID = workspaceID,
					FolderId = folderId,
					IdentityField = identityField,
					DocumentsDataReader = dtDocument.CreateDataReader()
				};

				IImportBulkArtifactJob importJob = ImportApiFactory.Instance.GetImportApiBulkArtifactJob(
					iapi,
					ImportJob_OnComplete,
					ImportJob_OnError,
					ImportJob_OnFatalException,
					importJobSettings);
				importJob.Execute();
			}
			catch (Exception ex)
			{
				LogError(ex);
				return new Tuple<string, string>(ex.Message, string.Empty);
			}
			finally
			{
				DeleteTempFile(documentInfo.TempFileLocation);
			}
			return new Tuple<string, string>(string.Empty, documentInfo.TempFileLocation);
		}
		private async Task ChangeFolderAsync(int folderID, int docID)
		{
			using (Services.Document.IDocumentManager docManager = _Repository.CreateProxy<Services.Document.IDocumentManager>())
			{
				await docManager.MoveDocumentsToFolderAsync(_Repository.WorkspaceID,
					new Services.Folder.FolderRef(folderID),
					new List<Services.Document.DocumentRef>()
					{
						new Services.Document.DocumentRef(docID)
					}
				);
			}
		}
		private int DefineFolder(int id)
		{
			return _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(Queries.GetDroppedFolder, SqlHelper.CreateSqlParameter("SupID", id));
		}
		private void UpdateNative(ExportedMetadata documentInfo, int docID)
		{
			FileInformation file = GetFileByArtifactId(docID);

			string workspaceRepo = GetRepositoryLocation();
			string replaceGuid = Guid.NewGuid().ToString();
			string newFileLocation = InstanceFile(replaceGuid, documentInfo.Native, workspaceRepo);

			_Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.ReplaceNativeFile, new[]
			{
				SqlHelper.CreateSqlParameter("FID", file == null ? -1 : file.FileID),
				SqlHelper.CreateSqlParameter("AID", docID),
				SqlHelper.CreateSqlParameter("RG", replaceGuid),
				SqlHelper.CreateSqlParameter("FN", Path.GetFileName(documentInfo.FileName)),
				SqlHelper.CreateSqlParameter("LOC", newFileLocation),
				SqlHelper.CreateSqlParameter("SZ", documentInfo.Native.LongLength),
				SqlHelper.CreateSqlParameter("RNT", GetNativeTypeByFilename(newFileLocation).Description)
			}
			, _timeOutValue);
		}

		private async Task<DocumentIdentifierField> GetDocumentIdentifierAsync()
		{
			QueryResult results;
			using (IObjectManager objectManager = _Repository.CreateProxy<IObjectManager>())
			{
				QueryRequest query = new QueryRequest()
				{
					ObjectType = new ObjectTypeRef() { ArtifactTypeID = (int)Client.ArtifactType.Field },
					Condition = $"'FieldCategoryID' == { (int)Client.FieldCategory.Identifier } AND 'FieldArtifactTypeID' == { (int)Client.ArtifactType.Document }",
					Fields = new[]
					{
						new FieldRef() {Name = "ArtifactID"},
						new FieldRef() {Name = "DisplayName"}
					},
					IncludeIDWindow = false,
				};
				results = await objectManager.QueryAsync(Repository.Instance.WorkspaceID, query, 1, 10);
			}

			var restricted = results.Objects[0];

			return new DocumentIdentifierField()
			{
				ArtifactId = Convert.ToInt32(restricted.FieldValues[0].Value),
				Name = restricted.FieldValues[1].Value?.ToString(),
			};
		}

		private string GetBearerToken()
		{
			string accessToken = System.Security.Claims.ClaimsPrincipal.Current.Claims?.FirstOrDefault(x => x.Type?.Equals("access_token") ?? false)?.Value ?? "";
			if (string.IsNullOrEmpty(accessToken))
			{
				accessToken = ExtensionPointServiceFinder.SystemTokenProvider?.GetLocalSystemToken();
			}
			return accessToken;
		}

		private void ImportJob_OnError(System.Collections.IDictionary row)
		{
			LogError(new FileLoadException(row["Message"].ToString()));
			throw new FileLoadException(row["Message"].ToString().Split('.')[1].Trim());
		}
		private void ImportJob_OnComplete(JobReport jobReport)
		{
			Repository.Instance.GetLogFactory().GetLogger().LogInformation($"SFU: Document upload succes: {jobReport.TotalRows} row(s) created.");
		}
		private void ImportJob_OnFatalException(JobReport jobReport)
		{
			LogError(jobReport.FatalException);
			throw jobReport.FatalException;
		}

		public void DeleteTempFile(string tempLocation)
		{
			try
			{
				string directory = Path.GetDirectoryName(tempLocation);
				if (Directory.Exists(directory))
				{
					Directory.Delete(directory, true);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}

		}

	}
}
