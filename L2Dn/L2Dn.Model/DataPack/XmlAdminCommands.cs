using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlAdminCommands: XmlBase
{
    [XmlElement("admin")]
    public List<XmlAdminCommand> Commands { get; set; } = [];
}