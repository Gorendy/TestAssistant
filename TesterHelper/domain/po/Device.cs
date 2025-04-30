using System;
using System.Xml.Serialization;
using TesterHelper.constant;
using TesterHelper.context;
using TesterHelper.domain.abstractclass;
using TesterHelper.domain.dto;
using TesterHelper.domain.vo;
using TesterHelper.util;

namespace TesterHelper.domain.po
{
    public class Device : AbstractDevice
    {
        /// <summary>
        /// 创建时间戳
        /// </summary>
        [XmlElement("Id")]
        public string id;
        /// <summary>
        /// 设备商首字符
        /// </summary>
        [XmlElement("FirstSpell")]
        public string firstSpell { get; set; }
        /// <summary>
        /// 设备作业类型
        /// </summary>
        [XmlElement("DeviceType")]
        public string deviceType { get; set; }
        
        /// <summary>
        /// 产线类型
        /// </summary>
        [XmlElement("Processing")]
        public string processing { get; set; }
        
        [XmlElement("CreateTime")]
        public string createTime { get; set; } 
        [XmlElement("UpdateTime")]
        public string updateTime { get; set; }
        [XmlElement("CompleteTime")]
        public string completeTime { get; set; }

        public Device() {
            
        }
        public Device(DeviceDTO vo) {
            deviceCode = vo.deviceCode;
            deviceOwner = vo.deviceOwner;
            deviceNum = vo.deviceNum;
            deviceId = vo.deviceId;
            ip = vo.ip;
            port = vo.port;
            var time = DateTime.Now;
            createTime = time.ToString("yyyy-MM-dd");
            updateTime = createTime;
            completeTime = createTime;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2020, 1, 1));
            id = Convert.ToInt64((time - startTime).TotalSeconds).ToString();
            firstSpell = StringUtil.convertToFirstSpell(deviceOwner);
        }
        public Device(int index, DeviceDTO vo) {
            deviceCode = vo.deviceCode;
            deviceOwner = vo.deviceOwner;
            deviceNum = vo.deviceNum;
            deviceId = vo.deviceId;
            ip = vo.ip;
            port = vo.port;
            var time = DateTime.Now;
            createTime = time.ToString("yyyy-MM-dd");
            updateTime = createTime;
            completeTime = createTime;
            id = index.ToString();
            firstSpell = StringUtil.convertToFirstSpell(deviceOwner);
        }

        public bool updateInfo(DeviceVO vo) {
            bool result = false;
            if (!string.IsNullOrEmpty(vo.deviceOwner) && !vo.deviceOwner.Equals(deviceOwner)) {
                deviceOwner = vo.deviceOwner;
                result = true;
            }
            if (!string.IsNullOrEmpty(vo.deviceCode) && !vo.deviceCode.Equals(deviceCode)) {
                deviceCode = vo.deviceCode;
                result = true;
            }
            if (!string.IsNullOrEmpty(vo.deviceNum) && !vo.deviceNum.Equals(deviceNum)) {
                deviceNum = vo.deviceNum;
                result = true;
            }
            if (!string.IsNullOrEmpty(vo.deviceId) && !vo.deviceId.Equals(deviceId)) {
                deviceId = vo.deviceId;
                result = true;
            }
            if (!string.IsNullOrEmpty(vo.ip) && !vo.ip.Equals(ip)) {
                ip = vo.ip;
                result = true;
            }
            if (!string.IsNullOrEmpty(vo.port) && !vo.port.Equals(port)) {
                port = vo.port;
                result = true;
            }

            return result;
        }

        public override int GetHashCode() {
            return id.GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj is Device device && id.Equals(device.id);
        }

        public override string ToString() {
            return $"DeviceVO:{{deviceOwner:'{deviceOwner}'deviceNum:'{deviceNum}',deviceCode:'{deviceCode}',ip:'{ip}',port:'{port}',deviceId:'{deviceId}'}}";
        }
    }
}