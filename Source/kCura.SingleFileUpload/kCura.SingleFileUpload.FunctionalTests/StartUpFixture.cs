using System;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using NUnit.Framework;
using Platform.Keywords.RSAPI;
using Relativity.Services.ServiceProxy;

namespace kcura.SingleFileUpload.FunctionalTests
{
	[SetUpFixture]
	public class StartUpFixture
	{
		public static int TestWorkspaceID;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			TestWorkspaceID = CreateWorkspace();
		}

		public int CreateWorkspace()
		{
			int ID = 0;
			Uri servicesUri = new Uri(TestContext.Parameters["RsapiServicesHostAddress"]);
			Uri restUri = new Uri(TestContext.Parameters["RestServicesHostAddress"]);
			var credentials = new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(TestContext.Parameters["AdminUsername"], TestContext.Parameters["AdminPassword"]);
			ServiceFactorySettings settings = new ServiceFactorySettings(servicesUri, restUri, credentials);
			ServiceFactory factory = new ServiceFactory(settings);
			using (IRSAPIClient RsapiClient = factory.CreateProxy<IRSAPIClient>())
			{
				ID = WorkspaceHelpers.CreateWorkspace(RsapiClient, SharedTestVariables.TEST_WORKSPACE_NAME,
					SharedTestVariables.TEST_WORKSPACE_TEMPLATE_NAME);
			}
			return ID;
		}
	}
}
