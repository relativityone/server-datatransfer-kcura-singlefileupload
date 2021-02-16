namespace Relativity.SimpleFileUpload.FunctionalTests
{
	public class FunctionalTestsSetupFixture : SimpleFileUploadTestsSetUpFixture
	{
		public FunctionalTestsSetupFixture()
			: base(Const.FUNCTIONAL_TEMPLATE_NAME, Const.FUNCTIONAL_STANDARD_ACCOUNT_EMAIL_FORMAT)
		{ }
	}
}