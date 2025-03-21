using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.BeautyShop;

public class XmlBeautyShopSex
{
    [XmlElement("hair")]
    public List<XmlBeautyShopHairItem> Hairs { get; set; } = [];

    [XmlElement("face")]
    public List<XmlBeautyShopItem> Faces { get; set; } = [];

    [XmlAttribute("type")]
    public Sex Sex { get; set; }
}