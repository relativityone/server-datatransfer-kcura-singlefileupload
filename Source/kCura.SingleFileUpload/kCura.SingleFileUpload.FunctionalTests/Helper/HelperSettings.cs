using NUnit.Framework;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace kcura.SingleFileUpload.FunctionalTests.Helper
{
	class HelperSettings : AppConfigSettings
	{
		public override string RsapiServerAddress => RelativityInstanceAddress;

		public override string RestServerAddress => RelativityInstanceAddress;

		public override string RelativityInstanceAddress => TestContext.Parameters["RelativityHostAddress"];

		public override string AdminUserName => TestContext.Parameters["AdminUsername"];

		public override string AdminPassword => TestContext.Parameters["AdminPassword"];

		public override string SqlServerAddress => TestContext.Parameters["SqlServer"];

		public override string SqlUserName => TestContext.Parameters["SqlUsername"];

		public override string SqlPassword => TestContext.Parameters["SqlPassword"];

		public override string ServerBindingType => TestContext.Parameters["ServerBindingType"];
	}
}
