using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillEffectItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("count")]
    public long Count { get; set; }

    [XmlAttribute("minEnchant")]
    public byte MinEnchant { get; set; }

    [XmlIgnore]
    public bool MinEnchantSpecified { get; set; }

    [XmlAttribute("maxEnchant")]
    public byte MaxEnchant { get; set; }

    [XmlIgnore]
    public bool MaxEnchantSpecified { get; set; }
}