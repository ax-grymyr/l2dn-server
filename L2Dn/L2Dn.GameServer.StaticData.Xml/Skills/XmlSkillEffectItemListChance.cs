using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillEffectItemListChance
{
    [XmlElement("item")]
    public List<XmlSkillEffectItem> Items { get; set; } = [];

    [XmlAttribute("chance")]
    public decimal Chance { get; set; }
}