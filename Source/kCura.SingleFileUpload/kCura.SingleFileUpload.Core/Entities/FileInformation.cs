﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Entities
{
    public class FileInformation
    {
        public int FileID { get; set; }
        public int FileType { get; set; }
        public int DocumentArtifactID { get; set; }
        public string FileName { get; set; }
        public string FileLocation { get; set; }
    }
}