using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Vip;

public sealed class XmlVipTierInfo
{
    [XmlElement("bonus")]
    public List<XmlVipTierBonusInfo> Bonuses { get; set; } = [];

    [XmlAttribute("tier")]
    public int Tier { get; set; }

    [XmlAttribute("points-required")]
    public long PointsRequired { get; set; }

    [XmlAttribute("points-lose")]
    public long PointsLose { get; set; }
}