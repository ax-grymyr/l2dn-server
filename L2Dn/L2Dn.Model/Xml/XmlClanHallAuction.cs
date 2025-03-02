using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlClanHallAuction
{
    [XmlAttribute("minBid")]
    public long MinBid { get; set; }

    [XmlAttribute("lease")]
    public long Lease { get; set; }

    [XmlAttribute("deposit")]
    public long Deposit { get; set; }
}