using System;
using System.IO;
using System.Linq;
using Atata;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
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
		private const string _SFU_GUID = "1738ceb6-9546-44a7-8b9b-e64c88e47320";

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

		[Test]
		public void UploadNativeFile_ShouldPreventXSS_Js()
		{
			// Arrange
			string fileName = Const.File._FILE_NAME_PDF_XSS_JS;

			// Act
			UploadFile(fileName);

			// Assert  
			AssertXss();
		}

		[Test]
		public void UploadNativeFile_ShouldPreventXSS_Html()
		{
			// Arrange
			string fileName = Const.File._FILE_NAME_PDF_XSS_HTML;

			// Act
			UploadFile(fileName);
			
			// Assert  
			AssertXss();
		}

		private void UploadFile(string fileName)
		{
			string fileLocation = Path.GetFullPath(TestFileHelper.GetFileLocation(fileName));
			DocumentListPage documentListPage = Being.On<DocumentListPage>(WorkspaceId);
 
			documentListPage
				.NewDocument
				.ClickAndGo()
				.Document
				.Set(fileLocation)
				.UploadButton
				.Click();
		}

		private void AssertXss()
		{
			object scriptResult = AtataContext.Current.Driver.ExecuteScript("return window.relativityXSS === undefined");
			scriptResult.Should().BeAssignableTo<bool>().Which.Should().BeTrue("XSS attack should not execute malicious code");

			string errors = string.Join(Environment.NewLine, AtataContext.Current.Driver
				.Manage()
				.Logs
				.GetLog(LogType.Browser)
				.Where(x => x.Level == OpenQA.Selenium.LogLevel.Severe)
				.Where(x => x.Message.Contains($"/{_SFU_GUID}/"))
				.Select(x => x.Message));
			errors.Should().BeNullOrWhiteSpace($"XSS attack should not cause JavaScript errors");
		}
	}
}
