using Atata;
using System;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Web;
using Relativity.SimpleFileUpload.Tests.Core;
using Relativity.SimpleFileUpload.Tests.Core.Templates;

namespace kcura.SingleFileUpload.FunctionalTests
{
	public class FunctionalTestsTemplate : SimpleFileUploadTestsTemplate
	{
		private CookieContainer _userCookies;
		private static readonly Object _synchronizationRoot = new Object();

		public FunctionalTestsTemplate(string workspaceName)
			: base(Const.FUNCTIONAL_WORKSPACE_PREFIX + workspaceName, Const.FUNCTIONAL_TEMPLATE_NAME)
		{ }

		[OneTimeSetUp]
		public override void OneTimeSetUp()
		{
			base.OneTimeSetUp();

			AuthenticateUser();
		}

		public HttpClient GetUserHttpClient()
		{
			var handler = new HttpClientHandler { CookieContainer = _userCookies };

			var client = new HttpClient(handler)
			{
				BaseAddress = SharedVariables.SimpleFileUploadCustomPageUri
			};

			if (_userCookies.GetCookies(SharedVariables.RelativityFrontedUri)["CSRFHolder"] != null)
			{
				string XCSFRHeader = _userCookies.GetCookies(SharedVariables.RelativityFrontedUri)["CSRFHolder"].Value;
				client.DefaultRequestHeaders.Add("X-CSRF-Header", XCSFRHeader);
			}

			return client;
		}

		public async Task<HttpResponseMessage> UploadFileAsync(FileInfo file, bool fdv, bool img)
		{
			var query = HttpUtility.ParseQueryString(string.Empty);
			query["AppID"] = WorkspaceId.ToString();
			query["fdv"] = fdv.ToString();
			query["img"] = img.ToString();

			using (var content = new MultipartFormDataContent())
			using (var fileStream = File.OpenRead(file.FullName))
			using (var client = GetUserHttpClient())
			{
				StreamContent stream = new StreamContent(fileStream);
				content.Add(stream, "file", file.Name);

				return await client.PostAsync($"sfu/Upload?{query}", content).ConfigureAwait(false);
			}
		}

		private void AuthenticateUser()
		{
			lock (_synchronizationRoot)
			{
				if (AtataContext.Current is null)
				{
					Go.To<LoginPage>()
						.EnterCredentials(
							RelativityFacade.Instance.Config.RelativityInstance.AdminUsername,
							RelativityFacade.Instance.Config.RelativityInstance.AdminPassword)
						.Login.Click();
				}

				CookieCollection cookieCollection = new CookieCollection();
				foreach (var cookie in AtataContext.Current.Driver.Manage().Cookies.AllCookies)
				{
					cookieCollection.Add(new Cookie(cookie.Name, cookie.Value));
				}

				_userCookies = new CookieContainer();
				_userCookies.Add(SharedVariables.RelativityFrontedUri, cookieCollection);
			}
		}
	}
}
