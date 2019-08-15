using kCura.Relativity.Client;
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

		[OneTimeSetUp]

		public void Setup()
		{
			Mock<IRSAPIClient> rsapi = RSAPIClientMockHelper.GetMockedHelper();

			Mock<ICPHelper> mockingHelper = MockHelper.GetMockingHelper<ICPHelper>();

			mockingHelper
				.MockIServiceMgr();

			ConfigureSingletoneRepositoryScope(mockingHelper.Object);

		}

		[Test]
		public void ExportToSearchMLTest()
		{
			ExportedMetadata result = SearchExportManager.instance.ExportToSearchML(TestsConstants._FILE_NAME, new byte[1080], OutsideIn.OutsideIn.NewLocalExporter());
			Assert.AreEqual(result.FileName, TestsConstants._FILE_NAME);
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
