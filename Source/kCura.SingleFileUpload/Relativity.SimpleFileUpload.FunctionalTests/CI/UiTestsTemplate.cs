﻿namespace Relativity.SimpleFileUpload.FunctionalTests.CI
{
	public abstract class UiTestsTemplate : SimpleFileUploadTestsTemplate
	{
		protected UiTestsTemplate(string workspaceName)
			: base(Const.UI_WORKSPACE_PREFIX + workspaceName, Const.FUNCTIONAL_TEMPLATE_NAME)
		{ }
	}
}
