using Atata;
using Relativity.Testing.Framework.Web.Components;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD.Controls
{
	using _ = DocumentViewerPage;
	public class DocumentViewerPage : WorkspacePage<_>
	{
		[FindById("_ReviewInterface")]
		public Frame<ReviewInterfaceFramePage, _> ReviewInterfaceFrame { get; private set; }

		[FindById("DocumentUploadModal")]
		public Frame<UploadDocumentWindow, _> DocumentUploadModalFrame { get; private set; }
	}
}