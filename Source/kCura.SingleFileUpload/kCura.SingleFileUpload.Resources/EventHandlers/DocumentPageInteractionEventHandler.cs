using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Helpers;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity;
using NSerio.Relativity.Infrastructure;
using System.Threading.Tasks;

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
                    _repository = new DocumentManager();
                return _repository;
            }
        }
        IDocumentManager _repository;



        public override Response PopulateScriptBlocks()
        {
            RepositoryHelper.ConfigureRepository(this.Helper);
            using (CacheContextScope d = RepositoryHelper.InitializeRepository(this.Helper.GetActiveCaseID()))
            {
                var hasImages = Repository.ValidateDocImages(this.ActiveArtifact.ArtifactID);

                if (this.PageMode == kCura.EventHandler.Helper.PageMode.View)
                {
                    PermissionHelper permissionHelper = new PermissionHelper(this.Helper);
                    var taskPermissions = Task.Run(async () => await permissionHelper.CurrentUserHasPermissionToObjectType(this.Helper.GetActiveCaseID(), Core.Helpers.Constants.DocumentObjectType, Core.Helpers.Constants.ReplaceImageUploadDownload));
                    
                    ScriptBlock sb = new ScriptBlock();
                    sb.Key = "sfuSource";
                    sb.Script = string.Concat("<script type=\"text/javascript\">", Javascript.SingleFileUploadScript.Replace("{{APPID}}", this.Application.ArtifactID.ToString()).
                                                                                                                     Replace("{{HasImages}}", hasImages.ToString().ToLower()).
                                                                                                                     Replace("{{DocID}}", this.ActiveArtifact.ArtifactID.ToString()), "</script>");
                    this.RegisterStartupScriptBlock(sb);
                }
            }
            return new Response { Success = true };
        }
    }
}
