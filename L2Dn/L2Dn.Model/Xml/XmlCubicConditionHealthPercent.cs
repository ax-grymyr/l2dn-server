using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlCubicConditionHealthPercent
{
    [XmlAttribute("min")]
    public int Min { get; set; }

    [XmlAttribute("max")]
    public int Max { get; set; }
}