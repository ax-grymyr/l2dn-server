using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.AdminCommands;

[XmlRoot("list")]
public class XmlAdminCommandList
{
    [XmlElement("admin")]
    public List<XmlAdminCommand> Commands { get; set; } = [];
}