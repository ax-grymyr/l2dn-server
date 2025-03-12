using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillLevelRestriction: IXmlSkillLevelRestriction
{
    [XmlAttribute("level")]
    public byte Level { get; set; }

    [XmlIgnore]
    public bool LevelSpecified { get; set; }

    [XmlAttribute("fromLevel")]
    public byte FromLevel { get; set; }

    [XmlIgnore]
    public bool FromLevelSpecified { get; set; }

    [XmlAttribute("toLevel")]
    public byte ToLevel { get; set; }

    [XmlIgnore]
    public bool ToLevelSpecified { get; set; }

    [XmlAttribute("fromSubLevel")]
    public ushort FromSubLevel { get; set; }

    [XmlIgnore]
    public bool FromSubLevelSpecified { get; set; }

    [XmlAttribute("toSubLevel")]
    public ushort ToSubLevel { get; set; }

    [XmlIgnore]
    public bool ToSubLevelSpecified { get; set; }
}