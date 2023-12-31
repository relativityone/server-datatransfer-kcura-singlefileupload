﻿
namespace kCura.SingleFileUpload.Core.Helpers
{
    public class Constants
    {
        public const string PROCESSINGERROROBJECTTYPE = "EC75C63E-9666-456B-A0E2-CAD13EBF62A1";
        public const string DOCUMENTOBJECTTYPE = "15C36703-74EA-4FF8-9DFB-AD30ECE7530D";

        public const string CONTROL_NUMBER_FIELD = "2A3F1212-C8CA-4FA9-AD6B-F76C97F05438";
        public const string DOCUMENT_HAS_IMAGES_FIELD = "2BAACA72-790C-4B87-A7D8-C18C45CAC63D";
        public const string DOCUMENT_HAS_IMAGES_YES_CHOICE = "5002224A-59F9-4C19-AA57-3765BDBFB676";

        /*Telemetry Buckets*/
        public const string METRICS_CATEGORY = "Single File Upload";
        public const string BUCKET_DOCUMENTSUPLOADED = "Required.SingleFileUpload.Main.Upload.DocumentsUploaded";
        public const string BUCKET_DOCUMENTSREPLACED = "Required.SingleFileUpload.Main.Replace.DocumentsReplaced";
        public const string BUCKET_TOTALSIZEDOCUMENTUPLOADED = "Required.SingleFileUpload.Main.Upload.TotalSizeDocumentUploaded";
        public const string BUCKET_FILETYPE = "Required.SingleFileUpload.Main.Upload.FileType";

        /*Permission Constants*/
        public const string PERMISSIONPROCESSINGERRORUPLOADDOWNLOAD = "Download and Upload files with processing errors";
        public const string PERMISSIONREPLACEIMAGEUPLOADDOWNLOAD = "Upload Image";
        public const string PERMISSIONADDIMAGE = "Add Image";
        public const string PERMISSIONDELETEIMAGE = "Delete Image";
        public const string PERMISSIONREPLACEDOCUMENT = "Replace Document";

        /*WinEDDS settings to avoid distribution issue*/
        public const string TAPI_FORCE_HTTP_CLIENT = "TapiForceHttpClient";
        public const string TAPI_FORCE_WEB_UPLOAD = "ForceWebUpload";
        public const string TAPI_FORCE_BCP_HTTP_CLIENT = "TapiForceBcpHttpClient";

        public const string ADD_DOCUMENT_CUSTOM_PERMISSION = "New Document";

        public const string INSTANCE_SETTING_SECTION = "SFU";

        public const string MAX_FILES_TO_UPLOAD_INSTANCE_SETTING_NAME = "SFUMaxFilesToUpload";
        public const string MAX_FILES_TO_UPLOAD_INSTANCE_SETTING_DESCRIPTION = "Determines the maximum value of files to upload using Simple File Upload. Maximum value should be 100.";
        public const string MAX_FILES_TO_UPLOAD_INSTANCE_SETTING_DEFAULT_VALUE = "20";
    }
}