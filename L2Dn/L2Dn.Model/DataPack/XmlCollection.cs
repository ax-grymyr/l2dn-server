using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlCollection
{
    [XmlElement("item")]
    public List<XmlCollectionItem> Items { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("optionId")]
    public int OptionId { get; set; }

    [XmlAttribute("category")]
    public int Category { get; set; }

    [XmlAttribute("completeCount")]
    public int CompleteCount { get; set; }
}