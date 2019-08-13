using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Relativity.ImportAPI;
using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Factories;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.SQL;
using kCura.SingleFileUpload.Core.Tests.Constants;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.DataExchange.Io;
using Relativity.Services.Document;
using Relativity.Services.InternalMetricsCollection;
using Relativity.Services.ObjectQuery;
using Relativity.Telemetry.Services.Metrics;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	public class DocumentManagerTest : TestBase
	{


		private Mock<IHelper> mockingHelper;
		private readonly string tempFilePath = FileHelper.GetTempDirectoryPath();

		#region SetUp

		[OneTimeSetUp]
		public void Setup()
		{


			Mock<IRSAPIClient> rsapi = RSAPIClientMockHelper.GetMockedHelper();

			rsapi.Setup(p => p.Read(It.IsAny<APIOptions>(), It.IsAny<List<ArtifactRequest>>()))
				.Returns<APIOptions, List<ArtifactRequest>>((apiOptions, artifactRequests) =>
				{
					if (artifactRequests.All(p => p.ArtifactTypeID == (int)ArtifactType.Document))
					{
						return new ReadResultSet()
						{
							Success = true,
							ReadResults = new List<ReadResult>
							{
								new ReadResult
								{
									Success = true,
									Artifact = new Relativity.Client.Artifact
									{
										Fields = new List<Relativity.Client.Field>
										{
											(new Relativity.Client.Field(Guid.Empty, TestsConstants._DOC_CONTROL_NUMBER)),
											(new Relativity.Client.Field(DocumentFieldNames.HasImages, new Relativity.Client.Choice(){ Name = "Yes" })),
											(new Relativity.Client.Field(DocumentFieldNames.HasNative, true)),
											(new Relativity.Client.Field(DocumentFieldNames.TextIdentifier, TestsConstants._DOC_NAME)),
											(new Relativity.Client.Field("A Custom No Named and Sad Field", true))
										}
									}
								}

							}
						};
					}
					else
					{
						throw new NotImplementedException(string.Format("Type {0} hasn't been mocked for RSAPI Repositories.", artifactRequests.FirstOrDefault()?.ArtifactTypeID));
					}
				});

			rsapi.Setup(p => p.Query(It.IsAny<APIOptions>(), It.IsAny<Relativity.Client.Query>(), It.IsAny<int>()))
				.Returns<APIOptions, Relativity.Client.Query, int>((apiOptions, query, index) =>
				{
					QueryResult queryResult = null;
					if (query.ArtifactTypeID == (int)ArtifactType.Document)
					{
						queryResult = new QueryResult()
						{
							Success = true
						};
						if (query.Condition is TextCondition && ((TextCondition)query.Condition).Value == TestsConstants._DOC_NAME)
						{
							queryResult.QueryArtifacts.Add(new Relativity.Client.Artifact(apiOptions.WorkspaceID, (int)ArtifactType.Document, TestsConstants._DOC_ARTIFACT_ID));
						}
					}
					return queryResult;
				});



			mockingHelper = MockHelper
				.GetMockingHelper<IHelper>();

			Mock<IAPILog> mockApiLog = new Mock<IAPILog>();
			mockApiLog.Setup(p => p.LogError(It.IsAny<string>()));
			mockApiLog.Setup(p => p.ForContext<BaseManager>())
				.Returns(mockApiLog.Object);

			Mock<ILogFactory> mockLogFactory = new Mock<ILogFactory>();
			ILogFactory logFactory = mockLogFactory.Object;


			mockLogFactory.Setup(p => p.GetLogger())
				.Returns(mockApiLog.Object);

			mockingHelper.Setup(p => p.GetLoggerFactory())
				.Returns(mockLogFactory.Object);


			DataTable dt = new DataTable();
			dt.Columns.Add("FileID", typeof(int));
			dt.Columns.Add("DocumentArtifactID", typeof(int));
			dt.Columns.Add("FileName", typeof(string));
			dt.Columns.Add("Location", typeof(string));
			dt.Rows.Add(TestsConstants._DOC_FILE_ID, TestsConstants._DOC_ARTIFACT_ID, TestsConstants._FILE_NAME, TestsConstants._FILE_LOCATION);

			mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsDbDataReaderWithSqlParametersArray(Queries.GetFileInfoByDocumentArtifactID, dt.CreateDataReader())
				.MockExecuteSqlStatementAsScalar(Queries.GetRepoLocationByCaseID, TestsConstants._FILE_LOCATION)
				.MockExecuteSqlStatementAsScalar(Queries.GetFieldsInstanceSetting, TestsConstants._JSON_RESULT)
				.MockExecuteSqlStatementAsDataTable(Queries.GetFileInfoByDocumentArtifactID, dt)
				.MockExecuteSqlStatementAsScalar(Queries.GetWorkspaceGuidByArtifactID, Guid.NewGuid().ToString());

			Mock<IFileTypeIdentifier> mockFileTypeIdentifier = new Mock<IFileTypeIdentifier>();

			Mock<IFileTypeInfo> mockFileTypeInfo = new Mock<IFileTypeInfo>();

			mockFileTypeInfo.Setup(p => p.Description).Returns("Test");

			mockFileTypeIdentifier.Setup(p => p.Identify(It.IsAny<string>())).Returns(mockFileTypeInfo.Object);

			Mock<IProvideSystemTokens> mockingProvideSystemToken = new Mock<IProvideSystemTokens>();

			mockingProvideSystemToken.Setup(p => p.GetLocalSystemToken()).Returns(Guid.NewGuid().ToString());

			ExtensionPointServiceFinder.SystemTokenProvider = mockingProvideSystemToken.Object;



			Mock<IServicesMgr> mockingServicesMgr = mockingHelper
				.MockIServiceMgr()
				.MockService(rsapi)
				.MockService<IMetricsManager>()
				.MockService<IInternalMetricsCollectionManager>()
				.MockService<IDocumentManager>()
				.MockService(mockFileTypeIdentifier)
				;


			Mock<IImportApiFactory> mockImportApi = new Mock<IImportApiFactory>();
			Mock<IExtendedImportAPI> mockExtendedIApi = new Mock<IExtendedImportAPI>();

			mockImportApi.Setup(p => p.GetImportAPI(It.IsAny<ImportSettings>())).Returns<ExtendedImportAPI>((rs) =>
			{
				return mockExtendedIApi.Object;

			});

			mockingServicesMgr.MockServiceInstance<IObjectQueryManager>()
				.Mock(TestsConstants._FILE_TYPE)
				.Mock(true);



			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		#endregion

		#region TearDown
		[OneTimeTearDown]
		public void TearDown()
		{


			if (System.IO.File.Exists(tempFilePath))
			{
				string tempDirectory = Path.GetDirectoryName(tempFilePath);
				Directory.Delete(tempDirectory, true);
			}

		}
		#endregion

		[Test]
		public void IsFileTypeSupportedTest()
		{
			bool result = DocumentManager.instance.IsFileTypeSupported(TestsConstants._FILE_TYPE);
			Assert.IsTrue(result);
		}

		[Test]
		public void ValidateDocImagesTest()
		{
			bool result = DocumentManager.instance.ValidateDocImages(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(result);
		}

		[Test]
		public void ValidateDocNativeTest()
		{
			bool result = DocumentManager.instance.ValidateDocNative(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(result);
		}
		[Test]
		public void GetDocumentControlNumberTest()
		{
			string result = DocumentManager.instance.GetDocumentControlNumber(TestsConstants._DOC_ARTIFACT_ID);
			Assert.AreEqual(TestsConstants._DOC_CONTROL_NUMBER, result);
		}

		[Test]
		public void ValidateHasRedactionsTest()
		{
			bool result = DocumentManager.instance.ValidateHasRedactions(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(result);
		}

		[Test]
		public void UpdateDocumentLastModificationFieldsTest()
		{
			DocumentManager.instance.UpdateDocumentLastModificationFields(TestsConstants._DOC_ARTIFACT_ID, TestsConstants._USER_ID, true);
			Assert.IsTrue(true);
		}

		[Test]
		public void GetFileByArtifactIdTest()
		{
			FileInformation result = DocumentManager.instance.GetFileByArtifactId(TestsConstants._DOC_ARTIFACT_ID);

			Assert.AreEqual(TestsConstants._DOC_ARTIFACT_ID, result.DocumentArtifactID, "document id is not the same");
			Assert.AreEqual(TestsConstants._DOC_FILE_ID, result.FileID, "file id is not the same");
		}

		[Test]
		public void DeleteRedactionsTest()
		{
			DocumentManager.instance.DeleteRedactions(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(true);
		}

		[Test]
		public void DeleteExistingImagesTest()
		{
			DocumentManager.instance.DeleteExistingImages(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(true);
		}

		[Test]
		public void InsertImageTest()
		{
			DocumentManager.instance.InsertImage(new FileInformation());
			Assert.IsTrue(true);
		}

		[Test]
		public void UpdateHasImagesTest()
		{
			DocumentManager.instance.UpdateHasImages(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(true);
		}

		[TestCase(TestsConstants._DOC_NAME, TestsConstants._DOC_ARTIFACT_ID)]
		[TestCase("It'll return empty", -1)]
		public void GetDocByNameTest(string docName, int expectedArtifactID)
		{
			int result = DocumentManager.instance.GetDocByName(docName);
			Assert.AreEqual(result, expectedArtifactID);

		}

		[Test]
		public void SetCreateInstanceSettingsTest()
		{
			DocumentManager.instance.SetCreateInstanceSettings();
			Assert.IsTrue(true);
		}

		[Test]
		public void RemovePageInteractionEvenHandlerFromDocumentObjectTest()
		{
			DocumentManager.instance.RemovePageInteractionEvenHandlerFromDocumentObject();
			Assert.IsTrue(true);

		}

		[Test]
		public async Task ValidateFileTypesTestAsync()
		{
			bool result = await DocumentManager.instance.ValidateFileTypes(TestsConstants._FILE_TYPE);
			Assert.IsTrue(result);
		}

		[Test]
		public async Task IsDataGridEnabledTestAsync()
		{
			bool result = await DocumentManager.instance.IsDataGridEnabled(-1);
			Assert.IsTrue(result);
		}

		[Test]
		public void GetRepositoryLocationTest()
		{
			string result = DocumentManager.instance.GetRepositoryLocation();
			Assert.AreEqual(!TestsConstants._FILE_LOCATION.EndsWith("\\") ? string.Concat(TestsConstants._FILE_LOCATION, "\\") : TestsConstants._FILE_LOCATION, result);
		}

		[Test]
		public async Task CreatedMetricsTestAsync()
		{
			await DocumentManager.instance.CreateMetricsAsync(TestsConstants._EXP_METADATA, Core.Helpers.Constants.BUCKET_DOCUMENTSUPLOADED);
			Assert.IsTrue(true);
		}

		[Test]
		public void WriteFileTest()
		{
			DocumentManager.instance.WriteFile(System.IO.File.ReadAllBytes(TestsConstants._FILE_LOCATION), new FileInformation { FileLocation = TestsConstants._FILE_LOCATION });
			Assert.IsTrue(true);
		}

		[Test]
		public void InstanceFileTest()
		{
			bool assert = false;
			string result = DocumentManager.instance.InstanceFile(TestsConstants._FILE_NAME, System.IO.File.ReadAllBytes(TestsConstants._FILE_LOCATION), true, TestsConstants._DOC_GUID.ToString());
			string path = $"{Directory.GetCurrentDirectory()}\\{TestsConstants._DOC_GUID.ToString()}";

			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
				assert = true;
			}

			Assert.IsTrue(assert);
		}

		[Test]
		public async Task ReplaceSingleDocumentTest()
		{
			mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsScalar(Queries.GetRepoLocationByCaseID, TestsConstants._FILE_LOCATION_UPDATE_NATIVE);

			await DocumentManager.instance.ReplaceSingleDocument(TestsConstants._EXP_METADATA, TestsConstants._DOC_EXTRA_INFO);
			Assert.IsTrue(true);
		}

		[Test]
		public void GetNativeTypeByFilenameTest()
		{

			var nativeType = DocumentManager.instance.GetNativeTypeByFilename(TestsConstants._FILE_LOCATION);
			Assert.AreEqual(TestsConstants._OI_FILE_TYPE_, nativeType.Description);
		}

		[Test]
		public void DeleteTempFileTest()
		{
			DocumentManager.instance.DeleteTempFile(tempFilePath);
			Assert.IsTrue(true);
		}

		[Test]
		public void DeleteTempFileTestWhenException()
		{
			string emptyPath = "";
			DocumentManager.instance.DeleteTempFile(emptyPath);
			Assert.IsTrue(true);
		}
	}
}
