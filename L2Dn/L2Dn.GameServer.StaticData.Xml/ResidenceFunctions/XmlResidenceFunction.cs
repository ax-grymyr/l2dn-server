using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.ResidenceFunctions;

public class XmlResidenceFunction
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("costId")]
    public int ItemId { get; set; }

    [XmlIgnore]
    public bool ItemIdSpecified { get; set; }

    [XmlAttribute("costCount")]
    public int ItemCount { get; set; }

    [XmlAttribute("duration")]
    public string Duration { get; set; } = string.Empty;

    [XmlAttribute("value")]
    public double Value { get; set; }

    [XmlIgnore]
    public bool ValueSpecified { get; set; }
}