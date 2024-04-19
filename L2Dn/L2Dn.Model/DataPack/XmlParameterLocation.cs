using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlParameterLocation: XmlParameter
{
    [XmlAttribute("x")]
    public int X { get; set; }

    [XmlAttribute("y")]
    public int Y { get; set; }

    [XmlAttribute("z")]
    public int Z { get; set; }

    [XmlAttribute("heading")]
    public int Heading { get; set; }
}