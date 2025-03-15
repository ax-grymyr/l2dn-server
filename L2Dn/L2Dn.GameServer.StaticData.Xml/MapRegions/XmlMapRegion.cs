using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.MapRegions;

public sealed class XmlMapRegion
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("town")]
    public string Town { get; set; } = string.Empty;

    [XmlAttribute("castle")]
    public int Castle { get; set; }

    [XmlAttribute("locId")]
    public int LocationId { get; set; }

    [XmlAttribute("bbs")]
    public int Bbs { get; set; }

    [XmlElement("respawnPoint")]
    public List<XmlMapRegionRespawnPoint> RespawnPoints { get; set; } = [];

    [XmlElement("map")]
    public List<XmlMapRegionMap> MapTiles { get; set; } = [];

    [XmlElement("banned")]
    public List<XmlMapRegionBanned> Banned { get; set; } = [];
}