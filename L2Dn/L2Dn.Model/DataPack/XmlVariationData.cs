using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlVariationData: XmlBase
{
    [XmlArray("variations")]
    [XmlArrayItem("variation")]
    public List<XmlVariation> Variations { get; set; } = [];

    [XmlArray("itemGroups")]
    [XmlArrayItem("itemGroup")]
    public List<XmlVariationItemGroup> ItemGroups { get; set; } = [];

    [XmlArray("fees")]
    [XmlArrayItem("fee")]
    public List<XmlVariationFee> Fees { get; set; } = [];
}