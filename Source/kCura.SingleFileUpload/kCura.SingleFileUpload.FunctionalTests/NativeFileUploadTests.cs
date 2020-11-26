using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Relativity.Testing.Identification;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using kCura.SingleFileUpload.Core.Tests.Constants;

namespace kcura.SingleFileUpload.FunctionalTests
{
	[TestFixture]
	[TestExecutionCategory.CI]
	public class NativeFileUploadTests : FunctionalTestsTemplate
	{
		public NativeFileUploadTests() : base(nameof(NativeFileUploadTests))
		{ }

		[IdentifiedTest("A5391B33-7FC8-444F-BE17-77162434E714")]
		public async Task UploadNativeFile_GoldFlow()
		{
			// Arrange
			string expectedContent =
				$"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"{TestsConstants._DOC_CONTROL_NUMBER}\",\"Success\":true,\"Message\":null}}'</script>";

			bool fdv = false;
			bool img = false;
			FileInfo file = new FileInfo(TestsConstants._FILE_LOCATION);

			// Act
			HttpResponseMessage result = await UploadFileAsync(file, fdv, img).ConfigureAwait(false);

			// Assert
			await AssertResponseContentAsync(result, expectedContent).ConfigureAwait(false);
		}

		[IdentifiedTestCase("5b85c4ff-b52b-4941-b17f-3bf3d084fb1d", TestsConstants._FILE_NAME_EXE)]
		[IdentifiedTestCase("91f77dc1-b0e1-43dc-abcb-da82e8f1c385", TestsConstants._FILE_NAME_DLL)]
		[IdentifiedTestCase("6cd2d2f6-d7fb-45e0-b8aa-d87f98dcdcc6", TestsConstants._FILE_NAME_JS)]
		[IdentifiedTestCase("3f96dac2-27d7-4927-a2c6-142b8aff3b2d", TestsConstants._FILE_NAME_HTM)]
		[IdentifiedTestCase("ddd08669-1a72-4e08-a15f-7d2a625fc77a", TestsConstants._FILE_NAME_HTML)]
		public async Task Upload_ShouldFail_WhenUploadingForbiddenFileType(string fileName)
		{
			// Arrange
			string expectedContent = $"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"\",\"Success\":true,\"Message\":\"This file type is not supported.\"}}'</script>";
			bool fdv = false;
			bool img = false;
			FileInfo file = new FileInfo(FileHelper.GetFileLocation(fileName));
			
			// Act
			HttpResponseMessage result = await UploadFileAsync(file, fdv, img).ConfigureAwait(false);

			// Assert
			await AssertResponseContentAsync(result, expectedContent).ConfigureAwait(false);
		}

		private static async Task AssertResponseContentAsync(HttpResponseMessage response, string expected)
		{
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			string actualContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			actualContent.Should().Be(expected);
		}
	}
}