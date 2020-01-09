﻿using kCura.Relativity.Client;
using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.SQL;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Objects;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
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

		[OneTimeSetUp]
		public void Setup()
		{
			Mock<IRSAPIClient> rsapi = RSAPIClientMockHelper.GetMockedHelper();

			Mock<IHelper> mockingHelper = MockHelper.GetMockingHelper<IHelper>();

			var objectManager = new Mock<IObjectManager>();



			mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsScalar(Queries.GetArtifactTypeByArtifactGuid, _ARTIFACT_ID);


			Mock<IServicesMgr> mockingServicesMgr = mockingHelper
				.MockIServiceMgr()
				.MockService(rsapi)
				.MockService(objectManager.Mock("test"));

			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public void GetErrorInfoTest()
		{
			ProcessingDocument result = ProcessingManager.instance.GetErrorInfo(_ERROR_ID);
			Assert.AreEqual(result.ErrorID, _ERROR_ID);
		}

		[Test]
		public void ReplaceFile()
		{
			ProcessingManager.instance.ReplaceFile(new byte[1080], _processingDocument);
			Assert.IsTrue(true);
		}
	}
}