using System.Web.Mvc;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using Relativity.SingleFileUpload.MVC;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using System;
using System.IO;
using kCura.SingleFileUpload.Core.Entities;
using System.Threading.Tasks;
using kCura.SingleFileUpload.MVC.Models;
using Relativity.CustomPages;
using Relativity.Services.DataContracts.DTOs.MetricsCollection;
using Relativity.Telemetry.Services.Metrics;
using kCura.SingleFileUpload.Core.Entities.Enumerations;
using kCura.SingleFileUpload.Core.Helpers;

namespace kCura.SingleFileUpload.MVC.Controllers
{
    public class SFUController : BaseController
    {
        ISearchExportManager seManager = new SearchExportManager();
        IDocumentManager docManager = new DocumentManager();
        IProcessingManager processingManager = new ProcessingManager();
        IAuditManager auditManager = new AuditManager(ConnectionHelper.Helper());
        PermissionHelper permissionHelper = new PermissionHelper(ConnectionHelper.Helper());

        public ActionResult Index(bool fdv = false, int errorFile = 0, int docId = 0, bool image = false, bool newImage = false)
        {
            ViewBag.AppID = WorkspaceID;
            ViewBag.FDV = fdv.ToString().ToLower();
            ViewBag.ErrorID = errorFile;
            ViewBag.DocID = docId;
            ViewBag.ChangeImage = image.ToString().ToLower();
            ViewBag.NewImage = newImage.ToString().ToLower();
            ViewBag.HasRedactions = docManager.ValidateHasRedactions(docId).ToString().ToLower();
            return View();
        }

        [HttpPost]
        public async Task Upload(int fid = 0, int did = 0, bool fdv = false, bool force = false, bool img = false)
        {
            var result = await HandleResponseDynamicResponseAsync<string>(async (response) =>
            {
                string resultStr = string.Empty;
                if (img)
                {
                    var hasPermission = true; // await permissionHelper.CurrentUserHasPermissionToObjectType(this.WorkspaceID, Core.Helpers.Constants.ProcessingErrorObjectType, Core.Helpers.Constants.ReplaceImageUploadDownload);
                    if (!hasPermission)
                    {
                        response.Success = false;
                        response.Message = "You do not have enough permissions to perform the current action.";
                        return resultStr;
                    }
                }

                var file = Request.Files[0];
                string fileName = file.FileName;
                if (fileName.Contains("\\"))
                    fileName = Path.GetFileName(fileName);
                var fileExt = Path.GetExtension(fileName);
                var res = await docManager.ValidateFileTypes(fileExt);
                var suported = docManager.IsFileTypeSupported(fileExt);
                if (!res)
                {
                    response.Success = false;
                    response.Message = "This file type is not supported.";
                }
                else
                {
                    if (suported)
                    {
                        var isDataGrid = await docManager.IsDataGridEnabled(WorkspaceID);
                        var docIDByName = docManager.GetDocByName(fileName);
                        if (!fdv)
                        {
                            did = docIDByName;
                            if (did == -1 || force)
                            {
                                var transientMetadata = getTransient(file, fileName);
                                if (did == -1)
                                {
                                    did = docManager.SaveSingleDocument(transientMetadata, fid, GetWebAPIURL(), WorkspaceID, this.RelativityUserInfo.WorkspaceUserArtifactID);
                                    auditManager.CreateAuditRecord(WorkspaceID, did, AuditAction.Create, string.Empty, this.RelativityUserInfo.AuditWorkspaceUserArtifactID);
                                }
                                else
                                {
                                    await docManager.ReplaceSingleDocument(transientMetadata, did, false, true, isDataGrid, GetWebAPIURL(), WorkspaceID, this.RelativityUserInfo.WorkspaceUserArtifactID, fid);
                                    auditManager.CreateAuditRecord(WorkspaceID, did, AuditAction.Update, string.Empty, this.RelativityUserInfo.AuditWorkspaceUserArtifactID);
                                }
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "R";
                            }
                        }
                        else
                        {
                            if (img)
                            {
                                if (!fileExt.Equals(".pdf") && !fileExt.Equals(".tif") && !fileExt.Equals(".tiff") && !fileExt.Equals(".jpeg") && !fileExt.Equals(".jpg"))
                                {
                                    response.Success = false;
                                    response.Message = "Loaded file is not a supported format. Please select PDF, TIFF, or JPEG.";
                                }
                                else
                                {
                                    response.Success = true;
                                    response.Message = "I";
                                    var transientMetadata = getTransient(file, fileName);
                                    did = docManager.SaveTempDocument(transientMetadata, fid);
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
                                    await docManager.ReplaceSingleDocument(transientMetadata, did, true, did == docIDByName, isDataGrid, GetWebAPIURL(), WorkspaceID, this.RelativityUserInfo.WorkspaceUserArtifactID);
                                    auditManager.CreateAuditRecord(WorkspaceID, did, AuditAction.Update, string.Empty, this.RelativityUserInfo.AuditWorkspaceUserArtifactID);
                                }
                            }
                        }
                        if (response.Success && !img)
                            resultStr = $"AppID={WorkspaceID}&ArtifactID={did}";
                        else
                            resultStr = did.ToString();
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "This file type is not supported.";
                    }
                }
                return resultStr;
            });
            Response.Clear();
            Response.ClearContent();
            Response.Write($"<script>sessionStorage['____pushNo'] = '{Newtonsoft.Json.JsonConvert.SerializeObject(result)}'</script>");
            Response.End();
        }

        [HttpPost]
        public async Task UploadProcessingError(int errorID)
        {
            var result = await HandleResponseDynamicResponseAsync<string>(async (response) =>
            {
                string resultStr = string.Empty;
                var isAdmin = permissionHelper.IsSytemAdminUser(RelativityUserInfo.ArtifactID);
                var hasPermission = !isAdmin ? await permissionHelper.CurrentUserHasPermissionToObjectType(this.WorkspaceID, Core.Helpers.Constants.ProcessingErrorObjectType, Core.Helpers.Constants.ProcessingErrorUploadDownload) : true;

                if (hasPermission)
                {
                    var error = processingManager.GetErrorInfo(errorID);
                    var file = Request.Files[0];
                    string fileName = file.FileName;

                    if (Path.GetExtension(fileName).ToLower() != Path.GetExtension(error.DocumentFileLocation).ToLower())
                    {
                        response.Success = false;
                        response.Message = "The file must be of the same type.";
                        return resultStr;
                    }
                    var transientMetadata = getTransient(file, fileName);
                    processingManager.ReplaceFile(transientMetadata.Native, error);
                    var details = auditManager.GenerateAuditDetailsForFileUpload(error.DocumentFileLocation, 0, "Processing Error File Replacement");
                    auditManager.CreateAuditRecord(WorkspaceID, error.ErrorID, AuditAction.File_Upload, details, this.RelativityUserInfo.AuditWorkspaceUserArtifactID);
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
        public JsonResult ReplaceImages(int oArtifactId, int tArtifactId, bool newImage)
        {
            var result = HandleResponse<string>((response) =>
            {
                string resultStr = string.Empty;
                response.Success = true;
                FileInformation file = docManager.getFileByArtifactId(oArtifactId);
                docManager.DeleteRedactions(oArtifactId, tArtifactId);
                resultStr = docManager.ReplaceDocumentImages(oArtifactId, tArtifactId).ToString();
                string details = string.Empty;
                if (!newImage)
                {
                    details = auditManager.GenerateAuditDetailsForFileUpload(file.FileLocation, file.FileID, "Images Deleted");
                    auditManager.CreateAuditRecord(WorkspaceID, oArtifactId, AuditAction.Images_Deleted, details, this.RelativityUserInfo.AuditWorkspaceUserArtifactID);
                }
                details = auditManager.GenerateAuditDetailsForFileUpload(file.FileLocation, file.FileID, "Images Replaced");
                auditManager.CreateAuditRecord(WorkspaceID, oArtifactId, AuditAction.File_Upload, details, this.RelativityUserInfo.AuditWorkspaceUserArtifactID);
                return resultStr;
            });

            return Json(result);
        }

        [HttpPost]
        public JsonResult CheckForImages(int tArtifactId)
        {
            var result = HandleResponse<string>((response) =>
            {
                string resultStr = string.Empty;
                response.Success = true;
                resultStr = docManager.ValidateDocImages(tArtifactId).ToString();
                return resultStr;
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
                transientMetadata = seManager.ExportToSearchML(fileName, native);
            }
            catch
            {
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
            //return ConnectionHelper.Helper().GetServicesManager().GetRESTServiceUrl().ToString().ToLower().Replace("relativity.rest/api", "RelativityWebAPI");
        }
    }
}