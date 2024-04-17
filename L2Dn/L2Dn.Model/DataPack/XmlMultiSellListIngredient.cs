using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlMultiSellListIngredient: XmlMultiSellListItemEntry
{
    [XmlAttribute("maintainIngredient")]
    public bool MaintainIngredient { get; set; }
}