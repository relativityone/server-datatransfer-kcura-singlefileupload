namespace kCura.SingleFileUpload.Core.Entities
{
    public class FileInformation
    {
        public int FileID { get; set; }
        public int FileType { get; set; }
        public int DocumentArtifactID { get; set; }
        public int DocumentIdentifier { get; set; }
        public int Order { get; set; }
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public long FileSize { get; set; }
    }
}
