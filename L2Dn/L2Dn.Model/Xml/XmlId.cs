using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlId
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlIgnore]
    public bool IdSpecified { get; set; }
}