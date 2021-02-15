namespace kcura.SimpleFileUpload.FunctionalTests
{
	public class UiTestsSetupFixture : SimpleFileUploadTestsSetUpFixture
	{
		public UiTestsSetupFixture()
			: base(Const.UI_TEMPLATE_NAME, Const.UI_STANDARD_ACCOUNT_EMAIL_FORMAT)
		{ }
	}
}
