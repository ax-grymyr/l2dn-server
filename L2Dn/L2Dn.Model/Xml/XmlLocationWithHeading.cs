using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlLocationWithHeading: XmlLocation
{
    [XmlAttribute("heading")]
    public int Heading { get; set; }

    [XmlIgnore]
    public bool HeadingSpecified { get; set; }
}