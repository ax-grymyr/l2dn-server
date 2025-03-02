using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlBase
{
    [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string Xsi { get; set; } = "http://www.w3.org/2001/XMLSchema-instance";

    [XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string NoNamespaceSchemaLocation { get; set; } = string.Empty;
}