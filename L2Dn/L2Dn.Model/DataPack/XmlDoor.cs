using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlDoor 
{
    [XmlElement("nodes")]
    public XmlDoorNodes? Nodes { get; set; }

    [XmlElement("location")]
    public XmlDoorLocation? Location { get; set; }

    [XmlElement("stats")]
    public XmlDoorStats? Stats { get; set; }

    [XmlElement("status")]
    public XmlDoorStatus? Status { get; set; }

    [XmlElement("openStatus")]
    public XmlDoorOpenStatus? OpenStatus { get; set; }

    [XmlElement("event")]
    public XmlDoorEvent? Event { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("emmiterId")]
    public int EmmiterId { get; set; }

    [XmlAttribute("isInverted")]
    public bool IsInverted { get; set; }

    [XmlAttribute("group")]
    public string Group { get; set; } = string.Empty;

    [XmlAttribute("childId")]
    public int ChildId { get; set; }

    [XmlIgnore]
    public bool ChildIdSpecified { get; set; }
}