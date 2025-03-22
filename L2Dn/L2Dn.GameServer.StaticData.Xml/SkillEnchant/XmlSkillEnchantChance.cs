using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.SkillEnchant;

public sealed class XmlSkillEnchantChance
{
    [XmlAttribute("enchantLevel")]
    public int EnchantLevel { get; set; }

    [XmlAttribute("chance")]
    public int Chance { get; set; }
}