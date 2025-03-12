using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillStringList
{
    [XmlElement("item")]
    public List<string> Items { get; set; } = [];
}