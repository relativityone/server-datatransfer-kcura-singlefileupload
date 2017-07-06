using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Entities
{
    public class ProcessingDocument
    {
        public int ErrorID { get; set; }

        public string DocumentFileLocation { get; set; }

        public string SourceLocation { get; set; }
    }
}
