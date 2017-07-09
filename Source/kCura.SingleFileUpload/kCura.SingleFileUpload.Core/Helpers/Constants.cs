﻿
namespace kCura.SingleFileUpload.Core.Helpers
{
    public class Constants
    {
        public const string ProcessingErrorObjectType = "EC75C63E-9666-456B-A0E2-CAD13EBF62A1";
        public const string DocumentObjectType = "15C36703-74EA-4FF8-9DFB-AD30ECE7530D";

        public const string CONTROL_NUMBER_FIELD = "2A3F1212-C8CA-4FA9-AD6B-F76C97F05438";
        public const string EXTRACTED_TEXT_FIELD = "58D35076-1B1D-43B4-BFF4-D6C089DE51B2";
        public const string FILE_NAME_FIELD = "72C9C4FA-9C18-4B57-B855-D7F1E496179D";
        public const string FILE_SIZE_FIELD = "1287C045-CF79-44B6-8A0A-0C8D7D60D745";
        public const string DOCUMENT_EXTENSION_FIELD = "1798944A-6A7F-46FB-A1DA-938304C10689";
        public const string DOCUMENT_HAS_IMAGES_FIELD = "2BAACA72-790C-4B87-A7D8-C18C45CAC63D";
        public const string DOCUMENT_HAS_IMAGES_YES_CHOICE = "5002224A-59F9-4C19-AA57-3765BDBFB676";
        /*Telemetry Buckets*/
        public const string METRICS_CATEGORY = "Single File Upload";
        public const string BUCKET_DocumentsUploaded = "Required.SingleFileUpload.Main.Upload.DocumentsUploaded";
        public const string BUCKET_DocumentsReplaced = "Required.SingleFileUpload.Main.Replace.DocumentsReplaced";
        public const string BUCKET_TotalSizeDocumentUploaded = "Required.SingleFileUpload.Main.Upload.TotalSizeDocumentUploaded";
        public const string BUCKET_FileType = "Required.SingleFileUpload.Main.Upload.FileType";
        /*Permission Constants*/
        public const string ProcessingErrorUploadDownload = "Download and Upload files with processing errors";
        public const string ReplaceImageUploadDownload = "Replace Image";
    }
}