using FluentAssertions;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity;
using Relativity.API;
using Relativity.AutomatedWorkflows.Services.Interfaces;
using Relativity.Services.Error;
using Relativity.Services.Interfaces.ObjectType;
using Relativity.Services.Interfaces.ObjectType.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Testing.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private const int _RELATIVITY_APP_ARTIFACT_TYPE_ID = 123456;
        private const int _RELATIVITY_OBJECT_ARTIFACT_ID = 654321;      

        [TestCase(1, true)]
        [TestCase(0, false)]
        public async Task AutomatedWorkflowInstalledAsync_ShouldReturnCorrectValue(int automatedWorkflowsInstalledInstances, bool expectedResult)
        {
            //Arrange
            Mock<IHelper> mockingHelper = new Mock<IHelper>();
            Mock<IObjectManager> objectManagerFake = PrepareObjectManagerFake(automatedWorkflowsInstalledInstances);
            Mock<IObjectTypeManager> objectTypeManagerFake = PrepareObjectTypeManagerFake();
            mockingHelper
                .MockIServiceMgr()
                .MockService(objectManagerFake)
                .MockService(objectTypeManagerFake);               

            ConfigureSingletoneRepositoryScope(mockingHelper.Object);

            //Act
            bool result = await WorkflowsManager.Instance.AutomatedWorkflowInstalledAsync().ConfigureAwait(false);

            //Assert
            result.Should().Be(expectedResult);
        }

        [Test]
        public async Task SendAutomatedWorkflowsTriggerAsync_ShouldRunOnce()
        {
            //Arrange
            Mock<IHelper> mockingHelper = new Mock<IHelper>();
            Mock<IAutomatedWorkflowsService> automatedWorkflowsService = new Mock<IAutomatedWorkflowsService>();
            mockingHelper
                .MockIServiceMgr()
                .MockService(automatedWorkflowsService);                

            ConfigureSingletoneRepositoryScope(mockingHelper.Object);

            //Act
            await WorkflowsManager.Instance.SendAutomatedWorkflowsTriggerAsync(true);

            //Assert

        }


        private Mock<IObjectManager> PrepareObjectManagerFake(int automatedWorkflowsInstances)
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

        private Mock<IObjectTypeManager> PrepareObjectTypeManagerFake()
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
