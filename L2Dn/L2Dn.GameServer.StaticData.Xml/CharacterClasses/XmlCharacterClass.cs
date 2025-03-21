using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.CharacterClasses;

public sealed class XmlCharacterClass
{
    [XmlAttribute("classId")]
    public int ClassId { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("parentClassId")]
    public int ParentClassId { get; set; }

    [XmlIgnore]
    public bool ParentClassIdSpecified { get; set; }
}