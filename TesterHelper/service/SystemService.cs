using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using TesterHelper.constant;
using TesterHelper.context;
using TesterHelper.domain.config;
using TesterHelper.exception;
using TesterHelper.util;
using TesterHelper.util.logger;

namespace TesterHelper.service
{
    public class SystemService
    {
        private readonly ILogger logger = LogFactory.getLogger(typeof(SystemService));

        public SystemService() {
            LogFactory.initLog();
            SystemContext.setConfiguration(systemInit());
        }
        /// <summary>
        /// 程序初始化
        /// </summary>
        private Configuration systemInit() {
            // 判断配置文件是否存在
            if (!File.Exists(SystemConstant.systemConfigFilePath)) {
                throw new ServiceException("系统配置文件不存在请检查");
            }
            // 将配置文件反序列化
            var cfg = SerializeUtil.deserialization<Configuration>(SystemConstant.systemConfigFilePath);
            return cfg ?? throw new ServerException("文件反序列化失败，无法继续执行程序");
        }

        public void runProcess(string processFile) {
            logger.debug("{} will be run", processFile);
            if (!FileUtil.checkFile(processFile)) {
                logger.error($"{processFile} is not file and can not run");
                return;
            }

            try {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = processFile
                };
                Process process = new Process { StartInfo = startInfo };
                process.Start();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            logger.debug("{} run successfully", processFile);
        }
    }
}