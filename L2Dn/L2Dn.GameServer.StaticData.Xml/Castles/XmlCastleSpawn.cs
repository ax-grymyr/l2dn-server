using System.Xml.Serialization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.Common;

namespace L2Dn.GameServer.StaticData.Xml.Castles;

public sealed class XmlCastleSpawn: XmlLocationWithHeading
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("castleSide")]
    public CastleSide CastleSide { get; set; } = CastleSide.NEUTRAL;
}