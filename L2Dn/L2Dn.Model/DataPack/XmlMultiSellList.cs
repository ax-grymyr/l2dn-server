using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot(ElementName="list")]
public class XmlMultiSellList
{
    [XmlAttribute(AttributeName = "applyTaxes")]
    public bool ApplyTaxes { get; set; }
    
    [XmlAttribute(AttributeName = "isChanceMultisell")]
    public bool IsChanceMultiSell { get; set; }
    
    [XmlAttribute(AttributeName = "maintainEnchantment")]
    public bool MaintainEnchantment { get; set; }

    [XmlAttribute(AttributeName = "ingredientMultiplier")]
    public double IngredientMultiplier { get; set; } = 1.0;

    [XmlAttribute(AttributeName = "productMultiplier")]
    public double ProductMultiplier { get; set; } = 1.0;

    [XmlArray(ElementName = "npcs")]
    [XmlArrayItem(ElementName = "npc")] 
    public List<int> Npcs { get; set; } = [];

    [XmlElement(ElementName = "item")]
    public List<XmlMultiSellListItem> Items { get; set; } = [];
}