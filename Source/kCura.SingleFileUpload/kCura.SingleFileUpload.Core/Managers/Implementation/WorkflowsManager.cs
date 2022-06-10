using Relativity;
using Relativity.AutomatedWorkflows.Services.Interfaces.v1.Models.Triggers;
using Relativity.AutomatedWorkflows.Services.Interfaces.v1.Services;
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
        private const string _TRIGGER_STATUS_COMPLETED = "completed";
        private const string _TRIGGER_STATUS_COMPLETED_ERRORS = "completed-with-errors";

        private static readonly Lazy<IWorkflowsManager> _instance = new Lazy<IWorkflowsManager>(() => new WorkflowsManager());
        public static IWorkflowsManager Instance => _instance.Value;

        public async Task SendAutomatedWorkflowsTriggerAsync(bool jobEndedWithErrors)
        {
            try
            {               
                using (IAutomatedWorkflowsService workflowsService = _Repository.CreateProxy<IAutomatedWorkflowsService>())
                {                    
                    SendTriggerBody body = new SendTriggerBody
                    {
                        Inputs = new List<TriggerInput>() { new TriggerInput { ID = _TRIGGER_INPUT_ID, Value = _TRIGGER_INPUT_VALUE } },
                        State = jobEndedWithErrors ? _TRIGGER_STATUS_COMPLETED_ERRORS : _TRIGGER_STATUS_COMPLETED                        
                    };
                    await workflowsService.SendTriggerAsync(_Repository.WorkspaceID, _TRIGGER_ID, body).ConfigureAwait(false);
                }                   
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public async Task<bool> AutomatedWorkflowInstalledAsync()
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

                        QueryResult automatedWorkflowsQueryResult = await objectManager.QueryAsync(_Repository.WorkspaceID, CreateRequest(objectTypeMetadata.ArtifactTypeID, _AUTOMATED_WORKFLOWS_APP_NAME), 0, 0)
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
