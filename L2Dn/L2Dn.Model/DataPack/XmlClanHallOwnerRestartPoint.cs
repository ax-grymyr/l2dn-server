using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlClanHallOwnerRestartPoint 
{
    [XmlAttribute("x")]
    public int X { get; set; }
    
    [XmlAttribute("y")]
    public int Y { get; set; }
    
    [XmlAttribute("z")]
    public int Z { get; set; }
}