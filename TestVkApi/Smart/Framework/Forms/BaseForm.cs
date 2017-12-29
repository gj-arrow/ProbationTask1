using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using demo.framework.Elements;

namespace demo.framework.forms
{
    public class BaseForm : BaseEntity
    {
        private readonly String _name;
        private readonly By _locator;
        protected BaseForm(By locator, String name)
        {   _locator = locator;
            _name = name;
            AssertIsPresent();
        }

        private void AssertIsPresent()
        {
            BaseElement.WaitForElementPresent(_locator, "Form " + _name);
            Log.Info(String.Format("Form '{0}' has appeared", _name));
        }

        public void CheckTextOnForm(String text)
        {
            BaseElement.WaitForElementPresent(By.XPath("//*[contains(.,'" + text + "')]"), text);
            Log.Info(String.Format("Text '{0}' is shown on the page", text));
        }
    }
}
