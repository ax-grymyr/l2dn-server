using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.LuckyGame;

public sealed class XmlLuckyGameChanceRewardItem: XmlLuckyGameRewardItem
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }
}