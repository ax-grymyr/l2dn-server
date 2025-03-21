using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Vip;

public sealed class XmlVipTierBonusInfo
{
    [XmlAttribute("silverChance")]
    public double SilverChance { get; set; }

    [XmlAttribute("goldChance")]
    public double GoldChance { get; set; }

    [XmlAttribute("skill")]
    public int SkillId { get; set; }
}