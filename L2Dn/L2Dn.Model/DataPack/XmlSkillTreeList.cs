using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlSkillTreeList: XmlBase
{
    [XmlElement("skillTree")]
    public List<XmlSkillTree> SkillTrees { get; set; } = [];
}