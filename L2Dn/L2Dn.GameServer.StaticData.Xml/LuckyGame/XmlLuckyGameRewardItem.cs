using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.LuckyGame;

public class XmlLuckyGameRewardItem
{
    [XmlAttribute("count")]
    public int Count { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }
}