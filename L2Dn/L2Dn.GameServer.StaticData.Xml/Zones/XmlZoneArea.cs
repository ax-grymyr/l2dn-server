using System.Xml.Serialization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.Common;

namespace L2Dn.GameServer.StaticData.Xml.Zones;

public class XmlZoneArea
{
    [XmlElement("node")]
    public List<XmlLocation2D> Nodes { get; set; } = [];

    [XmlAttribute("minZ")]
    public int MinZ { get; set; }

    [XmlAttribute("maxZ")]
    public int MaxZ { get; set; }

    [XmlAttribute("shape")]
    public ZoneShape Shape { get; set; }

    [XmlAttribute("rad")]
    public int Radius { get; set; }
}