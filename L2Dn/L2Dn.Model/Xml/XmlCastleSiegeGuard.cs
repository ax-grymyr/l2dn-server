using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlCastleSiegeGuard
{
    [XmlAttribute("itemId")]
    public int ItemId { get; set; }

    [XmlAttribute("type")]
    public SiegeGuardType Type { get; set; }

    [XmlAttribute("stationary")]
    public bool Stationary { get; set; }

    [XmlAttribute("npcId")]
    public int NpcId { get; set; }

    [XmlAttribute("npcMaxAmount")]
    public int NpcMaxAmount { get; set; }
}