namespace kCura.SingleFileUpload.Core.Entities
{
	public class DocumentExtraInfo
	{
		public int DocID { get; set; }
		public bool FromDocumentViewer { get; set; }
		public bool AvoidControlNumber { get; set; }
		public bool IsDataGrid { get; set; }
		public string WebApiUrl { get; set; }
		public int WorkspaceID { get; set; }
		public int UserID { get; set; }
		public int FolderID { get; set; }
	}
}
