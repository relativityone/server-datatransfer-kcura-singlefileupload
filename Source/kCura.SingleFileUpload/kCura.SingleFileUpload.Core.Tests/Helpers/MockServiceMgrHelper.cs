using Moq;
using Relativity.API;
using System;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Tests.Helpers
{
	public static class MockServiceMgrHelper
	{
		public static Mock<IServicesMgr> MockIServiceMgr<THelper>(this Mock<THelper> mockingHelper)
			where THelper : class, IHelper
		{
			Mock<IServicesMgr> mockingServiceManager = new Mock<IServicesMgr>();
			mockingHelper
				.Setup(p => p.GetServicesManager())
				.Returns(() => mockingServiceManager.Object);
			return mockingServiceManager;
		}
		
		public static Mock<IManager> MockServiceInstance<IManager>(this Mock<IServicesMgr> mockingServiceManager)
			where IManager : class, IDisposable
		{
			Mock<IManager> mockingManager = new Mock<IManager>();
			mockingManager.DefaultValue = DefaultValue.Mock;
			mockingManager.SetReturnsDefault(Task.FromResult(MockDBContextHelper.DEFAULT_ID));
			mockingManager.SetReturnsDefault(Task.CompletedTask);

			mockingServiceManager.MockService(mockingManager);

			return mockingManager;
		}

		public static Mock<IServicesMgr> MockService<IManager>(this Mock<IServicesMgr> mockingServiceManager, Mock<IManager> mockingManager = null)
			where IManager : class, IDisposable
		{
			if (mockingManager == null)
			{
				mockingManager = mockingServiceManager.MockServiceInstance<IManager>();
			}

			mockingServiceManager
				.Setup(p => p.CreateProxy<IManager>(It.IsAny<ExecutionIdentity>()))
				.Returns(mockingManager.Object);
			return mockingServiceManager;
		}
	}
}
