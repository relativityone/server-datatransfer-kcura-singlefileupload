﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Entities.Enumerations
{
    public enum AuditAction
    {
        Create = 2,
        Update = 3,
        File_Upload = 52,
        File_Download = 53,
        Images_Deleted = 14
    }
}