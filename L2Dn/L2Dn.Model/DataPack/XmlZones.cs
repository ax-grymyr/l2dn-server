using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlZones: XmlBase
{
    [XmlElement("zone")]
    public List<XmlZone> Zones { get; set; } = [];
    
    [XmlAttribute("enabled")]
    public bool Enabled { get; set; }    
}