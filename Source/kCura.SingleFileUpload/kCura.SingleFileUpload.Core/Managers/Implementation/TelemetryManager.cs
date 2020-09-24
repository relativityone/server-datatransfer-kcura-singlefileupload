using kCura.SingleFileUpload.Core.SQL;
using Relativity.API;
using Relativity.Services.DataContracts.DTOs.MetricsCollection;
using Relativity.Services.InternalMetricsCollection;
using Relativity.Telemetry.APM;
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

		private const string _JOB_START_TIME_STAMP = "JobStartTimeStamp";
		private const string _JOB_END_TIME_STAMP = "JobEndTimeStamp";
		private const string _JOB_STATUS = "JobStatus";
		private const string _WORKSPACE_ID = "WorkspaceID";
		private const string _WORKFLOW_NAME = "WorkflowName";
		private const string _SFU_WORKFLOW_NAME = "SimpleFileUpload";
		private const string _STAGE_NAME = "StageName";
		private const string _OVERALL_STAGE = "Overall";
		private const string _RECORD_NUMBER = "RecordNumber";
		private const string _RECORD_TYPE = "RecordType";
		private const string _RECORD_TYPE_DOCUMENTS = "Documents";
		private const string _APM_CATEGORY = "APMCategory";
		private const string _PERFORMANCE_BATCH_JOB_CATEGORY = "PerformanceBatchJob";

		private static readonly Lazy<ITelemetryManager> _INSTANCE = new Lazy<ITelemetryManager>(() => new TelemetryManager());

		public const string DOCUMENT_BATCH_JOB_STATUS_STARTED = "Started";
		public const string DOCUMENT_BATCH_JOB_STATUS_COMPLETED = "Completed";
		public const string DOCUMENT_BATCH_JOB_STATUS_FAILED = "Failed";

		public static ITelemetryManager Instance => _INSTANCE.Value;

		private Guid WorkSpaceGuid
		{
			get
			{
				if (_workSpaceGuid == Guid.Parse("00000000-0000-0000-0000-000000000000"))
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
					List<CategoryTarget> categories = await metricCollectionManager.GetCategoryTargetsAsync().ConfigureAwait(false);
					CategoryTarget categoryTarget = categories.FirstOrDefault(x => x.Category.Name == Helpers.Constants.METRICS_CATEGORY);
					CategoryRef sfuCategory = categoryTarget?.Category;

					if (string.IsNullOrEmpty(sfuCategory?.Name))
					{
						sfuCategory = new Category { Name = Helpers.Constants.METRICS_CATEGORY };
						sfuCategory.ID = await metricCollectionManager.CreateCategoryAsync((Category)sfuCategory, false).ConfigureAwait(false);

						// if no category target... re-set it
						categories = await metricCollectionManager.GetCategoryTargetsAsync().ConfigureAwait(false);
						categoryTarget = categories.FirstOrDefault(x => x.Category.Name == Helpers.Constants.METRICS_CATEGORY);
					}

					List<MetricIdentifier> metricIdentifiers = new List<MetricIdentifier>(),
						metrics = await metricCollectionManager.GetMetricIdentifiersByCategoryNameAsync(Helpers.Constants.METRICS_CATEGORY).ConfigureAwait(false);

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
						await metricCollectionManager.CreateMetricIdentifierAsync(metricIdentifier, false).ConfigureAwait(false);
					}

					if (!categoryTarget.IsCategoryMetricTargetEnabled[CategoryMetricTarget.SUM])
					{
						categoryTarget.IsCategoryMetricTargetEnabled[CategoryMetricTarget.SUM] = true;
						await metricCollectionManager.UpdateCategoryTargetSingleAsync(categoryTarget).ConfigureAwait(false);
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
					category.ID = await metricCollectionManager.CreateCategoryAsync(category, false).ConfigureAwait(false);

					var metricId = new MetricIdentifier
					{
						Categories = new List<CategoryRef> { category },
						Name = bucket,
						Description = description
					};

					await metricCollectionManager.CreateMetricIdentifierAsync(metricId, false).ConfigureAwait(false);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

		public Guid LogImportDocumentBatchJobStarted(string operationName)
		{
			Guid correlationId = Guid.NewGuid();

			Client.APMClient.GaugeOperation(name:
				operationName, operation: 
				() => 1, 
				correlationID: correlationId.ToString(), 
				unitOfMeasure: "Job", 
				customData: GetImportDocumentBatchJobCustomData(_JOB_START_TIME_STAMP, DOCUMENT_BATCH_JOB_STATUS_STARTED))
				.Write();

			return correlationId;
		}

		public void LogImportDocumentBatchJobEnded(Guid correlationId, string operationName, string status)
		{
			Client.APMClient.GaugeOperation(name:
					operationName, operation:
					() => 1,
					correlationID: correlationId.ToString(),
					unitOfMeasure: "Job",
					customData: GetImportDocumentBatchJobCustomData(_JOB_END_TIME_STAMP, status))
				.Write();
		}

		private Dictionary<string, object> GetImportDocumentBatchJobCustomData(string jobTimeStampType, string status)
		{
			return new Dictionary<String, object>
			{
				{ jobTimeStampType, (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() },
				{ _JOB_STATUS, status },
				{ _WORKSPACE_ID, WorkspaceID },
				{ _WORKFLOW_NAME, _SFU_WORKFLOW_NAME },
				{ _STAGE_NAME, _OVERALL_STAGE },
				// SFU is always sending one document at a time
				{ _RECORD_NUMBER, 1 },
				{ _RECORD_TYPE, _RECORD_TYPE_DOCUMENTS },
				{ _APM_CATEGORY, _PERFORMANCE_BATCH_JOB_CATEGORY }
			};
		}
	}
}
