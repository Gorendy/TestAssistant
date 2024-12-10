using TesterHelper.domain.dto;

namespace TesterHelper.domain.vo
{
    public class DeviceVO
    {
        /// <summary>
        /// 设备商
        /// </summary>
        public  string deviceOwner { get; set; }
        /// <summary>
        /// 设备型号
        /// </summary>
        public string deviceNum { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public  string deviceCode { get; set; }
        /// <summary>
        /// 设备ip
        /// </summary>
        public  string ip { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public string port { get; set; } = "5000";

        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceId { get; set; } = "0";

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