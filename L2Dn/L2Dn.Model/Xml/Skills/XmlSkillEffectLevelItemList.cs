using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillEffectLevelItemList: XmlSkillLevelRestriction
{
    [XmlElement("item")]
    public List<XmlSkillEffectItemListChance> Items { get; set; } = [];
}