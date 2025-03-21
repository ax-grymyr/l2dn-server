using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.BeautyShop;

public class XmlBeautyShopHairItem: XmlBeautyShopItem
{
    [XmlElement("color")]
    public List<XmlBeautyShopItem> Colors { get; set; } = [];
}