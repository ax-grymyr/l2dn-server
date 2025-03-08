using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlAdminCommands
{
    [XmlElement("admin")]
    public List<XmlAdminCommand> Commands { get; set; } = [];
}