using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.Tests.Constants;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using System.IO;

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
			string fileName = TestsConstants._FILE_NAME;
			Mock<IInstanceSettingsBundle> MockInstanceSettingsBundle = new Mock<IInstanceSettingsBundle>();
			mockingHelper.Setup(x => x.GetInstanceSettingBundle()).Returns(MockInstanceSettingsBundle.Object);
			ExportedMetadata result = SearchExportManager.instance.ExportToSearchML(fileName, File.ReadAllBytes(FileHelper.GetFileLocation(fileName)), mockingHelper.Object);
			Assert.AreEqual(result.FileName, fileName);
		}

		[Test]
		public void ProcessSearchMLStringTest()
		{
			byte[] native = File.ReadAllBytes(TestsConstants._FILE_LOCATION);
			ExportedMetadata result = SearchExportManager.instance.ProcessSearchMLString(native);
			Assert.AreEqual(result.ExtractedText, TestsConstants._EXTRACTED_TEXT);
		}

		[Test]
		public void ConfigureOutsideInTest()
		{
			SearchExportManager.instance.ConfigureOutsideIn();
			Assert.IsTrue(true);
		}
	}
}
