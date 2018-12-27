using kCura.OI.FileID;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Helpers;
using kCura.SingleFileUpload.Core.SQL;
using Newtonsoft.Json.Linq;
using NSerio.Relativity;
using Relativity.API;
using Relativity.Services.ObjectQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client = kCura.Relativity.Client;
using DTOs = kCura.Relativity.Client.DTOs;
using Services = Relativity.Services;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class DocumentManager : BaseManager, IDocumentManager
    {
        public async Task<Response> SaveSingleDocument(ExportedMetadata documentInfo, int folderID, string webApiUrl, int workspaceID, int userID)
        {
            Tuple<string, string> importResult = await ImportDocumentAsync(documentInfo, webApiUrl, workspaceID, folderID);
            if (string.IsNullOrEmpty(importResult.Item1))
            {
                await CreateMetricsAsync(documentInfo, Constants.BUCKET_DocumentsUploaded);
                return new Response() { Result = Path.GetFileNameWithoutExtension(documentInfo.FileName), Success = true };
            }
            return new Response() { Result = importResult.Item1, Success = false };
        }
        public async Task ReplaceSingleDocument(ExportedMetadata documentInfo, int docID, bool fromDocumentViewer, bool avoidControlNumber, bool isDataGrid, string webApiUrl, int workspaceID, int userID, int folderID = 0)
        {
            if (!avoidControlNumber || !fromDocumentViewer)
            {
                DTOs.Document replacedDocument = new DTOs.Document(docID);
                if (!avoidControlNumber)
                {
                    replacedDocument.TextIdentifier = Path.GetFileNameWithoutExtension(documentInfo.FileName);
                }
                if (!fromDocumentViewer)
                {
                    replacedDocument.ParentArtifact = new DTOs.Artifact(defineFolder(folderID));
                    await ChangeFolderAsync(folderID, docID);
                }
            }

            updateNative(documentInfo, docID);
            Tuple<string, string> importResult = await ImportDocumentAsync(documentInfo, webApiUrl, workspaceID, folderID, docID);
            await CreateMetricsAsync(documentInfo, Constants.BUCKET_DocumentsUploaded);
            UpdateDocumentLastModificationFields(docID, userID, false);
        }
        public bool ValidateDocImages(int docArtifactId)
        {
            bool result = false;

            DTOs.Document document = new DTOs.Document(docArtifactId);
            document.Fields.Add(new DTOs.FieldValue(DTOs.DocumentFieldNames.HasImages));
            var docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
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
            var docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
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
            var docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
            if (docResults.Success)
            {
                DTOs.Document documentArtifact = docResults.Results.FirstOrDefault().Artifact;
                result = documentArtifact.Fields[0].Value.ToString();
            }

            return result;
        }
        public int GetDocumentArtifactIdByControlNumber(string controlNumber)
        {
            var result = _Repository.CaseDBContext.ExecuteSqlStatementAsScalar(Queries.GetDocumentArtifactIdByControlNumber,
                new[] {
                    SqlHelper.CreateSqlParameter("@ControlNumber", controlNumber)
                });
            return int.Parse(result.ToString());
        }
        public bool ValidateHasRedactions(int docArtifactId)
        {
            var query = Queries.DocumentHasRedactions;
            return _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(query,
                 new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
                 }) > 0;
        }
        public void UpdateDocumentLastModificationFields(int docArtifactId, int userID, bool isNew)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.UpdateDocumentLastModificationFields,
                 new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
                    SqlHelper.CreateSqlParameter("@UserID", userID),
                    SqlHelper.CreateSqlParameter("@New", isNew),
                 });
        }
        public void DeleteRedactions(int docArtifactId)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.DeleteDocumentRedactions,
                 new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
                 });
        }
        public void DeleteExistingImages(int dArtifactId)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.DeleteDocumentImages,
                new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", dArtifactId),
                });
        }
        public void InsertImage(FileInformation image)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.InsertImageInFileTable,
                new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", image.DocumentArtifactID),
                    SqlHelper.CreateSqlParameter("@FileName", image.FileName),
                    SqlHelper.CreateSqlParameter("@Order", image.Order),
                    SqlHelper.CreateSqlParameter("@DocIdentifier", image.DocumentIdentifier),
                    SqlHelper.CreateSqlParameter("@Location", image.FileLocation),
                    SqlHelper.CreateSqlParameter("@Size", image.FileSize),
                    SqlHelper.CreateSqlParameter("@Type", image.FileType)
                });
        }
        public void UpdateHasImages(int dArtifactId)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.UpdateHasImages,
                new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", dArtifactId),
                    SqlHelper.CreateSqlParameter("@HasImagesFieldGuid", Helpers.Constants.DOCUMENT_HAS_IMAGES_FIELD),
                    SqlHelper.CreateSqlParameter("@HasImagesCodeYesGuid", Helpers.Constants.DOCUMENT_HAS_IMAGES_YES_CHOICE)
                });
        }
        public FileInformation getFileByArtifactId(int docArtifactId)
        {
            FileInformation fInformation = null;

            System.Data.Common.DbDataReader reader = _Repository.CaseDBContext.ExecuteSqlStatementAsDbDataReader(Queries.GetFileInfoByDocumentArtifactID, new[] { SqlHelper.CreateSqlParameter("@documentArtifactId", docArtifactId) });

            if (reader.HasRows)
            {
                reader.Read();
                fInformation = new FileInformation()
                {
                    FileID = reader.GetInt32(0),
                    DocumentArtifactID = reader.GetInt32(1),
                    FileName = reader.GetString(2),
                    FileLocation = reader.GetString(3)
                };
            }

            reader.Close();

            return fInformation;
        }
        public int GetDocByName(string docName)
        {
            DTOs.Query<DTOs.Document> qDocs = new DTOs.Query<DTOs.Document>();
            qDocs.Condition = new Client.TextCondition(DTOs.DocumentFieldNames.TextIdentifier, Relativity.Client.TextConditionEnum.EqualTo, docName);
            qDocs.Fields = DTOs.FieldValue.NoFields;
            return _Repository.RSAPIClient.Repositories.Document.Query(qDocs, 1).Results.FirstOrDefault()?.Artifact?.ArtifactID ?? -1;
        }
        public void SetCreateInstanceSettings()
        {
            Repository.Instance.MasterDBContext.ExecuteNonQuerySQLStatement(Queries.InsertInstanceSettings);
            var isResult = Repository.Instance.MasterDBContext.ExecuteNonQuerySQLStatement(Queries.InsertFieldsInstanceSetting);

            // Read Default fields configuration from instance setting
            if (isResult > 0)
            {
                CreateWorkspaceFieldSettings();
            }
        }
		public void RemovePageInteractionEvenHandlerFromDocumentObject()
		{
			Repository.Instance.MasterDBContext.ExecuteNonQuerySQLStatement(Queries.RemovePageInteractionEvenHandlerFromDocumentObject);
		}
		public async Task<bool> ValidateFileTypes(string extension)
        {
            ObjectQueryResultSet results;
            using (IObjectQueryManager _objectQueryManager = _Repository.CreateProxy<IObjectQueryManager>(ExecutionIdentity.System))
            {
                Query query = new Query { Fields = new[] { "Value", "Section" }, IncludeIdWindow = false, TruncateTextFields = true, Condition = $"'Name' IN ['RestrictedNativeFileTypes']" };
                results = await _objectQueryManager.QueryAsync(-1, (int)Client.ArtifactType.InstanceSetting, query, 1, int.MaxValue, new int[] { 1, 2, 3, 4, 5, 6 }, string.Empty);
            }
            var restricted = results.Data.DataResults[0].Fields[0].Value.ToString().Split(';').Select(x => x.ToLower()).ToList();
            restricted.AddRange(new[] { "dll", "exe", "js" });
            return !restricted.Contains(extension.Replace(".", ""));
        }
        public async Task<bool> IsDataGridEnabled(int workspaceID)
        {
            ObjectQueryResultSet results;
            using (IObjectQueryManager _objectQueryManager = _Repository.CreateProxy<IObjectQueryManager>(ExecutionIdentity.System))
            {
                Query query = new Query { Fields = new[] { DTOs.WorkspaceFieldNames.EnableDataGrid }, IncludeIdWindow = false, TruncateTextFields = true, Condition = $"'ArtifactID' IN [{workspaceID}]" };
                results = await _objectQueryManager.QueryAsync(-1, (int)Client.ArtifactType.Case, query, 1, int.MaxValue, new int[] { 1, 2, 3, 4, 5, 6 }, string.Empty);
            }
            var isDataGrid = (bool)results.Data.DataResults[0].Fields[0].Value;
            return isDataGrid;

        }
        public async Task<DataTable> GetDocumentDataTableAsync(string identifierName)
        {
            DataTable DocumentsDataTable = new DataTable();

            DocumentsDataTable.Columns.Add(identifierName, typeof(string));
            DocumentsDataTable.Columns.Add("Extracted Text", typeof(string));


            if (await ToggleManager.Instance.GetCheckSFUFieldsAsync())
            {
                var wsResult = Repository.Instance.CaseDBContext.ExecuteSqlStatementAsScalar(Queries.GetFieldsWorspaceSetting);
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
                        throw new Exception("Fields Settings does not exist.");
                    }
                }

                foreach (var wsItem in wsFields)
                {
                    DocumentsDataTable.Columns.Add(wsItem.Value.ToString(), typeof(string));
                }
            }
            else
            {
                DocumentsDataTable.Columns.Add("Document Extension", typeof(string));
                DocumentsDataTable.Columns.Add("File Extension", typeof(string));
                DocumentsDataTable.Columns.Add("FileExtension", typeof(string));
                DocumentsDataTable.Columns.Add("File Name", typeof(string));
                DocumentsDataTable.Columns.Add("FileName", typeof(string));
                DocumentsDataTable.Columns.Add("File Size");
                DocumentsDataTable.Columns.Add("FileSize");
            }

            DocumentsDataTable.Columns.Add("Native File", typeof(string));
            return DocumentsDataTable;
        }
        public void WriteFile(byte[] file, FileInformation document)
        {
            File.WriteAllBytes(document.FileLocation, file);
        }
        public string GetRepositoryLocation()
        {
            var location = _Repository.MasterDBContext.ExecuteSqlStatementAsScalar<string>(Queries.GetRepoLocationByCaseID, new[] { SqlHelper.CreateSqlParameter("AID", _Repository.WorkspaceID) });
            return !location.EndsWith("\\") ? string.Concat(location, "\\") : location;
        }
        public async Task CreateMetricsAsync(ExportedMetadata documentInfo, string bucket)
        {
            if (!string.IsNullOrEmpty(bucket))
            {
                ITelemetryManager telManager = new TelemetryManager();
                await telManager.LogCountAsync(bucket, 1L);
				await telManager.LogCountAsync(Helpers.Constants.BUCKET_TotalSizeDocumentUploaded, documentInfo.Native.LongLength);
				//Create File tipe metric
				await telManager.CreateMetricAsync(string.Concat(Helpers.Constants.BUCKET_FileType, Path.GetExtension(documentInfo.FileName)), $"Number of {Path.GetExtension(documentInfo.FileName).Remove(0, 1)} uploaded");
				await telManager.LogCountAsync(string.Concat(Helpers.Constants.BUCKET_FileType, Path.GetExtension(documentInfo.FileName)), 1L);
            }
        }
        public string instanceFile(string fileName, byte[] fileBytes, bool isTemp, string baseRepo = null)
        {
            string folder = Path.Combine(baseRepo ?? Path.GetTempPath(), $"RV_{Guid.NewGuid()}");
            Directory.CreateDirectory(folder);
            string tmpFileName = Path.Combine(folder, isTemp ? $"{Guid.NewGuid()}" + fileName : fileName);
            File.WriteAllBytes(tmpFileName, fileBytes);
            return tmpFileName;
        }
        public FileIDData GetNativeTypeByFilename(string fileName)
        {
            return Manager.Instance.GetFileIDDataByFilePath(fileName);
        }
        private void CreateWorkspaceFieldSettings()
        {

            var fieldNames = Repository.Instance.MasterDBContext.ExecuteSqlStatementAsScalar(Queries.GetFieldsInstanceSetting).ToString();

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

                var wsValue = builder.ToString();
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
        private void forceTapiSettings()
        {
            setWinEDDSSetting(Constants.TAPI_FORCE_WEB_UPLOAD);
            setWinEDDSSetting(Constants.TAPI_FORCE_HTTP_CLIENT);
            setWinEDDSSetting(Constants.TAPI_FORCE_BCP_HTTP_CLIENT);
        }
        private void setWinEDDSSetting(string key, string value = "True")
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
                forceTapiSettings();
                string value = getBearerToken(webApiUrl);
                webApiUrl = webApiUrl.Replace("/Relativity", "/RelativityWebAPI");                
                ImportAPI iapi = new ExtendedImportAPI("XxX_BearerTokenCredentials_XxX", value, webApiUrl);
                var importJob = iapi.NewNativeDocumentImportJob();

                importJob.Settings.CaseArtifactId = workspaceID;
                importJob.Settings.ExtractedTextFieldContainsFilePath = false;
                importJob.Settings.DisableExtractedTextEncodingCheck = true;
                importJob.Settings.DisableExtractedTextFileLocationValidation = true;
                importJob.Settings.DisableNativeLocationValidation = true;
                importJob.Settings.DisableNativeValidation = false;
                importJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
                importJob.OnComplete += ImportJob_OnComplete;
                importJob.OnError += ImportJob_OnError;
                importJob.OnFatalException += ImportJob_OnFatalException;

                if (folderId != 0)
                {
                    importJob.Settings.DestinationFolderArtifactID = folderId;
                }

                var IdentityField = await GetDocumentIdentifierAsync();

                importJob.Settings.SelectedIdentifierFieldName = IdentityField.Name;
                importJob.Settings.NativeFilePathSourceFieldName = "Native File";
                importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
                importJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
                // Specify the ArtifactID of the document identifier field, such as a control number.
                importJob.Settings.IdentityFieldId = IdentityField.ArtifactId;

                DataTable dtDocument = await GetDocumentDataTableAsync(IdentityField.Name);

                // upper case extension and remove period
                var extension = Path.GetExtension(documentInfo.FileName).ToUpper().Remove(0, 1);
                var fileName = Path.GetFileNameWithoutExtension(documentInfo.FileName);
                var fullFileName = documentInfo.FileName;
                var fileSize = decimal.Parse(documentInfo.Native.LongLength.ToString());

                // Add file to load
                if (await ToggleManager.Instance.GetCheckSFUFieldsAsync())
                {


                    dtDocument.Rows.Add(
                    !string.IsNullOrEmpty(documentInfo.ControlNumber) ? documentInfo.ControlNumber : (documentId.HasValue ? GetDocumentControlNumber(documentId.Value) : Path.GetFileNameWithoutExtension(documentInfo.FileName)),
                    documentInfo.ExtractedText,
                    extension,
                    fullFileName,
                    fileSize,
                    documentInfo.TempFileLocation);
                }
                else
                {
                    dtDocument.Rows.Add(
                    !string.IsNullOrEmpty(documentInfo.ControlNumber) ? documentInfo.ControlNumber : (documentId.HasValue ? GetDocumentControlNumber(documentId.Value) : Path.GetFileNameWithoutExtension(documentInfo.FileName)),
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

                importJob.SourceData.SourceData = dtDocument.CreateDataReader();

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
                    new Services.Folder.FolderRef(folderID), new List<Services.Document.DocumentRef>() {
                        new Services.Document.DocumentRef(docID)
                    });
            }
        }
        private int getFieldIDByNameAndType(Client.FieldType type, params string[] fNames)
        {
            int id = 0;
            if (fNames.Length > 0)
            {
                id = _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(string.Format(Queries.GetFieldIDByNameAndType, string.Join(",", fNames.Select(p => $"'{p}'"))), SqlHelper.CreateSqlParameter("Type", (int)type));
            }
            return id;
        }
        private void addFieldToNewDocument(ExportedMetadata documentInfo, DTOs.Document newDocument, string fieldName, Client.FieldType type, params string[] possibleMatch)
        {
            if (documentInfo.Fields.ContainsKey(fieldName))
            {
                int fid = getFieldIDByNameAndType(type, possibleMatch);
                if (fid > 0)
                {
                    newDocument.Fields.Add(new DTOs.FieldValue(fid, documentInfo.Fields[fieldName], false));
                }
            }
        }
        private int getDocumentFieldByCategory(Client.FieldCategory category)
        {
            return _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(Queries.GetDocumentIdentifierField, SqlHelper.CreateSqlParameter("CATEGORYID", (int)category));
        }
        private int defineFolder(int id)
        {
            return _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(Queries.GetDroppedFolder, SqlHelper.CreateSqlParameter("SupID", id));
        }
        private void updateNative(ExportedMetadata documentInfo, int docID)
        {
            var file = getFileByArtifactId(docID);

            var workspaceRepo = GetRepositoryLocation();
            var replaceGuid = Guid.NewGuid().ToString();
            var newFileLocation = instanceFile(replaceGuid, documentInfo.Native, false, workspaceRepo);
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.ReplaceNativeFile, new[]
            {
                SqlHelper.CreateSqlParameter("FID", file == null ? -1 : file.FileID),
                SqlHelper.CreateSqlParameter("AID", docID),
                SqlHelper.CreateSqlParameter("RG", replaceGuid),
                SqlHelper.CreateSqlParameter("FN", Path.GetFileName(documentInfo.FileName)),
                SqlHelper.CreateSqlParameter("LOC", newFileLocation),
                SqlHelper.CreateSqlParameter("SZ", documentInfo.Native.LongLength),
                SqlHelper.CreateSqlParameter("RNT", GetNativeTypeByFilename(newFileLocation).FileType)
            }, 300);
        }
        private void updateMatchedField(ExportedMetadata documentInfo, int docID)
        {
            var matched = _Repository.CaseDBContext.ExecuteSqlStatementAsDataTable(string.Format(Queries.GetMatchedFields, "'File Name','Document Extension','File Size'")).Rows.Cast<DataRow>().Select(p => p[0] as string).ToArray();
            if (matched.Length > 0)
            {
                string setString = string.Join(",", matched.Select(p => $"{p} = @{p}"));
                _Repository.CaseDBContext.ExecuteNonQuerySQLStatement($"UPDATE EDDSDBO.[Document] SET { setString } WHERE ArtifactID = @AID", new[]
                {
                    SqlHelper.CreateSqlParameter("AID", docID),
                    SqlHelper.CreateSqlParameter("FileName", Path.GetFileName(documentInfo.FileName)),
                    SqlHelper.CreateSqlParameter("FileSize", documentInfo.Native.LongLength),
                    SqlHelper.CreateSqlParameter("DocumentExtension", Path.GetExtension(documentInfo.FileName))
                }, 300);
            }
        }
        private async Task<DocumentIdentifierField> GetDocumentIdentifierAsync()
        {
            ObjectQueryResultSet results;
            using (IObjectQueryManager _objectQueryManager = _Repository.CreateProxy<IObjectQueryManager>())
            {
                Query query = new Query()
                {
                    Condition = $"'FieldCategoryID' == 2 AND 'FieldArtifactTypeID' == 10",
                    Fields = new string[] { "ArtifactID", "DisplayName" },
                    IncludeIdWindow = false,
                    RelationalField = null,
                    SampleParameters = null,
                    SearchProviderCondition = null,
                    Sorts = new string[] { },
                    TruncateTextFields = false
                };
                results = await _objectQueryManager.QueryAsync(Repository.Instance.WorkspaceID, 14, query, 1, int.MaxValue, new int[] { 1, 2, 3, 4, 5, 6 }, string.Empty);
            }

            var restricted = results.Data.DataResults[0];

            return new DocumentIdentifierField()
            {
                ArtifactId = Convert.ToInt32(restricted.Fields[0].Value),
                Name = restricted.Fields[1].Value?.ToString(),
            };
        }
        private string getBearerToken(string instanceUrl)
        {
            string token = ExtensionPointServiceFinder.SystemTokenProvider.GetLocalSystemToken();
            return token;
        }
        private KeyValuePair<int, string> GetFieldinfo(string artifactGuid)
        {
            KeyValuePair<int, string> fieldData = default(KeyValuePair<int, string>);
            System.Data.Common.DbDataReader reader = _Repository.CaseDBContext.ExecuteSqlStatementAsDbDataReader(Queries.GetFieldInfoByGuid, new[] { SqlHelper.CreateSqlParameter("@artifactGuid", artifactGuid) });

            if (reader.HasRows)
            {
                reader.Read();
                fieldData = new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1));
            }

            reader.Close();
            return fieldData;
        }
        private Tuple<string, string> GetClientCredentials()
        {
            Tuple<string, string> clientData = default(Tuple<string, string>);
            System.Data.Common.DbDataReader reader = _Repository.MasterDBContext.ExecuteSqlStatementAsDbDataReader(Queries.GetClientCredentials);

            if (reader.HasRows)
            {
                reader.Read();
                clientData = new Tuple<string, string>(reader.GetString(0), reader.GetString(1));
            }

            reader.Close();
            return clientData;
        }
        private void ImportJob_OnError(System.Collections.IDictionary row)
        {
            LogError(new Exception(row["Message"].ToString()));
            throw new Exception(row["Message"].ToString().Split('.')[1].Trim());
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

        private void DeleteTempFile(string tempLocation)
        {
            try
            {
                var directory = Path.GetDirectoryName(tempLocation);
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
            catch (Exception ex) {
                LogError(ex);
            }

        }

    }
}
