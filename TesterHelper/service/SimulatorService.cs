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
            XmlDocument configDoc = null;
            string configFile;
            try {
                configDoc = loadSimulatorConfig(config, out configFile);
            }
            catch (Exception e) {
                logger.error($"addTestDeviceInfo : config of simulator load error,{e}");
                return;
            }
            logger.debug("addTestDeviceInfo : config xml file is load success..");
            
            try {
                // 将节点插入配置文件中
                insertDeviceInfoNode(config, configDoc, dto, dto.simulatorNodeName);
                logger.info("addTestDeviceInfo : insert node cloned to simulator config success");
                // 选择当前设备节点
                selectDeviceNodeInSimulator(configDoc, config, dto.simulatorNodeName);
                configDoc.Save(configFile);
                logger.info("addTestDeviceInfo : select device node success");
                logger.debug("addTestDeviceInfo : end consist and save config file ---------------");
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_OPERATION, "模拟器配置解析失败", e);
            }
        }

        /// <summary>
        /// 更新模拟器配置文件中程序信息，并在模拟器配置中选择当前程序id
        /// </summary>
        /// <param name="config"></param>
        /// <param name="dto"></param>
        public void updateDeviceInfo(SimulatorConfig config, DeviceDTO dto) {
            if (config == null || dto == null) {
                logger.error(RCode.PARAM_NOTFOUND, "SimulatorConfig or vo is null,'{}','{}'", config, dto);
                return;
            }
            logger.debug("updateDeviceInfo : start consist params:'{}','{}'", config, dto);
            logger.info("updateDeviceInfo : start consist params:'{}','{}'", config, dto);
            //1 获取设备配置文件
            //2 加载配置文件
            XmlDocument configDoc = null;
            string configFile;
            try {
                configDoc = loadSimulatorConfig(config, out configFile);
            }
            catch (Exception e) {
                logger.error($"updateDeviceInfo : config of simulator load error,{e}");
                return;
            }
            logger.debug("updateDeviceInfo : config xml file is load success..");
            try {
                // 删除当前设备信息
                deleteDeviceInfoNode(config, configDoc, dto.simulatorNodeName);
                // 将节点新增
                insertDeviceInfoNode(config, configDoc, dto, dto.simulatorNodeName);
                logger.info("updateDeviceInfo : insert node cloned to simulator config success");
                selectDeviceNodeInSimulator(configDoc, config, dto.simulatorNodeName);
                // 保存文件
                configDoc.Save(configFile);
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_UPT_XMLNODE, "the node update fail", e);
                return;
            }
            logger.debug("updateDeviceInfo : update success");
            logger.info("updateDeviceInfo : update success");
        }
        /// <summary>
        /// 删除模拟器配置文件中的所有设备id信息
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
            XmlDocument configDoc = null;
            string configFile;
            try {
                configDoc = loadSimulatorConfig(config, out configFile);
            }
            catch (Exception e) {
                logger.error($"delDeviceInfo : config of simulator load error,{e}");
                return;
            }
            logger.debug("delDeviceInfo : config xml file is load success..");
            // 获取模拟器配置文件删除节点规则
            try {
                deleteDeviceInfoNode(config, configDoc, deviceName);
                configDoc.Save(configFile);
                logger.info("delDeviceInfo : delete node success.");
                logger.debug("delDeviceInfo : delete node success and save config file");
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_DEL_XMLNODE, $"the node named '{deviceName}' in {config.id} file delete fail", e);
            }
        }

        public void selectDeviceIdInSimulatorConfig(DeviceDTO dto) {
            if (dto == null) {
                logger.error(RCode.PARAM_NOTFOUND);
                return;
            }
            logger.debug("selectDeviceIdInSimulatorConfig : start consist params:'{}'", dto);
            logger.info("selectDeviceIdInSimulatorConfig : start consist params:'{}'", dto);
            //1 获取设备配置文件
            //2 加载配置文件
            XmlDocument configDoc = null;
            string configFile;
            try {
                configDoc = loadSimulatorConfig(SystemContext.simulatorConfig, out configFile);
            }
            catch (Exception e) {
                logger.error($"selectDeviceIdInSimulatorConfig : config of simulator load error", e);
                return;
            }
            logger.debug("selectDeviceIdInSimulatorConfig : config xml file is load success..");
            try {
                if (!string.IsNullOrEmpty(SystemContext.simulatorConfig.deviceItemSelectXpath)) {
                    var selectInfo =XmlUtil.selectSingleNodeByPattern(configDoc, SystemContext.simulatorConfig.deviceItemSelectXpath);
                    if (selectInfo != null) {
                        if (!dto.simulatorNodeName.Equals(selectInfo.InnerText)) {
                            selectInfo.InnerText = dto.simulatorNodeName;
                            configDoc.Save(configFile);
                        }
                    }
                }
            }
            catch (Exception e) {
                logger.error($"selectDeviceIdInSimulatorConfig : select item error", e);
                return;
            }
            logger.info("selectDeviceIdInSimulatorConfig : select end");
            logger.debug("selectDeviceIdInSimulatorConfig : select end");
        }
        /// <summary>
        /// 加载xml配置文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        private XmlDocument loadSimulatorConfig(SimulatorConfig config, out string path) {
            //1 获取设备配置文件
            //2 加载配置文件
            var configDoc = new XmlDocument();
            path = null;
            logger.debug("loadSimulatorConfig - config xml file is loading..");
            string configFile = FileUtil.findFileRecur(config.basePath, config.programConfigName);
            try {
                configDoc.Load(configFile);
            }
            catch (Exception e) {
                throw new ServiceException($"模拟器配置文件加载失败,{e}");
            }

            path = configFile;
            logger.debug("loadSimulatorConfig - config xml file load success");
            return configDoc;
        }

        /// <summary>
        /// 向配置文件中插入新节点，如果存在则删后插入
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configDoc"></param>
        /// <param name="obj"></param>
        /// <param name="nodeName"></param>
        /// <exception cref="ServiceException"></exception>
        private void insertDeviceInfoNode(SimulatorConfig config, XmlDocument configDoc, object obj, string nodeName) {
            //3 添加配置信息
            if (!(config.template is XmlNode[] node)) {
                throw new ServiceException($"获取模拟器配置模板失败,{config}");
            }
            // 替换节点信息并插入文件
            var cloneNode = node[0].Clone();
            // 将模板和设备信息组装
            logger.debug("insertDeviceInfoNode - xml node is cloning..");
            var result = XmlUtil.consistXmlInfo(cloneNode, obj);
            if (result != null) {
                logger.warn($"insertDeviceInfoNode - 模拟器{config}中配置文件替换参数存在未替换值，{result}");
            }
            // 判断模拟器配置文件中是否含有该设备编号的配置信息
            var temp = XmlUtil.selectSingleNodeByPattern(configDoc, $"{config.tempXpath}/{nodeName}");
            if (temp != null) {
                logger.info("config of simulator have {} info, and will be delete the node {}", nodeName, temp.InnerXml);
                logger.debug("config of simulator have {} info, and will be delete the node {}", nodeName, temp.InnerXml);
                XmlUtil.delNode(temp);
            }
            // 将更新好的节点插入到文件中
            logger.debug("insertDeviceInfoNode - insert node cloned to simulator config..");
            XmlUtil.insertNode(configDoc, config.tempXpath, cloneNode, nodeName);
            logger.debug("insertDeviceInfoNode - insert node success");
        }

        /// <summary>
        /// 选择当前设备id
        /// </summary>
        /// <param name="configDoc"></param>
        /// <param name="config"></param>
        /// <param name="deviceNode"></param>
        private void selectDeviceNodeInSimulator(XmlDocument configDoc, SimulatorConfig config, string deviceNode) {
            // 如果存在选择模拟器主动加载当前设备信息，修改
            if (!string.IsNullOrEmpty(config.deviceItemSelectXpath)) {
                var selectInfo =XmlUtil.selectSingleNodeByPattern(configDoc, config.deviceItemSelectXpath);
                if (selectInfo != null) {
                    selectInfo.InnerText = deviceNode;
                    logger.debug("selectDeviceNodeInSimulator - update config of simulator listen device '{}' success", deviceNode);
                }
            }
            logger.debug("selectDeviceNodeInSimulator - select device node success");
        }
        /// <summary>
        /// 删除所有符合规则的节点
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configDoc"></param>
        /// <param name="deviceName"></param>
        private void deleteDeviceInfoNode(SimulatorConfig config, XmlDocument configDoc, string deviceName) {
            // 获取模拟器配置文件删除节点规则
            string delXpath;
            if (string.IsNullOrEmpty(config.deviceInfoDelXpath)) {
                // 如果程序配置中没有添加删除规则，则使用添加节点规则
                delXpath = config.tempXpath;
            }
            else {
                delXpath = config.deviceInfoDelXpath;
            }
            logger.debug("deleteDeviceInfoNode - will be delete node matched '{}' xpath ruler", delXpath);
            delXpath = $"{delXpath}/{deviceName}";
            var list = XmlUtil.selectNodeByPattern(configDoc, delXpath);
            if (list != null && list.Count > 0) {
                foreach (XmlNode node in list) {
                    XmlUtil.delNode(node);
                }
                logger.debug("deleteDeviceInfoNode - the node matched deleted complete");
            }
            else {
                logger.warn($"deleteDeviceInfoNode - the node named '{deviceName}' in {config.id} file is not find.");
            }
        }
    }
}