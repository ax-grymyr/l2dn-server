using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public class XmlItemConditions
{
    [XmlElement("using", typeof(XmlItemConditionUsing))]
    [XmlElement("and", typeof(XmlItemConditionAnd))]
    [XmlElement("or", typeof(XmlItemConditionOr))]
    [XmlElement("not", typeof(XmlItemConditionNot))]
    [XmlElement("player", typeof(XmlItemConditionPlayer))]
    [XmlElement("target", typeof(XmlItemConditionTarget))]
    [XmlElement("game", typeof(XmlItemConditionGame))]
    [XmlChoiceIdentifier(nameof(ConditionTypes))]
    public object[]? Conditions { get; set; }

    public XmlItemConditionType[]? ConditionTypes { get; set; }
}