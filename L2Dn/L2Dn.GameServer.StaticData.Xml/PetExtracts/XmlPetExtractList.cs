using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetExtracts;

[XmlRoot("list")]
public sealed class XmlPetExtractList
{
    [XmlElement("extraction")]
    public List<XmlPetExtractItem> Items { get; set; } = [];
}