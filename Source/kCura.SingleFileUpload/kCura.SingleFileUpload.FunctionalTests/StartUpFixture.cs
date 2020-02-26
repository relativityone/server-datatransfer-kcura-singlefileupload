using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using NUnit.Framework;
using Platform.Keywords.Connection;
using Platform.Keywords.RSAPI;

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
			using (IRSAPIClient RsapiClient = ServiceFactory.GetProxy<IRSAPIClient>(SharedTestVariables.ADMIN_USERNAME,
				SharedTestVariables.DEFAULT_PASSWORD, DevMode.Default))
			{
				ID = WorkspaceHelpers.CreateWorkspace(RsapiClient, SharedTestVariables.TEST_WORKSPACE_NAME,
					SharedTestVariables.TEST_WORKSPACE_TEMPLATE_NAME);
			}
			return ID;
		}

	}
}
