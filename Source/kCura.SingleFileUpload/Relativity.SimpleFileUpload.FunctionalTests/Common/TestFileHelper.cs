using System.IO;
using NUnit.Framework;

namespace Relativity.SimpleFileUpload.FunctionalTests.Common
{
	public static class TestFileHelper
	{
		private const string _TEST_FILES_FOLDER = "Resources";

		public static string GetFileLocation(string fileName)
		{
			string fileLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, _TEST_FILES_FOLDER, fileName);
			return fileLocation;
		}
	}
}
