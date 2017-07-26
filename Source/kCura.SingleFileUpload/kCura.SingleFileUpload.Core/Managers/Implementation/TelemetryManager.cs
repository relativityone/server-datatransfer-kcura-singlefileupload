using System;
using System.Collections.Generic;
using System.Linq;
using Relativity.API;
using System.Threading.Tasks;
using Relativity.Services.DataContracts.DTOs.MetricsCollection;
using Relativity.Telemetry.Services.Metrics;
using Relativity.Services.InternalMetricsCollection;
using System.Data.SqlClient;
using kCura.SingleFileUpload.Core.SQL;
using kCura.SingleFileUpload.Core.Helpers;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class TelemetryManager:BaseManager , ITelemetryManager
    {
        public TelemetryManager()
        {
        }

        Guid workSpaceGuid
        {
            get
            {
                if (_workSpaceGuid == null || _workSpaceGuid == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    _workSpaceGuid = new Guid(_Repository.MasterDBContext.ExecuteSqlStatementAsScalar(Queries.GetWorkspaceGuidByArtifactID, new SqlParameter[] { new SqlParameter("@artifactId", _Repository.WorkspaceID) }).ToString());
                }
                return _workSpaceGuid;
            }
        }

        private Guid _workSpaceGuid;

        public async Task LogCountAsync(string bucket, long count)
        {
            using (dynamic metricManger = _Repository.CreateProxy<IMetricsManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    await metricManger.LogCountAsync(bucket, workSpaceGuid, count);
                }
                catch (Exception)
                {
                    try
                    {
                        await metricManger.LogCountAsync(bucket, workSpaceGuid, MetricTargets.SUM, count);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, ex.Message);
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
                    await metricManger.LogGaugeAsync(bucket, workSpaceGuid, count);
                }
                catch (Exception)
                {
                    try
                    {
                        await metricManger.LogGaugeAsync(bucket, workSpaceGuid, MetricTargets.SUM, count);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, ex.Message);
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
                    return metricManger.LogDuration(bucket, workSpaceGuid,workflowId);
                }
                catch (Exception ex)
                {
                    LogError(ex, ex.Message);
                    return null;
                }
            }
        }
        public async Task LogPointInTimeLongAsync(string bucket, string WorkFlowId, long value)
        {
            using (var metricManger = _Repository.CreateProxy<IMetricsManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    await metricManger.LogPointInTimeLongAsync(bucket, workSpaceGuid, WorkFlowId, value);
                }
                catch (Exception ex)
                {
                    LogError(ex, ex.Message);
                }
            }
        }
        public async Task LogPointInTimeStringAsync(string bucket, string WorkFlowId, string value)
        {
            using (var metricManger = _Repository.CreateProxy<IMetricsManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    await metricManger.LogPointInTimeStringAsync(bucket, workSpaceGuid, WorkFlowId, value);
                }
                catch (Exception ex)
                {
                    LogError(ex, ex.Message);
                }
            }
        }
        public async Task LogPointInTimeStringAsync(string bucket, double value)
        {
            using (var metricManger = _Repository.CreateProxy<IMetricsManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    //await metricManger.LogTimerAsDoubleAsync(bucket, workSpaceGuid, MetricTargets.SUM, value);
                    await metricManger.LogTimerAsDoubleAsync(bucket, workSpaceGuid, value);
                }
                catch (Exception ex)
                {
                    LogError(ex, ex.Message);
                }
            }
        }
        public async Task LogTimerAsLongAsync(string bucket, long value)
        {
            using (var metricManger = _Repository.CreateProxy<IMetricsManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    //await metricManger.LogTimerAsLongAsync(bucket, workSpaceGuid, MetricTargets.SUM, value);
                    await metricManger.LogTimerAsLongAsync(bucket, workSpaceGuid, value);
                }
                catch (Exception ex)
                {
                    LogError(ex, ex.Message);
                }
            }
        }
        public async Task CreateMetricsAsync()
        {
            try
            {
                using (var metricCollectionManager = _Repository.CreateProxy<IInternalMetricsCollectionManager>(ExecutionIdentity.System))
                {
                    Category category = new Category { Name = Helpers.Constants.METRICS_CATEGORY };
                    category.ID = await metricCollectionManager.CreateCategoryAsync(category, false);

                    List<MetricIdentifier> metricIdentifiers = new List<MetricIdentifier>
                    {
                        new MetricIdentifier
                        {
                            Categories = new List<CategoryRef> { category},
                            Name = Helpers.Constants.BUCKET_DocumentsUploaded,
                            Description = "Number of documents uploaded"
                        },

                        new MetricIdentifier
                        {
                            Categories = new List<CategoryRef> { category},
                            Name = Helpers.Constants.BUCKET_DocumentsReplaced,
                            Description = "Number of documents replaced"
                        },

                        new MetricIdentifier
                        {
                            Categories = new List<CategoryRef> { category},
                            Name = Helpers.Constants.BUCKET_TotalSizeDocumentUploaded,
                            Description = "Total size of documents uploaded in bytes"
                        }
                    };

                    foreach (var metricIdentifier in metricIdentifiers)
                        await metricCollectionManager.CreateMetricIdentifierAsync(metricIdentifier, false);

                    List<CategoryTarget> targets = await metricCollectionManager.GetCategoryTargetsAsync();
                    CategoryTarget categoryTarget = targets.FirstOrDefault(x => x.Category.Name == Helpers.Constants.METRICS_CATEGORY);
                    categoryTarget.IsCategoryMetricTargetEnabled[CategoryMetricTarget.SUM] = true;
                    await metricCollectionManager.UpdateCategoryTargetsAsync(targets);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, ex.Message);
                throw ex;
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
                LogError(ex, ex.Message);
            }
        }
        private void LogError(Exception e, string msg)
        {
            var error = new Relativity.Client.DTOs.Error
            {
                FullError = e.ToString(),
                Message = EventLogHelper.GetRecursiveExceptionMsg(e),
                Server = Environment.MachineName,
                Source = "WEB - Single File Upload",
                SendNotification = false,
                Workspace = new Relativity.Client.DTOs.Workspace(-1),
                URL = string.Empty
            };
            _Repository.RSAPISystem.Repositories.Error.CreateSingle(error);
            _Repository.GetLogFactory().GetLogger().ForContext<TelemetryManager>().LogError(e, "Something occurred in Single File Upload {@message}", e.Message);
        }

      
    }
}
