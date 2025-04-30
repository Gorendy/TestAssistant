using System;
using System.ComponentModel;
using TesterHelper.domain.config;
using TesterHelper.exception;
using TesterHelper.service;
using TesterHelper.util.logger;

namespace TesterHelper.context
{
    public class SystemContext
    {
        private static bool isInit = false;
        private static Configuration config;
        private readonly FileService _fileService = new FileService();
        private readonly SimulatorService _simulatorService = new SimulatorService();
        private readonly SystemService _systemService = new SystemService();
        private static Lazy<SystemContext> lazy => new Lazy<SystemContext>(() => new SystemContext());
        public static SystemContext Current => lazy.Value;

        public static DeviceF deviceF { get; set; }

        #region param

        public FileService fileService {
            get {
                checkInit();
                return _fileService;
            }
        }

        public SystemService systemService {
            get {
                checkInit();
                return _systemService;
            }
        }

        public SimulatorService simulatorService {
            get {
                checkInit();
                return _simulatorService;
            }
        }
        public static SystemConfig systemConfig {
            get {
                checkInit();
                return config.systemConfig;
            }
        }

        public static SimulatorConfig simulatorConfig {
            get {
                checkInit();
                return config.simulatorConfigs[0];
            }
        }

        #endregion
        

        public static void setConfiguration(Configuration cfg) {
            if (cfg == null) {
                throw new ServiceException(RCode.CONF_INIT_ERROR);
            }
            config = cfg;
            // 校验配置文件中参数信息
            config.checkField();
            isInit = true;
        }

        private static void checkInit() {
            if (!isInit) {
                throw new ServiceException(RCode.CONF_INIT_ERROR);
            }
        }
    }
}