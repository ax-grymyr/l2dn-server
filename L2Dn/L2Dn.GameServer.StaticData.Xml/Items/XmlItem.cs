using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItem
{
    [XmlElement("set")]
    public List<XmlItemParameter> Parameters { get; set; } = [];

    [XmlArray("unequip_skills")]
    [XmlArrayItem("skill")]
    public List<XmlItemSkill> UnequipSkills { get; set; } = [];

    [XmlArray("capsuled_items")]
    [XmlArrayItem("item")]
    public List<XmlItemCapsuledItem> CapsuledItems { get; set; } = [];

    [XmlElement("cond")]
    public List<XmlItemConditionList> Conditions { get; set; } = [];

    [XmlArray("skills")]
    [XmlArrayItem("skill")]
    public List<XmlItemSkill> Skills { get; set; } = [];

    [XmlElement("stats")]
    public XmlItemStats? Stats { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("additionalName")]
    public string AdditionalName { get; set; } = string.Empty;

    [XmlAttribute("type")]
    public XmlItemType Type { get; set; }
}