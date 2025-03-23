using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.LuckyGame;

public sealed class XmlLuckyGameModifyReward
{
    [XmlElement("item")]
    public List<XmlLuckyGameChanceRewardItem> Items { get; set; } = [];

    [XmlAttribute("max_game")]
    public int MaxGame { get; set; }

    [XmlAttribute("min_game")]
    public int MinGame { get; set; }
}