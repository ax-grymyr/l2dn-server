using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlCubicBaseConditions
{
    [XmlElement("hp")]
    public XmlCubicConditionHp? Hp { get; set; }

    [XmlElement("range")]
    public XmlCubicConditionRange? Range { get; set; }
}