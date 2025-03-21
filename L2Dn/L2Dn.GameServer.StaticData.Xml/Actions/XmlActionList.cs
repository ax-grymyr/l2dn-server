using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Actions;

[XmlRoot("list")]
public class XmlActionList
{
    [XmlElement("action")]
    public List<XmlAction> Actions { get; set; } = [];
}