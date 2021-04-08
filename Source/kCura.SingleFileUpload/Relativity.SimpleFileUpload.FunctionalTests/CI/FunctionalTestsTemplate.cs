using System.Net.Http;
using Relativity.SimpleFileUpload.FunctionalTests.Common;

namespace Relativity.SimpleFileUpload.FunctionalTests.CI
{
	public abstract class FunctionalTestsTemplate : SimpleFileUploadTestsTemplate
	{
		protected HttpClient Client;

		protected FunctionalTestsTemplate(string workspaceName)
			: base(Const.FUNCTIONAL_WORKSPACE_PREFIX + workspaceName, Const.FUNCTIONAL_TEMPLATE_NAME)
		{
		}

		protected override void OnSetUpTest()
		{
			base.OnSetUpTest();

			Client = SimpleFileUploadHelper.GetUserHttpClient();
		}
	}
}
