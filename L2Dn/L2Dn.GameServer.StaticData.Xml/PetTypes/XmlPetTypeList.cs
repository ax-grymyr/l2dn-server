using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetTypes;

[XmlRoot("list")]
public sealed class XmlPetTypeList
{
    [XmlElement("pet")]
    public List<XmlPetType> Pets { get; set; } = [];
}