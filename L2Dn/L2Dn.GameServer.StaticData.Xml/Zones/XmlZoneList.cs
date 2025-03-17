using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Zones;

[XmlRoot("list")]
public sealed class XmlZoneList: IXmlRoot
{
    [XmlElement("zone")]
    public List<XmlZone> Zones { get; set; } = [];

    [XmlAttribute("enabled")]
    public bool Enabled { get; set; }

    [XmlIgnore]
    public string FilePath { get; set; } = string.Empty;
}