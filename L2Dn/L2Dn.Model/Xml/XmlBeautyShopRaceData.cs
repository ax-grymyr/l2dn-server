using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlBeautyShopRaceData
{
    [XmlElement("sex")]
    public List<XmlBeautyShopSexData> SexData { get; set; } = [];

    [XmlAttribute("type")]
    public string Type { get; set; } = string.Empty;
}