using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Sayune;

[XmlRoot("list")]
public sealed class XmlSayuneData
{
    [XmlElement("map")]
    public List<XmlSayuneItem> Items { get; set; } = [];
}