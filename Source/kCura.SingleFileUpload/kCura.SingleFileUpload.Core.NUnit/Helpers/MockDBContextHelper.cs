using Moq;
using Relativity.API;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace kCura.SingleFileUpload.Core.NUnit.Helpers
{
	public static class MockDBContextHelper
	{
		public static Mock<IDBContext> GetMockedDBContext()
		{
			return new Mock<IDBContext>();
		}

		public static Mock<IDBContext> MockIDBContextOnHelper(this Mock<IHelper> mockingHelper, int workspaceID = -1)
		{
			Mock<IDBContext> mockingDbContext = new Mock<IDBContext>();
			mockingHelper
				.Setup(p => p.GetDBContext(It.IsAny<int>()))
				.Returns(mockingDbContext.Object);

			mockingDbContext.DefaultValue = DefaultValue.Mock;
			mockingDbContext.SetReturnsDefault<int>(MockHelper.DEFAULT_ID);
			return mockingDbContext;
		}
		public static Mock<IDBContext> MockIDBContextOnHelper(this Mock<IEHHelper> mockingHelper, int workspaceID = -1)
		{
			Mock<IDBContext> mockingDbContext = new Mock<IDBContext>();
			mockingHelper
				.Setup(p => p.GetDBContext(It.IsAny<int>()))
				.Returns(mockingDbContext.Object);



			mockingDbContext.DefaultValue = DefaultValue.Mock;
			mockingDbContext.SetReturnsDefault<int>(MockHelper.DEFAULT_ID);
			return mockingDbContext;
		}

		public static Mock<IDBContext> MockExecuteSqlStatementAsDbDataReaderWithSqlParametersArray(this Mock<IDBContext> mockingDbContext, string sql, DbDataReader resultDataReader)
		{
			mockingDbContext
				.Setup(p => p.ExecuteSqlStatementAsDbDataReader(sql, It.IsAny<SqlParameter[]>()))
				.Returns(resultDataReader);

			return mockingDbContext;
		}

		public static Mock<IDBContext> MockExecuteSqlStatementAsScalar(this Mock<IDBContext> mockingDbContext, string sql, object resultObject)
		{
			mockingDbContext
				.Setup(p => p.ExecuteSqlStatementAsScalar(sql, It.IsAny<SqlParameter[]>()))
				.Returns(resultObject);

			return mockingDbContext;
		}

		public static Mock<IDBContext> MockExecuteSqlStatementAsDataTable(this Mock<IDBContext> mockingDbContext, string sql, DataTable resultObject)
		{
			mockingDbContext
				.Setup(p => p.ExecuteSqlStatementAsDataTable(sql))
				.Returns(resultObject);

			return mockingDbContext;
		}
	}
}
