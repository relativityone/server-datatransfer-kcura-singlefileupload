using kCura.Relativity.Client;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.NUnit.Helpers;
using kCura.SingleFileUpload.Core.SQL;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.InternalMetricsCollection;
using Relativity.Telemetry.Services.Metrics;
using System;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.NUnit.Managers.Implementations
{
	[TestFixture]
	public class TelemetryManagerTest : TestBase
	{
		private const string _BUCKET = "Test.Bucket";

		[OneTimeSetUp]
		public void Setup()
		{
			Mock<IRSAPIClient> rsapi = RSAPIClientMockHelper.GetMockedHelper();

			Mock<IHelper> mockingHelper =
				MockHelper.GetMockingHelper<IHelper>();

			mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsScalar(Queries.GetWorkspaceGuidByArtifactID, Guid.NewGuid().ToString());

			Mock<IInternalMetricsCollectionManager> mockCategory = new Mock<IInternalMetricsCollectionManager>();

			Mock<IServicesMgr> mockingServicesMgr = mockingHelper
				.MockIServiceMgr()
				.MockService(rsapi)
				.MockService<IMetricsManager>()
				.MockService(mockCategory.MockCreateCategory());


			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public async Task LogCountTest()
		{
			await TelemetryManager.instance.LogCountAsync(_BUCKET, 1);
			Assert.IsTrue(true);
		}

		[Test]
		public async Task LogGaugeTest()
		{
			await TelemetryManager.instance.LogGaugeAsync(_BUCKET, 1);
			Assert.IsTrue(true);
		}

		[Test]
		public void LogDurationTest()
		{
			DurationLogger result = TelemetryManager.instance.LogDuration(_BUCKET, "", 1);
			Assert.IsTrue(true);
		}

		[Test]
		public async Task CreateMetricsTest()
		{
			await TelemetryManager.instance.CreateMetricsAsync();
			Assert.IsTrue(true);
		}

		[Test]
		public async Task CreateMetricTest()
		{
			await TelemetryManager.instance.CreateMetricAsync(_BUCKET, "");
			Assert.IsTrue(true);
		}

	}
}
