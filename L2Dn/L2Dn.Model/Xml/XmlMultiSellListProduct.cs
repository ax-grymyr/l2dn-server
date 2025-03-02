using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlMultiSellListProduct: XmlMultiSellListItemEntry
{
    [XmlAttribute("chance")]
    public double Chance { get; set; } = 100.0;

    [XmlIgnore]
    public bool ChanceSpecified { get; set; }
}