using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlDoorOpenStatus
{
    [XmlAttribute("openMethod")]
    public DoorOpenType OpenMethod { get; set; }

    [XmlAttribute("default")]
    public XmlDoorDefaultOpenStatus Default { get; set; }

    [XmlAttribute("closeTime")]
    public int CloseTime { get; set; } = -1;

    [XmlIgnore]
    public bool CloseTimeSpecified { get; set; }

    [XmlAttribute("openTime")]
    public int OpenTime { get; set; }

    [XmlAttribute("randomTime")]
    public int RandomTime { get; set; }

    [XmlIgnore]
    public bool RandomTimeSpecified { get; set; }
}