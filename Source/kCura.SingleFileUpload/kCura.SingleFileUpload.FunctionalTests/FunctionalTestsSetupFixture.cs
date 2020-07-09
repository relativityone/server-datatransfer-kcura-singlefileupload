using System.IO;
using System.Reflection;
using NUnit.Framework;
using Relativity.SimpleFileUpload.Tests.Core;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Web;

namespace kcura.SingleFileUpload.FunctionalTests
{
	[SetUpFixture]
	public class FunctionalTestsSetupFixture
	{
		public static int TestWorkspaceID;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			RelativityFacade.Instance.RelyOn<CoreComponent>();
			RelativityFacade.Instance.RelyOn<ApiComponent>();
			RelativityFacade.Instance.RelyOn<WebComponent>();

			RelativityFacade.Instance.GetComponent<WebComponent>().Configuration.ChromeBinaryFilePath =
				Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SharedVariables.ChromeBinaryLocation);

			if (TemplateWorkspaceExists())
			{
				return;
			}
			
			int workspaceId = CreateTemplateWorkspace();

			InstallSimpleFileUploadToWorkspace(workspaceId);
		}

		private bool TemplateWorkspaceExists()
			=> RelativityFacade.Instance.Resolve<IWorkspaceService>().Get(Const.FUNCTIONAL_TEMPLATE_NAME) != null;

		private int CreateTemplateWorkspace()
		{
			Workspace newWorkspace = new Workspace()
			{
				Name = Const.FUNCTIONAL_TEMPLATE_NAME
			};

			return RelativityFacade.Instance.Resolve<IWorkspaceService>().Create(newWorkspace).ArtifactID;
		}

		private void InstallSimpleFileUploadToWorkspace(int workspaceId)
		{
			var applicationService = RelativityFacade.Instance.Resolve<IRelativityApplicationService>();

			int appId = applicationService.InstallToLibrary(SharedVariables.LocalRAPFileLocation);

			applicationService.InstallToWorkspace(workspaceId, appId);
		}
	}
}
