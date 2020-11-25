using Atata;
using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Web;
using Relativity.SimpleFileUpload.Tests.Core;
using Relativity.SimpleFileUpload.Tests.Core.Web;
using Relativity.SimpleFileUpload.Tests.Core.Templates;


namespace Relativity.SimpleFileUpload.UiTests
{
	public abstract class UiTestsTemplate : SimpleFileUploadTestsTemplate
	{
		protected UiTestsTemplate(string workspaceName)
			: base(Const.UI_WORKSPACE_PREFIX + workspaceName, Const.UI_TEMPLATE_NAME)
		{ }

		[SetUp]
		public virtual void SetUp()
		{
			Login();
		}

		[TearDown]
		public virtual void TearDown()
		{
			Logout();
		}

		private static void Login()
		{
			Go.To<LoginPage>()
				.EnterCredentials(
					RelativityFacade.Instance.Config.RelativityInstance.AdminUsername,
					RelativityFacade.Instance.Config.RelativityInstance.AdminPassword)
				.Login.Click();
		}

		private static void Logout()
		{
			Go.To<LogoutPage>();
		}
	}
}
