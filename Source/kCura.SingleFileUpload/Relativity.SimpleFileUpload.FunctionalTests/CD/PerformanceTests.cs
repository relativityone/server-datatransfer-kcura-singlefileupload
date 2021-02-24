using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Atata;
using FluentAssertions;
using NUnit.Framework;
using Relativity.SimpleFileUpload.FunctionalTests.Common;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.RingSetup;
using Relativity.Testing.Framework.Web;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD
{
	[IdentifiedTestFixture("4aaaf027-c073-4aa7-8e1a-213e357ccf10", Description = "SimpleFileUpload Performance Verification Tests")]
	[TestExecutionCategory.CD, TestLevel.L3]
	[TestType.Performance]
	public class PerformanceTests : TestSetup
	{
		private const int _MAX_PERFORMANCE_TEST_TIME_IN_MILLISECONDS = 30 * 1000;
		private const string _NOT_EXISTING_ARTIFACT_ID = "-1";

		public PerformanceTests() : base($"{Const.App._NAME}-{nameof(PerformanceTests)}", desiredNumberOfDocuments: 0)
		{ }

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			RelativityFacade.Instance.RelyOn<WebComponent>();

			Go.To<LoginPage>()
				.EnterCredentials(_user.Email, _user.Password)
				.Login.Click();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			AtataContext.Current?.Dispose();
		}

		[IdentifiedTest("883f9722-733c-4a5b-b892-05aa0d5af4d3")]
		[TestExecutionCategory.RAPCD.Verification.NonFunctional]
		[MaxTime(_MAX_PERFORMANCE_TEST_TIME_IN_MILLISECONDS)]
		public async Task UploadNativeFile_PerformanceTest()
		{
			// Arrange
			const string expectedControlNumber = Const.File._DOC_CONTROL_NUMBER;

			string expectedContent =
				$"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"{expectedControlNumber}\",\"Success\":true,\"Message\":null}}'</script>";

			bool fdv = false;
			bool img = false;

			string filePath = TestFileHelper.GetFileLocation(Const.File._FILE_NAME);
			FileInfo file = new FileInfo(filePath);

			// Act
			HttpResponseMessage result = await SimpleFileUploadHelper.UploadFileAsync(_workspace.ArtifactID, file, fdv, img).ConfigureAwait(false);

			// Assert
			await AssertResponseContentAsync(result, expectedContent).ConfigureAwait(false);

			await WaitForUploadCompletedAsync(expectedControlNumber);
		}

		private static async Task AssertResponseContentAsync(HttpResponseMessage response, string expected)
		{
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			string actualContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			actualContent.Should().Be(expected);
		}

		private async Task WaitForUploadCompletedAsync(string expectedControlNumber)
		{
			string uploadedDocArtifactId;
			do
			{
				var response = await SimpleFileUploadHelper.CheckUplaodStatus(_workspace.ArtifactID, expectedControlNumber)
					.ConfigureAwait(false);

				uploadedDocArtifactId = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			} while (uploadedDocArtifactId == _NOT_EXISTING_ARTIFACT_ID);
		}
	}
}
