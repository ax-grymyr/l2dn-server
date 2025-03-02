using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

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