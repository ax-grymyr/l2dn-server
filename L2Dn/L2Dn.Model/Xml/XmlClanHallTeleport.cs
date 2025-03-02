using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlClanHallTeleport: XmlLocation
{
    [XmlAttribute("npcStringId")]
    public int NpcStringId { get; set; }

    [XmlAttribute("minFunctionLevel")]
    public int MinFunctionLevel { get; set; }

    [XmlAttribute("cost")]
    public int Cost { get; set; }
}