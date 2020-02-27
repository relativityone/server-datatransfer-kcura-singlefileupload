using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers;
using Relativity.Test.Helpers.Authentication;
using System;

namespace kcura.SingleFileUpload.FunctionalTests.Helper
{
	class TestCustomPageHelper : TestHelper, ICPHelper
	{
		int caseID;
		IAuthenticationMgr authenticationMgr;
		IUserInfo userInfo;
		public TestCustomPageHelper(int caseID) : base(TestContext.Parameters["AdminUsername"], TestContext.Parameters["AdminPassword"])
		{
			this.caseID = caseID;
			this.userInfo = new UserInfo()
			{
				ArtifactID = 777,

			};
			this.authenticationMgr = new AuthenticationManager(userInfo);
		}

		public int GetActiveCaseID()
		{
			return caseID;
		}

		public IAuthenticationMgr GetAuthenticationManager()
		{
			return authenticationMgr;
		}

		public ICSRFManager GetCSRFManager()
		{
			throw new NotImplementedException();
		}

		public new IInstanceSettingsBundle GetInstanceSettingBundle()
		{
			var mockInstanceSettingbundle = new Mock<IInstanceSettingsBundle>();
			return mockInstanceSettingbundle.Object;
		}
	}
}
