namespace kCura.SingleFileUpload.Core.Entities
{
	public class Imaging
	{
		public bool Fdv { get; set; }
		public bool Image { get; set; }
		public bool? NewImage { get; set; }
		public int DocID { get; set; }
		public int? ProfileID { get; set; }
		public int ErrorFile { get; set; }
		public bool? Fri { get; set; }
	}
}
