using System.IO;
using TesterHelper.constant;
using TesterHelper.context;
using TesterHelper.domain.po;
using TesterHelper.domain.vo;
using TesterHelper.exception;
using TesterHelper.util;
using TesterHelper.util.logger;

namespace TesterHelper.domain.dto
{
    public class DeviceDTO : DeviceVO
    {
        private string _simulatorNodeName;
        private string _deviceLogPath;
        /// <summary>
        /// 待添加设备在模拟器配置文件中根节点名称
        /// 设备商+设备型号
        /// </summary>
        public string simulatorNodeName => 
            _simulatorNodeName ?? (_simulatorNodeName = $"{StringUtil.convertToFirstSpell(deviceOwner)}_{StringUtil.convertToLegal(deviceNum)}");

        public string deviceLogPath {
            get {
                return _deviceLogPath ?? (_deviceLogPath =
                    Path.Combine(SystemContext.systemConfig.deviceSaveLogPath, simulatorNodeName));
            }
        }
        public string getRelationPath() {
            return $"{deviceOwner}\\{deviceNum}";
        }
    }
}