using System;

namespace NSerio.Relativity.Infrastructure
{
	public class StandaloneUrlHelper
	{
		private const string C_DEFAULT_CP_COMPONENT = "Relativity/CustomPages";

		private string _baseUrl;

		public StandaloneUrlHelper(string baseUrl)
		{
			this._baseUrl = baseUrl;
		}

		public Uri GetApplicationURL(Guid appGuid)
		{
			return new Uri(string.Format("{0}/{1}/{2}", this._baseUrl, "Relativity/CustomPages", appGuid));
		}

		public string GetRelativePathToCustomPages(Guid appGuid)
		{
			return string.Format("/{0}/{1}", "Relativity/CustomPages", appGuid);
		}
	}
}