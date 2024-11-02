using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlBeautyShopSexData
{
    [XmlElement("hair")]
    public List<XmlBeautyShopHairData> Hairs { get; set; } = [];

    [XmlElement("face")]
    public List<XmlBeautyShopItem> Faces { get; set; } = [];

    [XmlAttribute("type")]
    public string Type { get; set; } = string.Empty;
}