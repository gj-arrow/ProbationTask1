using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace demo.framework
{
    public class RunConfigurator
    {   
        private static XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
            
        public static String GetValue(String tag)
        {   
            xmlDoc.Load("../../../Smart/Resources/run.xml"); // Load the XML document from the specified file
            XmlNodeList browser = xmlDoc.GetElementsByTagName(tag);
            return browser[0].InnerText;
        }
    }
}
