using System.Collections.Generic;
using System.Xml.Serialization;
using TesterHelper.domain.po;

namespace TesterHelper.domain.config
{
    [XmlRoot("List")]
    public class DeviceF
    {
        [XmlElement("DeviceIndex")]
        public int deviceIndex;
        [XmlArray("Devices")]
        public List<Device> devices;
    }
}