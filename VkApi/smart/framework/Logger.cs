using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace demo.framework
{
    public class Logger
    {
        private int count = 1;
        private ILog logger;
        public Logger(ILog logger)
        {
            this.logger = logger;
        }

        public void Step( string action)
        {
            logger.Info("== Step " + count + " :" + action + " ==");
            count++;
        }

        public void Info(String info)
        {
            logger.Info(info);
        }

        public void Fatal(String fatal)
        {
            logger.Fatal(fatal);
        }
    }
}
