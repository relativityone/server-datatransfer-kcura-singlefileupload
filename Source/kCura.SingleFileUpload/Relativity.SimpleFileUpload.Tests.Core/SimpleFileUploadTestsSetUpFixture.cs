using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Web;

namespace Relativity.SimpleFileUpload.Tests.Core
{
	[SetUpFixture]
	public abstract class SimpleFileUploadTestsSetUpFixture
	{
		private readonly string _workspaceTemplateName;
		private readonly string _standardAccountEmailFormat;

		protected SimpleFileUploadTestsSetUpFixture(string workspaceTemplateName, string standardAccountEmailFormat)
		{
			_workspaceTemplateName = workspaceTemplateName;
			_standardAccountEmailFormat = standardAccountEmailFormat;
		}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			RelativityFacade.Instance.RelyOn<CoreComponent>();
			RelativityFacade.Instance.RelyOn<ApiComponent>();
			RelativityFacade.Instance.RelyOn<WebComponent>();

			//RelativityFacade.Instance.Resolve<IAccountPoolService>().StandardAccountEmailFormat =
			//	_standardAccountEmailFormat;

			if (TemplateWorkspaceExists())
			{
				return;
			}

			int workspaceId = CreateTemplateWorkspace();

			InstallSimpleFileUploadToWorkspace(workspaceId);
		}

		private bool TemplateWorkspaceExists()
			=> RelativityFacade.Instance.Resolve<IWorkspaceService>().Get(_workspaceTemplateName) != null;

		private int CreateTemplateWorkspace()
		{
			Workspace newWorkspace = new Workspace()
			{
				Name = _workspaceTemplateName
			};

			return RelativityFacade.Instance.Resolve<IWorkspaceService>().Create(newWorkspace).ArtifactID;
		}

		private static void InstallSimpleFileUploadToWorkspace(int workspaceId)
		{
			var applicationService = RelativityFacade.Instance.Resolve<ILibraryApplicationService>();

			int appId = applicationService.InstallToLibrary(SharedVariables.LocalRAPFileLocation, new LibraryApplicationInstallOptions
			{
				IgnoreVersion = true
			});

			applicationService.InstallToWorkspace(workspaceId, appId);
		}
	}
}
