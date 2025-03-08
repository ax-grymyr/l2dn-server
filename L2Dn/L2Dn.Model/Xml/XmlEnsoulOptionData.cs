using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlEnsoulOptionData
{
    [XmlElement("option")]
    public List<XmlEnsoulOption> Options { get; set; } = [];
}