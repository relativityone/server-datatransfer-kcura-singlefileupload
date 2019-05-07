using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NUnit.Framework;
using System.IO;


namespace Relativity.SimpleFileUpload.Core.UnitTest
{

	[TestFixture]
	public class TestOutsideInFileSearhML
	{
		[OneTimeSetUp]
		public void OnStepUp()
		{
			
		}

		[Test]
		public void TestFile()
		{
			string executableLocation = TestContext.CurrentContext.TestDirectory;
			string fileName = "CTRL0192154.msg";
			string fileLocation = Path.Combine(executableLocation, "..\\", "..\\", "Resources", fileName);
			using (FileStream fileStream = new FileStream(fileLocation, FileMode.Open, FileAccess.Read))
			{

				Stream stream = fileStream;
				var native = new byte[stream.Length];
				stream.Read(native, 0, checked((int)stream.Length));

				SearchExportManager searchExportManager = new SearchExportManager();
				ExportedMetadata transientMetadata = new ExportedMetadata();

				searchExportManager.ConfigureOutsideIn();
				transientMetadata = searchExportManager.ExportToSearchML(fileName, native, () => OutsideIn.OutsideIn.NewLocalExporter());
				Assert.IsTrue(!string.IsNullOrEmpty(transientMetadata.ExtractedText));
				System.Console.Write(transientMetadata.ExtractedText);
			}



		}
	}
}
