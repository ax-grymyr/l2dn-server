using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlVariationOptionGroupCategory
{
    [XmlElement("option", Type = typeof(XmlVariationOptionGroupCategoryOption))]
    [XmlElement("optionRange", Type = typeof(XmlVariationOptionGroupCategoryOptionRange))]
    [XmlElement("item", typeof(XmlId))]
    [XmlElement("items", typeof(XmlIdRange))]
    public List<object> Items { get; set; } = [];

    [XmlAttribute("chance")]
    public float Chance { get; set; }

    [XmlIgnore]
    public bool ChanceSpecified { get; set; }
}