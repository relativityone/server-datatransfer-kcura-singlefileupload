using FluentAssertions;
using kCura.EventHandler;
using kCura.SingleFileUpload.Resources.EventHandlers;
using NUnit.Framework;

namespace kCura.SingleFileUpload.Resources.Tests
{
	[TestFixture]
	public class DocumentPageInteractionEventHandlerTest
	{
		[Test]
		public void PopulateScriptBlocksTest()
		{
			// Arrange
			DocumentPageInteractionEventHandler eventHandler = new DocumentPageInteractionEventHandler();

			// Act
			Response result = eventHandler.PopulateScriptBlocks();

			// Assert
			result.Success.Should().BeTrue();
		}

	}
}
