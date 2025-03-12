using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillEffectItemListChance
{
    [XmlElement("item")]
    public List<XmlSkillEffectItem> Items { get; set; } = [];

    [XmlAttribute("chance")]
    public decimal Chance { get; set; }
}