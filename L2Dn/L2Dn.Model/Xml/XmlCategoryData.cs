using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCategoryData: XmlBase
{
    [XmlElement("category")]
    public List<XmlCategory> Categories { get; set; } = [];
}