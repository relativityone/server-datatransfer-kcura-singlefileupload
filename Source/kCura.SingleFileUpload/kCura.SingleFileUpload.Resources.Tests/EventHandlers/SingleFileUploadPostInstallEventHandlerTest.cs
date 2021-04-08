using kCura.EventHandler;
using kCura.SingleFileUpload.Core.SQL;
using kCura.SingleFileUpload.Core.Tests.Constants;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using kCura.SingleFileUpload.Resources.EventHandlers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.InstanceSetting;
using Relativity.Toggles;
using System.Threading.Tasks;
using FluentAssertions;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Resources.Tests.EventHandlers
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
	class SingleFileUploadPostInstallEventHandlerTest
	{
		private Mock<IEHHelper> _mockingHelper;
		private SingleFileUploadPostInstallEventHandler _eventHandler;
		
		[SetUp]
		public void Setup()
		{
			Mock<IToggleProvider> mockToggleProvider = new Mock<IToggleProvider>
			{
				DefaultValue = DefaultValue.Mock
			};

			mockToggleProvider.SetReturnsDefault(Task.CompletedTask);
			mockToggleProvider.SetReturnsDefault(Task.FromResult(true));
			mockToggleProvider.SetReturnsDefault(1);
			ToggleProvider.Current = mockToggleProvider.Object;

			Mock<IInstanceSettingManager> mockInstanceSettingManager = new Mock<IInstanceSettingManager>();

			mockInstanceSettingManager
				.Setup(p => p.QueryAsync(It.IsAny<global::Relativity.Services.Query>()))
				.Returns(Task.FromResult(TestsConstants._INSTANCE_SETTING_RESULT_SET));

			_mockingHelper = new Mock<IEHHelper>();

			_mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsScalar(Queries.GetFieldsInstanceSetting, TestsConstants._JSON_RESULT);
			
			_mockingHelper
				.MockIServiceMgr()
				.MockService(mockInstanceSettingManager);

			_eventHandler = new SingleFileUploadPostInstallEventHandler();
		}
		
		[Test]
		public void Execute_ShouldSuccesfullyExecuteHandler()
		{
			// Arrange
			_eventHandler.Helper = _mockingHelper.Object;

			// Act
			Response result = _eventHandler.Execute();

			// Assert
			result.Success.Should().BeTrue();
		}

		[Test]
		public void Execute_ShouldReturnFalse_WhenExceptionWasThrown()
		{
			// Act
			Response result = _eventHandler.Execute();

			// Assert
			result.Success.Should().BeFalse();
		}
	}
}
