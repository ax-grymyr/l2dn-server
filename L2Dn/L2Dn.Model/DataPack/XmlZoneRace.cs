using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlZoneRace
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("point")]
    public string Point { get; set; } = string.Empty;
}