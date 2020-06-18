using System;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using NUnit.Framework;
using Platform.Keywords.RSAPI;
using Relativity.Services.ServiceProxy;
using Relativity.SimpleFileUpload.Tests.Core;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;

namespace kcura.SingleFileUpload.FunctionalTests
{
	[SetUpFixture]
	public class FunctionalTestsSetupFixture : SetUpFixture
	{
		public static int TestWorkspaceID;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			if(TemplateWorkspaceExists())
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
