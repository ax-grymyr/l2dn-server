using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlBeautyShopData: XmlBase
{
    [XmlElement("race")]
    public List<XmlBeautyShopRaceData> Races { get; set; } = [];
}