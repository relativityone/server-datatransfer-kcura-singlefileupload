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

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
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
		public void ExportToSearchMLTest()
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
		public void ProcessSearchMLStringTest()
		{
			// Arrange
			byte[] native = File.ReadAllBytes(TestsConstants._FILE_LOCATION);

			// Act
			ExportedMetadata result = SearchExportManager.instance.ProcessSearchMLString(native);

			// Assert
			result.ExtractedText.Should().Be(TestsConstants._EXTRACTED_TEXT);
		}

		[Test]
		public void ConfigureOutsideInTest()
		{
			// Act
			Action action = () => SearchExportManager.instance.ConfigureOutsideIn();

			// Assert
			action.Should().NotThrow();
		}
	}
}
