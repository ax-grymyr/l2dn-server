using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Zones;

public class XmlZoneStat
{
    [XmlAttribute("name")]
    public XmlZoneStatName Name { get; set; }

    [XmlAttribute("val")]
    public string Value { get; set; } = string.Empty;
}