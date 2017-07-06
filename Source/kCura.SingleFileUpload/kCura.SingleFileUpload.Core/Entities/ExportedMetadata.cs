using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Entities
{
    public class ExportedMetadata
    {
        public ExportedMetadata()
        {
            Fields = new Dictionary<string, object>();
        }
        public string FileName { get; set; }
        public byte[] Native { get; set; }
        public string ExtractedText { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }
}
