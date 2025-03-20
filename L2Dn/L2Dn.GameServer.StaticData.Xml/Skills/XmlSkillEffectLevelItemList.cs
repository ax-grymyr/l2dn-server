using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillEffectLevelItemList: XmlSkillLevelRestriction
{
    [XmlElement("item")]
    public List<XmlSkillEffectItemListChance> Items { get; set; } = [];
}