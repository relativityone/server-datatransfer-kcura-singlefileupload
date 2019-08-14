using NUnit.Framework;
using System;
using System.IO;

namespace kCura.SingleFileUpload.Core.Tests.Helpers
{
	public class FileHelper
	{
		private static readonly string currentTestDirectory = TestContext.CurrentContext.TestDirectory;
		private static readonly string path = Path.Combine(currentTestDirectory, "..\\", "..\\", "Resources\\TempTestFiles");

		public static string GetFileLocation(string fileName)
		{
			string fileLocation = Path.Combine(currentTestDirectory, "..\\", "..\\", "Resources", fileName);
			return fileLocation;
		}

		public static string GetTempFolderLocation()
		{

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return Path.Combine(path, Guid.NewGuid().ToString());
		}

		public static void DeleteTestTempFolder()
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

	}
}
