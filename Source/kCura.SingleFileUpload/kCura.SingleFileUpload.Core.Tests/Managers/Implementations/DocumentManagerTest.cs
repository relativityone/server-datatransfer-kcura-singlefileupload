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
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Relativity.Services.Error;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
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
		public void ValidateDocImages_ShouldValidateImages()
		{
			// Act
			bool result = DocumentManager.Instance.ValidateDocImages(TestsConstants._DOC_ARTIFACT_ID);
			
			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void ValidateDocNative_ShouldValidateNatives()
		{
			// Act
			bool result = DocumentManager.Instance.ValidateDocNative(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void GetDocumentControlNumber_ShouldReturnExpectedControlNumber()
		{
			// Act
			string result = DocumentManager.Instance.GetDocumentControlNumber(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			result.Should().Be(TestsConstants._DOC_CONTROL_NUMBER);
		}

		[Test]
		public void ValidateHasRedactions_ShouldValidateRedactionsExistence()
		{
			// Act
			bool result = DocumentManager.Instance.ValidateHasRedactions(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void UpdateDocumentLastModificationFields_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.UpdateDocumentLastModificationFields(TestsConstants._DOC_ARTIFACT_ID, TestsConstants._USER_ID, true);
			
			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void GetFileByArtifactId_ShouldReturnDocumentArtifactIdAndFileId()
		{
			// Act
			FileInformation result = DocumentManager.Instance.GetFileByArtifactId(TestsConstants._DOC_ARTIFACT_ID);

			//Assert
			result.DocumentArtifactID.Should().Be(TestsConstants._DOC_ARTIFACT_ID);
			result.FileID.Should().Be(TestsConstants._DOC_FILE_ID);
		}

		[Test]
		public void DeleteRedactions_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.DeleteRedactions(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void DeleteExistingImagesShouldRemoveExistingImagesFromDocument()
		{
			// Act
			Action action = () => DocumentManager.Instance.DeleteExistingImages(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void InsertImage_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.InsertImage(new FileInformation());

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void UpdateHasImages_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.UpdateHasImages(TestsConstants._DOC_ARTIFACT_ID);

			// Assert
			action.Should().NotThrow();
		}

		[TestCase(TestsConstants._DOC_CONTROL_NUMBER, TestsConstants._DOC_ARTIFACT_ID)]
		[TestCase("It'll return empty", -1)]
		public void GetDocByName_ShouldReturnDocArtifactId_OrMinusOneIfNotFound(string docName, int expectedArtifactID)
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
			actualArtifactID.Should().Be(expectedArtifactID);
		}

		[Test]
		public void SetCreateInstanceSettings_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.SetCreateInstanceSettings();

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void RemovePageInteractionEvenHandlerFromDocumentObject_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.RemovePageInteractionEvenHandlerFromDocumentObject();

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public async Task ValidateFileTypesAsync_ShouldValidateFileType()
		{
			// Act
			bool result = await DocumentManager.Instance.ValidateFileTypesAsync(TestsConstants._FILE_TYPE);
			
			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public async Task IsDataGridEnabled_ShouldCheckIfDataGridIsEnabled()
		{
			// Act
			bool result = await DocumentManager.Instance.IsDataGridEnabledAsync(-1);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void GetRepositoryLocation_ShouldReturnRepositoryLocation()
		{
			// Act
			string result = DocumentManager.Instance.GetRepositoryLocation();
			
			// Assert
			result.Should().Contain("TempTestFiles");
		}

		[Test]
		public void CreateMetricsAsync_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.CreateMetricsAsync(TestsConstants._EXP_METADATA, Core.Helpers.Constants.BUCKET_DOCUMENTSUPLOADED);
			
			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void WriteFile__ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.WriteFile(System.IO.File.ReadAllBytes(TestsConstants._FILE_LOCATION), new FileInformation { FileLocation = TestsConstants._FILE_LOCATION });

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void InstanceFile_ShouldReturnExistingPath()
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
		public void ReplaceSingleDocument_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.ReplaceSingleDocumentAsync(TestsConstants._EXP_METADATA, TestsConstants._DOC_EXTRA_INFO);
			
			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void GetNativeTypeByFilename_ShouldReturnExpectedNativeType()
		{
			// Act
			var nativeType = DocumentManager.Instance.GetNativeTypeByFilename(TestsConstants._FILE_LOCATION);
			
			// Assert
			nativeType.Description.Should().Be(TestsConstants._OI_FILE_TYPE_);
		}

		[Test]
		public void DeleteTempFile_ShouldNotThrow()
		{
			// Act
			Action action = () => DocumentManager.Instance.DeleteTempFile(FileHelper.GetTempFolderLocation());

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void DeleteTempFile_ShouldNotThrow_WhenEmptyFileDeletion()
		{
			// Act
			Action action = () => DocumentManager.Instance.DeleteTempFile("");

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void GetDocumentDataTable_ShouldReturnDataTableWithExpectedColumns()
		{
			// Arrange
			string identifierName = "Identifier";

			// Act
			DataTable dataTable = DocumentManager.Instance.GetDocumentDataTable(identifierName);

			// Assert
			Assert.IsNotNull(dataTable);
			Assert.AreEqual(10, dataTable.Columns.Count);
			Assert.IsTrue(dataTable.Columns.Contains(identifierName));
			Assert.IsTrue(dataTable.Columns.Contains("Extracted Text"));
			Assert.IsTrue(dataTable.Columns.Contains("Document Extension"));
			Assert.IsTrue(dataTable.Columns.Contains("File Extension"));
			Assert.IsTrue(dataTable.Columns.Contains("FileExtension"));
			Assert.IsTrue(dataTable.Columns.Contains("File Name"));
			Assert.IsTrue(dataTable.Columns.Contains("FileName"));
			Assert.IsTrue(dataTable.Columns.Contains("File Size"));
			Assert.IsTrue(dataTable.Columns.Contains("FileSize"));
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
