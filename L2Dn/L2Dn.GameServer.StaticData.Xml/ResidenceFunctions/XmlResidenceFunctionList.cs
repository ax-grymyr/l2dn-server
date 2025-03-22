using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.ResidenceFunctions;

[XmlRoot("list")]
public sealed class XmlResidenceFunctionList
{
    [XmlElement("function")]
    public List<XmlResidenceFunctionItem> Items { get; set; } = [];
}