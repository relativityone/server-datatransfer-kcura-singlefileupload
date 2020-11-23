using NUnit.Framework;
using Relativity.SimpleFileUpload.Tests.Core;

namespace kcura.SingleFileUpload.FunctionalTests
{
	[SetUpFixture]
	public class FunctionalTestsSetupFixture : SimpleFileUploadTestsSetUpFixture
	{
		public FunctionalTestsSetupFixture()
			: base(Const.FUNCTIONAL_TEMPLATE_NAME)
		{ }
	}
}