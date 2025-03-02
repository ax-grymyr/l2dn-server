using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnchantScrollGroup
{
    [XmlElement("enchantRate")]
    public List<XmlEnchantScrollGroupRate> EnchantRates { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }
}