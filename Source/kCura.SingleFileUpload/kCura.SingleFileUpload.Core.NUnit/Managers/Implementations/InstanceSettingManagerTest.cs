﻿using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.NUnit.Constants;
using kCura.SingleFileUpload.Core.NUnit.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.InstanceSetting;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.NUnit.Managers.Implementations
{
	[TestFixture]
	class InstanceSettingManagerTest : TestBase
	{

		[OneTimeSetUp]
		public void Setup()
		{
			Mock<IHelper> mockingHelper;
			mockingHelper = MockHelper
					.GetMockingHelper<IHelper>();

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
			int maxFiles = 5;
			int result = await InstanceSettingManager.Instance.GetMaxFilesInstanceSettingAsync();
			Assert.AreEqual(maxFiles, result);
		}

		[Test]
		public async Task CreateMaxFilesInstanceSettingTest()
		{
			await InstanceSettingManager.Instance.CreateMaxFilesInstanceSettingAsync();
			Assert.IsTrue(true);
		}


	}
}
