using System;
using System.IO;
using TesterHelper.context;
using TesterHelper.domain.dto;
using TesterHelper.domain.po;
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

        private delegate void runFunction();

        /// <summary>
        /// 点击开始后，开始准备测试设备
        /// </summary>
        public void startUpTester(DeviceVO deviceVo, out int index, bool run = true) {
            index = 0;
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
                // 保存到程序测试信息
                fileService.saveTestingDeviceInfo(dto,out index);
                //5，打开选择的模拟器
                if (run) {
                    runSimulator();
                }
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR, "程序执行失败，无法继续执行设备添加", e);
            }
            //6，关闭该程序
            logger.debug("startUpTester > end create device demo");
        }

        /// <summary>
        /// 更新模拟器配置文件中设备信息并选中
        /// </summary>
        /// <param name="deviceVo"></param>
        /// <returns></returns>
        public bool updateSimulatorConfig(DeviceVO deviceVo) {
            return baseFunction("updateSimulatorConfig", () => {
                var dto = deviceVo.toDTO();
                simulatorService.updateDeviceInfo(SystemContext.simulatorConfig, dto);
            });
        }

        /// <summary>
        /// 运行模拟器程序
        /// </summary>
        /// <param name="device"></param>
        public void runSimulatorOfDevice() {
            baseFunction("runSimulatorOfDevice", runSimulator);
        }

        public void deleteDeviceInfo(Device device) {
            baseFunction("deleteDeviceInfo", () => {
                // 保存到删除文件中
                
                // 删除测试程序文件程序信息
                SystemContext.deviceF.devices.Remove(device);
                fileService.updateTestingDevice();
                // 删除模拟器信息
                simulatorService.delDeviceInfo(SystemContext.simulatorConfig, device.simulatorNodeName);
                // 删除文件夹
                var devicePaperPath = SystemContext.systemConfig.devicePaperPath;
                var saveLogPath = SystemContext.systemConfig.deviceSaveLogPath;
                fileService.deleteDirectory(Path.Combine(saveLogPath, device.simulatorNodeName));
                fileService.deleteDirectory(Path.Combine(devicePaperPath, device.getRelationPath()));
            });
        }

        public void selectDeviceId(DeviceDTO dto) {
            baseFunction("selectDeviceId", () => simulatorService.selectDeviceIdInSimulatorConfig(dto));
        }
        /// <summary>
        /// 更新程序中测试设备信息
        /// </summary>
        /// <returns></returns>
        public bool updateDevice() {
            return baseFunction("updateDevice", () => {
                fileService.updateTestingDevice();
            });
        }
        /// <summary>
        /// 运行模拟器
        /// </summary>
        private void runSimulator() {
            var config = SystemContext.simulatorConfig;
            string path = fileService.findFile(config.basePath, config.executor);
            if (!string.IsNullOrEmpty(path)) {
                systemService.runProcess(path);
            }
        }

        private bool baseFunction(string name, runFunction function) {
            bool result = true;
            try {
                function();
            }
            catch (Exception e) {
                result = false;
                logger.error($"{name} > function run error;", e);
            }
            return result;
        }
    }
}