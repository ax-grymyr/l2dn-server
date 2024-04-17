using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlZoneNode
{
    [XmlAttribute("X")]
    public int X { get; set; }
    
    [XmlAttribute("Y")]
    public int Y { get; set; }
}