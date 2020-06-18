using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Drawing;

namespace Relativity.SimpleFileUpload.Tests.Core.Driver
{
	public static class DriverFactory
	{
		private const string _VERSION_CAPABILITY_NAME = "version";

		private const string _BROWSER_VERSION_CAPABILITY_NAME = "browserVersion";

		public static RemoteWebDriver Create()
		{
			RemoteWebDriver driver;
			switch (SharedVariables.UiBrowser)
			{
				case "chrome":
					driver = ChromeDriverFactory.Create();
					break;
				case "chromium":
				case "chromium-portable":
					driver = ChromiumDriverFactory.Create();
					break;
				case "firefox":
					driver = FirefoxDriverFactory.Create();
					break;
				default:
					throw new ArgumentException($"Unsupported browser '{SharedVariables.UiBrowser}'");
			}

			driver.Manage().Timeouts().ImplicitWait = DriverImplicitWait;
			driver.Manage().Window.Size = new Size(SharedVariables.UiBrowserWidth, SharedVariables.UiBrowserHeight);
			driver.Url = SharedVariables.RelativityFrontendUrlValue;

			return driver;
		}

		public static string GetBrowserVersion(IHasCapabilities driver)
		{
			ICapabilities caps = driver.Capabilities;
			if (caps.HasCapability(_VERSION_CAPABILITY_NAME))
			{
				return caps[_VERSION_CAPABILITY_NAME].ToString();
			}

			if (caps.HasCapability(_BROWSER_VERSION_CAPABILITY_NAME))
			{
				return caps[_BROWSER_VERSION_CAPABILITY_NAME].ToString();
			}

			return "Unknown";
		}

		private static TimeSpan DriverImplicitWait => TimeSpan.FromSeconds(SharedVariables.UiImplicitWaitInSec);
	}
}
