using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEquipmentUpgrade
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("item")]
    public string Item { get; set; } = string.Empty;

    [XmlAttribute("materials")]
    public string Materials { get; set; } = string.Empty;

    [XmlAttribute("adena")]
    public long Adena { get; set; }

    [XmlAttribute("result")]
    public string Result { get; set; } = string.Empty;

    [XmlAttribute("announce")]
    public bool Announce { get; set; }
}