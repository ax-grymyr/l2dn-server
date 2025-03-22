using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.SkillEnchant;

public sealed class XmlSkillEnchantSkill
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("starLevel")]
    public int StarLevel { get; set; }

    [XmlAttribute("maxEnchantLevel")]
    public int MaxEnchantLevel { get; set; }
}