using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace demo.framework
{
    public class BrowserFactory : BaseEntity
    {
        private const String DriverPath = "resources/";

        /// <summary>
        /// setup webdriver. chromedriver is a default value
        /// </summary>
        /// <returns>driver</returns>
        public static IWebDriver SetupBrowser()
        {
            var a = System.IO.Path.GetFullPath(DriverPath);
            String browserName = Configuration.GetBrowser();
            if (browserName == "chrome")
            {
              return new ChromeDriver(System.IO.Path.GetFullPath(DriverPath));
            }
            if (browserName == "iexplore")
            {
            return new InternetExplorerDriver(System.IO.Path.GetFullPath(DriverPath));
            }
            if (browserName == "firefox")
            {
                return new FirefoxDriver();
            }
            return new ChromeDriver(System.IO.Path.GetFullPath(DriverPath));
        }
    }
}
