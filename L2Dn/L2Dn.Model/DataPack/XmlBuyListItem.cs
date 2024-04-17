using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlBuyListItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("price")]
    public long Price { get; set; }

    [XmlAttribute("count")]
    public long Count { get; set; }

    [XmlIgnore]
    public bool CountSpecified { get; set; }

    [XmlAttribute("restock_delay")]
    public int RestockDelay { get; set; }

    [XmlIgnore]
    public bool RestockDelaySpecified { get; set; }

    [XmlAttribute("baseTax")]
    public int BaseTax { get; set; }
    
    [XmlIgnore]
    public bool BaseTaxSpecified { get; set; }
}