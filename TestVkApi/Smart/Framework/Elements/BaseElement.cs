using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace demo.framework.Elements
{
    public abstract class BaseElement : BaseEntity
    {
        private readonly RemoteWebElement _element;
        private readonly String _name;
        private readonly By _locator;

        protected BaseElement(By locator, String name)
        {
            this._name = name;
            this._locator = locator;
        }

        protected RemoteWebElement GetElement()
        {   
            WaitForElementPresent();
            return (RemoteWebElement)Browser.GetDriver().FindElement(_locator); 
        }

        protected String GetName()
        {
            return _name;
        }

        protected By GetLocator()
        {
            return _locator;
        }


        public void MoveAndClick()
        {
            ScrollToElement();
            var actions = new Actions(Browser.GetDriver());
            actions.MoveToElement(GetElement()).Click().Build().Perform();
        }

        public void ScrollToElement()
        {
            IJavaScriptExecutor js = Browser.GetDriver() as IJavaScriptExecutor;
            js.ExecuteScript("arguments[0].scrollIntoView(true);", GetElement());
        }

        public void Click()
        {
            WaitForElementPresent();
            GetElement().Click();
            Log.Info(String.Format("{0} :: click", GetName()));
        }

        public void ClickAndWait()
        {
            WaitForElementPresent();
            GetElement().Click();
            Browser.WaitForPageToLoad();
            Log.Info(String.Format("{0} :: click", GetName()));
        }

        public Boolean IsPresent()
        {
            bool isPresent = Browser.GetDriver().FindElements(_locator).Count > 0;
            Log.Info(GetName() + " : is present : " + isPresent);
            return isPresent;
        }

        public Boolean IsDisplayed()
        {
            bool isDisplyed = Browser.GetDriver().FindElements(_locator)[0].Displayed;
            Log.Info(GetName() + " : is displayed : " + isDisplyed);
            return isDisplyed;
        }

        protected void WaitForElementPresent()
        {
            var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromMilliseconds(Convert.ToDouble(Configuration.GetTimeout())));
            try
            {
                wait.Until(waiting =>
                {
                    var webElements = Browser.GetDriver().FindElements(_locator);
                    return webElements.Count != 0;
                });
            }
            catch (TimeoutException)
            {
                Log.Fatal(string.Format("Element with locator: '{0}' does not exists!", _locator));
            }
        }

        public static void WaitForElementPresent(By locator, String name)
        {
            var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromMilliseconds(Convert.ToDouble(Configuration.GetTimeout())));
            try
            {
                wait.Until(waiting =>
                {
                    try
                    {
                        var webElements = Browser.GetDriver().FindElements(locator);
                        return webElements.Count != 0;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
            }
            catch (TimeoutException)
            {
                Log.Fatal(string.Format("Element with locator: '{0}' does not exists!", locator));
            }
        }
    }
}
