using System.IO;
using Atata;
using kcura.SimpleFileUpload.FunctionalTests.Helpers;
using NUnit.Framework;
using Relativity.Testing.Framework.Web;
using Relativity.Testing.Identification;

namespace kcura.SimpleFileUpload.FunctionalTests.UITests
{
	[TestFixture]
	[TestExecutionCategory.CI]
	[TestType.UI, TestType.MainFlow]
	public class NativeFileUploadTests : UiTestsTemplate
	{
		public NativeFileUploadTests()
			: base(nameof(NativeFileUploadTests))
		{ }

		[IdentifiedTest("bf01c1a8-cf93-47e3-8c57-a2e77ea5725a")]
		public void UploadNativeFile_GoldFlow()
		{
			// Arrange
			string filePath = Path.GetFullPath(FileHelper.GetFileLocation(Const.File._FILE_NAME));

			DocumentListPage documentListPage = Being.On<DocumentListPage>(WorkspaceId);

			// Act

			documentListPage = documentListPage.NewDocument.ClickAndGo().Upload(filePath);

			// Assert
			documentListPage.Documents.Rows[x => x.ControlNumber == Path.GetFileNameWithoutExtension(Const.File._FILE_NAME)].Should.BeVisible();
		}
	}
}
