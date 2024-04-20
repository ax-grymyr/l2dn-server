using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlVariationFee
{
    [XmlElement("mineral", typeof(XmlId))]
    [XmlElement("mineralRange", typeof(XmlIdRange))]
    public List<object> Minerals { get; set; } = [];

    [XmlAttribute("itemGroup")]
    public int ItemGroup { get; set; }

    [XmlIgnore]
    public bool ItemGroupSpecified { get; set; }

    [XmlAttribute("itemId")]
    public int ItemId { get; set; }

    [XmlIgnore]
    public bool ItemIdSpecified { get; set; }

    [XmlAttribute("itemCount")]
    public long ItemCount { get; set; }

    [XmlIgnore]
    public bool ItemCountSpecified { get; set; }

    [XmlAttribute("adenaFee")]
    public long AdenaFee { get; set; }

    [XmlIgnore]
    public bool AdenaFeeSpecified { get; set; }

    [XmlAttribute("cancelFee")]
    public long CancelFee { get; set; }

    [XmlIgnore]
    public bool CancelFeeSpecified { get; set; }
}