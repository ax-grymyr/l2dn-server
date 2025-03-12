using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillIntList
{
    [XmlElement("item")]
    public List<int> Items { get; set; } = [];
}