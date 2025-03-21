using System.Xml.Serialization;
using L2Dn.Model.Xml;

namespace L2Dn.GameServer.StaticData.Xml.ClanLevels;

[XmlRoot("list")]
public class XmlClanLevelList
{
    [XmlElement("clan")]
    public List<XmlClanLevel> ClanLevels { get; set; } = [];
}