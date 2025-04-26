using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemStats
{
    [XmlElement("stat")]
    public List<XmlItemStat> Stats { get; set; } = [];

    [XmlElement("enchant")]
    public List<XmlItemEnchant> Enchants { get; set; } = [];

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlIgnore]
    public bool NameSpecified { get; set; }

    [XmlAttribute("val")]
    public int Value { get; set; }

    [XmlIgnore]
    public bool ValueSpecified { get; set; }
}