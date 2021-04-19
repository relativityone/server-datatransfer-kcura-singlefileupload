using System;
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

		public static TestFile PrepareTestFile()
		{
			string controlNumber = Guid.NewGuid().ToString();
			string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, $"{controlNumber}.xml");

			File.Copy(GetFileLocation(Const.File._FILE_NAME), filePath);

			return new TestFile
			{
				ControlNumber = controlNumber,
				File = new FileInfo(filePath)
			};
		}
	}
}
