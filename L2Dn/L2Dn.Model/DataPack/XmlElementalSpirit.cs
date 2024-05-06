using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlElementalSpirit
{
    [XmlElement("level")]
    public List<XmlElementalSpiritLevel> Levels { get; set; } = [];

    [XmlElement("itemToEvolve")]
    public List<XmlElementalSpiritItemToEvolve> ItemsToEvolve { get; set; } = [];

    [XmlElement("absorbItem")]
    public List<XmlElementalSpiritAbsorbItem> AbsorbItems { get; set; } = [];

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("type")]
    public byte Type { get; set; }

    [XmlAttribute("stage")]
    public byte Stage { get; set; }

    [XmlAttribute("npcId")]
    public int NpcId { get; set; }

    [XmlAttribute("extractItem")]
    public int ExtractItem { get; set; }

    [XmlAttribute("maxCharacteristics")]
    public int MaxCharacteristics { get; set; }
}