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
using FluentAssertions;
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
			
			_mockingHelper
				.MockIServiceMgr()
				.MockService(_objectManagerFake)
				.MockService<IErrorManager>()
				.MockService<IMetricsManager>()
				.MockService<IInternalMetricsCollectionManager>()
				.MockService<IDocumentManager>()
				.MockService(mockFileTypeIdentifier);

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

		[Test]
		public void ValidateDocImagesTest()
		{
			// Act
			bool result = DocumentManager.Instance.ValidateDocImages(TestsConstants._DOC_ARTIFACT_ID);
			Assert.IsTrue(result);
		}

		[Test]
		public void ValidateDocNativeTest()
		{
			// Act
			bool result = DocumentManager.Instance.ValidateDocNative(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void GetDocumentControlNumberTest()
		{
			// Act
			string result = DocumentManager.Instance.GetDocumentControlNumber(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			result.Should().Be(TestsConstants._DOC_CONTROL_NUMBER);
		}

		[Test]
		public void ValidateHasRedactionsTest()
		{
			// Act
			bool result = DocumentManager.Instance.ValidateHasRedactions(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void UpdateDocumentLastModificationFieldsTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.UpdateDocumentLastModificationFields(TestsConstants._DOC_ARTIFACT_ID, TestsConstants._USER_ID, true);
			
			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void GetFileByArtifactIdTest()
		{
			// Act
			FileInformation result = DocumentManager.Instance.GetFileByArtifactId(TestsConstants._DOC_ARTIFACT_ID);

			//Assert
			result.DocumentArtifactID.Should().Be(TestsConstants._DOC_ARTIFACT_ID);
			result.FileID.Should().Be(TestsConstants._DOC_FILE_ID);
		}

		[Test]
		public void DeleteRedactionsTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.DeleteRedactions(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void DeleteExistingImagesTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.DeleteExistingImages(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void InsertImageTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.InsertImage(new FileInformation());

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void UpdateHasImagesTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.UpdateHasImages(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			action.Should().NotThrow();
		}

		[TestCase(TestsConstants._DOC_CONTROL_NUMBER, TestsConstants._DOC_ARTIFACT_ID)]
		[TestCase("It'll return empty", -1)]
		public void GetDocByNameTest(string docName, int expectedArtifactID)
		{
			// Arrange
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
			// Act
			Action action = () => DocumentManager.Instance.SetCreateInstanceSettings();

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void RemovePageInteractionEvenHandlerFromDocumentObjectTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.RemovePageInteractionEvenHandlerFromDocumentObject();

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public async Task ValidateFileTypesTestAsync()
		{
			// Act
			bool result = await DocumentManager.Instance.ValidateFileTypesAsync(TestsConstants._FILE_TYPE);
			
			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public async Task IsDataGridEnabledTestAsync()
		{
			// Act
			bool result = await DocumentManager.Instance.IsDataGridEnabledAsync(-1);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void GetRepositoryLocationTest()
		{
			// Act
			string result = DocumentManager.Instance.GetRepositoryLocation();
			
			// Assert
			result.Should().Contain("TempTestFiles");
		}

		[Test]
		public void CreatedMetricsTestAsync()
		{
			// Act
			Action action = () => DocumentManager.Instance.CreateMetricsAsync(TestsConstants._EXP_METADATA, Core.Helpers.Constants.BUCKET_DOCUMENTSUPLOADED);
			
			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void WriteFileTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.WriteFile(System.IO.File.ReadAllBytes(TestsConstants._FILE_LOCATION), new FileInformation { FileLocation = TestsConstants._FILE_LOCATION });

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void InstanceFileTest()
		{
			// Act
			DocumentManager.Instance.InstanceFile(System.IO.File.ReadAllBytes(TestsConstants._FILE_LOCATION), null, TestsConstants._DOC_GUID.ToString());
			string path = $"{Directory.GetCurrentDirectory()}\\{TestsConstants._DOC_GUID}";
			
			// Assert
			Directory.Exists(path).Should().BeTrue();

			// Clean Up
			Directory.Delete(path, true);
		}

		[Test]
		public void ReplaceSingleDocumentTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.ReplaceSingleDocumentAsync(TestsConstants._EXP_METADATA, TestsConstants._DOC_EXTRA_INFO);
			
			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void GetNativeTypeByFilenameTest()
		{
			// Act
			var nativeType = DocumentManager.Instance.GetNativeTypeByFilename(TestsConstants._FILE_LOCATION);
			
			// Assert
			nativeType.Description.Should().Be(TestsConstants._OI_FILE_TYPE_);
		}

		[Test]
		public void DeleteTempFileTest()
		{
			// Act
			Action action = () => DocumentManager.Instance.DeleteTempFile(FileHelper.GetTempFolderLocation());

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void DeleteTempFileTestWhenException()
		{
			// Act
			Action action = () => DocumentManager.Instance.DeleteTempFile("");

			// Assert
			action.Should().NotThrow();
		}

		private Mock<IObjectManager> PrepareObjectManager()
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
	}
}
