//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Remote;
//using Relativity.SimpleFileUpload.Tests.Core.Driver;
//using SeleniumExtras.PageObjects;
//using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

//namespace Relativity.SimpleFileUpload.Tests.Core.Pages
//{
//	public class GeneralPage : Page
//	{
//		internal string _mainFrameNameNewUi = "ListPage";
//		internal string _mainFrameNameOldUi = "externalPage";

//		// TODO Move to some "SthBar", "Navigator" or something similar
//		[FindsBy(How = How.Id, Using = "GetNavigateHomeScript")]
//		protected IWebElement NavigateHome;

//		[FindsBy(How = How.ClassName, Using = "headerUpperRow")]
//		protected IWebElement Header;

//		[FindsBy(How = How.Id, Using = "horizontal-tabstrip")]
//		protected IWebElement MainMenu;

//		[FindsBy(How = How.Id, Using = "qnTextBox")]
//		protected IWebElement QuickNavigationInput;

//		[FindsBy(How = How.CssSelector, Using = "span[title = 'User Dropdown Menu']")]
//		protected IWebElement UserDropdownMenu;

//		public GeneralPage(RemoteWebDriver driver) : base(driver)
//		{
//			PageFactory.InitElements(driver, this);
//		}

//		public GeneralPage PassWelcomeScreen()
//		{
//			Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(7);
//			try
//			{
//				ReadOnlyCollection<IWebElement> buttons = Driver.FindElements(By.Id("_continue_button"));
//				if (buttons.Any())
//				{
//					buttons[0].ClickEx();
//				}
//			}
//			finally
//			{
//				Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(SharedVariables.UiImplicitWaitInSec);
//			}

//			return this;
//		}

//		public LoginPage LogOut()
//		{
//			UserDropdownMenu.ClickEx();
//			IWebElement logOutLink = Driver.FindElement(By.LinkText("Logout"));
//			logOutLink.ClickEx();
//			return new LoginPage(Driver);
//		}

//		public GeneralPage ChooseWorkspace(string name)
//		{
//			if (Driver.Title.Contains(name))
//			{
//				return this;
//			}
//			Driver.SwitchTo().DefaultContent();
//			GoToPage(name);
//			AcceptLeavingPage();
//			return this;
//		}

//		private void AcceptLeavingPage()
//		{
//			IAlert alert = ExpectedConditions.AlertIsPresent().Invoke(Driver);
//			alert?.Accept();
//		}

//		private void GoToPage(string pageName)
//		{
//			WaitForPage();
//			QuickNavigationInput.SendKeys(pageName);
//			IWebElement resultLinkLinkName = Driver.FindElementByLinkText(pageName);
//			resultLinkLinkName.ClickEx();
//		}
//	}
//}