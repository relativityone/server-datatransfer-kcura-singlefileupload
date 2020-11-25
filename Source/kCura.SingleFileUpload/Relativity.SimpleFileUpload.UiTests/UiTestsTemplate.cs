using Relativity.SimpleFileUpload.Tests.Core;
using Relativity.SimpleFileUpload.Tests.Core.Templates;


namespace Relativity.SimpleFileUpload.UiTests
{
	public abstract class UiTestsTemplate : SimpleFileUploadTestsTemplate
	{
		protected UiTestsTemplate(string workspaceName)
			: base(Const.UI_WORKSPACE_PREFIX + workspaceName, Const.UI_TEMPLATE_NAME)
		{ }
	}
}
