using System;

namespace Relativity.SimpleFileUpload.Tests.Core
{
	public static class Const
	{
		public const string FUNCTIONAL_WORKSPACE_PREFIX = "FunctionalTests_";
		public const string FUNCTIONAL_TEMPLATE_NAME = "Functional Tests Template";
		public const string FUNCTIONAL_STANDARD_ACCOUNT_EMAIL_FORMAT = "sfu_func_user{0}@mail.com";

		public const string UI_WORKSPACE_PREFIX = "UITests_";
		public const string UI_TEMPLATE_NAME = "UI Tests Template";
		public const string UI_STANDARD_ACCOUNT_EMAIL_FORMAT = "sfu_ui_user{0}@mail.com";

		public class CustomPage
		{
			public static readonly Guid Guid = new Guid("1738CEB6-9546-44A7-8B9B-E64C88E47320"); 
		}
	}
}
