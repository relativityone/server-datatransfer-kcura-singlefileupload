using NUnit.Framework;
using System;
using System.Collections.Generic;
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

		#region UI Tests Settings

		public static int UiTestRepeatOnErrorCount => GetParamInt("ui.testRepeatOnErrorCount");

		public static int UiImplicitWaitInSec => GetParamInt("ui.implicitWaitInSec");

		public static int UiWaitForAjaxCallsInSec => GetParamInt("ui.waitForAjaxCallsInSec");

		public static int UiWaitForPageInSec => GetParamInt("ui.waitForPageInSec");

		public static string UiUseThisExistingWorkspace => GetParamString("UI.UseThisExistingWorkspace");

		public static bool UiSkipDocumentImport => GetParamBool("UI.SkipDocumentImport");

		public static string UiTemplateWorkspace => GetParamString("UI.TemplateWorkspace");

		public static bool UiUseTapiForFileCopy => GetParamBool("UI.UseTapiForFileCopy");

		public static double UiTimeoutMultiplier => GetParamDouble("UI.TimeoutMultiplier");

		public static bool UiSkipUserCreation => GetParamBool("UI.SkipUserCreation");

		public static string UiBrowser => GetParamString("UI.Browser");

		public static string UiBrowserPath => GetParamString("UI.BrowserPath");

		public static int UiBrowserWidth => GetParamInt("UI.BrowserWidth");

		public static int UiBrowserHeight => GetParamInt("UI.BrowserHeight");

		public static bool UiDriverServiceHideCommandPromptWindow => GetParamBool("UI.DriverService.HideCommandPromptWindow");

		public static string UiDriverServiceLogPath => GetParamString("UI.DriverService.LogPath");

		public static bool UiOptionsAcceptInsecureCertificates => GetParamBool("UI.Options.AcceptInsecureCertificates");

		public static bool UiOptionsArgumentsDisableInfobars => GetParamBool("UI.Options.Arguments.DisableInfoBars");

		public static bool UiOptionsArgumentsHeadless => GetParamBool("UI.Options.Arguments.Headless");

		public static bool UiOptionsArgumentsIgnoreCertificateErrors => GetParamBool("UI.Options.Arguments.IgnoreCertificateErrors");

		public static bool UiOptionsArgumentsNoSandbox => GetParamBool("UI.Options.Arguments.NoSandbox");

		public static bool UiOptionsAdditionalCapabilitiesAcceptSslCertificates =>
			GetParamBool("UI.Options.AdditionalCapabilities.AcceptSslCertificates");

		public static bool UiOptionsAdditionalCapabilitiesAcceptInsecureCertificates =>
			GetParamBool("UI.Options.AdditionalCapabilities.AcceptInsecureCertificates");

		public static bool UiOptionsProfilePreferenceCredentialsEnableService =>
			GetParamBool("UI.Options.ProfilePreference.CredentialsEnableService");

		public static bool UiOptionsProfilePreferenceProfilePasswordManagerEnabled =>
			GetParamBool("UI.Options.ProfilePreference.ProfilePasswordManagerEnabled");

		#endregion

		#region Simple File Upload Settings

		public static Uri SimpleFileUploadCustomPageUri = new Uri($"{RelativityFrontendUrlValue}/custompages/{Const.CustomPage.Guid}/");

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
