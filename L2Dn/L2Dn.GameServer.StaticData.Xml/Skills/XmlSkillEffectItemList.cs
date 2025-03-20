using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillEffectItemList
{
    [XmlElement("item")]
    public List<XmlSkillEffectItemListChance> Items { get; set; } = [];

    [XmlElement("value")]
    public List<XmlSkillEffectLevelItemList> Values { get; set; } = [];
}