using NUnit.Framework;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace kcura.SingleFileUpload.FunctionalTests.Helper
{
	class HelperSettings : AppConfigSettings
	{
		public override string RsapiServerAddress
		{
			get { return RelativityInstanceAddress; }
		}

		public override string RestServerAddress
		{
			get { return RelativityInstanceAddress; }
		}

		public override string RelativityInstanceAddress
		{
			get { return TestContext.Parameters["RelativityHostAddress"]; }
		}

		public override string AdminUserName
		{
			get { return TestContext.Parameters["AdminUsername"]; }
		}

		public override string AdminPassword
		{
			get { return TestContext.Parameters["AdminPassword"]; }
		}

		public override string SqlServerAddress
		{
			get { return TestContext.Parameters["SqlServer"]; }
		}

		public override string SqlUserName
		{
			get { return TestContext.Parameters["SqlUsername"]; }
		}

		public override string SqlPassword
		{
			get { return TestContext.Parameters["SqlPassword"]; }
		}

		public override string ServerBindingType
		{
			get { return TestContext.Parameters["ServerBindingType"]; }
		}

	}
}
