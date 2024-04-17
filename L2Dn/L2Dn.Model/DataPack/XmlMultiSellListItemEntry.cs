using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlMultiSellListItemEntry
{
    [XmlAttribute("id")] 
    public int ItemId { get; set; } 

    [XmlAttribute("count")] 
    public long Count { get; set; }
    
    [XmlAttribute("enchantmentLevel")] 
    public byte EnchantLevel { get; set; }
}