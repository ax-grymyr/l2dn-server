using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlTeleport
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlIgnore]
    public bool NameSpecified { get; set; }
    
    [XmlAttribute("type")]
    public string Type { get; set; } = string.Empty;

    [XmlElement("location")]
    public List<XmlTeleportLocation> Locations { get; set; } = [];
}