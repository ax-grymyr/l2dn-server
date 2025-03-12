using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

[XmlRoot("list")]
public class XmlSkillList
{
    [XmlElement("skill")]
    public List<XmlSkill> Skills { get; set; } = [];
}