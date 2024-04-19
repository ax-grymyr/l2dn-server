using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlInstanceCondition
{
    [XmlElement("param")]
    public List<XmlParameterString> Parameters { get; set; } = [];

    [XmlAttribute("type")]
    public XmlInstanceConditionType Type { get; set; }

    [XmlAttribute("onlyLeader")]
    public bool OnlyLeader { get; set; }

    [XmlAttribute("showMessageAndHtml")]
    public bool ShowMessageAndHtml { get; set; }
}