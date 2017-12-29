using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using demo.framework.Elements;

namespace demo.framework.Elements
{
    public class Link : BaseElement
    {
        public Link(By locator, String name) : base(locator, name) { }
    }
}
