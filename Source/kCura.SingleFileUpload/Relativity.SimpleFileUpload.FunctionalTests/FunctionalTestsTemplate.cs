using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Atata;

namespace Relativity.SimpleFileUpload.FunctionalTests
{
	public abstract class FunctionalTestsTemplate : SimpleFileUploadTestsTemplate
	{
		protected FunctionalTestsTemplate(string workspaceName)
			: base(Const.FUNCTIONAL_WORKSPACE_PREFIX + workspaceName, Const.FUNCTIONAL_TEMPLATE_NAME)
		{ }

		public HttpClient GetUserHttpClient()
		{
			CookieCollection cookieCollection = new CookieCollection();
			foreach (var cookie in AtataContext.Current.Driver.Manage().Cookies.AllCookies)
			{
				cookieCollection.Add(new Cookie(cookie.Name, cookie.Value));
			}

			CookieContainer userCookies = new CookieContainer();
			userCookies.Add(SharedVariables.RelativityFrontedUri, cookieCollection);

			var handler = new HttpClientHandler { CookieContainer = userCookies };

			var client = new HttpClient(handler)
			{
				BaseAddress = SharedVariables.SimpleFileUploadCustomPageUri
			};

			if (userCookies.GetCookies(SharedVariables.RelativityFrontedUri)["CSRFHolder"] != null)
			{
				string xcsrfHeader = userCookies.GetCookies(SharedVariables.RelativityFrontedUri)["CSRFHolder"].Value;
				client.DefaultRequestHeaders.Add("X-CSRF-Header", xcsrfHeader);
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
	}
}
