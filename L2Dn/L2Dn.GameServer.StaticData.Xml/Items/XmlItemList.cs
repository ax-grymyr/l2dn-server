using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

[XmlRoot("list")]
public sealed class XmlItemList: IXmlRoot
{
    [XmlElement("item")]
    public List<XmlItem> Items { get; set; } = [];

    [XmlIgnore]
    public string FilePath { get; set; } = string.Empty;
}