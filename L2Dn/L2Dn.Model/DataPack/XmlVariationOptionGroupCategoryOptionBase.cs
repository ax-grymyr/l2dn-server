using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public abstract class XmlVariationOptionGroupCategoryOptionBase
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }

    [XmlIgnore]
    public bool ChanceSpecified { get; set; }
}