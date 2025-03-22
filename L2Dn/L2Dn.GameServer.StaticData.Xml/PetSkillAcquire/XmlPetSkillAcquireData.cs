using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetSkillAcquire;

[XmlRoot("list")]
public sealed class XmlPetSkillAcquireData
{
    [XmlElement("pet")]
    public List<XmlPetSkillAcquirePet> Pets { get; set; } = [];
}