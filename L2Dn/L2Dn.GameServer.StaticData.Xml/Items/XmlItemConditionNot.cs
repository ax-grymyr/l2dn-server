using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemConditionNot
{
    [XmlElement("using", typeof(XmlItemConditionUsing))]
    [XmlElement("and", typeof(XmlItemConditionAnd))]
    [XmlElement("or", typeof(XmlItemConditionOr))]
    [XmlElement("not", typeof(XmlItemConditionNot))]
    [XmlElement("player", typeof(XmlItemConditionPlayer))]
    [XmlElement("target", typeof(XmlItemConditionTarget))]
    [XmlElement("game", typeof(XmlItemConditionGame))]
    [XmlChoiceIdentifier(nameof(ConditionType))]
    public object? Condition { get; set; }

    public XmlItemConditionType ConditionType { get; set; }
}