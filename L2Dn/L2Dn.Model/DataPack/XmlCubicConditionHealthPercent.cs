using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlCubicConditionHealthPercent 
{
    [XmlAttribute("min")]
    public int Min { get; set; }

    [XmlAttribute("max")]
    public int Max { get; set; }
}