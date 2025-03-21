using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.ClanLevels;

public class XmlClanLevel
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("exp")]
    public int Exp { get; set; }
}