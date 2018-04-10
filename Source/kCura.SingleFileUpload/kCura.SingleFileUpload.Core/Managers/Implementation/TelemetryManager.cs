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
                    return metricManger.LogDuration(bucket, workSpaceGuid, workflowId);
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
                    var categories = await metricCollectionManager.GetCategoryTargetsAsync();
                    var categoryTarget = categories.FirstOrDefault(x => x.Category.Name == Helpers.Constants.METRICS_CATEGORY);
                    var sfuCategory = categoryTarget.Category;
                    if (sfuCategory == null)
                    {
                        sfuCategory = new Category { Name = Helpers.Constants.METRICS_CATEGORY };
                        sfuCategory.ID = await metricCollectionManager.CreateCategoryAsync((Category)sfuCategory, false);
                    }
                    var metrics = await metricCollectionManager.GetMetricIdentifiersByCategoryNameAsync(Helpers.Constants.METRICS_CATEGORY);
                    var numberOfDocsUploadedMetric = metrics.FirstOrDefault(x => x.Name == Helpers.Constants.BUCKET_DocumentsUploaded);
                    var numberOfDocsReplacedMetric = metrics.FirstOrDefault(x => x.Name == Helpers.Constants.BUCKET_DocumentsReplaced);
                    var numberOfDocsUploadedBytesMetric = metrics.FirstOrDefault(x => x.Name == Helpers.Constants.BUCKET_TotalSizeDocumentUploaded);
                    List<MetricIdentifier> metricIdentifiers = new List<MetricIdentifier>();

                    if(numberOfDocsUploadedMetric == null)
                    {
                        metricIdentifiers.Add(new MetricIdentifier
                        {
                            Categories = new List<CategoryRef> { sfuCategory },
                            Name = Helpers.Constants.BUCKET_DocumentsUploaded,
                            Description = "Number of documents uploaded"
                        });
                    }
                    if (numberOfDocsReplacedMetric == null)
                    {
                        metricIdentifiers.Add(new MetricIdentifier
                        {
                            Categories = new List<CategoryRef> { sfuCategory },
                            Name = Helpers.Constants.BUCKET_DocumentsReplaced,
                            Description = "Number of documents replaced"
                        });
                    }
                    if (numberOfDocsUploadedBytesMetric == null)
                    {
                        metricIdentifiers.Add(new MetricIdentifier
                        {
                            Categories = new List<CategoryRef> { sfuCategory },
                            Name = Helpers.Constants.BUCKET_TotalSizeDocumentUploaded,
                            Description = "Total size of documents uploaded in bytes"
                        });
                    }

                    foreach (var metricIdentifier in metricIdentifiers)
                        await metricCollectionManager.CreateMetricIdentifierAsync(metricIdentifier, false);
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
