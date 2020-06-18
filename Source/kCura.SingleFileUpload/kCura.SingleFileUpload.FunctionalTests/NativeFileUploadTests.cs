using FluentAssertions;
using kCura.SingleFileUpload.Core.Tests.Constants;
using NUnit.Framework;
using Relativity.SimpleFileUpload.Tests.Core.Templates;
using Relativity.Testing.Identification;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace kcura.SingleFileUpload.FunctionalTests
{
	[Feature.DataTransfer.SingleFileUpload]
	[Category("TestType.CI")]
	[TestFixture]
	public class NativeFileUploadTests : HttpFunctionalTestsTemplate
	{
		public NativeFileUploadTests() : base(nameof(NativeFileUploadTests)) { }

		[IdentifiedTest("A5391B33-7FC8-444F-BE17-77162434E714")]
		public async Task UploadNativeFile_GoldFlow()
		{
			// Arrange
			string expectedContent = $"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"{TestsConstants._DOC_CONTROL_NUMBER}\",\"Success\":true,\"Message\":null}}'</script>";

			bool fdv = false;
			bool img = false;
			FileInfo file = new FileInfo(TestsConstants._FILE_LOCATION);

			// Act
			var result = await UploadFileAsync(file, fdv, img).ConfigureAwait(false);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);

			string actualContent = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
			actualContent.Should().Be(expectedContent);
		}
	}
}
