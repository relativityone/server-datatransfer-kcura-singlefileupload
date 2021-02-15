using FluentAssertions;
using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using kCura.SingleFileUpload.Resources.EventHandlers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Permission;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Resources.Tests.EventHandlers
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
	public class SingleFileUploadPermissionPostInstallEventHandlerTest
	{
		private Mock<IEHHelper> mockingHelper;
		private SingleFileUploadPermissionPostInstallEventHandler eventHandler;
		
		[SetUp]
		public void Setup()
		{
			mockingHelper = new Mock<IEHHelper>();

			mockingHelper
				.MockIDBContextOnHelper();
			
			mockingHelper
				.MockIServiceMgr()
				.MockService<IPermissionManager>();

			eventHandler = new SingleFileUploadPermissionPostInstallEventHandler();
		}

		[Test]
		public void Execute_ShouldSuccesfullyExecuteHandler()
		{
			// Arrange
			eventHandler.Helper = mockingHelper.Object;

			// Act
			Response result = eventHandler.Execute();

			// Assert
			result.Success.Should().BeTrue();
		}

		[Test]
		public void Execute_ShouldReturnFalse_WhenExceptionWasThrown()
		{
			// Act
			Response result = eventHandler.Execute();

			// Assert
			result.Success.Should().BeFalse();
		}
	}
}