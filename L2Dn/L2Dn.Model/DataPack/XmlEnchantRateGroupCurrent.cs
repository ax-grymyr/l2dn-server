using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnchantRateGroupCurrent
{
    [XmlAttribute("enchant")]
    public string Enchant { get; set; } = string.Empty;

    [XmlAttribute("chance")]
    public double Chance { get; set; }
}