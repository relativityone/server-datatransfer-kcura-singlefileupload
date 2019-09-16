using kCura.SingleFileUpload.Core.Entities;
using Relativity.Services.Objects;
using System;
using Relativity.Services.Objects.DataContracts;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public class ProcessingManager : BaseManager, IProcessingManager
	{
		private readonly int[] _INCLUDE_PERMISSIONS = new int[] { 1, 2, 3, 4, 5, 6 };
		private readonly int _DATA_ITEM_RESULT = 2;

		public static readonly Lazy<IProcessingManager> _INSTANCE = new Lazy<IProcessingManager>(() => new ProcessingManager());
		public static IProcessingManager instance => _INSTANCE.Value;
		public ProcessingDocument GetErrorInfo(int errorID)
		{
			var processingErrorObjectType = GetArtifactTypeByArtifactGuid(Helpers.Constants.ProcessingErrorObjectType);
			QueryResult results;
			using (IObjectManager objectManager = _Repository.CreateProxy<IObjectManager>())
			{
				QueryRequest query = new QueryRequest
				{
					ObjectType = new ObjectTypeRef() { ArtifactTypeID = processingErrorObjectType },
					Fields = new[]
					{
						new FieldRef() { Name = "Document file location" },
						new FieldRef() { Name = "Source location" },
						new FieldRef() { Name = "Relativity Document Identifier" }
					},
					IncludeIDWindow = false,
					Condition = $"'ArtifactID' IN [{errorID}]"
				};
				results = objectManager.QueryAsync(WorkspaceID, query, 1, 10).ConfigureAwait(false).GetAwaiter().GetResult();
			}
			return new ProcessingDocument()
			{
				DocumentFileLocation = results.Objects[0].FieldValues[0].Value.ToString(),
				SourceLocation = results.Objects[0].FieldValues[1].Value?.ToString(),
				DocumentIdentifier = results.Objects[0].FieldValues[2].Value?.ToString(),
				ErrorID = errorID
			};
		}

		public void ReplaceFile(byte[] file, ProcessingDocument document)
		{
			System.IO.File.WriteAllBytes(document.DocumentFileLocation, file);
		}
	}
}
