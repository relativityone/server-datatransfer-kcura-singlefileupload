using System;
using System.IO;
using NUnit.Framework;

namespace Relativity.SimpleFileUpload.FunctionalTests
{
	public static class SharedVariables
	{
		#region Relativity Settings
		/// <summary>
		/// Returns RelativityHostAddress value from config file
		/// </summary>
		public static string RelativityHostAddress => GetParamString("RelativityHostAddress");

		/// <summary>
		/// Returns Relativity instance base URL
		/// </summary>
		public static string RelativityBaseAdressUrlValue => $"{ServerBindingType}://{RelativityHostAddress}";

		/// <summary>
		/// Returns Relativity fronted URL value
		/// </summary>
		public static string RelativityFrontendUrlValue => $"{RelativityBaseAdressUrlValue}/Relativity";

		/// <summary>
		/// Returns Relativity fronted URI
		/// </summary>
		public static Uri RelativityFrontedUri => new Uri(RelativityFrontendUrlValue);

		private static string ServerBindingType => GetParamString("ServerBindingType");
		#endregion Relativity Settings

		#region Simple File Upload Settings
		public static Uri SimpleFileUploadCustomPageUri = new Uri($"{RelativityFrontendUrlValue}/custompages/{Const.CustomPage.Guid}/");
		#endregion

		#region RAP Settings
		public static string LocalRAPFileLocation => Path.Combine(GetParamString("RAPDirectory"), "kCura.SimpleFileUpload.rap");
		#endregion

		private static string GetParamString(string name) => GetRunSettingsParameter(name);

		private static string GetRunSettingsParameter(string name)
			=> TestContext.Parameters.Exists(name)
				? TestContext.Parameters[name]
				: null;
	}
}
