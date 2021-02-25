﻿using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Relativity.SimpleFileUpload.FunctionalTests.Common;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.FunctionalTests.CI.Tests
{
	[TestFixture]
	[TestExecutionCategory.CI, TestLevel.L3]
	public class NativeFileUploadTests : FunctionalTestsTemplate
	{
		public NativeFileUploadTests() : base(nameof(NativeFileUploadTests))
		{ }

		[IdentifiedTest("a5391b33-7fc8-444f-be17-77162434e714")]
		public async Task UploadNativeFile_GoldFlow()
		{
			// Arrange
			string expectedContent =
				$"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"{Const.File._DOC_CONTROL_NUMBER}\",\"Success\":true,\"Message\":null}}'</script>";

			bool fdv = false;
			bool img = false;

			string filePath = TestFileHelper.GetFileLocation(Const.File._FILE_NAME);
			FileInfo file = new FileInfo(filePath);

			// Act
			HttpResponseMessage result = await SimpleFileUploadHelper.UploadFileAsync(Client, WorkspaceId, file, fdv, img).ConfigureAwait(false);

			// Assert
			await AssertResponseContentAsync(result, expectedContent).ConfigureAwait(false);
		}

		[IdentifiedTestCase("5b85c4ff-b52b-4941-b17f-3bf3d084fb1d", Const.File._FILE_NAME_EXE)]
		[IdentifiedTestCase("91f77dc1-b0e1-43dc-abcb-da82e8f1c385", Const.File._FILE_NAME_DLL)]
		[IdentifiedTestCase("6cd2d2f6-d7fb-45e0-b8aa-d87f98dcdcc6", Const.File._FILE_NAME_JS)]
		[IdentifiedTestCase("3f96dac2-27d7-4927-a2c6-142b8aff3b2d", Const.File._FILE_NAME_HTM)]
		[IdentifiedTestCase("ddd08669-1a72-4e08-a15f-7d2a625fc77a", Const.File._FILE_NAME_HTML)]
		public async Task Upload_ShouldFail_WhenUploadingForbiddenFileType(string fileName)
		{
			// Arrange
			string expectedContent = $"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"\",\"Success\":true,\"Message\":\"This file type is not supported.\"}}'</script>";
			bool fdv = false;
			bool img = false;
			FileInfo file = new FileInfo(TestFileHelper.GetFileLocation(fileName));
			
			// Act
			HttpResponseMessage response = await SimpleFileUploadHelper.UploadFileAsync(Client, WorkspaceId, file, fdv, img).ConfigureAwait(false);

			// Assert
			await AssertResponseContentAsync(response, expectedContent).ConfigureAwait(false);
		}

		[IdentifiedTest("0e88a8e6-6139-4c9c-9b2c-f9f0beebbd3d")]
		public async Task Upload_ShouldEncodeFileName()
		{
			// Arrange
			string expectedContent = $"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"SamplePDF\\\\u0027\",\"Success\":true,\"Message\":null}}'</script>";
			bool fdv = false;
			bool img = false;
			FileInfo file = new FileInfo(TestFileHelper.GetFileLocation(Const.File._FILE_NAME_PDF_INVALID_JS));

			// Act
			HttpResponseMessage response = await SimpleFileUploadHelper.UploadFileAsync(Client, WorkspaceId, file, fdv, img).ConfigureAwait(false);

			// Assert
			await AssertResponseContentAsync(response, expectedContent).ConfigureAwait(false);
		}

		private static async Task AssertResponseContentAsync(HttpResponseMessage response, string expected)
		{
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			string actualContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			actualContent.Should().Be(expected);
		}
	}
}