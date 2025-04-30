using System.IO;
using TesterHelper.constant;
using TesterHelper.context;
using TesterHelper.domain.abstractclass;

namespace TesterHelper.domain.dto
{
    public class DeviceDTO : AbstractDevice
    {
        
        private string _deviceLogPath;
        
        public string deviceLogPath {
            get {
                return _deviceLogPath ?? (_deviceLogPath =
                    Path.Combine(SystemContext.systemConfig.deviceSaveLogPath, simulatorNodeName));
            }
        }
        
    }
}