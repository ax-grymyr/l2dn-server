using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.SkillEnchant;

public sealed class XmlSkillEnchantPointStar
{
    [XmlElement("item")]
    public List<XmlSkillEnchantPointStarItem> Items { get; set; } = [];

    [XmlAttribute("level")]
    public int Level { get; set; }
}