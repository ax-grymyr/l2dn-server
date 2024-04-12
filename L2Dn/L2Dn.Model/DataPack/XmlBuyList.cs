using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot(ElementName="list")]
public class XmlBuyList: XmlBaseTaxHolder
{
    [XmlArray(ElementName = "npcs")]
    [XmlArrayItem(ElementName = "npc")] 
    public List<int> Npcs { get; set; } = [];

    [XmlElement(ElementName = "item")]
    public List<XmlBuyListItem> Items { get; set; } = [];
}