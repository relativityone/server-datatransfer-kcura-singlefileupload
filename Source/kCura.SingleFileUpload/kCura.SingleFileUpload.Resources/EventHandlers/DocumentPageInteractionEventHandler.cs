using kCura.EventHandler;

namespace kCura.SingleFileUpload.Resources.EventHandlers
{
	[kCura.EventHandler.CustomAttributes.Description("Single File Upload Page Interaction Event Handler for Document")]
	[System.Runtime.InteropServices.Guid("FC7DA2FE-200A-44EA-A3F5-BFBF6C306143")]
	public class DocumentPageInteractionEventHandler : PageInteractionEventHandler
	{
		public override Response PopulateScriptBlocks()
		{
			return new Response { Success = true };
		}
	}
}
