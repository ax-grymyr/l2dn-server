using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnchantChallengePointFeeOption
{
    [XmlAttribute("index")]
    public int Index { get; set; }

    [XmlAttribute("fee")]
    public int Fee { get; set; }
}