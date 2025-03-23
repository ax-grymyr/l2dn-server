using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.LuckyGame;

[XmlRoot("list")]
public sealed class XmlLuckyGameData
{
    [XmlElement("luckygame")]
    public List<XmlLuckyGameItem> Items { get; set; } = [];
}