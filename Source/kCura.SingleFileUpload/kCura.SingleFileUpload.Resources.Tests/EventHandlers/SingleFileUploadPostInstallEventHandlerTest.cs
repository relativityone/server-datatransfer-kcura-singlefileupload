using kCura.EventHandler;
using kCura.Relativity.Client;
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

namespace kCura.SingleFileUpload.Resources.Tests.EventHandlers
{
	[TestFixture]
	class SingleFileUploadPostInstallEventHandlerTest
	{
		private Mock<IEHHelper> mockingHelper;
		private SingleFileUploadPostInstallEventHandler eventHandler;


		[SetUp]
		public void Setup()
		{
			Mock<IRSAPIClient> rsapi = RSAPIClientMockHelper.GetMockedHelper();

			Mock<IToggleProvider> mockToggleProvider = new Mock<IToggleProvider>
			{
				DefaultValue = DefaultValue.Mock
			};

			mockToggleProvider.SetReturnsDefault(MockHelper.FakeTask());
			mockToggleProvider.SetReturnsDefault(Task.FromResult(true));
			mockToggleProvider.SetReturnsDefault(1);
			ToggleProvider.Current = mockToggleProvider.Object;

			Mock<IInstanceSettingManager> mockInstanceSettingManager = new Mock<IInstanceSettingManager>();

			mockInstanceSettingManager
				.Setup(p => p.QueryAsync(It.IsAny<global::Relativity.Services.Query>()))
				.Returns(Task.FromResult(TestsConstants._INSTANCE_SETTING_RESULT_SET));

			mockingHelper = MockHelper.GetMockingHelper<IEHHelper>();

			mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsScalar(Queries.GetFieldsInstanceSetting, TestsConstants._JSON_RESULT);
			
			Mock<IServicesMgr> mockingServicesMgr = mockingHelper
				.MockIServiceMgr()
				.MockService(rsapi)
				.MockService(mockInstanceSettingManager);

			eventHandler = new SingleFileUploadPostInstallEventHandler();
		}


		[Test]
		public void ExecuteTest()
		{
			eventHandler.Helper = mockingHelper.Object;
			Response result = eventHandler.Execute();
			Assert.IsTrue(result.Success);
		}

		[Test]
		public void ExecuteExceptionTest()
		{
			Response result = eventHandler.Execute();
			Assert.IsFalse(result.Success);

		}
	}
}
