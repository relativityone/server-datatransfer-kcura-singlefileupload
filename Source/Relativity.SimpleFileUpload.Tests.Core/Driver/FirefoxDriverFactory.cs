﻿using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Relativity.SimpleFileUpload.Tests.Core;

namespace Relativity.SimpleFileUpload.Tests.Core.Driver
{
	public static class FirefoxDriverFactory
	{
		public static RemoteWebDriver Create()
		{
			FirefoxDriverService driverService = FirefoxDriverService.CreateDefaultService();
			// Otherwise console window appears for ChromeDriver process
			driverService.HideCommandPromptWindow = SharedVariables.UiDriverServiceHideCommandPromptWindow;

			var options = new FirefoxOptions
			{
				AcceptInsecureCertificates = SharedVariables.UiOptionsAcceptInsecureCertificates
			};

			options.SetLoggingPreference(LogType.Browser, LogLevel.Warning);

			if (SharedVariables.UiOptionsArgumentsHeadless)
			{
				options.AddArgument("-headless");
			}

			return new FirefoxDriver(driverService, options);
		}

	}

}
