using FluentAssertions;
using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using kCura.SingleFileUpload.Resources.EventHandlers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Permission;

namespace kCura.SingleFileUpload.Resources.Tests.EventHandlers
{
	[TestFixture]
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
		public void ExecuteTest()
		{
			// Arrange
			eventHandler.Helper = mockingHelper.Object;

			// Act
			Response result = eventHandler.Execute();

			// Assert
			result.Success.Should().BeTrue();
		}

		[Test]
		public void ExecuteExceptionTest()
		{
			// Act
			Response result = eventHandler.Execute();

			// Assert
			result.Success.Should().BeFalse();
		}
	}
}