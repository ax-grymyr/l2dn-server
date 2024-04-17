using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlZoneSpawn
{
    [XmlAttribute("X")]
    public int X { get; set; }
    
    [XmlAttribute("Y")]
    public int Y { get; set; }
    
    [XmlAttribute("Z")]
    public int Z { get; set; }

    [XmlAttribute("type")]
    public string Type { get; set; } = string.Empty;
}