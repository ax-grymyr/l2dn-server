using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCategoryData
{
    [XmlElement("category")]
    public List<XmlCategory> Categories { get; set; } = [];
}