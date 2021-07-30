using Atata;
using Relativity.Testing.Framework.Web.Components;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD.Controls
{
	public class UploadDocumentWindow : PopupWindow<UploadDocumentWindow>
	{
		[Wait(1, TriggerEvents.AfterSet, TriggerPriority.Medium)]
		[WaitForElement(WaitBy.Id, "dropHere", Until.Visible, TriggerEvents.AfterSet, TriggerPriority.Medium)]
		public DocumentFileInput<UploadDocumentWindow> Document { get; private set; }

		[FindById("uploadDocumentModalTitle")]
		[WaitFor(Until.Visible, TriggerEvents.Init, TriggerPriority.Lowest, PresenceTimeout = 120)]
		public Text<UploadDocumentWindow> ModalTitle { get; set; }

		public DocumentViewerPage Upload(string filePath)
		{
			var up = Document.Set(filePath);
			
			return up.SwitchToRoot<DocumentViewerPage>();
		}
	}
}
