using System.Net.Http;
using Polly;
using Polly.Retry;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Web;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.FunctionalTests.CI
{
	[Feature.DataTransfer.SimpleFileUpload]
	public abstract class SimpleFileUploadTestsTemplate : UITestFixture
	{
		private readonly string _workspaceName;
		private readonly string _workspaceTemplateName;

		private readonly IWorkspaceService _workspaceService;

		public int WorkspaceId { get; private set; }

		protected SimpleFileUploadTestsTemplate(string workspaceName, string workspaceTemplateName)
		{
			_workspaceName = workspaceName;
			_workspaceTemplateName = workspaceTemplateName;

			_workspaceService = RelativityFacade.Instance.Resolve<IWorkspaceService>();
		}

		protected override void OnSetUpFixture()
		{
			base.OnSetUpFixture();

			Workspace workspace = new Workspace
			{
				Name = _workspaceName,
				TemplateWorkspace = new NamedArtifact { Name = _workspaceTemplateName }
			};

			WorkspaceId = _workspaceService.Create(workspace).ArtifactID;
		}

		protected override void OnTearDownFixture()
		{
			base.OnTearDownFixture();

			_workspaceService.Delete(WorkspaceId);
		}

		protected override void OnSetUpTest()
		{
			base.OnSetUpTest();

			RetryPolicy loginAsStandardAccountPolicy = Policy
				.Handle<HttpRequestException>(ex => ex.Message.Contains("The entered E-Mail Address is already associated with another user in the system."))
				.Retry(3);

			loginAsStandardAccountPolicy.Execute(() => LoginAsStandardAccount());
		}
	}
}
