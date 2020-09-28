﻿using NUnit.Framework;
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

			int appId = applicationService.InstallToLibrary(SharedVariables.LocalRAPFileLocation, new LibraryApplicationInstallOptions
			{
				IgnoreVersion = true
			});

			applicationService.InstallToWorkspace(workspaceId, appId);
		}
	}
}
