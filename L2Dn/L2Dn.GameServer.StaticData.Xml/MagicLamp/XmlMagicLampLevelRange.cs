using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.MagicLamp;

public class XmlMagicLampLevelRange
{
    [XmlElement("lamp")]
    public List<XmlMagicLampLevelRangeLamp> Lamps { get; set; } = [];

    [XmlAttribute("fromLevel")]
    public int FromLevel { get; set; }

    [XmlAttribute("toLevel")]
    public int ToLevel { get; set; }
}