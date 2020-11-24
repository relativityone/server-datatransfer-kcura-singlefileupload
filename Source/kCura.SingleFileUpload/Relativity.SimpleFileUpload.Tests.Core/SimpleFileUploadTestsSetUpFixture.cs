﻿using Atata;
using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Web;

namespace Relativity.SimpleFileUpload.Tests.Core
{
	[SetUpFixture]
	public class SimpleFileUploadTestsSetUpFixture
	{
		private readonly string _workspaceTemplateName;

		public SimpleFileUploadTestsSetUpFixture(string workspaceTemplateName)
		{
			_workspaceTemplateName = workspaceTemplateName;
		}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			RelativityFacade.Instance.RelyOn<CoreComponent>();
			RelativityFacade.Instance.RelyOn<ApiComponent>();
			RelativityFacade.Instance.RelyOn<WebComponent>();

			if (TemplateWorkspaceExists())
			{
				return;
			}

			int workspaceId = CreateTemplateWorkspace();

			InstallSimpleFileUploadToWorkspace(workspaceId);
		}

		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
		{
			AtataContext.Current?.Driver?.Quit();
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
			var applicationService = RelativityFacade.Instance.Resolve<IRelativityApplicationService>();

			int appId = applicationService.InstallToLibrary(SharedVariables.LocalRAPFileLocation, new LibraryApplicationInstallOptions
			{
				IgnoreVersion = true
			});

			applicationService.InstallToWorkspace(workspaceId, appId);
		}
	}
}
