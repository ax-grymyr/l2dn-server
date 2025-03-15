using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.MapRegions;

public sealed class XmlMapRegionBanned
{
    [XmlAttribute("race")]
    public Race Race { get; set; }

    [XmlAttribute("point")]
    public string Point { get; set; } = string.Empty;
}