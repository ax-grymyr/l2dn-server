using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlMultiSellList
{
    [XmlAttribute("applyTaxes")]
    public bool ApplyTaxes { get; set; }

    [XmlAttribute("isChanceMultisell")]
    public bool IsChanceMultiSell { get; set; }

    [XmlAttribute("maintainEnchantment")]
    public bool MaintainEnchantment { get; set; }

    [XmlAttribute("ingredientMultiplier")]
    public double IngredientMultiplier { get; set; } = 1.0;

    [XmlAttribute("productMultiplier")]
    public double ProductMultiplier { get; set; } = 1.0;

    [XmlArray("npcs")]
    [XmlArrayItem("npc")]
    public List<int> Npcs { get; set; } = [];

    [XmlElement("item")]
    public List<XmlMultiSellListItem> Items { get; set; } = [];
}