using System;
using System.Management;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Relativity.API;
using kCura.SingleFileUpload.Core.Entities.Enumerations;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IAuditManager
    {
        void CreateAuditRecord(int workspaceId, int artifactID, AuditAction action, string details, int userID);

        string GenerateAuditDetailsForFileUpload(string filePath, Int32 fileId, string message);

    }
}
