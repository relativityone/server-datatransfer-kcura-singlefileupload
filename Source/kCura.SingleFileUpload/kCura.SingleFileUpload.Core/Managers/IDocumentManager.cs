using kCura.SingleFileUpload.Core.Entities;
using Relativity.API;
using Relativity.Services.ObjectQuery;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IDocumentManager : IManager
    {
        int SaveSingleDocument(ExportedMetadata documentInfo, int folderID, string webApiUrl, int workspaceID, int userID);
        Task ReplaceSingleDocument(ExportedMetadata documentInfo, int docID, bool fromDocumentViewer, bool avoidControlNumber, bool isDataGrid, string webApiUrl, int workspaceID, int userID, int folderID = 0);
        int GetDocByName(string docName);
        bool SetDocumentCreateHref();
        Task<bool> ValidateFileTypes(string extension);
        Task<bool> IsDataGridEnabled(int workspaceID);
        bool ValidateDocImages(int docArtifactId);
        int SaveTempDocument(ExportedMetadata documentInfo, int folderID);
        bool ReplaceDocumentImages(int oArtifactId, int tArtifactId);
        void CreateMetrics(ExportedMetadata documentInfo, string bucket = null);
        void ImportDocument(ExportedMetadata documentInfo, string webApiUrl, int workspaceID, int folderId = 0, string bucket = null);
        FileInformation getFileByArtifactId(int docArtifactId);
        bool IsFileTypeSupported(string fileExtension);
        bool ValidateHasRedactions(int docArtifactId);
        void DeleteRedactions(int docArtifactId, int tArtifactId);
        void UpdateDocumentLastModificationFields(int docArtifactId, int userID, bool isNew);
    }
}
