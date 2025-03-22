using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.SkillEnchant;

public sealed class XmlSkillEnchantStar
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("expMax")]
    public int ExpMax { get; set; }

    [XmlAttribute("expOnFail")]
    public int ExpOnFail { get; set; }

    [XmlAttribute("feeAdena")]
    public long FeeAdena { get; set; }
}