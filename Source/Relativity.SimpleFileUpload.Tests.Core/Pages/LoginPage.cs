using kCura.IntegrationPoints.UITests.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Relativity.SimpleFileUpload.Tests.Core.Driver;
using SeleniumExtras.PageObjects;

namespace Relativity.SimpleFileUpload.Tests.Core.Pages
{

    public class LoginPage : Page
    {

        [FindsBy(How = How.Id, Using = "_email")]
        protected IWebElement Username;

        [FindsBy(How = How.Id, Using = "continue")]
        protected IWebElement ContinueButton;

        [FindsBy(How = How.Id, Using = "_password__password_TextBox")]
        protected IWebElement Password;

        [FindsBy(How = How.Id, Using = "_login")]
        protected IWebElement LoginButton;

        public LoginPage(RemoteWebDriver driver) : base(driver)
        {
            PageFactory.InitElements(driver, this);
        }

        public bool IsOnLoginPage()
        {
            return Username.Displayed || Password.Displayed;
        }

        public LoginPage ValidatePage()
        {
            if (!IsOnLoginPage())
            {
                throw new PageException("Can't find required elements.");
            }
            return this;
        }

        public GeneralPage Login(string username, string password)
        {
            Username.SetText(username);
            ContinueButton.ClickEx();
            Password.SetText(password);
            LoginButton.ClickEx();
            return new GeneralPage(Driver);
        }

    }
}
