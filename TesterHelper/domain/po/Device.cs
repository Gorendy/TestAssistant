using System;
using TesterHelper.domain.vo;
using TesterHelper.util;

namespace TesterHelper.domain.po
{
    public class Device
    {
        public string id;
        
        /// <summary>
        /// 设备商
        /// </summary>
        public string deviceOwner { get; set; }
        
        /// <summary>
        /// 设备型号
        /// </summary>
        public string deviceNum { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public  string deviceCode { get; set; }
        /// <summary>
        /// 设备作业类型
        /// </summary>
        public string deviceType { get; set; }
        /// <summary>
        /// 产线类型
        /// </summary>
        public string processing { get; set; }
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

        public DateTime createTime { get; set; } = DateTime.Now;
        public DateTime updateTime { get; set; }
        public DateTime completeTime { get; set; }

        public Device(DeviceVO vo) {
            deviceCode = vo.deviceCode;
            deviceOwner = vo.deviceOwner;
            deviceNum = vo.deviceNum;
            deviceId = vo.deviceId;
            ip = vo.ip;
            port = vo.port;
            var time = DateTime.Now;
            createTime = time;
            id = time.ToShortTimeString();
        }
        
        public override string ToString() {
            return $"DeviceVO:{{deviceOwner:'{deviceOwner}'deviceNum:'{deviceNum}',deviceCode:'{deviceCode}',ip:'{ip}',port:'{port}',deviceId:'{deviceId}'}}";
        }
    }
}