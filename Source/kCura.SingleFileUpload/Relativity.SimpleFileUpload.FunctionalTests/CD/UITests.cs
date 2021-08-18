using Atata;
using Relativity.SimpleFileUpload.FunctionalTests.CD.Controls;
using Relativity.SimpleFileUpload.FunctionalTests.Common;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api.ObjectManagement;
using Relativity.Testing.Framework.Api.Services;
using Relativity.Testing.Framework.Models;
using Relativity.Testing.Framework.Web.Components;
using Relativity.Testing.Framework.Web.Navigation;
using Relativity.Testing.Identification;
using System.Data;
using System.Linq;
using System.Text;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD
{
	[IdentifiedTestFixture("e4e10889-5d67-438b-bf9b-12c3a7cd206c", Description = "SimpleFileUpload UI Verification Tests")]
	[TestExecutionCategory.CD, TestLevel.L3]
	[TestType.UI, TestType.MainFlow]
	public class UITests : TestsBase
	{
		private const string _TAB_STATUS_CLASS = "ri-active";
		private const string _FILE_LOCATION_COLUMN_NAME = "FILE_LOCATION_COLUMN_NAME";
		private const string _BATES_NUMBER_FIELD = "Bates Beg";

		private IDocumentService DocumentService { get; set; }
		private IObjectService ObjectService { get; set; }
		public UITests() : base($"{Const.App._NAME}-{nameof(UITests)}")
		{ }

		public override void OneTimeSetUp()
		{
			base.OneTimeSetUp();

			Go.To<LoginPage>()
				.EnterCredentials(_user.Email, _user.Password)
				.Login.Click();

			DocumentService = RelativityFacade.Instance.Resolve<IDocumentService>();
			ObjectService = RelativityFacade.Instance.Resolve<IObjectService>();
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

		[IdentifiedTest("6750BD56-3BFA-4FF9-8322-E18FB335BF4A")]
		public void UploadNativeFile_FromDocumentViewer_GoldFlow()
		{
			// Arrange
			TestFile file = TestFileHelper.PrepareTestFile();
			Document document = ImportNatives(file, false);

			// Act
			DocumentViewerPage documentViewer = Retry.Do(() => Being.OnDocument<DocumentViewerPage>(_workspace.ArtifactID, document.ArtifactID));

			Retry.Do(() => documentViewer.ReviewInterfaceFrame.SwitchTo()
				.ExtractedTextTab.Click()
				.Dropdown.ClickAndGo()
				.UploadNativeDocumentAction.Click()
				.SwitchToRoot(documentViewer));


			// Assert

			var modal = Retry.Do(() => documentViewer.DocumentUploadModalFrame.SwitchTo());

			modal.Should.BeVisible()
				.ModalTitle.Should.Exist()
				.ModalTitle.Should.Contain("Upload Document")
				.Upload(file.File.FullName)
				.SwitchToRoot(documentViewer);

			documentViewer.ReviewInterfaceFrame.SwitchTo().NativeTab.Attributes.Class.Should.Contain(_TAB_STATUS_CLASS);
		}

		[IdentifiedTest("FB9B793F-FB72-4F92-A1F9-C6405D650B2E")]
		public void ReplaceNativeFile_FromDocumentViewer_GoldFlow()
		{
			// Arrange
			TestFile file = TestFileHelper.PrepareTestFile();
			Document document = ImportNatives(file, true);

			// Act
			DocumentViewerPage documentViewer = Retry.Do(() => Being.OnDocument<DocumentViewerPage>(_workspace.ArtifactID, document.ArtifactID));

			Retry.Do(() => documentViewer.ReviewInterfaceFrame.SwitchTo()
				.ExtractedTextTab.Click()
				.Dropdown.ClickAndGo()
				.ReplaceNativeDocumentAction.Click()
				.SwitchToRoot(documentViewer));

			var uploadModal = Retry.Do(() => documentViewer.DocumentUploadModalFrame.SwitchTo());

			// Assert
			uploadModal.Should.BeVisible()
				.ModalTitle.Should.Exist()
				.ModalTitle.Should.Contain("Replace Document")
				.Upload(file.File.FullName)
				.SwitchToRoot(documentViewer);

			documentViewer.ReviewInterfaceFrame.SwitchTo().NativeTab.Attributes.Class.Should.Contain(_TAB_STATUS_CLASS);
		}

		[IdentifiedTest("2D2F126F-7E55-477C-BF65-35234C3A82A5")]
		public void UploadImageFile_FromDocumentViewer_GoldFlow()
		{
			// Arrange
			TestFile nativeFile = TestFileHelper.PrepareTestFile();
			TestFile imageFile = TestFileHelper.PrepareTestFile(Const.File._FILE_NAME_PDF);
			Document document = ImportNatives(nativeFile, true);

			// Act
			DocumentViewerPage documentViewer = Retry.Do(() => Being.OnDocument<DocumentViewerPage>(_workspace.ArtifactID, document.ArtifactID));

			Retry.Do(() => documentViewer.ReviewInterfaceFrame.SwitchTo()
				.ExtractedTextTab.Click()
				.Dropdown.ClickAndGo()
				.UploadImageDocumentAction.ClickAndGo()
				.ImageProfile.Click()
				.SwitchToRoot(documentViewer));

			var uploadModal = Retry.Do(() => documentViewer.DocumentUploadModalFrame.SwitchTo());

			uploadModal.Should.BeVisible()
				.ModalTitle.Should.Exist()
				.ModalTitle.Should.Contain("Upload Image")
				.Upload(imageFile.File.FullName)
				.SwitchToRoot(documentViewer);

			// check image tab
			documentViewer.ReviewInterfaceFrame.SwitchTo().ImageTab.Attributes.Class.Should.Contain(_TAB_STATUS_CLASS);
		}

		[IdentifiedTest("E88D3D1C-7497-4B77-A8F1-E5139F6290D0")]
		public void ReplaceImageFile_FromDocumentViewer_GoldFlow()
		{
			// Arrange
			TestFile imageFile = TestFileHelper.PrepareTestFile(Const.File._FILE_NAME_PDF);
			ImportImages(imageFile);
			Document document = ImportNatives(imageFile, true);
			// Act
			DocumentViewerPage documentViewer = Retry.Do(() => Being.OnDocument<DocumentViewerPage>(_workspace.ArtifactID, document.ArtifactID));

			Retry.Do(() => documentViewer.ReviewInterfaceFrame.SwitchTo()
				.ExtractedTextTab.Click()
				.Dropdown.ClickAndGo()
				.ReplaceImageDocumentAction.ClickAndGo()
				.ImageProfile.Click()
				.SwitchToRoot(documentViewer));

			var uploadModal = Retry.Do(() => documentViewer.DocumentUploadModalFrame.SwitchTo());

			uploadModal.Should.BeVisible()
				.ModalTitle.Should.Exist()
				.ModalTitle.Should.Contain("Replace Image")
				.Upload(imageFile.File.FullName)
				.SwitchToRoot(documentViewer);

			// check image tab
			documentViewer.ReviewInterfaceFrame.SwitchTo().ImageTab.Attributes.Class.Should.Contain(_TAB_STATUS_CLASS);
		}

		private Document ImportNatives(TestFile file, bool loadNatives)
		{
			string identifierName = GetDocumentIdentifierName();

			DataTable nativesDataTable = new DataTable("Documents");
			nativesDataTable.Columns.Add(identifierName);
			nativesDataTable.Columns.Add(_FILE_LOCATION_COLUMN_NAME);
			nativesDataTable.Columns.Add("Extracted Text");

			nativesDataTable.Rows.Add(file.ControlNumber, file.File.FullName, file.File.FullName);

			NativeImportOptions importOptions = new NativeImportOptions
			{
				DocumentIdentifierField = identifierName,
				OverwriteMode = DocumentOverwriteMode.AppendOverlay,
				NativeFilePathColumnName = loadNatives ? _FILE_LOCATION_COLUMN_NAME : null,
				NativeFileCopyMode = loadNatives ? NativeFileCopyMode.CopyFiles : NativeFileCopyMode.DoNotImportNativeFiles,
				ExtractedTextEncoding = Encoding.UTF8,
			};

			DocumentService.ImportNatives(_workspace.ArtifactID, nativesDataTable, importOptions);

			return DocumentService.Get(_workspace.ArtifactID, file.ControlNumber);
		}

		private void ImportImages(TestFile file)
		{
			string identifierName = GetDocumentIdentifierName();
			DataTable dataTable = new DataTable();

			dataTable.Columns.Add(_BATES_NUMBER_FIELD);
			dataTable.Columns.Add(identifierName);
			dataTable.Columns.Add(_FILE_LOCATION_COLUMN_NAME);

			dataTable.Rows.Add(file.ControlNumber, file.File.FullName, file.File.FullName);

			ImageImportOptions imageImportOptions = new ImageImportOptions()
			{
				BatesNumberField = _BATES_NUMBER_FIELD,
				DocumentIdentifierField = identifierName,
				FileLocationField = _FILE_LOCATION_COLUMN_NAME,
				OverwriteMode = DocumentOverwriteMode.AppendOverlay,
				OverlayBehavior = DocumentOverlayBehavior.MergeAll
			};

			DocumentService.ImportImages(_workspace.ArtifactID, dataTable, imageImportOptions);
		}

		private string GetDocumentIdentifierName()
		{
			return ObjectService.Query<Field>().
				For(_workspace.ArtifactID).
				Where(x => x.IsIdentifier, 1).
				FirstOrDefault().Name;
		}
	}
}