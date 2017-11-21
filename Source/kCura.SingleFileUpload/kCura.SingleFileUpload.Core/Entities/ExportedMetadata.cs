using System.Collections.Generic;

namespace kCura.SingleFileUpload.Core.Entities
{
    public class ExportedMetadata
    {
        public ExportedMetadata()
        {
            Fields = new Dictionary<string, object>();
        }
        public string FileName { get; set; }
        public string ControlNumber { get; set; }
        public string FileType { get; set; }
        public byte[] Native { get; set; }
        public string ExtractedText { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }
}
