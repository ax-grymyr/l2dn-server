using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlCubicConditionHp
{
    [XmlAttribute("type")]
    public CubicHpConditionType Type { get; set; }

    [XmlAttribute("percent")]
    public int Percent { get; set; }
}