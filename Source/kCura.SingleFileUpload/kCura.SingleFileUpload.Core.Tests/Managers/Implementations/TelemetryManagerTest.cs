using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.SQL;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.InternalMetricsCollection;
using Relativity.Telemetry.Services.Metrics;
using System;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	public class TelemetryManagerTest : TestBase
	{
		private const string _BUCKET = "Test.Bucket";

		[SetUp]
		public void Setup()
		{
			Mock<IHelper> mockingHelper = new Mock<IHelper>();

			mockingHelper
				.MockIDBContextOnHelper()
				.MockExecuteSqlStatementAsScalar(Queries.GetWorkspaceGuidByArtifactID, Guid.NewGuid().ToString());

			Mock<IInternalMetricsCollectionManager> mockCategory = new Mock<IInternalMetricsCollectionManager>();

			Mock<IServicesMgr> mockingServicesMgr = mockingHelper
				.MockIServiceMgr()
				.MockService<IMetricsManager>()
				.MockService(mockCategory.MockCreateCategory());
			
			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public async Task LogCountTest()
		{
			await TelemetryManager.Instance.LogCountAsync(_BUCKET, 1);
			Assert.IsTrue(true);
		}

		[Test]
		public async Task LogGaugeTest()
		{
			await TelemetryManager.Instance.LogGaugeAsync(_BUCKET, 1);
			Assert.IsTrue(true);
		}

		[Test]
		public void LogDurationTest()
		{
			DurationLogger result = TelemetryManager.Instance.LogDuration(_BUCKET, "", 1);
			Assert.IsTrue(true);
		}

		[Test]
		public async Task CreateMetricsTest()
		{
			await TelemetryManager.Instance.CreateMetricsAsync();
			Assert.IsTrue(true);
		}

		[Test]
		public async Task CreateMetricTest()
		{
			await TelemetryManager.Instance.CreateMetricAsync(_BUCKET, "");
			Assert.IsTrue(true);
		}

	}
}
