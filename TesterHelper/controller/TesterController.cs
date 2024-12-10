using System;
using TesterHelper.context;
using TesterHelper.domain.vo;
using TesterHelper.service;
using TesterHelper.util.logger;

namespace TesterHelper.controller
{
    public class TesterController
    {
        private readonly ILogger logger = LogFactory.getLogger();
        private readonly FileService fileService = SystemContext.Current.fileService;
        private readonly SimulatorService simulatorService = SystemContext.Current.simulatorService;
        private readonly SystemService systemService = SystemContext.Current.systemService;

        /// <summary>
        /// 点击开始后，开始准备测试设备
        /// </summary>
        public void startUpTester(DeviceVO deviceVo) {
            logger.debug("startUpTester > start create device demo,info:{}", deviceVo);
            logger.info("startUpTester > start create device demo,info:{}", deviceVo);
            var dto = deviceVo.toDTO();
            //1，从前端获取数据
            //2，将数据解析
            string devicePaperPath = SystemContext.systemConfig.devicePaperPath;
            string saveLogPath = SystemContext.systemConfig.deviceSaveLogPath;
            try {
                //3，创建测试程序日志文件夹和测试文件夹
                fileService.createDirectory(devicePaperPath, dto.getRelationPath());
                fileService.createDirectory(saveLogPath, dto.simulatorNodeName);
                //4，新增模拟器测试程序配置信息
                simulatorService.addTestDeviceInfo(SystemContext.simulatorConfig, dto);
                //5，打开选择的模拟器
                var config = SystemContext.simulatorConfig;
                string path = fileService.findFile(config.basePath, config.executor);
                if (!string.IsNullOrEmpty(path)) {
                    systemService.runProcess(path);
                }
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR, "程序执行失败，无法继续执行设备添加", e);
            }
            //6，关闭该程序
            logger.debug("startUpTester > end create device demo");
        }
        
    }
}