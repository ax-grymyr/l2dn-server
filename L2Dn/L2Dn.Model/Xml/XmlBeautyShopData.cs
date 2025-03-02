using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlBeautyShopData: XmlBase
{
    [XmlElement("race")]
    public List<XmlBeautyShopRaceData> Races { get; set; } = [];
}