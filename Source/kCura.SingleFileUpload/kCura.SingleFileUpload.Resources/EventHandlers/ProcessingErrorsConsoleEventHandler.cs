using kCura.EventHandler;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity.Infrastructure;
using System.Collections.Generic;

namespace kCura.SingleFileUpload.Resources.EventHandlers
{
    [kCura.EventHandler.CustomAttributes.Description("Single File Upload Processing Errors Console Event Handler")]
    [System.Runtime.InteropServices.Guid("5A3586D2-E54C-4A87-BE60-D88B0114C57D")]
    public class ProcessingErrorsConsoleEventHandler : ConsoleEventHandler
    {
        IProcessingManager Repository
        {
            get
            {
                if (_repository == null)
                    _repository = new ProcessingManager();
                return _repository;
            }
        }

        public override FieldCollection RequiredFields
        {
            get
            {
                return new FieldCollection();
            }
        }

        IProcessingManager _repository;

        public override EventHandler.Console GetConsole(PageEvent pageEvent)
        {
            string basePath = this.Application.ApplicationUrl.Substring(0, this.Application.ApplicationUrl.IndexOf("/Case/Mask/"));
            EventHandler.Console returnConsole = new EventHandler.Console();
            NSerio.Relativity.RepositoryHelper.ConfigureRepository(this.Helper);
            using (CacheContextScope d = NSerio.Relativity.RepositoryHelper.InitializeRepository(this.Helper.GetActiveCaseID()))
            {
                returnConsole.Items = new List<IConsoleItem>();
                returnConsole.Title = "ERROR ACTIONS";
                
                string windowOpenJavaScript = "openSFUErrorModal();";

               
              var error = Repository.GetErrorInfo(this.ActiveArtifact.ArtifactID);
                ConsoleLiteral literal = new ConsoleLiteral(Javascript.SingleFileUploadErrorsScript.Replace("{APPID}", this.Helper.GetActiveCaseID().ToString()).Replace("{ERRORID}", this.ActiveArtifact.ArtifactID.ToString()));

                ConsoleButton openSFUButton = new ConsoleButton();
                openSFUButton.Name = "ReplaceFile";
                openSFUButton.DisplayText = "Replace File";
                openSFUButton.ToolTip = "Replace File";
                openSFUButton.Enabled = 
                    !(error.DocumentFileLocation == error.SourceLocation);
                openSFUButton.RaisesPostBack = false;
                openSFUButton.OnClickEvent = windowOpenJavaScript;
                returnConsole.Items.Add(openSFUButton);
                returnConsole.Items.Add(literal);
            }
            return returnConsole;
        }

       

        public override void OnButtonClick(ConsoleButton consoleButton)
        {

        }
    }
}
