using kCura.SingleFileUpload.Core.Managers.Implementation;
using Moq;
using NUnit.Framework;
using Relativity.Toggles;
using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace kCura.SingleFileUpload.Core.Tests.Managers.Implementations
{
	[TestFixture]
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
		public async Task GetChangeFileNameTestAsync()
		{
			// Act
			bool result = await ToggleManager.Instance.GetChangeFileNameAsync();

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void SetChangeFileNameTestAsync()
		{
			// Act
			Action action = () => ToggleManager.Instance.SetChangeFileNameAsync(true);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public async Task GetCheckSFUFieldsTestAsync()
		{
			// Act
			bool result = await ToggleManager.Instance.GetCheckSFUFieldsAsync();

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void SetCheckSFUFieldsTestAsync()
		{
			// Act
			Action action = () => ToggleManager.Instance.SetCheckSFUFieldsAsync(true);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public async Task GetValidateSFUCustomPermissionsTestAsync()
		{
			// Act
			bool result = await ToggleManager.Instance.GetValidateSFUCustomPermissionsAsync();

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void SetValidateSFUCustomPermissionsTestAsync()
		{
			// Act
			Action action = () => ToggleManager.Instance.SetValidateSFUCustomPermissionsAsync(true);

			// Assert
			action.Should().NotThrow();
		}

		[Test]
		public async Task GetCheckUploadMassiveTestAsync()
		{
			// Act
			bool result = await ToggleManager.Instance.GetCheckUploadMassiveAsync();

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void SetCheckUploadMassiveTestAsync()
		{
			// Act
			Action action = () => ToggleManager.Instance.SetCheckUploadMassiveAsync(true);

			// Assert
			action.Should().NotThrow();
		}



	}
}
