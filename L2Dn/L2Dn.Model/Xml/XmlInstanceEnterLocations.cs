using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlInstanceEnterLocations
{
    [XmlElement("location")]
    public List<XmlInstanceEnterLocation> Locations { get; set; } = [];

    [XmlAttribute("type")]
    public InstanceTeleportType Type { get; set; }
}