using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.MagicLamp;

public class XmlMagicLampLevelRangeLamp
{
    [XmlAttribute("type")]
    public LampType Type { get; set; }


    [XmlAttribute("exp")]
    public long Exp { get; set; }


    [XmlAttribute("sp")]
    public long Sp { get; set; }


    [XmlAttribute("chance")]
    public double Chance { get; set; }
}