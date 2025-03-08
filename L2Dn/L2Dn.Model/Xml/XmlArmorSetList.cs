using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlArmorSetList
{
    [XmlElement("set")]
    public List<XmlArmorSet> Sets { get; set; } = [];
}