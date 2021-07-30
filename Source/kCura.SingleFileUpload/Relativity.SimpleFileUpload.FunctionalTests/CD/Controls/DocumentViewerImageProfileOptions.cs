using Atata;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD.Controls
{
	using _ = DocumentViewerImageProfileOptions;
	public class DocumentViewerImageProfileOptions : Page<_>
	{
		[FindByXPath("//div[contains(@title,'Basic Default')]")]
		public Clickable<UploadDocumentWindow, _> ImageProfile { get; private set; }
	}
}
