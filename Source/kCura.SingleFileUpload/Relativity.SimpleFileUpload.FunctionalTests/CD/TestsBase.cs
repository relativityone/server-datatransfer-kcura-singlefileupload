using Atata;
using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.RingSetup;
using Relativity.Testing.Framework.Web;

namespace Relativity.SimpleFileUpload.FunctionalTests.CD
{
	public abstract class TestsBase : TestSetup
	{
		protected TestsBase(string name)
			: base(name, desiredNumberOfDocuments: 0, importImages: false)
		{
		}

		[OneTimeSetUp]
		public virtual void OneTimeSetUp()
		{
			RelativityFacade.Instance.RelyOn<WebComponent>();
		}

		[OneTimeTearDown]
		public virtual void TearDown()
		{
			AtataContext.Current?.Dispose();

			CleanUpTestUser();
		}

		private void CleanUpTestUser()
		{
			IUserService userService = RelativityFacade.Instance.Resolve<IUserService>();

			int userId = userService.GetByEmail(_user.Email).ArtifactID;

			userService.Delete(userId);
		}
	}
}
