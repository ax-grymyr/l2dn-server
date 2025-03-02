using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlCubicSkillConditions: XmlCubicBaseConditions
{
    [XmlElement("healthPercent")]
    public XmlCubicConditionHealthPercent? HealthPercent { get; set; }
}