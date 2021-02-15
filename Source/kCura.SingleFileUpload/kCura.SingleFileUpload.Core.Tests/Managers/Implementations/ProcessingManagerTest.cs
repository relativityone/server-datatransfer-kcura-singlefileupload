using System;
using FluentAssertions;
using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.SQL;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Objects;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
	public class ProcessingManagerTest : TestBase
	{
		private const int _ERROR_ID = 10;
		private const int _ARTIFACT_ID = 100000;
		private const string _FILE_NAME = "CTRL0192153.txt";
		private readonly ProcessingDocument _processingDocument = new ProcessingDocument
		{
			ErrorID = _ERROR_ID,
			DocumentIdentifier = "",
			DocumentFileLocation = FileHelper.GetFileLocation(_FILE_NAME),
			SourceLocation = ""
		};

		[SetUp]
		public void Setup()
		{
			Mock<IHelper> mockingHelper = new Mock<IHelper>();

			var objectManager = new Mock<IObjectManager>();

			mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsScalar(Queries.GetArtifactTypeByArtifactGuid, _ARTIFACT_ID);

			mockingHelper
				.MockIServiceMgr()
				.MockService(objectManager.Mock("test"));

			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public void GetErrorInfo_ShouldReturnErrorInfo()
		{
			// Act
			ProcessingDocument result = ProcessingManager.instance.GetErrorInfo(_ERROR_ID);
			
			// Assert
			result.ErrorID.Should().Be(_ERROR_ID);
		}

		[Test]
		public void ReplaceFile_ShouldNotThrow()
		{
			// Act
			Action action = () => ProcessingManager.instance.ReplaceFile(new byte[1080], _processingDocument);
			
			// Assert
			action.Should().NotThrow();
		}
	}
}
