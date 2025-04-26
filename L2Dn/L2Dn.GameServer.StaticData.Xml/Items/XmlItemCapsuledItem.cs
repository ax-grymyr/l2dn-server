using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemCapsuledItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("min")]
    public int MinCount { get; set; }

    [XmlAttribute("max")]
    public int MaxCount { get; set; }

    [XmlAttribute("chance")]
    public double Chance { get; set; }

    [XmlAttribute("minEnchant")]
    public int MinEnchant { get; set; }

    [XmlIgnore]
    public bool MinEnchantSpecified { get; set; }

    [XmlAttribute("maxEnchant")]
    public int MaxEnchant { get; set; }

    [XmlIgnore]
    public bool MaxEnchantSpecified { get; set; }
}