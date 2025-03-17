using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.Zones;

public class XmlZoneRace
{
    [XmlAttribute("name")]
    public Race Race { get; set; }

    [XmlAttribute("point")]
    public string Point { get; set; } = string.Empty;
}