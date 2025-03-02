using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlActionData: XmlBase
{
    [XmlElement("action")]
    public List<XmlAction> Actions { get; set; } = [];
}