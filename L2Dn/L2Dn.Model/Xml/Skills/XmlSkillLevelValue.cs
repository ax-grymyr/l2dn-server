using System.Globalization;
using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillLevelValue: IXmlSkillLevelValue
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

    [XmlText]
    public string Value { get; set; } = string.Empty;

    object IXmlSkillLevelValue.Value => Value;
}

public class XmlSkillLevelValue<T>: IXmlSkillLevelValue
    where T: unmanaged
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

    [XmlText]
    public T Value { get; set; }

    object IXmlSkillLevelValue.Value => Value;
}