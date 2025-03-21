using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Options;

public class XmlOptionChanceSkill: XmlOptionSkill
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }
}