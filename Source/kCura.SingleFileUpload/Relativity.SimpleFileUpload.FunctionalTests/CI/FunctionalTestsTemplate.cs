namespace Relativity.SimpleFileUpload.FunctionalTests.CI
{
	public abstract class FunctionalTestsTemplate : SimpleFileUploadTestsTemplate
	{
		protected FunctionalTestsTemplate(string workspaceName)
			: base(Const.FUNCTIONAL_WORKSPACE_PREFIX + workspaceName, Const.FUNCTIONAL_TEMPLATE_NAME)
		{ }
	}
}
