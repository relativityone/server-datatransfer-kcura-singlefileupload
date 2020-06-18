using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium.Remote;
using Relativity.SimpleFileUpload.Tests.Core.Driver;
using Relativity.SimpleFileUpload.Tests.Core.Pages;
using Relativity.Testing.Framework;
using Relativity.Testing.Framework.Api;
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

		public int WorkspaceId { get; private set; }

		public HttpFunctionalTestsTemplate()
		{ 
		}

		public HttpFunctionalTestsTemplate(string workspaceName)
		{
			RelativityFacade.Instance.RelyOn<CoreComponent>();
			RelativityFacade.Instance.RelyOn<ApiComponent>();

			_workspaceName = workspaceName;

			_workspaceService = RelativityFacade.Instance.Resolve<IWorkspaceService>();
		}

		[OneTimeSetUp]
		public virtual void OneTimeSetUp()
		{
			Workspace workspace = new Workspace()
			{
				Name = _workspaceName,
				TemplateWorkspace = new NamedArtifact { Name = Const.FUNCTIONAL_TEMPLATE_NAME }
			};

			WorkspaceId = _workspaceService.Create(workspace).ArtifactID;

			AuthenticateUser(SharedVariables.AdminUsername, SharedVariables.AdminPassword);
		}

		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
		{
			_workspaceService.Delete(WorkspaceId);
		}

		public HttpClient GetUserHttpClient()
		{
			var handler = new HttpClientHandler() { CookieContainer = _userCookies};

			var client = new HttpClient(handler);
			client.BaseAddress = SharedVariables.SimpleFileUploadCustomPageUri;

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

		private void AuthenticateUser(string userName, string password)
		{
			RemoteWebDriver driver = DriverFactory.Create();
			var loginPage = new LoginPage(driver);

			if (loginPage.IsOnLoginPage())
			{
				loginPage.Login(userName, password);
			}
			else
			{
				new GeneralPage(driver).PassWelcomeScreen();
			}

			_userCookies = new CookieContainer();

			CookieCollection cookieCollection = new CookieCollection();
			foreach(var cookie in driver.Manage().Cookies.AllCookies)
			{
				cookieCollection.Add(new Cookie(cookie.Name, cookie.Value));
			}

			_userCookies.Add(SharedVariables.RelativityFrontedUri, cookieCollection);

			driver.Quit();
		}
	}
}
