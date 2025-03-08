using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlClanLevelData
{
    [XmlElement("clan")]
    public List<XmlClanLevel> ClanLevels { get; set; } = [];
}