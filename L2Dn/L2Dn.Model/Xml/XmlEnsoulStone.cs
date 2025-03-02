using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnsoulStone
{
    [XmlElement("option")]
    public List<XmlEnsoulStoneOption> Options { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("slotType")]
    public int SlotType { get; set; }
}