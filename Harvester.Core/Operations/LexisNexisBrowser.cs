using System;
using System.Collections.Generic;
using NHtmlUnit;
using NHtmlUnit.Html;
using NHtmlUnit.W3C.Dom;

namespace ZondervanLibrary.Harvester.Core.Operations
{
    public class LexisNexisBrowser
    {
        readonly string _username = "sdenny";
        readonly string _password = "24TU6IN";

        public void Execute()
        {
            WebClient browser = new WebClient(BrowserVersion.CHROME);
            browser.Options.JavaScriptEnabled = true;
            browser.Options.RedirectEnabled = true;
            browser.Options.ThrowExceptionOnFailingStatusCode = false;
            browser.Options.ThrowExceptionOnScriptError = false;
            browser.Options.CssEnabled = false;
            //Login Page
            HtmlForm form = browser.GetHtmlPage("http://usage.lexisnexis.com:80//CounterLogin.jsp").GetFormByName("login");
            browser.WaitForBackgroundJavaScript(1000);
            form.GetInputByName("username").SetValueAttribute(_username);
            form.GetInputByName("password").SetValueAttribute(_password);
            HtmlPage redirect = (HtmlPage)form.GetInputByName("loginBtn").Click();

            browser.WaitForBackgroundJavaScript(1000);

            redirect = (HtmlPage)redirect.GetAnchorByText("COUNTER Reports").Click();

            HtmlSelect YearSelect = ((HtmlSelect)redirect.GetElementByName("year"));
            HtmlOption option = YearSelect.GetOptionByValue("2015");
            YearSelect.SetSelectedAttribute(option, true);

            redirect = (HtmlPage)((HtmlRadioButtonInput)redirect.GetElementById("CSV")).SetChecked(true);

            IList<INode> input = (redirect.GetElementsByTagName("input"));
            ((HtmlInput)input[3]).Click();
            //redirect = (HtmlPage)input.Click();

            Console.WriteLine();
            //var browser = new ChromeDriver();
            ////browser.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.99 Safari/537.36";
            //browser.Navigate().GoToUrl("http://usage.lexisnexis.com:80//CounterLogin.jsp");
            //var form = browser.FindElement(By.TagName("form"));
            //Console.WriteLine();
            //var usernameTextBox = browser.Find("username");
            //var passwordTextBox = browser.Find("password");
            //var submitButton = browser.Find(ElementType.Button, FindBy.Value, "Submit");
            //usernameTextBox.Value = _username;
            //passwordTextBox.Value = _password;
            
            //submitButton.Click();

            //var CounterReportsLink = browser.Find(ElementType.Anchor, FindBy.Text, "COUNTER Reports");
            //CounterReportsLink.Click();
            //Console.WriteLine(browser.CurrentHtml);

        }
    }
}
