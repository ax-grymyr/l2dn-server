using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlSkillTree
{
    [XmlElement("skill")]
    public List<XmlSkillTreeSkill> Skills { get; set; } = [];

    [XmlAttribute("classId")]
    public int ClassId { get; set; }

    [XmlIgnore]
    public bool ClassIdSpecified { get; set; }

    [XmlAttribute("parentClassId")]
    public int ParentClassId { get; set; }

    [XmlIgnore]
    public bool ParentClassIdSpecified { get; set; }

    [XmlAttribute("type")]
    public XmlSkillTreeType Type { get; set; }

    [XmlAttribute("race")]
    public Race Race { get; set; }

    [XmlIgnore]
    public bool RaceSpecified { get; set; }

    [XmlAttribute("subType")]
    public SubclassType SubType { get; set; }

    [XmlIgnore]
    public bool SubTypeSpecified { get; set; }
}