using OpenQA.Selenium.Remote;

namespace Relativity.SimpleFileUpload.Tests.Core.Driver
{
	public static class ChromeDriverFactory
	{
		public static RemoteWebDriver Create()
		{
			return ChromiumBasedDriverFactory.Create();
		}
	}
}
