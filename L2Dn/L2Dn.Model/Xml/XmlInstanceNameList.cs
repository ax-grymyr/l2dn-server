using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlInstanceNameList
{
    [XmlElement("instance")]
    public List<XmlInstanceName> Instances { get; set; } = [];
}