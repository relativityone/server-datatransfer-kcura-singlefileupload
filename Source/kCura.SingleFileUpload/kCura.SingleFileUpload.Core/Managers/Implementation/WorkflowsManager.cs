using kCura.SingleFileUpload.Core.Relativity;
using Relativity;
using Relativity.API;
using Relativity.AutomatedWorkflows.SDK;
using Relativity.AutomatedWorkflows.SDK.V2.Models;
using Relativity.AutomatedWorkflows.SDK.V2.Models.Triggers;
using Relativity.Services.Interfaces.ObjectType;
using Relativity.Services.Interfaces.ObjectType.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class WorkflowsManager : BaseManager, IWorkflowsManager
    {
        private const string _RELATIVITY_APP_NAME = "Relativity Application";
        private const string _AUTOMATED_WORKFLOWS_APP_NAME = "Automated Workflows";
        private const string _TRIGGER_INPUT_ID = "type";
        private const string _TRIGGER_INPUT_VALUE = "sfu";
        private const string _TRIGGER_ID = "relativity@on-new-documents-added";
        private const string _TRIGGER_STATUS_COMPLETED = "complete";
        private const string _TRIGGER_STATUS_COMPLETED_ERRORS = "complete-with-errors";

        private readonly IAutomatedWorkflowsManager _automatedWorkflowsManager;

        private static readonly Lazy<IWorkflowsManager> _instance = new Lazy<IWorkflowsManager>(() => new WorkflowsManager());

        public static IWorkflowsManager Instance => _instance.Value;
        private IAPILog _logger => Repository.Instance.GetLogFactory().GetLogger();

        private WorkflowsManager()
        {
            IHelper helper = Repository.Instance.Helper;
            _automatedWorkflowsManager = new AutomatedWorkflowsManager(helper);
        }

        public async Task SendAutomatedWorkflowsTriggerAsync(bool jobEndedWithErrors)
        {
            try
            {
                _logger.LogInformation("Entering SendAutomatedWorkflowsTriggerAsync. Job ended with errors: {0}", jobEndedWithErrors);               

                SendTriggerBody body = new SendTriggerBody
                {
                    Inputs = new List<TriggerInput>() { new TriggerInput { ID = _TRIGGER_INPUT_ID, Value = _TRIGGER_INPUT_VALUE } },
                    State = jobEndedWithErrors ? _TRIGGER_STATUS_COMPLETED_ERRORS : _TRIGGER_STATUS_COMPLETED
                };
                _logger.LogInformation("Trigger body content prepared. State: {0}", body.State);

                Response response = await _automatedWorkflowsManager.SendTriggerAsync(_Repository.WorkspaceID, _TRIGGER_ID, body).ConfigureAwait(false);
                _logger.LogInformation("Send trigger result: Success = {success}, Messages = {messages}", 
                    response.Success, string.Join(",", response.Messages));

            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public async Task<bool> IsAutomatedWorkflowInstalledAsync()
        {
            try
            {
                using (IObjectManager objectManager = _Repository.CreateProxy<IObjectManager>())
                {
                    using (IObjectTypeManager objectTypeManager = _Repository.CreateProxy<IObjectTypeManager>())
                    {
                        QueryResult relativityAppQueryResult = await objectManager.QueryAsync(_Repository.WorkspaceID, CreateRequest((int)ArtifactType.ObjectType, _RELATIVITY_APP_NAME), 0, 1)
                            .ConfigureAwait(false);

                        if (relativityAppQueryResult.ResultCount == 0)
                        {
                            LogError(new Exception($"Could not find the {_RELATIVITY_APP_NAME} object type."));
                            return false;
                        }

                        ObjectTypeResponse objectTypeMetadata = await objectTypeManager.ReadAsync(_Repository.WorkspaceID, relativityAppQueryResult.Objects[0].ArtifactID)
                            .ConfigureAwait(false);

                        QueryResult automatedWorkflowsQueryResult = await objectManager.QueryAsync(_Repository.WorkspaceID, CreateRequest(objectTypeMetadata.ArtifactTypeID, _AUTOMATED_WORKFLOWS_APP_NAME), 0, 1)
                            .ConfigureAwait(false);

                        return automatedWorkflowsQueryResult.TotalCount > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        private QueryRequest CreateRequest(int artifactTypeId, string appName)
        {
            return new QueryRequest
            {
                ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
                Condition = $"'Name' == '{appName}'"
            };
        }
    }
}
