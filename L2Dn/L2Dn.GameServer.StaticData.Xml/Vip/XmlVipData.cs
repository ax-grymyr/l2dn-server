using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Vip;

[XmlRoot("list")]
public sealed class XmlVipData
{
    [XmlElement("vip")]
    public List<XmlVipTierInfo> Tiers { get; set; } = [];
}