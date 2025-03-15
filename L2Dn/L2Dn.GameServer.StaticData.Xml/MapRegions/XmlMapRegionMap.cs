using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.MapRegions;

public sealed class XmlMapRegionMap
{
    [XmlAttribute("X")]
    public int X { get; set; }

    [XmlAttribute("Y")]
    public int Y { get; set; }
}