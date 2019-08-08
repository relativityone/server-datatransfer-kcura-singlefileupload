using kCura.Relativity.Client;
using kCura.Relativity.Client.Repositories;
using Moq;
using System.Linq;

namespace kCura.SingleFileUpload.Core.NUnit.Helpers
{
	public class RSAPIClientMockHelper
	{
		public static Mock<IRSAPIClient> GetMockedHelper(int workspaceID = -1)
		{
			var helper = new Mock<IRSAPIClient>();

			///--- base helper
			helper.Setup(p => p.APIOptions).Returns(new APIOptions(workspaceID));

			///--- hack repogroup
			var repoGroup = ForceRepositoryGroupMock(helper.Object);
			helper.SetupGet(p => p.Repositories).Returns(repoGroup);

			return helper;
		}

		internal static RepositoryGroup ForceRepositoryGroupMock(IRSAPIClient helper)
		{
			var repoGroupType = typeof(RepositoryGroup);
			var repositoryCtor = repoGroupType.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).First();
			var repoGroup = repositoryCtor.Invoke(new[] { helper }) as RepositoryGroup;

			return repoGroup;
		}
	}
}
