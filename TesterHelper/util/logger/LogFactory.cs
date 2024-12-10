using System;
using System.IO;
using log4net;
using log4net.Config;

namespace TesterHelper.util.logger
{
    public static class LogFactory
    {
        private static ILogger _logger;
        public static ILogger getLogger() {
            if (_logger == null) {
                lock (typeof(LogFactory)) {
                    if (_logger == null) {
                        _logger = new Logger("TesterHelper");
                    }
                }
            }

            return _logger;
        }

        public static ILogger getLogger(Type type) {
            return new Logger(type);
        }
        public static ILogger getLogger(string name) {
            return new Logger(name);
        }
        public static void initLog() {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "log.xml");
            FileInfo configFile = new FileInfo(configFilePath);
            XmlConfigurator.Configure(configFile);
        }
        
    }
}