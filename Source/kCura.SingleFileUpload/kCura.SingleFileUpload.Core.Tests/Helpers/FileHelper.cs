using NUnit.Framework;
using System.IO;

namespace kCura.SingleFileUpload.Core.Tests.Helpers
{
	public class FileHelper
	{

		public static string GetFileLocation(string fileName)
		{
			string executableLocation = TestContext.CurrentContext.TestDirectory;
			string fileLocation = Path.Combine(executableLocation, "..\\", "..\\", "Resources", fileName);

			return fileLocation;
		}
	}
}
