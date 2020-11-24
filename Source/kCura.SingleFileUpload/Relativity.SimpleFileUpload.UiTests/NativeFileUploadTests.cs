using NUnit.Framework;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.UiTests
{
	[TestFixture]
	[TestExecutionCategory.CI]
	[TestType.UI, TestType.MainFlow]
	public class NativeFileUploadTests : UiTestsTemplate
	{
		public NativeFileUploadTests()
			: base(nameof(NativeFileUploadTests))
		{ }
	}
}
