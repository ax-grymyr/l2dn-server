using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlBuyList: XmlBase
{
    [XmlAttribute("baseTax")]
    public int BaseTax { get; set; }

    [XmlIgnore]
    public bool BaseTaxSpecified { get; set; }

    [XmlArray("npcs")]
    [XmlArrayItem("npc")]
    public List<int> Npcs { get; set; } = [];

    [XmlElement("item")]
    public List<XmlBuyListItem> Items { get; set; } = [];
}