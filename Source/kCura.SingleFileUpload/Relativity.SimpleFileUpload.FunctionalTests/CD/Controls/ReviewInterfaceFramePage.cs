using Atata;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD.Controls
{
	using _ = ReviewInterfaceFramePage;
	public class ReviewInterfaceFramePage: Page<_>
	{
		[FindById("ri-universal-document-actions-list-toggle")]
		[WaitFor(Until.Visible, TriggerEvents.Init, TriggerPriority.Lowest, PresenceTimeout = 120)]
		public Clickable<DocumentActionsDropDown, _> Dropdown { get; set; }

		[FindById("ri-tab-native-viewer")]
		public Control<_> NativeTab { get; private set; }

		[FindById("ri-tab-image-viewer")]
		public Control<_> ImageTab { get; private set; }

		[FindById("ri-tab-text-viewer")]
		public Control<_> ExtractedTextTab { get; private set; }
	}
}
