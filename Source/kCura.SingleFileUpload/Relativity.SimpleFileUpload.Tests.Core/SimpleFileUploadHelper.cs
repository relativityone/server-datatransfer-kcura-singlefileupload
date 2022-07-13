using System.Net.Http;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Atata;
using Newtonsoft.Json;

namespace Relativity.SimpleFileUpload.Tests.Core
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

		public static async Task<HttpResponseMessage> UploadNativeFromReviewInterfaceAsync(HttpClient client, int workspaceId, int documentId, FileInfo file)
		{
			var query = HttpUtility.ParseQueryString(string.Empty);

			var meta = new
			{
				fid = -1,
				did = documentId,
				fdv = false,
				fri = true,
				force = false,
			};
			query["meta"] = JsonConvert.SerializeObject(meta);
			query["meta"] = meta.ToString();
			query["AppID"] = workspaceId.ToString();
			query["fdv"] = false.ToString();
			query["img"] = false.ToString();

			using (var content = new MultipartFormDataContent())
			using (var fileStream = File.OpenRead(file.FullName))
			{
				StreamContent stream = new StreamContent(fileStream);
				content.Add(stream, "file", file.Name);

				return await client.PostAsync($"sfu/Upload?{query}", content).ConfigureAwait(false);
			}
		}

		public static async Task<HttpResponseMessage> UploadImageFromReviewInterfaceAsync(HttpClient client, int workspaceId, int documentId, int profileId, FileInfo file, bool newImage)
		{
			var query = HttpUtility.ParseQueryString(string.Empty);
			query["AppID"] = workspaceId.ToString();
			query["docID"] = documentId.ToString();
			query["image"] = true.ToString();
			query["fri"] = true.ToString();
			query["newImage"] = newImage.ToString();
			query["profileID"] = profileId.ToString();

			using (var content = new MultipartFormDataContent())
			using (var fileStream = File.OpenRead(file.FullName))
			{
				StreamContent stream = new StreamContent(fileStream);
				content.Add(stream, "file", file.Name);

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
