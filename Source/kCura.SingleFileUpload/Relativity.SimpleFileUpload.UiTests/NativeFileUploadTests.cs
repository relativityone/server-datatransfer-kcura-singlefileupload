using Atata;
using System.IO;
using NUnit.Framework;
using Relativity.Testing.Framework.Web;
using Relativity.Testing.Identification;
using kCura.SingleFileUpload.Core.Tests.Constants;

namespace Relativity.SimpleFileUpload.UiTests
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
			DocumentListPage documentListPage = Being.On<DocumentListPage>(WorkspaceId);

			// Act
			documentListPage = documentListPage.NewDocument.ClickAndGo().Upload(Path.GetFullPath(TestsConstants._FILE_LOCATION));

			// Assert
			documentListPage.Documents.Rows[x => x.ControlNumber == Path.GetFileNameWithoutExtension(TestsConstants._FILE_NAME)].Should.BeVisible();
		}
	}
}
