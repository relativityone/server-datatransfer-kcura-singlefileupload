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
using FluentAssertions;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
	public class PermissionsManagerTest : TestBase
	{
		private const int _WORKSPACE_ID = 10000;
		private const int _ARTIFACT_TYPE_ID = 17;
		private const int _USER_ID = 777;
		private const string _OBJECT_GUID = "";
		private const string _PERMISSION_NAME = "";

		[SetUp]
		public void Setup()
		{
			Mock<IHelper> mockingHelper = new Mock<IHelper>();

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

			mockingHelper
				.MockIServiceMgr()
				.MockService(mockPermissionManager);

			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public async Task CurrentUserHasPermissionToObjectTypeAsync_ShouldCheckPermissionsToObjectType()
		{
			// Act
			bool result = await PermissionsManager.Instance.CurrentUserHasPermissionToObjectTypeAsync
				(_WORKSPACE_ID, _OBJECT_GUID, _PERMISSION_NAME);
			
			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public async Task PermissionCreateSingleAsync_ShouldCheckPermissionsToCreateSingleObject()
		{
			// Act
			bool result = await PermissionsManager.Instance.Permission_CreateSingleAsync(_PERMISSION_NAME, _ARTIFACT_TYPE_ID);
			
			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public async Task PermissionReadSelectedSingleAsync__ShouldCheckPermissionsToReadSingleObject()
		{
			// Act
			bool result = await PermissionsManager.Instance.Permission_ReadSelectedSingleAsync(_WORKSPACE_ID, _ARTIFACT_TYPE_ID, _PERMISSION_NAME);
			
			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public async Task PermissionExistsAsync_ShouldCheckPermissionExistence()
		{
			// Act
			bool result = await PermissionsManager.Instance.Permission_ExistAsync(_PERMISSION_NAME);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void IsUserAdministrator_ShouldCheckIfUserIsAdmin()
		{
			// Act
			bool result = PermissionsManager.Instance.IsUserAdministrator(_WORKSPACE_ID, _USER_ID);
			
			// Assert
			result.Should().BeTrue();
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
	}
}
