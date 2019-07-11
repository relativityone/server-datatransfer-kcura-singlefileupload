using kCura.SingleFileUpload.Core.Entities;
using Relativity.Services.ObjectQuery;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public class ProcessingManager : BaseManager, IProcessingManager
	{
		private readonly int[] _INCLUDE_PERMISSIONS = new int[] { 1, 2, 3, 4, 5, 6 };
		private readonly int _DATA_ITEM_RESULT = 2;
		public ProcessingDocument GetErrorInfo(int errorID)
		{
			int processingErrorObjectType = GetArtifactTypeByArtifactGuid(Helpers.Constants.PROCESSINGERROROBJECTTYPE);
			ObjectQueryResultSet results;
			using (IObjectQueryManager _objectQueryManager = _Repository.CreateProxy<IObjectQueryManager>())
			{
				Query query = new Query
				{
					Fields = new[]
					{
					"Document file location",
					"Source location",
					"Relativity Document Identifier"
					}
					,
					IncludeIdWindow = false,
					TruncateTextFields = true,
					Condition = $"'ArtifactID' IN [{errorID}]"
				};
				results = Task.Run(() => _objectQueryManager.QueryAsync(WorkspaceID, processingErrorObjectType, query, 1, int.MaxValue, _INCLUDE_PERMISSIONS, string.Empty)).Result;
			}
			return new ProcessingDocument()
			{
				DocumentFileLocation = results.Data.DataResults[0].Fields[0].Value.ToString(),
				SourceLocation = results.Data.DataResults[0].Fields[1].Value?.ToString(),
				DocumentIdentifier = results.Data.DataResults[0].Fields[_DATA_ITEM_RESULT].Value?.ToString(),
				ErrorID = errorID
			};
		}
		public void ReplaceFile(byte[] file, ProcessingDocument document)
		{
			System.IO.File.WriteAllBytes(document.DocumentFileLocation, file);
		}

	}
}
