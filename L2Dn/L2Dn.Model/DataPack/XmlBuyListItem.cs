using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlBuyListItem: XmlBaseTaxHolder
{
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlAttribute(AttributeName = "price")]
    public long Price { get; set; }

    [XmlAttribute(AttributeName = "count")]
    public long Count { get; set; }

    [XmlIgnore]
    public bool CountSpecified { get; set; }

    [XmlAttribute(AttributeName = "restock_delay")]
    public int RestockDelay { get; set; }

    [XmlIgnore]
    public bool RestockDelaySpecified { get; set; }
}