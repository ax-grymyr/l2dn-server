using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlMapRegionBanned
{
    [XmlAttribute("race")]
    public Race Race { get; set; }
    
    [XmlAttribute("point")]
    public string Point { get; set; } = string.Empty;
}