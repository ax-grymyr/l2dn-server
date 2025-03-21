using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Castles;

[XmlRoot("list")]
public sealed class XmlCastleList
{
    [XmlElement("castle")]
    public List<XmlCastle> Castles { get; set; } = [];
}