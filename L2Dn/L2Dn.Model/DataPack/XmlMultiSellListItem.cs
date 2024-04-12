using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlMultiSellListItem
{
    [XmlElement(ElementName = "ingredient")]
    public List<XmlMultiSellListIngredient> Ingredients { get; set; } = [];
    
    [XmlElement(ElementName = "production", Type = typeof(XmlMultiSellListProduct))]
    public List<XmlMultiSellListProduct> Products { get; set; } = [];
}