using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Web;

namespace Relativity.SimpleFileUpload.Tests.Core
{
	[SetUpFixture]
	public class SetUpFixture
	{
		public SetUpFixture()
		{
			RelativityFacade.Instance.RelyOn<CoreComponent>();
			RelativityFacade.Instance.RelyOn<ApiComponent>();
			RelativityFacade.Instance.RelyOn<WebComponent>();

			RelativityFacade.Instance.GetComponent<WebComponent>().Configuration.ChromeBinaryFilePath = @"C:\_Work\kcura.singlefileupload\buildtools\Relativity.Chromium.Portable\tools\chrome.exe";
		}
	}
}
