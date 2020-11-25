using Atata;
using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Web;
using Relativity.Testing.Identification;
using Relativity.SimpleFileUpload.Tests.Core.Web;

namespace Relativity.SimpleFileUpload.Tests.Core.Templates
{
	[Feature.DataTransfer.SimpleFileUpload]
	public abstract class SimpleFileUploadTestsTemplate
	{
		private readonly string _workspaceName;
		private readonly string _workspaceTemplateName;

		private readonly IWorkspaceService _workspaceService;

		private TestSession _fixtureSession;
		private TestSession _testSession;

		public int WorkspaceId { get; private set; }

		protected SimpleFileUploadTestsTemplate(string workspaceName, string workspaceTemplateName)
		{
			_workspaceName = workspaceName;
			_workspaceTemplateName = workspaceTemplateName;

			_workspaceService = RelativityFacade.Instance.Resolve<IWorkspaceService>();
		}

		[OneTimeSetUp]
		public virtual void OneTimeSetUp()
		{
			TestSession.Current = _fixtureSession = TestSession.Global.StartChildSession();

			Workspace workspace = new Workspace()
			{
				Name = _workspaceName,
				TemplateWorkspace = new NamedArtifact { Name = _workspaceTemplateName }
			};

			WorkspaceId = _workspaceService.Create(workspace).ArtifactID;
		}

		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
		{
			TestSession.Current = _fixtureSession;

			_workspaceService.Delete(WorkspaceId);

			_fixtureSession?.Dispose();
			_fixtureSession = null;
		}

		[SetUp]
		public void SetUp()
		{
			TestSession.Current = _testSession = _fixtureSession.StartChildSession();

			SwitchContext.CurrentFrame = null;

			AtataContext.Configure().Build();

			Login();
		}

		[TearDown]
		public void TearDown()
		{
			TestSession.Current = _testSession;

			Logout();

			AtataContext.Current?.CleanUp();

			SwitchContext.CurrentFrame = null;

			_testSession?.Dispose();
			_testSession = null;
		}

		private static void Login()
		{
			Go.To<LoginPage>()
				.EnterCredentials(
					RelativityFacade.Instance.Config.RelativityInstance.AdminUsername,
					RelativityFacade.Instance.Config.RelativityInstance.AdminPassword)
				.Login.Click();
		}

		private static void Logout()
		{
			Go.To<LogoutPage>();
		}
	}
}
