using kCura.Relativity.DataReaderClient;
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
using Relativity.Telemetry.Services.Metrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Relativity.Services.Error;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	public class DocumentManagerTest : TestBase
	{
		private Mock<IHelper> _mockingHelper;
		private Mock<IObjectManager> _objectManagerFake;

		#region SetUp

		[SetUp]
		public void Setup()
		{
			_mockingHelper = new Mock<IHelper>();
			_objectManagerFake = PrepareObjectManager();

			Mock<IAPILog> mockApiLog = new Mock<IAPILog>();
			mockApiLog.Setup(p => p.LogError(It.IsAny<string>()));
			mockApiLog.Setup(p => p.ForContext<BaseManager>())
				.Returns(mockApiLog.Object);

			Mock<ILogFactory> mockLogFactory = new Mock<ILogFactory>();

			mockLogFactory.Setup(p => p.GetLogger())
				.Returns(mockApiLog.Object);

			_mockingHelper.Setup(p => p.GetLoggerFactory())
				.Returns(mockLogFactory.Object);

			_mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsDataTableWithSqlParametersArray(Queries.GetFileInfoByDocumentArtifactID, TestsConstants._GetdataTable())
				.MockExecuteSqlStatementAsScalar(Queries.GetRepoLocationByCaseID, TestsConstants._TEMP_FOLDER_LOCATION)
				.MockExecuteSqlStatementAsScalar(Queries.GetFieldsInstanceSetting, TestsConstants._JSON_RESULT)
				.MockExecuteSqlStatementAsScalar(Queries.GetWorkspaceGuidByArtifactID, Guid.NewGuid().ToString());

			Mock<IFileTypeIdentifier> mockFileTypeIdentifier = new Mock<IFileTypeIdentifier>();

			Mock<IFileTypeInfo> mockFileTypeInfo = new Mock<IFileTypeInfo>();

			mockFileTypeInfo.Setup(p => p.Description).Returns("Test");

			mockFileTypeIdentifier.Setup(p => p.Identify(It.IsAny<string>())).Returns(mockFileTypeInfo.Object);

			Mock<IProvideSystemTokens> mockingProvideSystemToken = new Mock<IProvideSystemTokens>();

			mockingProvideSystemToken.Setup(p => p.GetLocalSystemToken()).Returns(Guid.NewGuid().ToString());

			ExtensionPointServiceFinder.SystemTokenProvider = mockingProvideSystemToken.Object;
			
			Mock<IServicesMgr> mockingServicesMgr = _mockingHelper
				.MockIServiceMgr()
				.MockService(_objectManagerFake)
				.MockService<IErrorManager>()
				.MockService<IMetricsManager>()
				.MockService<IInternalMetricsCollectionManager>()
				.MockService<IDocumentManager>()
				.MockService(mockFileTypeIdentifier)
				;

			Mock<IImportApiFactory> mockImportApi = new Mock<IImportApiFactory>();
			Mock<IExtendedImportAPI> mockExtendedIApi = new Mock<IExtendedImportAPI>();

			mockImportApi.Setup(p => 
				p.GetImportAPI(It.IsAny<ImportSettings>())).Returns<ExtendedImportAPI>((rs) => mockExtendedIApi.Object);

			Mock<IImportAPI> mockingImportApi = new Mock<IImportAPI>
			{
				DefaultValue = DefaultValue.Mock
			};
			mockingImportApi.SetReturnsDefault(true);

			Mock<IImportBulkArtifactJob> mockingImportBulkArtifactJob = new Mock<IImportBulkArtifactJob>
			{
				DefaultValue = DefaultValue.Mock
			};
			mockingImportBulkArtifactJob.SetReturnsDefault(true);

			ImportApiFactory.SetUpSingleton(mockingImportApi.Object, mockingImportBulkArtifactJob.Object);

			ConfigureSingletoneRepositoryScope(_mockingHelper.Object);
		}

		Mock<IObjectManager> PrepareObjectManager()
		{
			Mock<IObjectManager> objectManagerFake = new Mock<IObjectManager>();

			Dictionary<string, FieldValuePair> supportedFields = new Dictionary<string, FieldValuePair>();

			void AddSupportedField(string fieldName, object value)
			{
				supportedFields.Add(fieldName, new FieldValuePair()
				{
					Field = new Field()
					{
						Name = fieldName
					},
					Value = value
				});
			}

			AddSupportedField("Has Images", new Choice() { Name = "Yes" });
			AddSupportedField("Has Native", true);
			AddSupportedField("Enable Data Grid", true);
			AddSupportedField("Value", TestsConstants._FILE_TYPE);

			objectManagerFake
				.Setup(x => x.QueryAsync(It.IsAny<int>(), It.IsAny<QueryRequest>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((int workspaceID, QueryRequest request, int start, int length) =>
					new QueryResult()
					{
						Objects = new List<RelativityObject>()
						{
							new RelativityObject()
							{
								ArtifactID = TestsConstants._DOC_ARTIFACT_ID,
								Name = TestsConstants._DOC_CONTROL_NUMBER,
								FieldValues = request.Fields.Select(field => supportedFields[field.Name]).ToList()
							}
						}
					});

			return objectManagerFake;
		}

		#endregion
		
		[Test]
		public void ValidateDocImagesTest()
		{
			bool result = DocumentManager.Instance.ValidateDocImages(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(result);
		}

		[Test]
		public void ValidateDocNativeTest()
		{
			bool result = DocumentManager.Instance.ValidateDocNative(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(result);
		}

		[Test]
		public void GetDocumentControlNumberTest()
		{
			string result = DocumentManager.Instance.GetDocumentControlNumber(TestsConstants._DOC_ARTIFACT_ID);
			Assert.AreEqual(TestsConstants._DOC_CONTROL_NUMBER, result);
		}

		[Test]
		public void ValidateHasRedactionsTest()
		{
			bool result = DocumentManager.Instance.ValidateHasRedactions(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(result);
		}

		[Test]
		public void UpdateDocumentLastModificationFieldsTest()
		{
			DocumentManager.Instance.UpdateDocumentLastModificationFields(TestsConstants._DOC_ARTIFACT_ID, TestsConstants._USER_ID, true);
			Assert.IsTrue(true);
		}

		[Test]
		public void GetFileByArtifactIdTest()
		{
			FileInformation result = DocumentManager.Instance.GetFileByArtifactId(TestsConstants._DOC_ARTIFACT_ID);

			Assert.AreEqual(TestsConstants._DOC_ARTIFACT_ID, result.DocumentArtifactID, "document id is not the same");
			Assert.AreEqual(TestsConstants._DOC_FILE_ID, result.FileID, "file id is not the same");
		}

		[Test]
		public void DeleteRedactionsTest()
		{
			DocumentManager.Instance.DeleteRedactions(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(true);
		}

		[Test]
		public void DeleteExistingImagesTest()
		{
			DocumentManager.Instance.DeleteExistingImages(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(true);
		}

		[Test]
		public void InsertImageTest()
		{
			DocumentManager.Instance.InsertImage(new FileInformation());
			Assert.IsTrue(true);
		}

		[Test]
		public void UpdateHasImagesTest()
		{
			DocumentManager.Instance.UpdateHasImages(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(true);
		}

		[TestCase(TestsConstants._DOC_CONTROL_NUMBER, TestsConstants._DOC_ARTIFACT_ID)]
		[TestCase("It'll return empty", -1)]
		public void GetDocByNameTest(string docName, int expectedArtifactID)
		{
			//Arrange
			_objectManagerFake
				.Setup(x => x.QueryAsync(It.IsAny<int>(), It.IsAny<QueryRequest>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync((int workspaceID, QueryRequest request, int start, int length) =>
				{
					QueryResult queryResult = new QueryResult();
					if (request.Condition.Contains(TestsConstants._DOC_CONTROL_NUMBER))
					{
						queryResult.Objects.Add(new RelativityObject()
						{
							ArtifactID = TestsConstants._DOC_ARTIFACT_ID
						});
					}
					return queryResult;
				});

			// Act
			int actualArtifactID = DocumentManager.Instance.GetDocByName(docName);

			// Assert
			Assert.AreEqual(expectedArtifactID, actualArtifactID);
		}

		[Test]
		public void SetCreateInstanceSettingsTest()
		{
			DocumentManager.Instance.SetCreateInstanceSettings();
			Assert.IsTrue(true);
		}

		[Test]
		public void RemovePageInteractionEvenHandlerFromDocumentObjectTest()
		{
			DocumentManager.Instance.RemovePageInteractionEvenHandlerFromDocumentObject();
			Assert.IsTrue(true);

		}

		[Test]
		public async Task ValidateFileTypesTestAsync()
		{
			bool result = await DocumentManager.Instance.ValidateFileTypesAsync(TestsConstants._FILE_TYPE);
			Assert.IsTrue(result);
		}

		[Test]
		public async Task IsDataGridEnabledTestAsync()
		{

			bool result = await DocumentManager.Instance.IsDataGridEnabledAsync(-1);
			Assert.IsTrue(result);
		}

		[Test]
		public void GetRepositoryLocationTest()
		{
			string result = DocumentManager.Instance.GetRepositoryLocation();
			Assert.IsTrue(result.Contains("TempTestFiles"));
		}

		[Test]
		public async Task CreatedMetricsTestAsync()
		{
			await DocumentManager.Instance.CreateMetricsAsync(TestsConstants._EXP_METADATA, Core.Helpers.Constants.BUCKET_DOCUMENTSUPLOADED);
			Assert.IsTrue(true);
		}

		[Test]
		public void WriteFileTest()
		{
			DocumentManager.Instance.WriteFile(System.IO.File.ReadAllBytes(TestsConstants._FILE_LOCATION), new FileInformation { FileLocation = TestsConstants._FILE_LOCATION });
			Assert.IsTrue(true);
		}

		[Test]
		public void InstanceFileTest()
		{
			bool assert = false;
			string result = DocumentManager.Instance.InstanceFile(System.IO.File.ReadAllBytes(TestsConstants._FILE_LOCATION), null, TestsConstants._DOC_GUID.ToString());
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
			await DocumentManager.Instance.ReplaceSingleDocumentAsync(TestsConstants._EXP_METADATA, TestsConstants._DOC_EXTRA_INFO);
			Assert.IsTrue(true);
		}

		[Test]
		public void GetNativeTypeByFilenameTest()
		{

			var nativeType = DocumentManager.Instance.GetNativeTypeByFilename(TestsConstants._FILE_LOCATION);
			Assert.AreEqual(TestsConstants._OI_FILE_TYPE_, nativeType.Description);
		}

		[Test]
		public void DeleteTempFileTest()
		{
			DocumentManager.Instance.DeleteTempFile(FileHelper.GetTempFolderLocation());
			Assert.IsTrue(true);
		}

		[Test]
		public void DeleteTempFileTestWhenException()
		{
			string emptyPath = "";
			DocumentManager.Instance.DeleteTempFile(emptyPath);
			Assert.IsTrue(true);
		}
	}
}
