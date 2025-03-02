using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlParameterSkill: XmlParameter
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }
}