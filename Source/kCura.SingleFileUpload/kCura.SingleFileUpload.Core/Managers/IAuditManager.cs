using System;
using kCura.SingleFileUpload.Core.Entities.Enumerations;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IAuditManager
    {
        void CreateAuditRecord(int workspaceId, int artifactID, AuditAction action, string details, int userID);

        string GenerateAuditDetailsForFileUpload(string filePath, Int32 fileId, string message);

    }
}
