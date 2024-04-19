using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlInstanceRates 
{
    [XmlAttribute("exp")]
    public float Exp { get; set; }

    [XmlIgnore]
    public bool ExpSpecified { get; set; }

    [XmlAttribute("sp")]
    public float Sp { get; set; }

    [XmlIgnore]
    public bool SpSpecified { get; set; }

    [XmlAttribute("partyExp")]
    public float PartyExp { get; set; }

    [XmlIgnore]
    public bool PartyExpSpecified { get; set; }

    [XmlAttribute("partySp")]
    public float PartySp { get; set; }

    [XmlIgnore]
    public bool PartySpSpecified { get; set; }
}