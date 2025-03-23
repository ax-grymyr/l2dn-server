using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.LuckyGame;

public sealed class XmlLuckyGameItem
{
    [XmlArray("common_reward")]
    [XmlArrayItem("item")]
    public List<XmlLuckyGameChanceRewardItem> CommonReward { get; set; } = [];

    [XmlArray("unique_reward")]
    [XmlArrayItem("item")]
    public List<XmlLuckyGameUniqueRewardItem> UniqueReward { get; set; } = [];

    [XmlElement("modify_reward")]
    public XmlLuckyGameModifyReward? ModifyReward { get; set; }

    [XmlAttribute("turning_point")]
    public int TurningPoint { get; set; }

    [XmlAttribute("index")]
    public int Index { get; set; }
}