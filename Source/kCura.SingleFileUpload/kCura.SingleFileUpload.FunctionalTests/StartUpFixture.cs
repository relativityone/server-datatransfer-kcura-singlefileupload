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
	public class StartUpFixture
	{
		public static int TestWorkspaceID;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			RelativityFacade.Instance.RelyOn<CoreComponent>();
			RelativityFacade.Instance.RelyOn<ApiComponent>();

			int workspaceId = CreateTemplateWorkspace();

			InstallSimpleFileUploadToWorkspace(workspaceId);
		}

		private int CreateTemplateWorkspace()
		{
			var workspaceService = RelativityFacade.Instance.Resolve<IWorkspaceService>();

			var templateWorkspace = workspaceService.Get(Const.FUNCTIONAL_TEMPLATE_NAME);
			if (templateWorkspace == null)
			{
				Workspace newWorkspace = new Workspace()
				{
					Name = Const.FUNCTIONAL_TEMPLATE_NAME
				};

				return workspaceService.Create(newWorkspace).ArtifactID;
			}

			return templateWorkspace.ArtifactID;
		}

		private void InstallSimpleFileUploadToWorkspace(int workspaceId)
		{
			var applicationService = RelativityFacade.Instance.Resolve<IRelativityApplicationService>();

			int appId = applicationService.InstallToLibrary(SharedVariables.LocalRAPFileLocation);

			applicationService.InstallToWorkspace(workspaceId, appId);
		}
	}
}
