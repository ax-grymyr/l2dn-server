using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlClanHallTeleport
{
    [XmlAttribute("npcStringId")]
    public int NpcStringId { get; set; }

    [XmlAttribute("x")]
    public int X { get; set; }
    
    [XmlAttribute("y")]
    public int Y { get; set; }
    
    [XmlAttribute("z")]
    public int Z { get; set; }

    [XmlAttribute("minFunctionLevel")]
    public int MinFunctionLevel { get; set; }

    [XmlAttribute("cost")]
    public int Cost { get; set; }
}