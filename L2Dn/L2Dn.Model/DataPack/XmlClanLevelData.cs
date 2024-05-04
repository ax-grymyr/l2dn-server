using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlClanLevelData: XmlBase
{
    [XmlElement("clan")]
    public List<XmlClanLevel> ClanLevels { get; set; } = [];
}