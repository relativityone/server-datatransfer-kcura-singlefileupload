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
			
			Mock<IServicesMgr> mockingServicesMgr = mockingHelper
				.MockIServiceMgr()
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