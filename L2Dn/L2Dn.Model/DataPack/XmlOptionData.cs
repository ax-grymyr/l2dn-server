using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlOptionData: XmlBase
{
    [XmlElement("option")]
    public List<XmlOption> Options { get; set; } = [];
}

public class XmlOption
{
    [XmlArray("effects")]
    [XmlArrayItem("effect")]
    public List<XmlOptionEffect> Effects { get; set; } = [];

    [XmlElement("active_skill")]
    public List<XmlOptionSkill> ActiveSkills { get; set; } = [];

    [XmlElement("passive_skill")]
    public List<XmlOptionSkill> PassiveSkills { get; set; } = [];

    [XmlElement("attack_skill")]
    public List<XmlOptionChanceSkill> AttackSkills { get; set; } = [];

    [XmlElement("critical_skill")]
    public List<XmlOptionChanceSkill> CriticalSkills { get; set; } = [];

    [XmlElement("magic_skill")]
    public List<XmlOptionChanceSkill> MagicSkills { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}

public class XmlOptionEffect 
{
    [XmlElement("amount")]
    public float Amount { get; set; }

    [XmlIgnore]
    public bool AmountSpecified { get; set; }

    [XmlElement("mode")]
    public string Mode { get; set; } = string.Empty;

    [XmlElement("attribute")]
    public string Attribute { get; set; } = string.Empty;

    [XmlElement("magicType")]
    public int MagicType { get; set; }

    [XmlIgnore]
    public bool MagicTypeSpecified { get; set; }

    [XmlElement("stat")]
    public string Stat { get; set; } = string.Empty;

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}

public class XmlOptionSkill 
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }
}

public class XmlOptionChanceSkill: XmlOptionSkill 
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }
}