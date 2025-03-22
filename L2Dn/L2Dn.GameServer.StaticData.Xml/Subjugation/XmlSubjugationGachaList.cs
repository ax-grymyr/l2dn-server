using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Subjugation;

[XmlRoot("list")]
public sealed class XmlSubjugationGachaList
{
    [XmlElement("purge")]
    public List<XmlSubjugationGachaItem> Items { get; set; } = [];
}