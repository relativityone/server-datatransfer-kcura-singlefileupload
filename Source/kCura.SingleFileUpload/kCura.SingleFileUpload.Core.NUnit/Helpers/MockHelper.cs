using Moq;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.NUnit.Helpers
{
	public static class MockHelper
	{
		public const int WORKSPACE_ID = 1028496;
		public const int DEFAULT_ID = 1000000;
		public static Mock<THelper> GetMockingHelper<THelper>()
			where THelper : class, IHelper
		{
			Mock<THelper> mockingHelper = new Mock<THelper>();

			return mockingHelper;
		}

		public static Mock<IEHHelper> MockGetActiveCaseID(this Mock<IEHHelper> mockingHelper)
		{
			mockingHelper.Setup(p => p.GetActiveCaseID()).Returns(WORKSPACE_ID);
			return mockingHelper;
		}
		public static Mock<ICPHelper> MockGetActiveCaseID(this Mock<ICPHelper> mockingHelper)
		{
			mockingHelper.Setup(p => p.GetActiveCaseID()).Returns(WORKSPACE_ID);
			return mockingHelper;
		}

		public static async Task FakeTask() => await Task.Yield();
	}
}
