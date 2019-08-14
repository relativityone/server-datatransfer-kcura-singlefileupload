using System.Data;

namespace kCura.SingleFileUpload.Core.Entities
{
	public class ImportJobSettings
	{
		public int WorkspaceID { get; set; }
		public int FolderId { get; set; }
		public DocumentIdentifierField IdentityField { get; set; }
		public IDataReader DocumentsDataReader { get; set; }
	}
}
