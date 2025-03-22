using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Subjugation;

public sealed class XmlSubjugationGachaItem
{
    [XmlAttribute("category")]
    public int Category { get; set; }

    [XmlElement("item")]
    public List<XmlSubjugationPurgeItem> Items { get; set; } = [];

}