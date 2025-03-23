using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.LuckyGame;

public sealed class XmlLuckyGameUniqueRewardItem
{
    [XmlAttribute("count")]
    public int Count { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("points")]
    public int Points { get; set; }
}