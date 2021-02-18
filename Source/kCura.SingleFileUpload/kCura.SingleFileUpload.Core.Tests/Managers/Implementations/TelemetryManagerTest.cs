using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.SQL;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.InternalMetricsCollection;
using Relativity.Telemetry.Services.Metrics;
using System;
using FluentAssertions;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
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

			mockingHelper
				.MockIServiceMgr()
				.MockService<IMetricsManager>()
				.MockService(mockCategory.MockCreateCategory());
			
			ConfigureSingletoneRepositoryScope(mockingHelper.Object);
		}

		[Test]
		public void LogCount_ShouldNotThrow()
		{
			// Act
			Action action = () => TelemetryManager.Instance.LogCountAsync(_BUCKET, 1);
			
			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void LogGauge_ShouldNotThrow()
		{
			// Act
			Action action = () => TelemetryManager.Instance.LogGaugeAsync(_BUCKET, 1);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void LogDuration_ShouldNotThrow()
		{
			// Act
			Func<DurationLogger> action = () => TelemetryManager.Instance.LogDuration(_BUCKET, "", 1);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void CreateMetrics_ShouldNotThrow()
		{
			// Act
			Action action = () => TelemetryManager.Instance.CreateMetricsAsync();

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public void CreateMetric_ShouldNotThrow()
		{
			// Act
			Action action = () => TelemetryManager.Instance.CreateMetricAsync(_BUCKET, "");

			// Assert
			action.Should().NotThrow();
		}

	}
}
