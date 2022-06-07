using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.InstanceSetting;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
    [TestFixture]
    [TestLevel.L0]
    [TestExecutionCategory.CI]
    class InstanceSettingManagerTest : TestBase
    {
        private const int _MAX_FILES = 5;

        [SetUp]
        public void Setup()
        {
            Mock<IHelper> mockingHelper = new Mock<IHelper>();
            Mock<IInstanceSettingManager> mockInstanceSettingManager = new Mock<IInstanceSettingManager>();

            mockInstanceSettingManager
                .Setup(p => p.QueryAsync(It.IsAny<Query>()))
                .Returns(Task.FromResult(new InstanceSettingQueryResultSet
                {
                    Success = true,
                    Results = new List<Result<InstanceSetting>>
                    {
                        new Result<InstanceSetting>
                        {
                            Success = true,
                            Artifact = new InstanceSetting
                            {
                                Value = _MAX_FILES.ToString()
                            }
                        }
                    }
                }));

            mockingHelper
                .MockIServiceMgr()
                .MockService(mockInstanceSettingManager);

            ConfigureSingletoneRepositoryScope(mockingHelper.Object);
        }

        [Test]
        public async Task GetMaxFilesInstanceSetting_ShouldReturnInstanceSettingValue()
        {
            // Arrange

            // Act
            int result = await InstanceSettingManager.Instance.GetMaxFilesInstanceSettingAsync();

            // Assert
            result.Should().Be(_MAX_FILES);
        }

        [Test]
        public void CreateMaxFilesInstanceSetting_ShouldNotThrow()
        {
            // Act
            Action action = () => InstanceSettingManager.Instance.CreateMaxFilesInstanceSettingAsync();

            // Assert
            action.Should().NotThrow();
        }
    }
}
