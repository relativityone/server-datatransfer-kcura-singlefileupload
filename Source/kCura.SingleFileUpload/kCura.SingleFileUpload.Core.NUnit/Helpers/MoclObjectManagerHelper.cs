using Moq;
using Relativity.Services.ObjectQuery;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.NUnit.Helpers
{
	public static class MockObjectQueryManagerHelper
	{
		public static Mock<IObjectQueryManager> Mock(this Mock<IObjectQueryManager> mockingOQM, object result)
		{
			mockingOQM
				.Setup(x => x.QueryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Query>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<string>()))
				.Returns(Task.FromResult(ObjectQueryResultSet(result)));

			return mockingOQM;
		}

		private static ObjectQueryResultSet ObjectQueryResultSet(object result)
		{
			var queryResult = new ObjectQueryResultSet
			{
				Data = new DataResult
				{
					DataResults = new[]
					{
						new QueryDataItemResult
						{
							ArtifactId = 1,
							Fields = new[]
							{
								new DataItemFieldResult { Name = "Name", Value = result },
								new DataItemFieldResult { Name = "Name", Value = result },
								new DataItemFieldResult { Name = "Name", Value = result },
							}
						},
					},
				},
				Success = true,
			};

			return queryResult;
		}

	}
}
