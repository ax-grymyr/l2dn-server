using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlIdRange
{
    [XmlAttribute("from")]
    public int From { get; set; }

    [XmlIgnore]
    public bool FromSpecified { get; set; }

    [XmlAttribute("to")]
    public int To { get; set; }

    [XmlIgnore]
    public bool ToSpecified { get; set; }
}