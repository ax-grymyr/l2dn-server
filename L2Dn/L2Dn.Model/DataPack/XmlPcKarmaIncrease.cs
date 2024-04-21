using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlPcKarmaIncrease
{
    [XmlAttribute("lvl")]
    public int Level { get; set; }

    [XmlAttribute("val")]
    public double Value { get; set; }
}