using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.Zones;

public class XmlZone: XmlZoneArea
{
    [XmlElement("stat")]
    public List<XmlZoneStat> Stats { get; set; } = [];

    [XmlArray("bannedAreas")]
    [XmlArrayItem("area")]
    public List<XmlZoneArea> BannedAreas { get; set; } = [];

    [XmlElement("race")]
    public List<XmlZoneRace> Races { get; set; } = [];

    [XmlElement("spawn")]
    public List<XmlZoneSpawn> Spawns { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlIgnore]
    public bool IdSpecified { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("type")]
    public ZoneType Type { get; set; }
}