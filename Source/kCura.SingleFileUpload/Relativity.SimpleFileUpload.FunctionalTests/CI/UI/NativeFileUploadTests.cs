using System.IO;
using Atata;
using NUnit.Framework;
using Relativity.SimpleFileUpload.FunctionalTests.Common;
using Relativity.Testing.Framework.Web;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.FunctionalTests.CI.UI
{
	[TestFixture]
	[TestExecutionCategory.CI, TestLevel.L3]
	[TestType.UI, TestType.MainFlow]
	public class NativeFileUploadTests : UiTestsTemplate
	{
		public NativeFileUploadTests()
			: base(nameof(NativeFileUploadTests))
		{ }

		[IdentifiedTest("bf01c1a8-cf93-47e3-8c57-a2e77ea5725a")]
		[TestExecutionCategory.RAPCD.Verification.Functional]
		public void UploadNativeFile_GoldFlow()
		{
			// Arrange
			string filePath = TestFileHelper.GetFileLocation(Const.File._FILE_NAME);

			DocumentListPage documentListPage = Being.On<DocumentListPage>(WorkspaceId);

			// Act
			documentListPage = documentListPage.NewDocument.ClickAndGo().Upload(filePath);

			// Assert
			documentListPage.Documents.Rows[x => x.ControlNumber == Path.GetFileNameWithoutExtension(Const.File._FILE_NAME)].Should.BeVisible();
		}
	}
}
