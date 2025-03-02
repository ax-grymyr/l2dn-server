using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlInstanceTime
{
    [XmlAttribute("duration")]
    public int DurationMinutes { get; set; }


    [XmlAttribute("empty")]
    public int EmptyCloseMinutes { get; set; }


    [XmlAttribute("eject")]
    public int EjectMinutes { get; set; }
}