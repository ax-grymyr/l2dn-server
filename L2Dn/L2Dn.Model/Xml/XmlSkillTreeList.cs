using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlSkillTreeList
{
    [XmlElement("skillTree")]
    public List<XmlSkillTree> SkillTrees { get; set; } = [];
}