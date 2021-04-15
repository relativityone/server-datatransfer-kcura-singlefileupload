using System.IO;
using Atata;
using NUnit.Framework;
using Relativity.SimpleFileUpload.FunctionalTests.Common;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.RingSetup;
using Relativity.Testing.Framework.Web;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD
{
	[IdentifiedTestFixture("e4e10889-5d67-438b-bf9b-12c3a7cd206c", Description = "SimpleFileUpload UI Verification Tests")]
	[TestExecutionCategory.CD, TestLevel.L3]
	[TestType.UI, TestType.MainFlow]
	public class UITests : TestSetup
	{
		public UITests() : base($"{Const.App._NAME}-{nameof(UITests)}", desiredNumberOfDocuments: 0)
		{ }

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			RelativityFacade.Instance.RelyOn<WebComponent>();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			AtataContext.Current?.Dispose();
		}

		[SetUp]
		public void SetUp()
		{
			Go.To<LoginPage>()
				.EnterCredentials(_user.Email, _user.Password)
				.Login.Click();
		}

		[IdentifiedTest("77c34414-cfa3-4303-8fba-443ba124f36b")]
		[TestExecutionCategory.RAPCD.Verification.Functional]
		public void UploadNativeFile_GoldFlow()
		{
			// Arrange
			TestFile file = TestFileHelper.PrepareTestFile();

			DocumentListPage documentListPage = Retry.Do(() => Being.On<DocumentListPage>(_workspace.ArtifactID));

			// Act
			documentListPage = documentListPage.NewDocument.ClickAndGo().Upload(file.File.FullName);

			// Assert
			documentListPage.Documents.Rows[x => x.ControlNumber == file.ControlNumber].Should.BeVisible();
		}
	}
}