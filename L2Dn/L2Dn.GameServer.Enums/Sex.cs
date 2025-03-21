using System.Xml.Serialization;

namespace L2Dn.GameServer.Enums;

public enum Sex: byte
{
    [XmlEnum("MALE")]
    Male,

    [XmlEnum("FEMALE")]
    Female,

    Etc,
}