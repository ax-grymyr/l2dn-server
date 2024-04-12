using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlMultiSellListItemEntry
{
    [XmlAttribute(AttributeName="id")] 
    public int ItemId { get; set; } 

    [XmlAttribute(AttributeName="count")] 
    public long Count { get; set; }
    
    [XmlAttribute(AttributeName="enchantmentLevel")] 
    public byte EnchantLevel { get; set; }
}