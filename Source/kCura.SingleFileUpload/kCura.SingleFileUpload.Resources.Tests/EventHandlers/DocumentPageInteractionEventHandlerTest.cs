using FluentAssertions;
using kCura.EventHandler;
using kCura.SingleFileUpload.Resources.EventHandlers;
using NUnit.Framework;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Resources.Tests
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
	public class DocumentPageInteractionEventHandlerTest
	{
		[Test]
		public void PopulateScriptBlocksTest_ShouldHandleScriptBlocksPopulation()
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
