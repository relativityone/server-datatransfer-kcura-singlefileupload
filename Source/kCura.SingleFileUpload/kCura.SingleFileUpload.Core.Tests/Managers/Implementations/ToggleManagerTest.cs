using kCura.SingleFileUpload.Core.Managers.Implementation;
using Moq;
using NUnit.Framework;
using Relativity.Toggles;
using System.Threading.Tasks;
using FluentAssertions;
using Relativity.Testing.Identification;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
	[TestLevel.L0]
	[TestExecutionCategory.CI]
	public class ToggleManagerTest : TestBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			Mock<IToggleProvider> toggleProvider = new Mock<IToggleProvider>();
			toggleProvider.DefaultValue = DefaultValue.Mock;
			toggleProvider.SetReturnsDefault(Task.CompletedTask);
			toggleProvider.SetReturnsDefault(Task.FromResult(true));
			toggleProvider.SetReturnsDefault(true);
			ToggleProvider.Current = toggleProvider.Object;
		}

        [Test]
		public async Task GetCheckSFUFieldsAsync_ShouldReturnToggle()
		{
			// Act
			bool result = await ToggleManager.Instance.GetCheckSFUFieldsAsync();

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public async Task GetCheckUploadMassiveAsync_ShouldReturnToggle()
		{
			// Act
			bool result = await ToggleManager.Instance.GetCheckUploadMassiveAsync();

			// Assert
			result.Should().BeTrue();
		}
	}
}
