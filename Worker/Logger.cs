using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using System.Xml;
using System;

// ============================================================
//        Настройки для формирования файлов kогов Log4Net.
// ===============================================================
namespace Worker
{
    public static class Logger
    {
        //private static readonly string LOG_CONFIG_FILE = @"log4net.config";
        //private static readonly log4net.ILog _log = LogManager.GetLogger(typeof(Logger));

        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void InitLogger()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(@"Config/log4net.config"));
            var logRepository = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                   typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(logRepository, log4netConfig["log4net"]);
        }

        public static void LInfo(string info, params object[] args)
        {
            log.InfoFormat(info, args);
        }
        public static void LDebug(string info, params object[] args)
        {
            log.DebugFormat(info, args);
        }
        public static void LError(string info, params object[] args)
        {
            log.ErrorFormat(info, args);
        }
        public static void LWarn(string info, params object[] args)
        {
            log.WarnFormat(info, args);
        }
        public static void LFattal(string info, params object[] args)
        {
            log.FatalFormat(info, args);
        }

    }
}
