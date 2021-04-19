using Atata;
using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Api.Services;
using Relativity.Testing.Framework.Models;
using Relativity.Testing.Framework.Web;

namespace Relativity.SimpleFileUpload.FunctionalTests.CI
{
	[SetUpFixture]
	public class SimpleFileUploadTestsSetUpFixture
	{
		private readonly string _workspaceTemplateName;
		private readonly string _standardAccountEmailFormat;

		public SimpleFileUploadTestsSetUpFixture()
		{
			_workspaceTemplateName = Const.FUNCTIONAL_TEMPLATE_NAME;
			_standardAccountEmailFormat = Const.FUNCTIONAL_STANDARD_ACCOUNT_EMAIL_FORMAT;
		}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			RelativityFacade.Instance.RelyOn<CoreComponent>();
			RelativityFacade.Instance.RelyOn<ApiComponent>();
			RelativityFacade.Instance.RelyOn<WebComponent>();

			RelativityFacade.Instance.Resolve<IAccountPoolService>().StandardAccountEmailFormat =
				_standardAccountEmailFormat;

			if (TemplateWorkspaceExists())
			{
				return;
			}

			int workspaceId = CreateTemplateWorkspace();

			InstallSimpleFileUploadToWorkspace(workspaceId);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			AtataContext.Current?.Dispose();
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
