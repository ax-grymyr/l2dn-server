using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.CharacterClasses;

[XmlRoot("list")]
public sealed class XmlCharacterClassList
{
    [XmlElement("class")]
    public List<XmlCharacterClass> Classes { get; set; } = [];
}