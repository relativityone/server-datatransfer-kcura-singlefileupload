using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.SimpleFileUpload.Tests.Core
{
	public static class SharedVariables
	{
		#region User Settings

		public static string AdminUsername => GetParamString("AdminUsername");

		public static string AdminPassword => GetParamString("AdminPassword");

		#endregion User Settings

		#region UI Tests Settings

		public static string ChromeBinaryLocation => GetParamString("ChromeBinaryLocation");

		#endregion

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

		/// <summary>
		/// Returns RSAPI URL
		/// </summary>
		public static Uri RsapiUri => new Uri($"{ServerBindingType}://{RsapiServerAddress}/Relativity.Services/");

		/// <summary>
		/// Returns Relativity REST URL
		/// </summary>
		public static Uri RelativityRestUri => new Uri($"{RelativityBaseAdressUrlValue}/Relativity.Rest/api");

		/// <summary>
		/// Returns Relativity WebAPI URL
		/// </summary>
		public static string RelativityWebApiUrl => $"{RelativityBaseAdressUrlValue}/RelativityWebAPI/";

		private static string ServerBindingType => GetParamString("ServerBindingType");

		private static string RsapiServerAddress => GetParamString("RsapiServicesHostAddress");

		#endregion Relativity Settings

		#region Simple File Upload Settings

		public static Uri SimpleFileUploadCustomPageUri = new Uri($"{RelativityFrontendUrlValue}/custompages/{Const.CustomPage.Guid}/");

		#endregion

		#region RAP Settings

		public static string LocalRAPFileLocation => Path.Combine(GetParamString("RAPDirectory"), "kCura.SimpleFileUpload.rap");

		#endregion

		private static string GetParamString(string name) => GetRunSettingsParameter(name);
		private static bool GetParamBool(string name) => bool.Parse(GetParamString(name));
		private static int GetParamInt(string name) => int.Parse(GetParamString(name));
		private static double GetParamDouble(string name) => double.Parse(GetParamString(name));

		private static string GetRunSettingsParameter(string name)
			=> TestContext.Parameters.Exists(name)
				? TestContext.Parameters[name]
				: null;
	}
}
