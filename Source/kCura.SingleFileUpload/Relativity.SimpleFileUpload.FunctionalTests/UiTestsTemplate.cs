namespace Relativity.SimpleFileUpload.FunctionalTests
{
	public abstract class UiTestsTemplate : SimpleFileUploadTestsTemplate
	{
		protected UiTestsTemplate(string workspaceName)
			: base(Const.UI_WORKSPACE_PREFIX + workspaceName, Const.UI_TEMPLATE_NAME)
		{ }
	}
}
