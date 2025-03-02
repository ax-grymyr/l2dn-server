using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlCombinationItemReward
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("count")]
    public int Count { get; set; }

    [XmlAttribute("enchant")]
    public int Enchant { get; set; }

    [XmlAttribute("type")]
    public CombinationItemType Type { get; set; }
}