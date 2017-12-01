namespace kCura.SingleFileUpload.Core.Entities
{
    public class ProcessingDocument
    {
        public int ErrorID { get; set; }

        public int DocumentArtifactID { get; set; }

        public string DocumentFileLocation { get; set; }

        public string SourceLocation { get; set; }
    }
}
