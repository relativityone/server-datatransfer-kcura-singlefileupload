﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
		private const int _UPLOAD_FILE_BENCHMARK_IN_MILLISECONDS = 7 * 1000;
		private const double _PCT_TOLERANCE_RATE = 0.05;

		private const int _NUMBER_OF_ITERATIONS = 50;
		private const int _DELAY_BETWEEN_UPLOADS_IN_MILLISECONDS = 500;

		private const int _MAX_PERFORMANCE_TEST_TIME_IN_MILLISECONDS = 10 * 60 * 1000;
		private const string _NOT_EXISTING_ARTIFACT_ID = "-1";

		private HttpClient _client;

		public PerformanceTests() : base($"{Const.App._NAME}-{nameof(PerformanceTests)}", desiredNumberOfDocuments: 0)
		{ }

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			RelativityFacade.Instance.RelyOn<WebComponent>();

			Go.To<LoginPage>()
				.EnterCredentials(_user.Email, _user.Password)
				.Login.Click();

			_client = SimpleFileUploadHelper.GetUserHttpClient();
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
			var uploadDurations = new List<double>();

			// Act
			for (int i = 0; i < _NUMBER_OF_ITERATIONS; ++i)
			{
				var testFile = PrepareTestFile();
				try
				{
					var stopwatch = Stopwatch.StartNew();
					
					await UploadFileAsync(testFile.ControlNumber, testFile.File).ConfigureAwait(false);

					uploadDurations.Add(stopwatch.Elapsed.TotalMilliseconds);

					await Task.Delay(_DELAY_BETWEEN_UPLOADS_IN_MILLISECONDS).ConfigureAwait(false);
				}
				finally
				{
					File.Delete(testFile.File.FullName);
				}
			}

			// Assert
			uploadDurations.Count.Should().BeGreaterOrEqualTo(GetMinimumSuccessfulUploads());
			uploadDurations.Average().Should().BeLessOrEqualTo(GetReferenceBenchmark());
		}
		
		private (string ControlNumber, FileInfo File) PrepareTestFile()
		{
			string controlNumber = Guid.NewGuid().ToString();
			string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, $"{controlNumber}.xml");

			File.Copy(TestFileHelper.GetFileLocation(Const.File._FILE_NAME), filePath);

			return (ControlNumber: controlNumber, File: new FileInfo(filePath));
		}

		private async Task UploadFileAsync(string controlNumber, FileInfo file)
		{
			// Arrange
			string expectedContent =
				$"<script>sessionStorage['____pushNo'] = '{{\"Data\":\"{controlNumber}\",\"Success\":true,\"Message\":null}}'</script>";

			bool fdv = false;
			bool img = false;

			// Act
			HttpResponseMessage result = await SimpleFileUploadHelper.UploadFileAsync(_client, _workspace.ArtifactID, file, fdv, img).ConfigureAwait(false);

			// Assert
			await AssertResponseContentAsync(result, expectedContent).ConfigureAwait(false);

			await WaitForUploadCompletedAsync(controlNumber).ConfigureAwait(false);
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
				var response = await SimpleFileUploadHelper.CheckUploadStatusAsync(_client, _workspace.ArtifactID, expectedControlNumber)
					.ConfigureAwait(false);

				uploadedDocArtifactId = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

				await Task.Delay(100).ConfigureAwait(false);

			} while (uploadedDocArtifactId == _NOT_EXISTING_ARTIFACT_ID);
		}

		private static int GetMinimumSuccessfulUploads()
		{
			return (int) (_NUMBER_OF_ITERATIONS * (1 - _PCT_TOLERANCE_RATE));
		}

		private static double GetReferenceBenchmark()
		{
			return _UPLOAD_FILE_BENCHMARK_IN_MILLISECONDS +
			       _UPLOAD_FILE_BENCHMARK_IN_MILLISECONDS * _PCT_TOLERANCE_RATE;
		}
	}
}
