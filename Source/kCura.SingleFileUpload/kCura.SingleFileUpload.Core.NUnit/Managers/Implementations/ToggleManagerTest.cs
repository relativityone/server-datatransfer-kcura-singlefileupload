using kCura.SingleFileUpload.Core.Managers.Implementation;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.Toggles;
using System;
using System.Threading.Tasks;

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
			toggleProvider.SetReturnsDefault(MockHelper.FakeTask());
			toggleProvider.SetReturnsDefault(Task.FromResult(true));
			toggleProvider.SetReturnsDefault(true);
			ToggleProvider.Current = toggleProvider.Object;
		}

		[Test]
		public async Task GetChangeFileNameTestAsync()
		{
			bool result = await ToggleManager.Instance.GetChangeFileNameAsync();
			Assert.IsTrue(result);
		}

		[Test]
		public async Task SetChangeFileNameTestAsync()
		{
			try
			{
				await ToggleManager.Instance.SetChangeFileNameAsync(true);
				Assert.IsTrue(true);
			}
			catch (Exception)
			{
				Assert.IsTrue(false);
				throw;
			}

		}

		[Test]
		public async Task GetCheckSFUFieldsTestAsync()
		{
			bool result = await ToggleManager.Instance.GetCheckSFUFieldsAsync();
			Assert.IsTrue(result);
		}

		[Test]
		public async Task SetCheckSFUFieldsTestAsync()
		{
			try
			{
				await ToggleManager.Instance.SetCheckSFUFieldsAsync(true);
				Assert.IsTrue(true);
			}
			catch (Exception)
			{
				Assert.IsTrue(false);
			}
		}

		[Test]
		public async Task GetValidateSFUCustomPermissionsTestAsync()
		{
			bool result = await ToggleManager.Instance.GetValidateSFUCustomPermissionsAsync();
			Assert.IsTrue(result);
		}

		[Test]
		public async Task SetValidateSFUCustomPermissionsTestAsync()
		{
			try
			{
				await ToggleManager.Instance.SetValidateSFUCustomPermissionsAsync(true);
				Assert.IsTrue(true);
			}
			catch (Exception)
			{
				Assert.IsTrue(false);
			}
		}

		[Test]
		public async Task GetCheckUploadMassiveTestAsync()
		{
			bool result = await ToggleManager.Instance.GetCheckUploadMassiveAsync();
			Assert.IsTrue(result);
		}

		[Test]
		public async Task SetCheckUploadMassiveTestAsync()
		{
			try
			{
				await ToggleManager.Instance.SetCheckUploadMassiveAsync(true);
				Assert.IsTrue(true);
			}
			catch (Exception)
			{
				Assert.IsTrue(false);
			}
		}



	}
}
