using kCura.SingleFileUpload.Core.Entities;
using System.Threading.Tasks;
using DataExchange = Relativity.DataExchange;

namespace kCura.SingleFileUpload.Core.Managers
{
	public interface IDocumentManager : IManager
	{

		bool IsFileTypeSupported(string fileExtension);
		Task<Response> SaveSingleDocument(ExportedMetadata documentInfo, int folderID, string webApiUrl, int workspaceID, int userID);
		Task ReplaceSingleDocument(ExportedMetadata documentInfo, DocumentExtraInfo documentExtraInfo);
		int GetDocByName(string docName);
		void SetCreateInstanceSettings();
		Task<bool> ValidateFileTypes(string extension);
		Task<bool> IsDataGridEnabled(int workspaceID);
		bool ValidateDocImages(int docArtifactId);
		bool ValidateDocNative(int docArtifactId);
		string GetDocumentControlNumber(int docArtifactId);
		string InstanceFile(string fileName, byte[] fileBytes, bool isTemp, string baseRepo = null);
		void UpdateHasImages(int dArtifactId);
		Task CreateMetricsAsync(ExportedMetadata documentInfo, string bucket = null);
		FileInformation GetFileByArtifactId(int docArtifactId);
		bool ValidateHasRedactions(int docArtifactId);
		void DeleteRedactions(int docArtifactId);
		void UpdateDocumentLastModificationFields(int docArtifactId, int userID, bool isNew);
		void DeleteExistingImages(int dArtifactId);
		void InsertImage(FileInformation image);
		void WriteFile(byte[] file, FileInformation document);
		string GetRepositoryLocation();
		DataExchange.Io.IFileTypeInfo GetNativeTypeByFilename(string fileName);
		void RemovePageInteractionEvenHandlerFromDocumentObject();
		void DeleteTempFile(string tempLocation);
	}
}
