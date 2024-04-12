using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlMultiSellListProduct: XmlMultiSellListItemEntry
{
    [XmlAttribute(AttributeName="chance")] 
    public double Chance { get; set; } = 100.0;
    
    [XmlIgnore]
    public bool ChanceSpecified { get; set; }
}