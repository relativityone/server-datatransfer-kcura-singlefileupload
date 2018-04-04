using Relativity.Telemetry.Services.Metrics;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface ITelemetryManager
    {
        Task LogCountAsync(string bucket, long count);
        Task LogGaugeAsync(string bucket, long count);
        DurationLogger LogDuration(string bucket, string workflowId, long count);
      
        Task CreateMetricsAsync();
        Task CreateMetricAsync(string bucket, string description);
    }
}
