using System.Threading.Tasks;
using Relativity.Services.ObjectQuery;
using kCura.SingleFileUpload.Core.Entities;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class ProcessingManager : BaseManager, IProcessingManager
    {
        public ProcessingDocument GetErrorInfo(int errorID)
        {
            var processingErrorObjectType = GetArtifactTypeByArtifactGuid(Helpers.Constants.ProcessingErrorObjectType);
            ObjectQueryResultSet results;
            using (IObjectQueryManager _objectQueryManager = _Repository.CreateProxy<IObjectQueryManager>())
            {
                Query query = new Query { Fields = new[] { "Document file location", "Source location", "Relativity Document Identifier" }, IncludeIdWindow = false, TruncateTextFields = true, Condition = $"'ArtifactID' IN [{errorID}]" };
                results = Task.Run(() => _objectQueryManager.QueryAsync(WorkspaceID, processingErrorObjectType, query, 1, int.MaxValue, new int[] { 1, 2, 3, 4, 5, 6 }, string.Empty)).Result;
            }
            return new ProcessingDocument()
            {
                DocumentFileLocation = results.Data.DataResults[0].Fields[0].Value.ToString(),
                SourceLocation = results.Data.DataResults[0].Fields[1].Value?.ToString(),
                DocumentIdentifier = results.Data.DataResults[0].Fields[2].Value?.ToString(),
                ErrorID = errorID
            };
        }
        public void ReplaceFile(byte[] file, ProcessingDocument document)
        {
            System.IO.File.WriteAllBytes(document.DocumentFileLocation, file);
        }

    }
}
