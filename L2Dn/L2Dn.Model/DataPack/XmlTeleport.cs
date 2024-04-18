using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlTeleport
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlIgnore]
    public bool NameSpecified { get; set; }
    
    [XmlAttribute("type")]
    public TeleportType Type { get; set; }

    [XmlElement("location")]
    public List<XmlTeleportLocation> Locations { get; set; } = [];
}