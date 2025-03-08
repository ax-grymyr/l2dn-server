using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCharacterClassList
{
    [XmlElement("class")]
    public List<XmlCharacterClass> Classes { get; set; } = [];
}