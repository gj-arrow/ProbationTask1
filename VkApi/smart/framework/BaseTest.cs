using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenQA.Selenium;
using demo.framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace demo.framework
{
    public class BaseTest : BaseEntity
    {
       [TestInitialize]
        public void SetUp()
       {
            Browser.GetInstance();
            Browser.GetDriver().Navigate().GoToUrl(Configuration.GetBaseUrl());
        }

        [TestCleanup]
        public void TearDown()
        {
            var processes = Process.GetProcessesByName(Configuration.GetBrowser());
            foreach (var process in processes)
            {
                process.Kill();
            }
            Browser.GetDriver().Quit();
        }
    }
}
