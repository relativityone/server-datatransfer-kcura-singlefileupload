using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Entities.Enumerations;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using Relativity.CustomPages;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Relativity.OIFactory;

namespace kCura.SingleFileUpload.MVC.Controllers
{
	public class SFUController : BaseController
	{
		private IAuditManager __repositoryAuditManager;
		private IProcessingManager __repositoryProcessingManager;
		private ISearchExportManager __repositorySearchManager;

		protected ISearchExportManager _RepositorySearchManager
		{
			get
			{
				if (__repositorySearchManager == null)
				{
					__repositorySearchManager = new SearchExportManager();
				}

				return __repositorySearchManager;
			}
		}

		protected IProcessingManager _RepositoryProcessingManager
		{
			get
			{
				if (__repositoryProcessingManager == null)
				{
					__repositoryProcessingManager = new ProcessingManager();
				}

				return __repositoryProcessingManager;
			}
		}

		protected IAuditManager _RepositoryAuditManager
		{
			get
			{
				if (__repositoryAuditManager == null)
				{
					__repositoryAuditManager = new AuditManager(ConnectionHelper.Helper());
				}

				return __repositoryAuditManager;
			}
		}


		public async Task<ActionResult> Index(bool fdv = false, int errorFile = 0, int docId = 0, bool image = false, bool newImage = false, int profileID = 0)
		{
			ViewBag.AppID = WorkspaceID;
			ViewBag.FDV = fdv.ToString().ToLower();
			ViewBag.ErrorID = errorFile;
			ViewBag.DocID = docId;
			ViewBag.ChangeImage = image.ToString().ToLower();
			ViewBag.NewImage = newImage.ToString().ToLower();
			ViewBag.HasRedactions = _RepositoryDocumentManager.ValidateHasRedactions(docId).ToString().ToLower();
			ViewBag.HasImages = docId == 0 ? "false" : _RepositoryDocumentManager.ValidateDocImages(docId).ToString().ToLower();
			ViewBag.HasNative = docId == 0 ? "false" : _RepositoryDocumentManager.ValidateDocNative(docId).ToString().ToLower();
			ViewBag.ProfileID = profileID;
			ViewBag.UploadMassiveDocuments = await ToggleManager.Instance.GetCheckUploadMassiveAsync();
			ViewBag.MaxFilesToUpload = await InstanceSettingManager.Instance.GetMaxFilesInstanceSettingAsync();
			return View();
		}

		[HttpPost]
		public async Task Upload(int fid = 0, int did = 0, bool fdv = false, 
			bool force = false, bool img = false, 
			bool newImage = false, string controlNumberText = null)
		{
			Models.ResponseWithElements<string> result = await HandleResponseDynamicResponseAsync<string>(async (response) =>
			{
				bool isAdmin = PermissionsManager.Instance.IsUserAdministrator(WorkspaceID, RelativityUserInfo.ArtifactID);
				string resultStr = string.Empty;
				try
				{
					if (!isAdmin)
					{
						bool hasPermission = false;
						if (img)
						{
							bool hasUploadPermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID,
								Core.Helpers.Constants.DocumentObjectType, Core.Helpers.Constants.PermissionReplaceImageUploadDownload);
							bool hasAddPermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID, 
								Core.Helpers.Constants.DocumentObjectType, Core.Helpers.Constants.PermissionAddImage);
							bool hasdeletePermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID, 
								Core.Helpers.Constants.DocumentObjectType, Core.Helpers.Constants.PermissionDeleteImage);
							hasPermission = hasUploadPermission && hasAddPermission && hasdeletePermission;
						}
						else
						{
							if (fdv)
							{
								hasPermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID, Core.Helpers.Constants.DocumentObjectType, Core.Helpers.Constants.PermissionReplaceDocument);
							}
							else
							{
								hasPermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID, 
									Core.Helpers.Constants.DocumentObjectType, Core.Helpers.Constants.ADD_DOCUMENT_CUSTOM_PERMISSION);
							}
						}

						if (!hasPermission)
						{
							response.Success = false;
							response.Message = "You do not have enough permissions to perform the current action.";
							return resultStr;
						}
					}

					HttpPostedFileBase file = Request.Files[0];
					string fileName = file.FileName;
					if (fileName.Contains("\\"))
					{
						fileName = Path.GetFileName(fileName);
					}
					string fileExt = Path.GetExtension(fileName).ToLower();
					bool res = await _RepositoryDocumentManager.ValidateFileTypes(fileExt);
					
					if (!res)
					{
						response.Success = false;
						response.Message = img ? "Loaded file is not a supported format. Please select TIFF, JPEG or PDF." : "This file type is not supported.";
					}
					else
					{
						
							var isDataGrid = await _RepositoryDocumentManager.IsDataGridEnabled(WorkspaceID);
							var documentName = string.IsNullOrEmpty(controlNumberText) ? Path.GetFileNameWithoutExtension(fileName) : controlNumberText;
							var docIDByName = _RepositoryDocumentManager.GetDocByName(documentName);
							if (!fdv)
							{
								did = docIDByName;
								if (did == -1 || force)
								{
									var transientMetadata = getTransient(file, fileName);
									transientMetadata.TempFileLocation = _RepositoryDocumentManager.instanceFile(transientMetadata.FileName, transientMetadata.Native, false);
									if (ValidateFile(transientMetadata.TempFileLocation))
									{
										response.Success = false;
										response.Message = "This file is not supported.";
										_RepositoryDocumentManager.DeleteTempFile(transientMetadata.TempFileLocation);
										return resultStr;
									}
									if (!string.IsNullOrEmpty(controlNumberText))
									{
										transientMetadata.ControlNumber = controlNumberText;
									}
									if (did == -1)
									{
										var resultUpload = await _RepositoryDocumentManager.SaveSingleDocument(transientMetadata, fid, GetWebAPIURL(), WorkspaceID, this.RelativityUserInfo.WorkspaceUserArtifactID);
										if (resultUpload.Success)
										{
											resultStr = string.IsNullOrEmpty(controlNumberText) ? resultUpload.Result : controlNumberText;
										}
										else
										{
											response.Success = false;
											response.Message = resultUpload.Result;
											return resultStr;
										}
									}
								}
								else
								{
									response.Success = false;
									response.Message = "The Control Number you selected is being used in another document, please select a different one.";
									return resultStr;
								}
							}
							else
							{
								if (img)
								{
									if (!fileExt.Equals(".tif") && !fileExt.Equals(".tiff") && !fileExt.Equals(".jpeg") && !fileExt.Equals(".jpg") && !fileExt.Equals(".pdf"))
									{
										response.Success = false;
										response.Message = "Loaded file is not a supported format. Please select TIFF, JPEG or PDF File.";
									}
									else
									{
										FileInformation fileInfo = _RepositoryDocumentManager.getFileByArtifactId(did);
										var transientMetadata = getTransient(file, fileName);
										transientMetadata.TempFileLocation = _RepositoryDocumentManager.instanceFile(transientMetadata.FileName, transientMetadata.Native, false);
										if (ValidateFile(transientMetadata.TempFileLocation))
										{
											response.Success = false;
											response.Message = "This file is not supported.";
											_RepositoryDocumentManager.DeleteTempFile(transientMetadata.TempFileLocation);
											return resultStr;
										}
										FileInformation imageInfo = fileInfo;

									if (fileInfo == null)
									{
										imageInfo = new FileInformation();
									}

									string guidFileName = $"{Guid.NewGuid().ToString().ToLower()}{fileExt}";
									string location = $@"{_RepositoryDocumentManager.GetRepositoryLocation()}EDDS{WorkspaceID}\Temp\";
									if (!Directory.Exists(location))
									{
										Directory.CreateDirectory(location);
									}

									imageInfo.FileName = $"{guidFileName}";
									imageInfo.FileSize = transientMetadata.Native.Length;
									imageInfo.FileType = 1;
									imageInfo.Order = 0;
									imageInfo.FileLocation = string.Concat(location, guidFileName);
									_RepositoryDocumentManager.WriteFile(transientMetadata.Native, imageInfo);

									string details = _RepositoryAuditManager.GenerateAuditDetailsForFileUpload(imageInfo.FileLocation, imageInfo.FileID, "Images Replaced");
									_RepositoryAuditManager.CreateAuditRecord(WorkspaceID, did, AuditAction.File_Upload, details, RelativityUserInfo.AuditWorkspaceUserArtifactID);
									response.Success = true;
									response.Message = imageInfo.FileLocation;
								}

								}
								else
								{
									if (did != docIDByName && docIDByName > 0)
									{
										response.Success = false;
										response.Message = "A document with the same name already exists.";
									}
									else
									{
										var transientMetadata = getTransient(file, fileName);
										transientMetadata.TempFileLocation = _RepositoryDocumentManager.instanceFile(transientMetadata.FileName, transientMetadata.Native, false);
										if (ValidateFile(transientMetadata.TempFileLocation))
										{
											response.Success = false;
											response.Message = "This file is not supported.";
											_RepositoryDocumentManager.DeleteTempFile(transientMetadata.TempFileLocation);
											return resultStr;
										}
										await _RepositoryDocumentManager.ReplaceSingleDocument(transientMetadata, did, true, docIDByName == did, isDataGrid, GetWebAPIURL(), WorkspaceID, this.RelativityUserInfo.WorkspaceUserArtifactID);
										var details = _RepositoryAuditManager.GenerateAuditDetailsForFileUpload(string.Empty, did, "Document Replacement");
										_RepositoryAuditManager.CreateAuditRecord(WorkspaceID, did, AuditAction.File_Upload, details, RelativityUserInfo.AuditWorkspaceUserArtifactID);
										_RepositoryAuditManager.CreateAuditRecord(WorkspaceID, did, AuditAction.Update, details, RelativityUserInfo.AuditWorkspaceUserArtifactID);
									}
								}
							}
							if (string.IsNullOrEmpty(resultStr))
							{
								if (response.Success && !img)
								{
									resultStr = $"AppID={WorkspaceID}&ArtifactID={did}";
								}
								else
								{
									resultStr = did.ToString();
								}
							}
					
					}
				}
				catch (Exception ex)
				{
					response.Success = false;
					response.Message = ex.Message;
				}

				return resultStr;
			});
			Response.Clear();
			Response.ClearContent();
			result.Data = result.Data.Replace("'", "/39/").Replace("\"", "/34/");
			Response.Write($"<script>sessionStorage['____pushNo'] = '{Newtonsoft.Json.JsonConvert.SerializeObject(result).Replace("'", "\"")}'</script>");
			Response.End();
		}

		[HttpPost]
		public int checkUploadStatus(string documentName)
		{
			int documentID = _RepositoryDocumentManager.GetDocByName(documentName);
			if (documentID != -1)
			{
				_RepositoryDocumentManager.UpdateDocumentLastModificationFields(documentID, RelativityUserInfo.WorkspaceUserArtifactID, true);
			}
			return documentID;
		}

		[HttpPost]
		public async Task UploadProcessingError(int errorID)
		{
			var result = await HandleResponseDynamicResponseAsync<string>(async (response) =>
			{
				string resultStr = string.Empty;
				var isAdmin = PermissionsManager.Instance.IsUserAdministrator(WorkspaceID, RelativityUserInfo.ArtifactID);
				var hasPermission = !isAdmin ? await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID, Core.Helpers.Constants.ProcessingErrorObjectType, Core.Helpers.Constants.PermissionProcessingErrorUploadDownload) : true;
				var isDataGrid = await _RepositoryDocumentManager.IsDataGridEnabled(WorkspaceID);
				if (hasPermission)
				{
					var error = _RepositoryProcessingManager.GetErrorInfo(errorID);
					var file = Request.Files[0];
					string fileName = file.FileName;

					if (Path.GetExtension(fileName).ToLower() != Path.GetExtension(error.DocumentFileLocation).ToLower())
					{
						response.Success = false;
						response.Message = "The file must be of the same type.";
						return resultStr;
					}
					var transientMetadata = getTransient(file, fileName);
					_RepositoryProcessingManager.ReplaceFile(transientMetadata.Native, error);

					var details = _RepositoryAuditManager.GenerateAuditDetailsForFileUpload(error.DocumentFileLocation, 0, "Processing Error File Replacement");
					_RepositoryAuditManager.CreateAuditRecord(WorkspaceID, error.ErrorID, AuditAction.File_Upload, details, this.RelativityUserInfo.AuditWorkspaceUserArtifactID);
					return resultStr;
				}
				else
				{
					response.Success = false;
					response.Message = "You do not have enough permissions to perform the current action.";
					return resultStr;
				}
			});
			Response.Clear();
			Response.ClearContent();
			Response.Write($"<script>sessionStorage['____pushNo'] = '{Newtonsoft.Json.JsonConvert.SerializeObject(result)}'</script>");
			Response.End();
		}

		[HttpPost]
		public JsonResult CheckForImages(int tArtifactId)
		{
			var result = HandleResponse<string>((response) =>
			{
				string resultStr = string.Empty;
				response.Success = true;
				resultStr = _RepositoryDocumentManager.ValidateDocImages(tArtifactId).ToString();
				return resultStr;
			});

			return Json(result);
		}

		[HttpPost]
		public JsonResult GetRepLocation()
		{
			var result = HandleResponse<string>((response) =>
			{
				response.Success = true;
				return $@"{_RepositoryDocumentManager.GetRepositoryLocation()}EDDS{WorkspaceID}\Temp";
			});

			return Json(result);
		}

		private ExportedMetadata getTransient(HttpPostedFileBase file, string fileName)
		{
			ExportedMetadata transientMetadata = new ExportedMetadata();
			var stream = file.InputStream;
			var native = new byte[stream.Length];
			stream.Read(native, 0, checked((int)stream.Length));
			try
			{
				_RepositorySearchManager.ConfigureOutsideIn();
				transientMetadata = _RepositorySearchManager.ExportToSearchML(fileName, native, () => ConnectionHelper.Helper().BuildExporter());
			}
			catch (Exception ex)
			{
				_RepositoryDocumentManager.LogError(ex);
				transientMetadata.Native = native;
				transientMetadata.FileName = fileName;
				transientMetadata.ExtractedText = string.Empty;
			}
			return transientMetadata;
		}

		private string GetWebAPIURL()
		{
			var url = ConnectionHelper.Helper().GetServicesManager().GetRESTServiceUrl().ToString();
			return url.ToString().ToLower().Replace("relativity.rest/api", "Relativity");
		}

		private bool ValidateFile(string tempFile)
		{
			var fileType = _RepositoryDocumentManager.GetNativeTypeByFilename(tempFile);
			int[] finder = new int[] 
			{
			1800, //"EXE / DLL File"
			1101, //"Internet HTML"
			};
			return finder.Contains(fileType.FileID);
		}
	}
}