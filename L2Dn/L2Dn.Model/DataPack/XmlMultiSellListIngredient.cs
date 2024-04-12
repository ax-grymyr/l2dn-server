using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlMultiSellListIngredient: XmlMultiSellListItemEntry
{
    [XmlAttribute(AttributeName = "maintainIngredient")]
    public bool MaintainIngredient { get; set; }
}