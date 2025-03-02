using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlVariationOptionGroupCategoryOptionRange: XmlIdRange 
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }

    [XmlIgnore]
    public bool ChanceSpecified { get; set; }
}