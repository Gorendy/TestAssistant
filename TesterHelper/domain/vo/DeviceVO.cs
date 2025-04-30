using TesterHelper.domain.abstractclass;
using TesterHelper.domain.dto;

namespace TesterHelper.domain.vo
{
    public class DeviceVO : AbstractDevice
    {
        public DeviceDTO toDTO() {
            var dto = new DeviceDTO();
            dto.deviceOwner = deviceOwner;
            dto.deviceNum = deviceNum;
            dto.deviceId = deviceId;
            dto.deviceCode = deviceCode;
            dto.ip = ip;
            dto.port = port;
            return dto;
        }

        public override string ToString() {
            return $"DeviceVO:{{deviceOwner:'{deviceOwner}'deviceNum:'{deviceNum}',deviceCode:'{deviceCode}',ip:'{ip}',port:'{port}',deviceId:'{deviceId}'}}";
        }
    }
}