using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IWorkflowsManager : IManager
    {
        Task<bool> IsAutomatedWorkflowInstalledAsync();
        Task SendAutomatedWorkflowsTriggerAsync(bool jobEndedWithErrors);
    }
}
