using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.MapRegions;

public sealed class XmlMapRegionRespawnPoint
{
    [XmlAttribute("X")]
    public int X { get; set; }

    [XmlAttribute("Y")]
    public int Y { get; set; }

    [XmlAttribute("Z")]
    public int Z { get; set; }

    [XmlAttribute("isOther")]
    public bool IsOther { get; set; }

    [XmlAttribute("isChaotic")]
    public bool IsChaotic { get; set; }

    [XmlAttribute("isBanish")]
    public bool IsBanish { get; set; }
}