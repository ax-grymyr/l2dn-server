using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlCubicConditionRange
{
    [XmlAttribute("value")]
    public int Value { get; set; }
}