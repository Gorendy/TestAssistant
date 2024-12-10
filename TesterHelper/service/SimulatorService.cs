using System;
using System.Xml;
using TesterHelper.context;
using TesterHelper.domain.config;
using TesterHelper.domain.dto;
using TesterHelper.domain.vo;
using TesterHelper.exception;
using TesterHelper.util;
using TesterHelper.util.logger;

namespace TesterHelper.service
{
    public class SimulatorService
    {
        private readonly ILogger logger = LogFactory.getLogger(typeof(SimulatorService));
        /// <summary>
        /// 向模拟器中添加新增设备信息
        /// </summary>
        public void addTestDeviceInfo(SimulatorConfig config, DeviceDTO dto) {
            if (config == null || dto == null) {
                logger.error(RCode.PARAM_NOTFOUND, "SimulatorConfig or vo is null,'{}','{}'", config, dto);
                return;
            }
            logger.debug("addTestDeviceInfo : start consist params:'{}','{}'", config, dto);
            logger.info("addTestDeviceInfo : start consist params:'{}','{}'", config, dto);
            //1 获取设备配置文件
            //2 加载配置文件
            var configDoc = new XmlDocument();
            logger.debug("addTestDeviceInfo : config xml file is loading..");
            string configFile = FileUtil.findFileRecur(config.basePath, config.programConfigName);
            try {
                configDoc.Load(configFile);
            }
            catch (Exception e) {
                logger.error($"模拟器配置文件加载失败,{e}");
                return;
            }
            //3 添加配置信息
            if (!(config.template is XmlNode[] node)) {
                logger.debug("addTestDeviceInfo : xml template is error..");
                logger.error($"获取模拟器配置模板失败,{config}");
                return;
            }
            // 替换节点信息并插入文件
            var cloneNode = node[0].Clone();
            
            try {
                // 将模板和设备信息组装
                logger.debug("addTestDeviceInfo : xml node is cloning..");
                var result = XmlUtil.consistXmlInfo(cloneNode, dto);
                if (result != null) {
                    logger.warn($"模拟器{config}中配置文件替换参数存在未替换值，{result}");
                }
                // 判断模拟器配置文件中是否含有该设备编号的配置信息
                var temp = XmlUtil.selectSingleNodeByPattern(configDoc, $"{config.tempXpath}/{dto.simulatorNodeName}");
                if (temp != null) {
                    logger.info("config of simulator have {} info, and will be delete the node {}", dto.simulatorNodeName, temp.InnerXml);
                    logger.debug("config of simulator have {} info, and will be delete the node {}", dto.simulatorNodeName, temp.InnerXml);
                    XmlUtil.delNode(temp);
                }
                // 将更新好的节点插入到文件中
                logger.debug("addTestDeviceInfo : insert node cloned to simulator config..");
                XmlUtil.insertNode(configDoc, config.tempXpath, cloneNode, dto.simulatorNodeName);
                // 如果存在选择模拟器主动加载当前设备信息，修改
                if (!string.IsNullOrEmpty(config.deviceItemSelectXpath)) {
                    var selectInfo =XmlUtil.selectSingleNodeByPattern(configDoc, $"{config.deviceItemSelectXpath}");
                    if (selectInfo != null) {
                        selectInfo.InnerText = dto.simulatorNodeName;
                        logger.debug("addTestDeviceInfo : update config of simulator listen device '{}' success", dto.simulatorNodeName);
                    }
                }
                configDoc.Save(configFile);
                logger.info("addTestDeviceInfo : insert node cloned to simulator config success");
                logger.debug("addTestDeviceInfo : end consist and save config file ---------------");
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_OPERATION, "模拟器配置解析失败", e);
            }
            
        }
        /// <summary>
        /// 删除模拟器配置文件中的设备id信息
        /// </summary>
        /// <param name="config"></param>
        /// <param name="deviceName">设备id</param>
        public void delDeviceInfo(SimulatorConfig config,string deviceName) {
            if (config == null || deviceName == null) {
                logger.error(RCode.PARAM_NOTFOUND, "SimulatorConfig or vo is null,'{}','{}'", config, deviceName);
                return;
            }
            logger.debug("delDeviceInfo : start delete device info params:'{}','{}'", config, deviceName);
            logger.info("delDeviceInfo : start delete device info params:'{}','{}'", config, deviceName);
            //1 获取设备配置文件
            //2 加载配置文件
            var configDoc = new XmlDocument();
            logger.debug("delDeviceInfo : config xml file is loading..");
            string configFile = FileUtil.findFileRecur(config.basePath, config.programConfigName);
            try {
                configDoc.Load(configFile);
            }
            catch (Exception e) {
                logger.error($"模拟器配置文件加载失败,{e}");
                return;
            }
            logger.debug("delDeviceInfo : config xml file is load success..");
            // 获取模拟器配置文件删除节点规则
            string delXpath;
            if (string.IsNullOrEmpty(config.deviceInfoDelXpath)) {
                // 如果程序配置中没有添加删除规则，则使用添加节点规则
                delXpath = config.tempXpath;
            }
            else {
                delXpath = config.deviceInfoDelXpath;
            }

            delXpath = $"{delXpath}/{deviceName}";
            logger.debug("delDeviceInfo : will be delete node matched '{}' xpath ruler", delXpath);
            // 获取当前所有符合规则的节点
            try {
                var list = XmlUtil.selectNodeByPattern(configDoc, delXpath);
                if (list != null && list.Count > 0) {
                    foreach (XmlNode node in list) {
                        XmlUtil.delNode(node);
                    }
                    logger.debug("delDeviceInfo : the node matched deleted complete");
                }
                else {
                    logger.warn($"the node named '{deviceName}' in {config.id} file is not find.");
                }
                configDoc.Save(configFile);
                logger.info("delDeviceInfo : delete node success.");
                logger.debug("delDeviceInfo : delete node success and save config file");
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_DEL_XMLNODE, $"the node named '{deviceName}' in {config.id} file delete fail", e);
            }
        }
    }
}