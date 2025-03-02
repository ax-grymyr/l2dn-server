using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlInstanceMisc
{
    [XmlAttribute("allowPlayerSummon")]
    public bool AllowPlayerSummon { get; set; }

    [XmlAttribute("isPvP")]
    public bool IsPvP { get; set; }
}