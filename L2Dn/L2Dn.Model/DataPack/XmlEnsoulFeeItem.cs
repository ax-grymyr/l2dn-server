using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnsoulFeeItem
{
    [XmlAttribute("itemId")]
    public int ItemId { get; set; }

    [XmlAttribute("count")]
    public long Count { get; set; }
}