using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.AccessLevels;

[XmlRoot("list")]
public class XmlAccessLevelList
{
    [XmlElement("access")]
    public List<XmlAccessLevel> AccessLevels { get; set; } = [];
}