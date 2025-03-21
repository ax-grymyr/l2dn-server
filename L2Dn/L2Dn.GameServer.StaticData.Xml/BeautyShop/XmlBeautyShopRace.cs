using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.BeautyShop;

public class XmlBeautyShopRace
{
    [XmlElement("sex")]
    public List<XmlBeautyShopSex> SexData { get; set; } = [];

    [XmlAttribute("type")]
    public Race Race { get; set; }
}