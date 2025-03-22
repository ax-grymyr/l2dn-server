using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.SkillEnchant;

public sealed class XmlSkillEnchantPointStarItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("exp")]
    public int Exp { get; set; }

    [XmlAttribute("starLevel")]
    public int StarLevel { get; set; }
}