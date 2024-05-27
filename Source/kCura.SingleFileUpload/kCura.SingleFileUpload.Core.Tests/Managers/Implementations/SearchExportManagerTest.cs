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
using OutsideIn;
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
			var mockInstanceSettingsBundle = new Mock<IInstanceSettingsBundle>();
			mockingHelper.Setup(x => x.GetInstanceSettingBundle()).Returns(mockInstanceSettingsBundle.Object);

			byte[] fileBytes = File.ReadAllBytes(FileHelper.GetFileLocation(fileName));

			// Act & Assert
			var exception = Assert.Throws<OutsideInException>(() =>
				SearchExportManager.instance.ExportToSearchML(fileName, fileBytes, mockingHelper.Object));

			Assert.That(exception.Message, Is.EqualTo("OI EXOpenExport failed - 4: no filter available for this file type [4]"));
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
