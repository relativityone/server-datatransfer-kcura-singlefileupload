using kCura.SingleFileUpload.Core.SQL;
using Relativity.API;
using Relativity.Services.DataContracts.DTOs.MetricsCollection;
using Relativity.Services.InternalMetricsCollection;
using Relativity.Telemetry.Services.Metrics;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public class TelemetryManager : BaseManager, ITelemetryManager
	{

		private Guid _workSpaceGuid;

		private const string _NUMBER_OF_DOCUMENT_UPLOADED = "Number of documents uploaded";

		private static readonly Lazy<ITelemetryManager> _INSTANCE = new Lazy<ITelemetryManager>(() => new TelemetryManager());

		public static ITelemetryManager Instance => _INSTANCE.Value;
		public TelemetryManager()
		{
		}

		private Guid WorkSpaceGuid
		{
			get
			{
				if (_workSpaceGuid == null || _workSpaceGuid == Guid.Parse("00000000-0000-0000-0000-000000000000"))
				{
					string workspaceWuid = _Repository.MasterDBContext.ExecuteSqlStatementAsScalar(Queries.GetWorkspaceGuidByArtifactID,
						new SqlParameter[]
						{
							new SqlParameter("@artifactId", _Repository.WorkspaceID)
						}
					).ToString();


					_workSpaceGuid = new Guid(workspaceWuid);
				}
				return _workSpaceGuid;
			}
		}

		public async Task LogCountAsync(string bucket, long count)
		{
			using (dynamic metricManger = _Repository.CreateProxy<IMetricsManager>(ExecutionIdentity.CurrentUser))
			{
				try
				{
					await metricManger.LogCountAsync(bucket, WorkSpaceGuid, count);
				}
				catch (Exception)
				{
					try
					{
						await metricManger.LogCountAsync(bucket, WorkSpaceGuid, MetricTargets.SUM, count);
					}
					catch (Exception ex)
					{
						LogError(ex);
					}
				}
			}
		}
		public async Task LogGaugeAsync(string bucket, long count)
		{
			using (dynamic metricManger = _Repository.CreateProxy<IMetricsManager>(ExecutionIdentity.CurrentUser))
			{
				try
				{
					await metricManger.LogGaugeAsync(bucket, WorkSpaceGuid, count);
				}
				catch (Exception)
				{
					try
					{
						await metricManger.LogGaugeAsync(bucket, WorkSpaceGuid, MetricTargets.SUM, count);
					}
					catch (Exception ex)
					{
						LogError(ex);
					}
				}
			}
		}
		public DurationLogger LogDuration(string bucket, string workflowId, long count)
		{
			using (var metricManger = _Repository.CreateProxy<IMetricsManager>(ExecutionIdentity.CurrentUser))
			{
				try
				{
					//return metricManger.LogDuration(bucket, workSpaceGuid, MetricTargets.SUM);
					return metricManger.LogDuration(bucket, WorkSpaceGuid, workflowId);
				}
				catch (Exception ex)
				{
					LogError(ex);
					return null;
				}
			}
		}

		public async Task CreateMetricsAsync()
		{
			try
			{
				using (var metricCollectionManager = _Repository.CreateProxy<IInternalMetricsCollectionManager>(ExecutionIdentity.System))
				{
					List<CategoryTarget> categories = await metricCollectionManager.GetCategoryTargetsAsync();
					CategoryTarget categoryTarget = categories.FirstOrDefault(x => x.Category.Name == Helpers.Constants.METRICS_CATEGORY);
					CategoryRef sfuCategory = categoryTarget?.Category;

					if (string.IsNullOrEmpty(sfuCategory?.Name))
					{
						sfuCategory = new Category { Name = Helpers.Constants.METRICS_CATEGORY };
						sfuCategory.ID = await metricCollectionManager.CreateCategoryAsync((Category)sfuCategory, false);
						/// if no category target... re-set it
						categories = await metricCollectionManager.GetCategoryTargetsAsync();
						categoryTarget = categories.FirstOrDefault(x => x.Category.Name == Helpers.Constants.METRICS_CATEGORY);

					}

					List<MetricIdentifier> metricIdentifiers = new List<MetricIdentifier>(),
						metrics = await metricCollectionManager.GetMetricIdentifiersByCategoryNameAsync(Helpers.Constants.METRICS_CATEGORY);

					MetricIdentifier numberOfDocsUploadedMetric = metrics.FirstOrDefault(x => x.Name == Helpers.Constants.BUCKET_DOCUMENTSUPLOADED),
						numberOfDocsReplacedMetric = metrics.FirstOrDefault(x => x.Name == Helpers.Constants.BUCKET_DOCUMENTSREPLACED),
						numberOfDocsUploadedBytesMetric = metrics.FirstOrDefault(x => x.Name == Helpers.Constants.BUCKET_TOTALSIZEDOCUMENTUPLOADED);

					if (string.IsNullOrEmpty(numberOfDocsUploadedMetric?.Name))
					{
						metricIdentifiers.Add(
							new MetricIdentifier
							{
								Categories = new List<CategoryRef> { sfuCategory },
								Name = Helpers.Constants.BUCKET_DOCUMENTSUPLOADED,
								Description = _NUMBER_OF_DOCUMENT_UPLOADED,
							}
						);
					}
					if (string.IsNullOrEmpty(numberOfDocsReplacedMetric?.Name))
					{
						metricIdentifiers.Add(
							new MetricIdentifier
							{
								Categories = new List<CategoryRef> { sfuCategory },
								Name = Helpers.Constants.BUCKET_DOCUMENTSREPLACED,
								Description = "Number of documents replaced"
							}
						);
					}
					if (string.IsNullOrEmpty(numberOfDocsUploadedBytesMetric?.Name))
					{
						metricIdentifiers.Add(
							new MetricIdentifier
							{
								Categories = new List<CategoryRef> { sfuCategory },
								Name = Helpers.Constants.BUCKET_TOTALSIZEDOCUMENTUPLOADED,
								Description = "Total size of documents uploaded in bytes"
							}
						);
					}

					foreach (var metricIdentifier in metricIdentifiers)
					{
						await metricCollectionManager.CreateMetricIdentifierAsync(metricIdentifier, false);
					}

					if (!categoryTarget.IsCategoryMetricTargetEnabled[CategoryMetricTarget.SUM])
					{
						categoryTarget.IsCategoryMetricTargetEnabled[CategoryMetricTarget.SUM] = true;
						await metricCollectionManager.UpdateCategoryTargetSingleAsync(categoryTarget);
					}
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
				throw;
			}
		}
		public async Task CreateMetricAsync(string bucket, string description)
		{
			try
			{
				using (var metricCollectionManager = _Repository.CreateProxy<IInternalMetricsCollectionManager>(ExecutionIdentity.System))
				{
					Category category = new Category { Name = Helpers.Constants.METRICS_CATEGORY };
					category.ID = await metricCollectionManager.CreateCategoryAsync(category, false);

					var metricId = new MetricIdentifier
					{
						Categories = new List<CategoryRef> { category },
						Name = bucket,
						Description = description
					};

					await metricCollectionManager.CreateMetricIdentifierAsync(metricId, false);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}


	}
}
