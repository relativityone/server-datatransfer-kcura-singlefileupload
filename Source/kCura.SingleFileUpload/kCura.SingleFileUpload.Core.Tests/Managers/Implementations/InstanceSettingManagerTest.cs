using System;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.Tests.Constants;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.InstanceSetting;
using System.Threading.Tasks;
using FluentAssertions;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
	class InstanceSettingManagerTest : TestBase
	{
		[SetUp]
		public void Setup()
		{
			Mock<IHelper> mockingHelper = new Mock<IHelper>();
			Mock<IInstanceSettingManager> mockInstanceSettingManager = new Mock<IInstanceSettingManager>();

			mockInstanceSettingManager
				.Setup(p => p.QueryAsync(It.IsAny<global::Relativity.Services.Query>()))
				.Returns(Task.FromResult(TestsConstants._INSTANCE_SETTING_RESULT_SET));

			mockingHelper
				.MockIServiceMgr()
				.MockService(mockInstanceSettingManager);

			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public async Task GetMaxFilesInstanceSetting_ShouldReturnInstanceSettingValue()
		{
			// Arrange
			const int maxFiles = 5;

			// Act
			int result = await InstanceSettingManager.Instance.GetMaxFilesInstanceSettingAsync();
			
			// Assert
			result.Should().Be(maxFiles);
		}

		[Test]
		public void CreateMaxFilesInstanceSetting_ShouldNotThrow()
		{
			// Act
			Action action = () => InstanceSettingManager.Instance.CreateMaxFilesInstanceSettingAsync();
			
			// Assert
			action.Should().NotThrow();
		}
	}
}
