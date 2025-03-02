using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCharacterClassList: XmlBase
{
    [XmlElement("class")]
    public List<XmlCharacterClass> Classes { get; set; } = [];
}