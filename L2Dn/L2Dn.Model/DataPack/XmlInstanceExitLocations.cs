using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlInstanceExitLocations
{
    [XmlElement("location")]
    public List<XmlInstanceExitLocation> Locations { get; set; } = [];

    [XmlAttribute("type")]
    public InstanceTeleportType Type { get; set; }
}