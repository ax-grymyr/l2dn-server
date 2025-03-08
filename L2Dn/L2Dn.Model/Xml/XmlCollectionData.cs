using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCollectionData
{
    [XmlElement("collection")]
    public List<XmlCollection> Collections { get; set; } = [];
}