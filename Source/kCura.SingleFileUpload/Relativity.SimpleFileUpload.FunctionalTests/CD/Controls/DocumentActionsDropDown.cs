using Atata;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD.Controls
{
	using _ = DocumentActionsDropDown;
	public class DocumentActionsDropDown : Page<_>
	{
		[FindById("ri-document-action-add-native")]
		public Clickable<UploadDocumentWindow, _> UploadNativeDocumentAction { get; private set; }

		[FindById("ri-document-action-replace-native")]
		public Clickable<UploadDocumentWindow, _> ReplaceNativeDocumentAction { get; private set; }

		[FindById("ri-document-action-upload-images-for-this-document")]
		public Clickable<DocumentViewerImageProfileOptions, _> UploadImageDocumentAction { get; private set; }


		[FindById("ri-document-action-replace-images-for-this-document")]
		public Clickable<DocumentViewerImageProfileOptions, _> ReplaceImageDocumentAction { get; private set; }
	}
}