using System.Collections.Generic;
using System.Xml.Serialization;
using TesterHelper.exception;
using TesterHelper.util.logger;

namespace TesterHelper.domain.config
{
    /// <summary>
    /// 程序配置文件
    /// </summary>
    [XmlRoot("CONFIGURATION")]
    public class Configuration : ICheckable
    {
        /// <summary>
        /// 程序基本配置信息
        /// </summary>
        [XmlElement("SYSTEMCONFIG")]
        public SystemConfig systemConfig;
        /// <summary>
        /// 模拟器信息
        /// </summary>
        [XmlArray("SIMULATORCONFIGS")]
        [XmlArrayItem("SimulatorConfig")]
        public List<SimulatorConfig> simulatorConfigs;

        public void checkField() {
            if (systemConfig == null) {
                throw new ServiceException("SYSTEMCONFIG is null");
            }
            else {
                systemConfig.checkField();
            }

            if (simulatorConfigs == null || simulatorConfigs.Count == 0) {
                throw new ServiceException("SIMULATORCONFIGS is null");
            }
            else {
                foreach (var s in simulatorConfigs) {
                    s.checkField();
                }
            }
        }
    }

    public class SystemConfig : ICheckable
    {
        /// <summary>
        /// 程序log路径
        /// </summary>
        [XmlElement("LogBasePath")]
        public string logBasePath;
        /// <summary>
        /// 当前使用模拟器
        /// </summary>
        [XmlElement("CurrentUseSimulatorID")]
        public string currentUseSimulatorID;
        /// <summary>
        /// 测试设备基本log存放路径
        /// </summary>
        [XmlElement("DeviceSaveLogPath")]
        public string deviceSaveLogPath;
        /// <summary>
        /// 测试设备存放资料路径
        /// </summary>
        [XmlElement("DevicePaperPath")]
        public string devicePaperPath;

        public override string ToString() {
            return $"SystemConfig:{{LogBasePath:'{logBasePath}',CurrentUseSimulatorID:'{currentUseSimulatorID}'," +
                   $"DeviceSaveLogPath:'{deviceSaveLogPath}',DevicePaperPath:'{devicePaperPath}'}}";
        }

        public void checkField() {
            LogFactory.getLogger().info(this.ToString());
        }
    }
    public class SimulatorConfig : ICheckable
    {
        [XmlAttribute("Id")]
        public string id;
        /// <summary>
        /// 模拟器根文件夹
        /// </summary>
        [XmlElement("SimulatorBasePath")]
        public string basePath;
        /// <summary>
        /// 模拟器配置文件名称
        /// </summary>
        [XmlElement("ProgramConfigName")]
        public string programConfigName;
        /// <summary>
        /// 启动程序路径
        /// </summary>
        [XmlElement("Executor")]
        public string executor;

        /// <summary>
        /// 模板在模拟器配置文件中的xpath路径
        /// </summary>
        [XmlElement("TempXpath")]
        public string tempXpath;
        /// <summary>
        /// 选择当前设备项xpath
        /// </summary>
        [XmlElement("DeviceItemSelectXpath")]
        public string deviceItemSelectXpath;
        /// <summary>
        /// 删除某一设备信息规则
        /// </summary>
        [XmlElement("DeviceInfoDelXpath")]
        public string deviceInfoDelXpath;
        /// <summary>
        /// 新增样例配置文件模板
        /// </summary>
        [XmlElement("Template")]
        public object template;

        public override string ToString() {
            return $"SimulatorConfig:{{ID:'{id}',ProgramConfigName:'{programConfigName}',Executor:'{executor}',TempXpath:'{tempXpath}',Template:'{template}'}}";
        }

        public void checkField() {
            LogFactory.getLogger().info(this.ToString());
        }
    }
}