using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillCond
{
    public XmlSkillCondAnd and { get; set; } = new();

    [XmlIgnore]
    public bool andSpecified { get; set; }

    public XmlSkillCondNot not { get; set; } = new();

    [XmlIgnore]
    public bool notSpecified { get; set; }

    [XmlAttribute]
    public byte msgId { get; set; }

    [XmlAttribute]
    public byte addName { get; set; }
}