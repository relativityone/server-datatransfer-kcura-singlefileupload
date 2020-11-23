using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.Tests.Core.Templates
{
	[Feature.DataTransfer.SimpleFileUpload]
	public abstract class SimpleFileUploadTestsTemplate
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

		[OneTimeSetUp]
		public virtual void OneTimeSetUp()
		{
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
			_workspaceService.Delete(WorkspaceId);
		}
	}
}
