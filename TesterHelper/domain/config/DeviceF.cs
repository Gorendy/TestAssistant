using System.Collections.Generic;
using System.Xml.Serialization;
using TesterHelper.domain.po;

namespace TesterHelper.domain.config
{
    [XmlRoot("List")]
    public class DeviceF
    {
        [XmlArray("Device")]
        public List<Device> devices;
    }
}