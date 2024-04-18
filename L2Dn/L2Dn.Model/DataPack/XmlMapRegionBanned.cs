using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlMapRegionBanned
{
    [XmlAttribute("race")]
    public string Race { get; set; } = string.Empty;
    
    [XmlAttribute("point")]
    public string Point { get; set; } = string.Empty;
}