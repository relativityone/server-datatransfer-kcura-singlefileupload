using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Helpers;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;
using System.Threading.Tasks;
using System.Linq;

namespace kCura.SingleFileUpload.Resources.EventHandlers
{
    [kCura.EventHandler.CustomAttributes.Description("Single File Upload Page Interaction Event Handler for Document")]
    [System.Runtime.InteropServices.Guid("FC7DA2FE-200A-44EA-A3F5-BFBF6C306143")]
    public class DocumentPageInteractionEventHandler : PageInteractionEventHandler
    {

        IDocumentManager Repository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = new DocumentManager();
                }
                return _repository;
            }
        }
        IDocumentManager _repository;



        public override Response PopulateScriptBlocks()
        {
            return new Response { Success = true };
        }
    }
}
