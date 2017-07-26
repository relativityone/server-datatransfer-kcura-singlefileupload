using System.Threading.Tasks;
using Relativity.Telemetry.Services.Metrics;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface ITelemetryManager
    {
        Task LogCountAsync(string bucket, long count);
        Task LogGaugeAsync(string bucket, long count);
        DurationLogger LogDuration(string bucket, string workflowId, long count);
        Task LogPointInTimeLongAsync(string bucket, string WorkFlowId, long value);
        Task LogPointInTimeStringAsync(string bucket, string WorkFlowId, string value);
        Task LogPointInTimeStringAsync(string bucket, double value);
        Task LogTimerAsLongAsync(string bucket, long value);
        Task CreateMetricsAsync();
        Task CreateMetricAsync(string bucket, string description);
    }
}
