using System;
using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.Tests.Constants;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using System.IO;
using FluentAssertions;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
	public class SearchExportManagerTest : TestBase
	{
		Mock<ICPHelper> mockingHelper;

		[SetUp]
		public void Setup()
		{
			mockingHelper = new Mock<ICPHelper>();

			mockingHelper
				.MockIServiceMgr();

			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public void ExportToSearchML_ShouldExport()
		{
			// Arrange
			string fileName = TestsConstants._FILE_NAME;
			Mock<IInstanceSettingsBundle> MockInstanceSettingsBundle = new Mock<IInstanceSettingsBundle>();
			mockingHelper.Setup(x => x.GetInstanceSettingBundle()).Returns(MockInstanceSettingsBundle.Object);
			
			// Act
			ExportedMetadata result = SearchExportManager.instance.ExportToSearchML(fileName, File.ReadAllBytes(FileHelper.GetFileLocation(fileName)), mockingHelper.Object);

			// Assert
			result.FileName.Should().Be(fileName);
		}

		[Test]
		public void ProcessSearchMLString_ShouldReturnExpectedText()
		{
			// Arrange
			byte[] native = File.ReadAllBytes(TestsConstants._FILE_LOCATION);

			// Act
			ExportedMetadata result = SearchExportManager.instance.ProcessSearchMLString(native);

			// Assert
			result.ExtractedText.Should().Be(TestsConstants._EXTRACTED_TEXT);
		}

		[Test]
		public void ConfigureOutsideIn_ShouldNotThrow()
		{
			// Act
			Action action = () => SearchExportManager.instance.ConfigureOutsideIn();

			// Assert
			action.Should().NotThrow();
		}
	}
}
