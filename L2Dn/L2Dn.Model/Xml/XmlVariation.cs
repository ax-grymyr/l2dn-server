using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlVariation
{
    [XmlElement("optionGroup")]
    public List<XmlVariationOptionGroup> OptionGroups { get; set; } = [];

    [XmlAttribute("mineralId")]
    public int MineralId { get; set; }

    [XmlIgnore]
    public bool MineralIdSpecified { get; set; }

    [XmlAttribute("itemGroup")]
    public int ItemGroup { get; set; }

    [XmlIgnore]
    public bool ItemGroupSpecified { get; set; }
}