using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlVariationOptionGroup
{
    [XmlElement("optionCategory")]
    public List<XmlVariationOptionGroupCategory> OptionCategories { get; set; } = [];

    [XmlAttribute("order")]
    public int Order { get; set; }

    [XmlIgnore]
    public bool OrderSpecified { get; set; }
}