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

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
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
		public async Task GetMaxFilesInstanceSettingTest()
		{
			// Arrange
			const int maxFiles = 5;

			// Act
			int result = await InstanceSettingManager.Instance.GetMaxFilesInstanceSettingAsync();
			
			// Assert
			result.Should().Be(maxFiles);
		}

		[Test]
		public void CreateMaxFilesInstanceSettingTest()
		{
			// Act
			Action action = () => InstanceSettingManager.Instance.CreateMaxFilesInstanceSettingAsync();
			
			// Assert
			action.Should().NotThrow();
		}
	}
}
