using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.BeautyShop;

[XmlRoot("list")]
public class XmlBeautyShopList
{
    [XmlElement("race")]
    public List<XmlBeautyShopRace> Races { get; set; } = [];
}