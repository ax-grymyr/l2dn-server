using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlCastleSpawn 
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("castleSide")]
    public CastleSide CastleSide { get; set; } = CastleSide.NEUTRAL;

    [XmlAttribute("x")]
    public int X { get; set; }
    
    [XmlAttribute("y")]
    public int Y { get; set; }
    
    [XmlAttribute("z")]
    public int Z { get; set; }
    
    [XmlAttribute("heading")]
    public int Heading { get; set; }
    
    [XmlIgnore]
    public bool HeadingSpecified { get; set; }
}