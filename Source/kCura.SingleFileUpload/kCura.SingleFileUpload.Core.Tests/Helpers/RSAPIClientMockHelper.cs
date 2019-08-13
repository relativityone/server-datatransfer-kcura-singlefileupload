using kCura.Relativity.Client;
using kCura.Relativity.Client.Repositories;
using Moq;
using System;
using System.Linq;

namespace kCura.SingleFileUpload.Core.Tests.Helpers
{
	public class RSAPIClientMockHelper
	{
		public static Mock<IRSAPIClient> GetMockedHelper(int workspaceID = -1)
		{
			var helper = new Mock<IRSAPIClient>();
			helper.DefaultValue = DefaultValue.Mock;
			helper.SetReturnsDefault(true);
			helper.SetReturnsDefault(GetResulSetMock());

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

		internal static ResultSet GetResulSetMock()
		{
			return new ResultSet()
			{
				Results = new System.Collections.Generic.List<Result>()
				{
					new Result()
					{
						MetaDataArtifact=  new Artifact()
						{
							ArtifactGuids =  new System.Collections.Generic.List<System.Guid>()
							{
								Guid.NewGuid()
							}
						},
						WarningMessage =  new System.Collections.Generic.List<string>()
						{
							"Empty"
						},
						Success = true

					}
				},
				ResultSetType = ResultSetType.Create,
				Success = true,
			};
		}
	}
}
