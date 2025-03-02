using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlArmorSetItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }
}