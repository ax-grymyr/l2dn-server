using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlBeautyShopHairData: XmlBeautyShopItem
{
    [XmlElement("color")]
    public List<XmlBeautyShopItem> Colors { get; set; } = [];
}