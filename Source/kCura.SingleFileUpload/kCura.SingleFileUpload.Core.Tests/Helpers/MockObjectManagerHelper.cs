using Moq;
using System.Threading.Tasks;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace kCura.SingleFileUpload.Core.Tests.Helpers
{
	public static class MockObjectManagerHelper
	{
		public static Mock<IObjectManager> Mock(this Mock<IObjectManager> mockingOQM, object result)
		{
			mockingOQM
				.Setup(x => x.QueryAsync(It.IsAny<int>(), It.IsAny<QueryRequest>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(Task.FromResult(ObjectManagerResultSet(result)));

			return mockingOQM;
		}

		private static QueryResult ObjectManagerResultSet(object result)
		{
			return new QueryResult()
			{
				Objects = new System.Collections.Generic.List<RelativityObject>()
				{
					new RelativityObject()
					{
						ArtifactID = 1,
						FieldValues = new System.Collections.Generic.List<FieldValuePair>()
						{
							new FieldValuePair()
							{
								Field = new Field() { Name = "Name" },
								Value = result
							},
							new FieldValuePair()
							{
								Field = new Field() { Name = "Name" },
								Value = result
							},
							new FieldValuePair()
							{
								Field = new Field() { Name = "Name" },

								Value = result
							},
						}
					}
				}
			};
		}

	}
}
