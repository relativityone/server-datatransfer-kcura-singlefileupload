using kCura.SingleFileUpload.Core.Entities;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IDocumentManager : IManager
    {
        Task<Response> SaveSingleDocument(ExportedMetadata documentInfo, int folderID, string webApiUrl, int workspaceID, int userID);
        Task ReplaceSingleDocument(ExportedMetadata documentInfo, int docID, bool fromDocumentViewer, bool avoidControlNumber, bool isDataGrid, string webApiUrl, int workspaceID, int userID, int folderID = 0);
        int GetDocByName(string docName);
        void SetCreateInstanceSettings();
        Task<bool> ValidateFileTypes(string extension);
        Task<bool> IsDataGridEnabled(int workspaceID);
        bool ValidateDocImages(int docArtifactId);
        bool ValidateDocNative(int docArtifactId);
        string GetDocumentControlNumber(int docArtifactId);
        int GetDocumentArtifactIdByControlNumber(string controlNumber);
        //  int SaveTempDocument(ExportedMetadata documentInfo, int folderID);
        string instanceFile(string fileName, byte[] fileBytes, bool isTemp, string baseRepo = null);
        void UpdateHasImages(int dArtifactId);
        void CreateMetrics(ExportedMetadata documentInfo, string bucket = null);
        FileInformation getFileByArtifactId(int docArtifactId);
        FileType IsFileTypeSupported(string fileExtension);
        bool ValidateHasRedactions(int docArtifactId);
        void DeleteRedactions(int docArtifactId);
        void UpdateDocumentLastModificationFields(int docArtifactId, int userID, bool isNew);
        void DeleteExistingImages(int dArtifactId);
        void InsertImage(FileInformation image);
        void WriteFile(byte[] file, FileInformation document);
        string GetRepositoryLocation();
    }
}
