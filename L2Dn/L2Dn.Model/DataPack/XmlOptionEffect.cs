using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlOptionEffect 
{
    [XmlElement("amount")]
    public float Amount { get; set; }

    [XmlIgnore]
    public bool AmountSpecified { get; set; }

    [XmlElement("mode")]
    public string Mode { get; set; } = string.Empty;

    [XmlElement("attribute")]
    public string Attribute { get; set; } = string.Empty;

    [XmlElement("magicType")]
    public int MagicType { get; set; }

    [XmlIgnore]
    public bool MagicTypeSpecified { get; set; }

    [XmlElement("stat")]
    public string Stat { get; set; } = string.Empty;

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}