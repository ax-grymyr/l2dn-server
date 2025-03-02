using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlClanLevel
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("exp")]
    public int Exp { get; set; }
}