using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public enum XmlItemConditionType
{
    [XmlEnum("using")]
    Using,

    [XmlEnum("and")]
    And,

    [XmlEnum("or")]
    Or,

    [XmlEnum("not")]
    Not,

    [XmlEnum("player")]
    Player,

    [XmlEnum("target")]
    Target,

    [XmlEnum("game")]
    Game,
}