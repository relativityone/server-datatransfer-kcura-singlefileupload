using Moq;
using Relativity.Services.InternalMetricsCollection;
using Relativity.Telemetry.Services.Metrics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.NUnit.Helpers
{
	public static class MockMetricsManagerHelper
	{
		public static Mock<IMetricsManager> GetMockedMetricsManager()
		{
			return new Mock<IMetricsManager>();
		}

		public static Mock<IMetricsManager> MockLogCounter(this Mock<IMetricsManager> mockingMetricsManager)
		{
			mockingMetricsManager
				.Setup(p => p.LogCountAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<long>()))
				.Returns(Task.CompletedTask);

			return mockingMetricsManager;
		}

		public static Mock<IInternalMetricsCollectionManager> MockCreateCategory(this Mock<IInternalMetricsCollectionManager> mockingInternalMetricsColletion)
		{
			mockingInternalMetricsColletion
				.Setup(p => p.CreateCategoryAsync(It.IsAny<Category>(), It.IsAny<bool>()))
				.Returns(Task.FromResult<int>(1));

			mockingInternalMetricsColletion
				.Setup(p => p.GetCategoryTargetsAsync())
				.Returns(Task.FromResult(Categories()));

			mockingInternalMetricsColletion
				.Setup(p => p.GetMetricIdentifiersByCategoryNameAsync(It.IsAny<string>()))
				.Returns(Task.FromResult(new List<MetricIdentifier>()));

			mockingInternalMetricsColletion
				.Setup(p => p.CreateMetricIdentifierAsync(It.IsAny<MetricIdentifier>(), It.IsAny<bool>()))
				.Returns(Task.FromResult<int>(1));

			return mockingInternalMetricsColletion;
		}

		private static List<CategoryTarget> Categories()
		{
			Dictionary<CategoryMetricTarget, bool> categoryMetricTarget = new Dictionary<CategoryMetricTarget, bool>();

			categoryMetricTarget.Add(CategoryMetricTarget.SUM, true);

			List<CategoryTarget> result = new List<CategoryTarget>
			{
				new CategoryTarget
				{
					IsCategoryMetricTargetEnabled = categoryMetricTarget,
					Category = new CategoryRef
					{
						Name = Core.Helpers.Constants.METRICS_CATEGORY,
						ID = 1
					},
				},
				new CategoryTarget
				{
					Category = new CategoryRef
					{
						Name = Core.Helpers.Constants.METRICS_CATEGORY,
						ID = 1
					},
				}

			};

			return result;
		}

	}
}
