﻿using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Entities.Enumerations;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.MVC.Models;
using Newtonsoft.Json.Linq;
using Relativity.CustomPages;
using Relativity.DataExchange.Io;
using Relativity.OIFactory;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Response = kCura.SingleFileUpload.Core.Entities.Response;

namespace kCura.SingleFileUpload.MVC.Controllers
{
	public class SFUController : BaseController
	{

		private int[] _finder = new int[]
		{
		1800, //"EXE / DLL File"
		1101, //"Internet HTML"
		};

		public async Task<ActionResult> Index(string parameters = "")
		{
			Imaging imaging = new Imaging();

			if (string.IsNullOrEmpty(parameters))
			{
				imaging.Fdv = false;
				imaging.ErrorFile = 0;
				imaging.DocID = 0;
				imaging.Image = false;
				imaging.NewImage = false;
				imaging.ProfileID = 0;
			}
			else
			{
				JObject jObject = JObject.Parse(parameters);
				imaging.Fdv = !string.IsNullOrEmpty(jObject["fdv"].ToString()) ? (bool)jObject["fdv"] : default(bool); 
				imaging.DocID = !string.IsNullOrEmpty(jObject["docID"].ToString()) ? (int)jObject["docID"] : default(int); 
				imaging.Image = !string.IsNullOrEmpty(jObject["image"].ToString()) ? (bool)jObject["image"] : default(bool);
				imaging.NewImage = !string.IsNullOrEmpty(jObject["newImage"].ToString()) ? (bool)jObject["newImage"] : default(bool);
				imaging.ProfileID = !string.IsNullOrEmpty(jObject["profileID"].ToString()) ? (int)jObject["profileID"] : default(int);
				imaging.ErrorFile = !string.IsNullOrEmpty(jObject["errorFile"].ToString()) ? (int)jObject["errorFile"] : default(int);
			}

			ViewBag.AppID = WorkspaceID;
			ViewBag.FDV = imaging.Fdv.ToString().ToLower();
			ViewBag.ErrorID = imaging.ErrorFile;
			ViewBag.DocID = imaging.DocID;
			ViewBag.ChangeImage = imaging.Image.ToString().ToLower();
			ViewBag.NewImage = imaging.NewImage.ToString().ToLower();
			ViewBag.HasRedactions = DocumentManager.Instance.ValidateHasRedactions(imaging.DocID).ToString().ToLower();
			ViewBag.HasImages = imaging.DocID == 0 ? "false" : DocumentManager.Instance.ValidateDocImages(imaging.DocID).ToString().ToLower();
			ViewBag.HasNative = imaging.DocID == 0 ? "false" : DocumentManager.Instance.ValidateDocNative(imaging.DocID).ToString().ToLower();
			ViewBag.ProfileID = imaging.ProfileID;
			ViewBag.UploadMassiveDocuments = await ToggleManager.Instance.GetCheckUploadMassiveAsync();
			ViewBag.MaxFilesToUpload = await InstanceSettingManager.Instance.GetMaxFilesInstanceSettingAsync();
			return View();
		}

		private async Task<bool> ValidatePermissionAsync(bool img, bool fdv)
		{
			bool hasPermission = false;
			if (img)
			{
				bool hasUploadPermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID,
					Core.Helpers.Constants.DOCUMENTOBJECTTYPE, Core.Helpers.Constants.PERMISSIONREPLACEIMAGEUPLOADDOWNLOAD);
				bool hasAddPermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID,
					Core.Helpers.Constants.DOCUMENTOBJECTTYPE, Core.Helpers.Constants.PERMISSIONADDIMAGE);
				bool hasdeletePermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID,
					Core.Helpers.Constants.DOCUMENTOBJECTTYPE, Core.Helpers.Constants.PERMISSIONDELETEIMAGE);
				hasPermission = hasUploadPermission && hasAddPermission && hasdeletePermission;
			}
			else
			{
				if (fdv)
				{
					hasPermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID, Core.Helpers.Constants.DOCUMENTOBJECTTYPE, Core.Helpers.Constants.PERMISSIONREPLACEDOCUMENT);
				}
				else
				{
					hasPermission = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(this.WorkspaceID,
						Core.Helpers.Constants.DOCUMENTOBJECTTYPE, Core.Helpers.Constants.ADD_DOCUMENT_CUSTOM_PERMISSION);
				}
			}

			return hasPermission;
		}

		[HttpPost]
		public async Task Upload(MetaUploadFile meta, bool img = false, string controlNumberText = null)
		{
			ResponseWithElements<string> result = await HandleResponseDynamicResponseAsync<string>(async (response) =>
			{
				bool isAdmin = PermissionsManager.Instance.IsUserAdministrator(WorkspaceID, RelativityUserInfo.ArtifactID);
				string resultStr = string.Empty;
				try
				{
					if (!isAdmin)
					{
						bool hasPermission = await ValidatePermissionAsync(img, meta.fdv);

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
					bool res = await DocumentManager.Instance.ValidateFileTypes(fileExt);

					if (!res)
					{
						response.Success = false;
						response.Message = img ? "Loaded file is not a supported format. Please select TIFF, JPEG or PDF." : "This file type is not supported.";
					}
					else
					{
						bool isDataGrid = await DocumentManager.Instance.IsDataGridEnabled(WorkspaceID);
						string documentName = string.IsNullOrEmpty(controlNumberText) ? Path.GetFileNameWithoutExtension(fileName) : controlNumberText;
						int docIDByName = DocumentManager.Instance.GetDocByName(documentName);
						if (!meta.fdv)
						{
							meta.did = docIDByName;
							if (meta.did == -1 || meta.force)
							{
								ExportedMetadata transientMetadata = GetTransient(file, fileName);
								transientMetadata.TempFileLocation = DocumentManager.Instance.InstanceFile(transientMetadata.FileName, transientMetadata.Native, false);

								if (ValidateFile(transientMetadata.TempFileLocation))
								{
									response.Success = false;
									response.Message = "This file type is unsupported";
									DocumentManager.Instance.DeleteTempFile(transientMetadata.TempFileLocation);
									return resultStr;
								}
								if (!string.IsNullOrEmpty(controlNumberText))
								{
									transientMetadata.ControlNumber = controlNumberText;
								}
								if (meta.did == -1)
								{
									Response resultUpload = await DocumentManager.Instance.SaveSingleDocument(transientMetadata, meta.fid, GetWebAPIURL(), WorkspaceID,
										this.RelativityUserInfo.WorkspaceUserArtifactID);
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
								response.Message = "The Control Number you selected is already in use. Try again.";
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
									FileInformation fileInfo = DocumentManager.Instance.GetFileByArtifactId(meta.did);
									ExportedMetadata transientMetadata = GetTransient(file, fileName);
									transientMetadata.TempFileLocation = DocumentManager.Instance.InstanceFile(transientMetadata.FileName, transientMetadata.Native, false);
									if (ValidateFile(transientMetadata.TempFileLocation))
									{
										response.Success = false;
										response.Message = "This file type is unsupported";
										DocumentManager.Instance.DeleteTempFile(transientMetadata.TempFileLocation);
										Directory.Delete(Path.GetDirectoryName(transientMetadata.TempFileLocation), true);
										return resultStr;
									}
									FileInformation imageInfo = fileInfo;

									if (fileInfo == null)
									{
										imageInfo = new FileInformation();
									}

									string guidFileName = $"{Guid.NewGuid().ToString().ToLower()}{fileExt}";
									string location = $@"{DocumentManager.Instance.GetRepositoryLocation()}EDDS{WorkspaceID}\Temp\";
									if (!Directory.Exists(location))
									{
										Directory.CreateDirectory(location);
									}

									imageInfo.FileName = $"{guidFileName}";
									imageInfo.FileSize = transientMetadata.Native.Length;
									imageInfo.FileType = 1;
									imageInfo.Order = 0;
									imageInfo.FileLocation = string.Concat(location, guidFileName);
									DocumentManager.Instance.WriteFile(transientMetadata.Native, imageInfo);

									string details = AuditManager.instance.GenerateAuditDetailsForFileUpload(imageInfo.FileLocation, imageInfo.FileID, "Images Replaced");
									AuditManager.instance.CreateAuditRecord(WorkspaceID, meta.did, AuditAction.File_Upload, details, RelativityUserInfo.AuditWorkspaceUserArtifactID);
									response.Success = true;
									response.Message = imageInfo.FileLocation;
								}

							}
							else
							{
								if (meta.did != docIDByName && docIDByName > 0)
								{
									response.Success = false;
									response.Message = "A document with the same name already exists.";
								}
								else
								{
									ExportedMetadata transientMetadata = GetTransient(file, fileName);
									transientMetadata.TempFileLocation = DocumentManager.Instance.InstanceFile(transientMetadata.FileName, transientMetadata.Native, false);
									if (ValidateFile(transientMetadata.TempFileLocation))
									{
										response.Success = false;
										response.Message = "This file type is unsupported";
										DocumentManager.Instance.DeleteTempFile(transientMetadata.TempFileLocation);
										return resultStr;
									}
									DocumentExtraInfo documentExtraInfo = new DocumentExtraInfo
									{
										DocID = meta.did,
										FromDocumentViewer = true,
										AvoidControlNumber = docIDByName == meta.did,
										IsDataGrid = isDataGrid,
										WebApiUrl = GetWebAPIURL(),
										WorkspaceID = WorkspaceID,
										UserID = this.RelativityUserInfo.WorkspaceUserArtifactID,
										FolderID = 0

									};

									await DocumentManager.Instance.ReplaceSingleDocument(transientMetadata, documentExtraInfo);
									string details = AuditManager.instance.GenerateAuditDetailsForFileUpload(string.Empty, meta.did, "Document Replacement");
									AuditManager.instance.CreateAuditRecord(WorkspaceID, meta.did, AuditAction.Update, details, RelativityUserInfo.AuditWorkspaceUserArtifactID);
									AuditManager.instance.CreateAuditRecord(WorkspaceID, meta.did, AuditAction.File_Upload, details, RelativityUserInfo.AuditWorkspaceUserArtifactID);
								}
							}
						}
						if (string.IsNullOrEmpty(resultStr))
						{
							if (response.Success && !img)
							{
								resultStr = $"AppID={WorkspaceID}&ArtifactID={meta.did}";
							}
							else
							{
								resultStr = meta.did.ToString();
							}
						}
					}
				}
				catch (Exception ex)
				{
                    DocumentManager.Instance.LogError(ex);
                    response.Success = false;
					response.Message = ex.Message;
				}

				return resultStr;
			}
			);
			Response.Clear();
			Response.ClearContent();
			result.Data = result.Data.Replace("'", "/39/").Replace("\"", "/34/");
			Response.Write($"<script>sessionStorage['____pushNo'] = '{Newtonsoft.Json.JsonConvert.SerializeObject(result).Replace("'", "\"")}'</script>");
			Response.End();
		}

		[HttpPost]
		public int CheckUploadStatus(string documentName)
		{
			int documentID = DocumentManager.Instance.GetDocByName(documentName);
			if (documentID != -1)
			{
				DocumentManager.Instance.UpdateDocumentLastModificationFields(documentID, RelativityUserInfo.WorkspaceUserArtifactID, true);
			}
			return documentID;
		}

		[HttpPost]
		public async Task UploadProcessingError(int errorID)
		{
			ResponseWithElements<string> result = await HandleResponseDynamicResponseAsync<string>(async (response) =>
			{
				string resultStr = string.Empty;
				bool isAdmin = PermissionsManager.Instance.IsUserAdministrator(WorkspaceID, RelativityUserInfo.ArtifactID);
				bool hasPermission = !isAdmin ? await PermissionsManager.Instance.CurrentUserHasPermissionToObjectType(
					this.WorkspaceID, Core.Helpers.Constants.PROCESSINGERROROBJECTTYPE, Core.Helpers.Constants.PERMISSIONPROCESSINGERRORUPLOADDOWNLOAD) : true;
				bool isDataGrid = await DocumentManager.Instance.IsDataGridEnabled(WorkspaceID);
				if (hasPermission)
				{
					ProcessingDocument error = ProcessingManager.instance.GetErrorInfo(errorID);
					HttpPostedFileBase file = Request.Files[0];
					string fileName = file.FileName;

					if (Path.GetExtension(fileName).ToLower() != Path.GetExtension(error.DocumentFileLocation).ToLower())
					{
						response.Success = false;
						response.Message = "The file must be of the same type.";
						return resultStr;
					}
					ExportedMetadata transientMetadata = GetTransient(file, fileName);
					ProcessingManager.instance.ReplaceFile(transientMetadata.Native, error);

					string details = AuditManager.instance.GenerateAuditDetailsForFileUpload(error.DocumentFileLocation, 0, "Processing Error File Replacement");
					AuditManager.instance.CreateAuditRecord(WorkspaceID, error.ErrorID, AuditAction.File_Upload, details, this.RelativityUserInfo.AuditWorkspaceUserArtifactID);
					return resultStr;
				}
				else
				{
					response.Success = false;
					response.Message = "You do not have enough permissions to perform the current action.";
					return resultStr;
				}
			}
			);
			Response.Clear();
			Response.ClearContent();
			Response.Write($"<script>sessionStorage['____pushNo'] = '{Newtonsoft.Json.JsonConvert.SerializeObject(result)}'</script>");
			Response.End();
		}

		[HttpPost]
		public JsonResult CheckForImages(int tArtifactId)
		{
			ResponseWithElements<string> result = HandleResponse<string>((response) =>
			{
				string resultStr = string.Empty;
				response.Success = true;
				resultStr = DocumentManager.Instance.ValidateDocImages(tArtifactId).ToString();
				return resultStr;
			}
			);

			return Json(result);
		}

		[HttpPost]
		public JsonResult GetRepLocation()
		{
			ResponseWithElements<string> result = HandleResponse<string>((response) =>
			{
				response.Success = true;
				return $@"{DocumentManager.Instance.GetRepositoryLocation()}EDDS{WorkspaceID}\Temp";
			}
			);

			return Json(result);
		}

		private ExportedMetadata GetTransient(HttpPostedFileBase file, string fileName)
		{
			ExportedMetadata transientMetadata = new ExportedMetadata();
			Stream stream = file.InputStream;
			var native = new byte[stream.Length];
			stream.Read(native, 0, checked((int)stream.Length));
			try
			{
                SearchExportManager.instance.ConfigureOutsideIn();
				transientMetadata = SearchExportManager.instance.ExportToSearchML(fileName, native, ConnectionHelper.Helper().BuildExporter());
			}
			catch (Exception ex)
			{
				DocumentManager.Instance.LogError(ex);
				transientMetadata.Native = native;
				transientMetadata.FileName = fileName;
				transientMetadata.ExtractedText = string.Empty;
                throw;
			}
			return transientMetadata;
		}

		private string GetWebAPIURL()
		{
			string url = ConnectionHelper.Helper().GetServicesManager().GetRESTServiceUrl().ToString();
			return url.ToString().ToLower().Replace("relativity.rest/api", "Relativity");
		}

		private bool ValidateFile(string tempFile)
		{
			IFileTypeInfo fileType = DocumentManager.Instance.GetNativeTypeByFilename(tempFile);
			return _finder.Contains(fileType.Id);
		}
	}
}