using kCura.EventHandler;
using kCura.SingleFileUpload.Resources.EventHandlers;
using NUnit.Framework;

namespace kCura.SingleFileUpload.Resources.NUnit
{
	[TestFixture]
	public class DocumentPageInteractionEventHandlerTest
	{

		[Test]
		public void PopulateScriptBlocksTest()
		{
			DocumentPageInteractionEventHandler eventHandler = new DocumentPageInteractionEventHandler();
			Response result = eventHandler.PopulateScriptBlocks();

			Assert.IsTrue(result.Success);
		}

	}
}
