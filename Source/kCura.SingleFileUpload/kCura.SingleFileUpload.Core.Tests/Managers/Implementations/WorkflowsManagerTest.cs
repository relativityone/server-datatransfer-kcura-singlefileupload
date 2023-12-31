﻿using FluentAssertions;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity;
using Relativity.API;
using Relativity.AutomatedWorkflows.Services.Interfaces.v1.Models.Triggers;
using Relativity.AutomatedWorkflows.Services.Interfaces.v1.Services;
using Relativity.Services.Interfaces.ObjectType;
using Relativity.Services.Interfaces.ObjectType.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Testing.Identification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
    [TestFixture]
    [TestLevel.L0]
    [TestExecutionCategory.CI]
    public class WorkflowsManagerTest : TestBase
    {
        private const string _RELATIVITY_APP_NAME = "Relativity Application";
        private const string _AUTOMATED_WORKFLOWS_APP_NAME = "Automated Workflows";
        private const string _TRIGGER_ID = "relativity@on-new-documents-added";
        private const string _TRIGGER_STATUS_COMPLETED = "complete";
        private const string _TRIGGER_STATUS_COMPLETED_ERRORS = "complete-with-errors";
        private const int _RELATIVITY_APP_ARTIFACT_TYPE_ID = 123456;
        private const int _RELATIVITY_OBJECT_ARTIFACT_ID = 654321;


        [TestCase(1, true)]
        [TestCase(0, false)]
        public async Task AutomatedWorkflowInstalledAsync_ShouldReturnCorrectValue(int automatedWorkflowsInstalledInstances, bool expectedResult)
        {
            //Arrange
            Mock<IHelper> mockingHelper = new Mock<IHelper>();
            Mock<IObjectManager> objectManagerFake = GetObjectManagerFake(automatedWorkflowsInstalledInstances);
            Mock<IObjectTypeManager> objectTypeManagerFake = GetObjectTypeManagerFake();
            mockingHelper
                .MockIServiceMgr()
                .MockService(objectManagerFake)
                .MockService(objectTypeManagerFake);

            ConfigureSingletoneRepositoryScope(mockingHelper.Object);
          
            //Act
            bool result = await WorkflowsManager.Instance.IsAutomatedWorkflowInstalledAsync().ConfigureAwait(false);

            //Assert
            result.Should().Be(expectedResult);
        }

        [TestCase(true, _TRIGGER_STATUS_COMPLETED_ERRORS)]
        [TestCase(false, _TRIGGER_STATUS_COMPLETED)]
        public async Task SendAutomatedWorkflowsTriggerAsync_ShouldCallSendTriggerAsyncMethod_WithCorrectState(bool uploadWithErrors, string expectedState)
        {
            //Arrange
            Mock<IHelper> mockingHelper = new Mock<IHelper>();
            Mock<IAutomatedWorkflowsService> automatedWorkflowsService = new Mock<IAutomatedWorkflowsService>();

            mockingHelper
                .MockIServiceMgr()
                .MockService(automatedWorkflowsService);
            ConfigureSingletoneRepositoryScope(mockingHelper.Object);

            Mock<IAPILog> mockApiLog = new Mock<IAPILog>();
            mockApiLog.Setup(p => p.ForContext<WorkflowsManager>())
                .Returns(mockApiLog.Object);

            Mock<ILogFactory> mockLogFactory = new Mock<ILogFactory>();

            mockLogFactory.Setup(p => p.GetLogger())
                .Returns(mockApiLog.Object);

            mockingHelper.Setup(p => p.GetLoggerFactory())
                .Returns(mockLogFactory.Object);

            //Act
            await WorkflowsManager.Instance.SendAutomatedWorkflowsTriggerAsync(uploadWithErrors).ConfigureAwait(false);

            //Assert
            automatedWorkflowsService.Verify(x => x.SendTriggerAsync(It.IsAny<int>(), _TRIGGER_ID,
                It.Is<SendTriggerBody>(tr => tr.State == expectedState)));
        }


        private Mock<IObjectManager> GetObjectManagerFake(int automatedWorkflowsInstances)
        {
            Mock<IObjectManager> fakeObjectManager = new Mock<IObjectManager>();

            fakeObjectManager
                .Setup(x => x.QueryAsync(
                    It.IsAny<int>(),
                    It.Is<QueryRequest>(qr => qr.ObjectType.ArtifactTypeID == (int)ArtifactType.ObjectType && qr.Condition.Contains(_RELATIVITY_APP_NAME)),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync((int workspaceID, QueryRequest request, int start, int length) =>
                    new QueryResult()
                    {
                        ResultCount = 1,
                        Objects = new List<RelativityObject>
                        {
                            new RelativityObject { ArtifactID = _RELATIVITY_OBJECT_ARTIFACT_ID }
                        }
                    });

            fakeObjectManager
                .Setup(x => x.QueryAsync(
                    It.IsAny<int>(),
                    It.Is<QueryRequest>(qr => qr.ObjectType.ArtifactTypeID == _RELATIVITY_APP_ARTIFACT_TYPE_ID && qr.Condition.Contains(_AUTOMATED_WORKFLOWS_APP_NAME)),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync((int workspaceID, QueryRequest request, int start, int length) =>
                    new QueryResult()
                    {
                        TotalCount = automatedWorkflowsInstances
                    });

            return fakeObjectManager;
        }

        private Mock<IObjectTypeManager> GetObjectTypeManagerFake()
        {
            Mock<IObjectTypeManager> fakeObjectTypeManager = new Mock<IObjectTypeManager>();

            fakeObjectTypeManager.Setup(m => m.ReadAsync(
                It.IsAny<int>(),
                It.IsAny<int>()
            )).Returns(Task.FromResult(new ObjectTypeResponse { ArtifactTypeID = _RELATIVITY_APP_ARTIFACT_TYPE_ID }));

            return fakeObjectTypeManager;
        }

    }
}
