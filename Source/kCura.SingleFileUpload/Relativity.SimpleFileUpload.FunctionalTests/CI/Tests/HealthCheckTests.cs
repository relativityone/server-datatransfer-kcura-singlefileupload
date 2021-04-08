using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Relativity.Testing.Identification;

namespace Relativity.SimpleFileUpload.FunctionalTests.CI.Tests
{
	[TestFixture]
	[TestExecutionCategory.CI, TestLevel.L3]
	public class HealthCheckTests : FunctionalTestsTemplate
	{
		public HealthCheckTests() : base(nameof(HealthCheckTests))
		{
		}

		[IdentifiedTest("8854021C-D93A-4A55-A3E7-6D127C144BD3")]
		public async Task UploadNativeFile_GoldFlow()
		{
			// Act
			HttpResponseMessage result = await Client.GetAsync("home/healthcheck").ConfigureAwait(false);

			// Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
		}
	}
}
