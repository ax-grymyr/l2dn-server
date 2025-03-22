using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetExtracts;

public sealed class XmlPetExtractItem
{
    [XmlAttribute("petId")]
    public int PetId { get; set; }

    [XmlAttribute("petLevel")]
    public int PetLevel { get; set; }

    [XmlAttribute("extractExp")]
    public long ExtractExp { get; set; }

    [XmlAttribute("extractItem")]
    public int ExtractItem { get; set; }

    [XmlAttribute("defaultCostId")]
    public int DefaultCostId { get; set; }

    [XmlAttribute("defaultCostCount")]
    public long DefaultCostCount { get; set; }

    [XmlAttribute("extractCostId")]
    public int ExtractCostId { get; set; }

    [XmlAttribute("extractCostCount")]
    public long ExtractCostCount { get; set; }
}