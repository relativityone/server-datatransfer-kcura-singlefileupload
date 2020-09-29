using System;
using Atata;
using NUnit.Framework;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
using Relativity.Testing.Framework.Web;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Relativity.SimpleFileUpload.Tests.Core.Templates
{
	[TestFixture]
	public class HttpFunctionalTestsTemplate
	{
		private readonly IWorkspaceService _workspaceService;

		private readonly string _workspaceName;

		private CookieContainer _userCookies;
		private static readonly Object _synchronizationRoot = new Object();

		public int WorkspaceId { get; private set; }

		public HttpFunctionalTestsTemplate()
		{
		}

		public HttpFunctionalTestsTemplate(string workspaceName)
		{
			_workspaceName = workspaceName;

			_workspaceService = RelativityFacade.Instance.Resolve<IWorkspaceService>();
		}

		[OneTimeSetUp]
		public virtual void OneTimeSetUp()
		{
			Workspace workspace = new Workspace()
			{
				Name = _workspaceName,
				TemplateWorkspace = new NamedArtifact {Name = Const.FUNCTIONAL_TEMPLATE_NAME}
			};

			WorkspaceId = _workspaceService.Create(workspace).ArtifactID;

			AuthenticateUser();
		}

		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
		{
			_workspaceService.Delete(WorkspaceId);
		}

		public HttpClient GetUserHttpClient()
		{
			var handler = new HttpClientHandler() {CookieContainer = _userCookies};

			var client = new HttpClient(handler)
			{
				BaseAddress = SharedVariables.SimpleFileUploadCustomPageUri
			};

			string XCSFRHeader = _userCookies.GetCookies(SharedVariables.RelativityFrontedUri)["CSRFHolder"].Value;
			client.DefaultRequestHeaders.Add("X-CSRF-Header", XCSFRHeader);

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