using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PcKarmaIncrease;

public class XmlPcKarmaIncrease
{
    [XmlAttribute("lvl")]
    public int Level { get; set; }

    [XmlAttribute("val")]
    public double Value { get; set; }
}