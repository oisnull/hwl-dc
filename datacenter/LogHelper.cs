using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    public class LogHelper
    {
        private LogHelper()
        {

        }

        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void SetConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.Configure(configFile);
        }

        public static void WriteInfo(string content)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(content);
            }
        }

        public static void WriteError(string content)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(content);
            }
        }

        public static void WriteError(string content, Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(content, ex);
            }
        }
    }
}
