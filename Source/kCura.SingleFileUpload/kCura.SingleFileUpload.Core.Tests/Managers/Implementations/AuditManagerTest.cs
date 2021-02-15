using System;
using FluentAssertions;
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
	public class AuditManagerTest : TestBase
	{
		[SetUp]
		public void Setup()
		{
			Mock<IHelper> mockingHelper = new Mock<IHelper>();
			mockingHelper.MockIDBContextOnHelper();
			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public void CreateAuditRecordTest()
		{
			// Arrange
			string details = "TestDetails";

			// Act
			Action action = () => AuditManager.instance.CreateAuditRecord(
				TestsConstants._WORKSPACE_ID, TestsConstants._DOC_ARTIFACT_ID, AuditAction.Update, details, TestsConstants._USER_ID);
			
			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void GenerateAuditDetailsForFileUploadTest()
		{
			// Arrange
			const int fileId = TestsConstants._DOC_ARTIFACT_ID;
			const string message = "Test message";

			// Act
			string result = AuditManager.instance.GenerateAuditDetailsForFileUpload(TestsConstants._FILE_LOCATION, fileId, message);
			
			// Assert
			result.Should()
				.Contain(fileId.ToString())
				.And
				.Contain(message);
		}
	}
}
