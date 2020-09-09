using kCura.Relativity.Client;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.SQL;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Permission;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	public class PermissionsManagerTest : TestBase
	{
		private const int _WORKSPACE_ID = 10000;
		private const int _ARTIFACT_ID = 100007;
		private const int _ARTIFACT_TYPE_ID = 17;
		private const int _USER_ID = 777;
		private const string _OBJECT_GUID = "";
		private const string _PERMISSION_NAME = "";

		[OneTimeSetUp]
		public void Setup()
		{
			Mock<IRSAPIClient> rsapi = RSAPIClientMockHelper.GetMockedHelper();

			Mock<IHelper> mockingHelper = MockHelper.GetMockingHelper<IHelper>();


			DataTable dt = new DataTable();
			dt.Columns.Add("FileID", typeof(int));
			dt.Columns.Add("DocumentArtifactID", typeof(string));
			dt.Rows.Add(_ARTIFACT_TYPE_ID, _PERMISSION_NAME);

			mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsDbDataReaderWithSqlParametersArray(Queries.GetObjectTypeByGuid, dt.CreateDataReader())
				.MockExecuteSqlStatementAsScalar(Queries.IsUserAdministrator, true);


			Mock<IPermissionManager> mockPermissionManager = new Mock<IPermissionManager>();

			mockPermissionManager
				.Setup(p => p.GetPermissionSelectedAsync(It.IsAny<int>(), It.IsAny<List<PermissionRef>>()))
				.Returns(Task.FromResult(PermissionValues()));

			mockPermissionManager
				.Setup(p => p.QueryAsync(It.IsAny<int>(), It.IsAny<global::Relativity.Services.Query>()))
				.Returns(Task.FromResult(PermissionQueryResult()));

			Mock<IServicesMgr> mockingServicesMgr = mockingHelper
				.MockIServiceMgr()
				.MockService(rsapi)
				.MockService(mockPermissionManager);

			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		private PermissionQueryResultSet PermissionQueryResult()
		{
			PermissionQueryResultSet result = new PermissionQueryResultSet();

			result.Results = new List<global::Relativity.Services.Result<Permission>>
			{
				new global::Relativity.Services.Result<Permission>
				{
					Artifact = new Permission
					{
						Name = _PERMISSION_NAME
					}
				}
			};
			return result;
		}

		private List<PermissionValue> PermissionValues()
		{
			List<PermissionValue> permissionValues = new List<PermissionValue>
			{
				new PermissionValue{
					Name = _PERMISSION_NAME,
					Selected = true,
				}
			};

			return permissionValues;
		}


		[Test]
		public async Task CurrentUserHasPermissionToObjectTypeTest()
		{
			bool result = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectTypeAsync
				(_WORKSPACE_ID, _OBJECT_GUID, _PERMISSION_NAME);
			Assert.IsTrue(result);
		}

		[Test]
		public async Task PermissionCreateSingleTest()
		{
			bool result = await PermissionsManager.Instance.Permission_CreateSingleAsync(_PERMISSION_NAME, _ARTIFACT_TYPE_ID);
			Assert.IsTrue(result);

		}

		[Test]
		public async Task PermissionReadSelectedSingleTest()
		{
			bool result = await PermissionsManager.Instance.Permission_ReadSelectedSingleAsync(_WORKSPACE_ID, _ARTIFACT_TYPE_ID, _PERMISSION_NAME);
			Assert.IsTrue(result);
		}

		[Test]
		public async Task PermissionExistsTest()
		{
			bool result = await PermissionsManager.Instance.Permission_ExistAsync(_PERMISSION_NAME);
			Assert.IsTrue(result);
		}

		[Test]
		public void IsUserAdministratorTest()
		{
			bool result = PermissionsManager.Instance.IsUserAdministrator(_WORKSPACE_ID, _USER_ID);
			Assert.IsTrue(result);
		}
	}
}
