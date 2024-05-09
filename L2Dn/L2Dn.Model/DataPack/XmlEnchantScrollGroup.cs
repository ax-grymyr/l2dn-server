using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnchantScrollGroup
{
    [XmlElement("enchantRate")]
    public List<XmlEnchantScrollGroupRate> EnchantRates { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }
}