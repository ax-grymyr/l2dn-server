using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemConditionList: XmlItemConditions
{
    [XmlAttribute("msgId")]
    public int MessageId { get; set; }

    [XmlIgnore]
    public bool MessageIdSpecified { get; set; }

    [XmlAttribute("addName")]
    public int AddName { get; set; }

    [XmlIgnore]
    public bool AddNameSpecified { get; set; }

    [XmlAttribute("msg")]
    public string Message { get; set; } = string.Empty;

    [XmlIgnore]
    public bool MessageSpecified { get; set; }
}