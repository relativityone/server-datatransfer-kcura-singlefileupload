using kCura.EventHandler;
using kCura.Relativity.Client;
using kCura.SingleFileUpload.Core.NUnit.Helpers;
using kCura.SingleFileUpload.Resources.EventHandlers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Permission;

namespace kCura.SingleFileUpload.Resources.NUnit.EventHandlers
{
	[TestFixture]
	public class SingleFileUploadPermissionPostInstallEventHandlerTest
	{

		private Mock<IEHHelper> mockingHelper;
		private SingleFileUploadPermissionPostInstallEventHandler eventHandler;


		[SetUp]
		public void Setup()
		{
			Mock<IRSAPIClient> rsapi = RSAPIClientMockHelper.GetMockedHelper();
			mockingHelper = MockHelper.GetMockingHelper<IEHHelper>();

			mockingHelper
				.MockIDBContextOnHelper();


			Mock<IServicesMgr> mockingServicesMgr = mockingHelper
				.MockIServiceMgr()
				.MockService(rsapi)
				.MockService<IPermissionManager>();

			eventHandler = new SingleFileUploadPermissionPostInstallEventHandler();

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