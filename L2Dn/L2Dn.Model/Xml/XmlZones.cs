using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlZones
{
    [XmlElement("zone")]
    public List<XmlZone> Zones { get; set; } = [];

    [XmlAttribute("enabled")]
    public bool Enabled { get; set; }
}