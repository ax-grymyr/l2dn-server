using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlOptionData
{
    [XmlElement("option")]
    public List<XmlOption> Options { get; set; } = [];
}