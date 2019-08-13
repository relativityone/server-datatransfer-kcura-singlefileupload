using NUnit.Framework;
using System;
using System.IO;

namespace kCura.SingleFileUpload.Core.Tests.Helpers
{
	public class FileHelper
	{
		public static readonly string currentTestDirectory = TestContext.CurrentContext.TestDirectory;

		public static string GetFileLocation(string fileName)
		{
			string fileLocation = Path.Combine(currentTestDirectory, "..\\", "..\\", "Resources", fileName);
			return fileLocation;
		}

		public static string GetTempDirectoryPath()
		{
			string tempFolderName = Guid.NewGuid().ToString();
			string tempFolderPath = Path.Combine(currentTestDirectory, tempFolderName);
			string tempDirectoryPath = Directory.CreateDirectory(tempFolderPath).FullName;
			string tempFilePath = Path.Combine(tempDirectoryPath, Guid.NewGuid().ToString());
			File.WriteAllText(tempFilePath, "Hello World");
			return tempFilePath;
		}

	}
}
