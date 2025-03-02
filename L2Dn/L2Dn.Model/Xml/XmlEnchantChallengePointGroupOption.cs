using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnchantChallengePointGroupOption
{
    [XmlAttribute("index")]
    public int Index { get; set; }

    [XmlAttribute("chance")]
    public int Chance { get; set; }

    [XmlAttribute("minEnchant")]
    public int MinEnchant { get; set; }

    [XmlAttribute("maxEnchant")]
    public int MaxEnchant { get; set; }
}