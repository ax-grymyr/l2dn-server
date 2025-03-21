using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Options;

[XmlRoot("list")]
public class XmlOptionData
{
    [XmlElement("option")]
    public List<XmlOption> Options { get; set; } = [];
}