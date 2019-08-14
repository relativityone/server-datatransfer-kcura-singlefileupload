using kCura.Relativity.Client;
using kCura.SingleFileUpload.Core.Entities.Enumerations;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.Tests.Constants;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	public class AuditManagerTests : TestBase
	{

		[OneTimeSetUp]
		public void Setup()
		{
			Mock<IRSAPIClient> rsapi = RSAPIClientMockHelper.GetMockedHelper();
			Mock<IHelper> mockingHelper;
			mockingHelper = MockHelper
					.GetMockingHelper<IHelper>();

			mockingHelper
				.MockIDBContextOnHelper();

			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public void CreateAuditRecordTest()
		{
			string details = "TestDetails";
			AuditManager.instance.CreateAuditRecord(TestsConstants._WORKSPACE_ID, TestsConstants._DOC_ARTIFACT_ID, AuditAction.Update, details, TestsConstants._USER_ID);
			Assert.IsTrue(true);
		}

		[Test]
		public void GenerateAuditDetailsForFileUploadTest()
		{
			int fileId = TestsConstants._DOC_ARTIFACT_ID;
			string message = "Test message";
			string result = AuditManager.instance.GenerateAuditDetailsForFileUpload(TestsConstants._FILE_LOCATION, fileId, message);
			Assert.IsTrue(result.Contains(fileId.ToString()));
			Assert.IsTrue(result.Contains(message));
		}



	}
}
