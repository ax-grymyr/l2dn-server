using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Subjugation;

[XmlRoot("list")]
public sealed class XmlSubjugationDataList
{
    [XmlElement("purge")]
    public List<XmlSubjugationDataItem> Items { get; set; } = [];
}