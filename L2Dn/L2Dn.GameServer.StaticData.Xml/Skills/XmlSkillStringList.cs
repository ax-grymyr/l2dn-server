using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillStringList
{
    [XmlElement("item")]
    public List<string> Items { get; set; } = [];
}