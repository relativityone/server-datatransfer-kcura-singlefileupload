using Relativity.SimpleFileUpload.Tests.Core;

namespace Relativity.SimpleFileUpload.UiTests
{
	public class UiTestsSetupFixture : SimpleFileUploadTestsSetUpFixture
	{
		public UiTestsSetupFixture()
			: base(Const.UI_TEMPLATE_NAME, "sfuuiuser{0}@mail.com")
		{ }
	}
}
