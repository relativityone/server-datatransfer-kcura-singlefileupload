using Atata;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Relativity.SimpleFileUpload.FunctionalTests.Common
{
	public static class SimpleFileUploadHelper
	{
		public static async Task<HttpResponseMessage> UploadFileAsync(HttpClient client, int workspaceId, FileInfo file, bool fdv, bool img)
		{
			var query = HttpUtility.ParseQueryString(string.Empty);
			query["AppID"] = workspaceId.ToString();
			query["fdv"] = fdv.ToString();
			query["img"] = img.ToString();

			using (var content = new MultipartFormDataContent())
			using (var fileStream = File.OpenRead(file.FullName))
			{
				StreamContent stream = new StreamContent(fileStream);
				content.Add(stream, "file", file.Name);

				return await client.PostAsync($"sfu/Upload?{query}", content).ConfigureAwait(false);
			}
		}

		public static async Task<HttpResponseMessage> UploadFromReviewInterfaceAsync(HttpClient client, int workspaceId, int documentId, FileInfo file, bool img)
		{
			var query = HttpUtility.ParseQueryString(string.Empty);

			query["AppID"] = workspaceId.ToString();
			query["img"] = img.ToString();

			using (var content = new MultipartFormDataContent())
			using (var fileStream = File.OpenRead(file.FullName))
			{
				StreamContent stream = new StreamContent(fileStream);
				content.Add(stream, "file", file.Name);
				content.Add(new StringContent("-1"), String.Format("\"{0}\"", "fid"));
				content.Add(new StringContent(documentId.ToString()), String.Format("\"{0}\"", "did"));
				content.Add(new StringContent("false"), String.Format("\"{0}\"", "fdv"));
				content.Add(new StringContent("true"), String.Format("\"{0}\"", "fri"));
				content.Add(new StringContent("false"), String.Format("\"{0}\"", "force"));

				return await client.PostAsync($"sfu/Upload?{query}", content).ConfigureAwait(false);
			}
		}

		public static Task<HttpResponseMessage> CheckUploadStatusAsync(HttpClient client, int workspaceId, string controlNumber)
		{
			var query = HttpUtility.ParseQueryString(string.Empty);
			query["AppID"] = workspaceId.ToString();
			query["DocumentName"] = controlNumber;

			return client.PostAsync($"sfu/checkUploadStatus?{query}", new StringContent(string.Empty));
		}

		public static HttpClient GetUserHttpClient()
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
	}
}
