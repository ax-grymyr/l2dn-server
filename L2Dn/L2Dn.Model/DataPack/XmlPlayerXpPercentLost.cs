using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlPlayerXpPercentLost
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("val")]
    public double Value { get; set; }
}