using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Subjugation;

public sealed class XmlSubjugationDataItem
{
    [XmlElement("npc")]
    public List<XmlSubjugationPurgeNpc> Npcs { get; set; } = [];

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("category")]
    public int Category { get; set; }

    [XmlAttribute("hottimes")]
    public string HotTimes { get; set; } = string.Empty;
}