using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlCombinationItem
{
    [XmlElement("reward")]
    public List<XmlCombinationItemReward> Rewards { get; set; } = [];

    [XmlAttribute("one")]
    public int One { get; set; }

    [XmlAttribute("enchantOne")]
    public int EnchantOne { get; set; }

    [XmlAttribute("two")]
    public int Two { get; set; }

    [XmlAttribute("enchantTwo")]
    public int EnchantTwo { get; set; }

    [XmlAttribute("commission")]
    public long Commission { get; set; }

    [XmlAttribute("chance")]
    public float Chance { get; set; } = 33;

    [XmlAttribute("announce")]
    public bool Announce { get; set; }
}