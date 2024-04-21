using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlCharacterClassList: XmlBase
{
    [XmlElement("class")]
    public List<XmlCharacterClass> Classes { get; set; } = [];
}