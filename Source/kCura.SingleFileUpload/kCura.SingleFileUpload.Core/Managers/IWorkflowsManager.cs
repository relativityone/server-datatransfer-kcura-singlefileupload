using kCura.SingleFileUpload.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IWorkflowsManager : IManager
    {
        Task<bool> AutomatedWorkflowInstalledAsync();
        Task SendAutomatedWorkflowsTriggerAsync(bool jobEndedWithErrors);
    }
}
