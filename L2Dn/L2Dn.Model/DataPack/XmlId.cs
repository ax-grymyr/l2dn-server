using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlId
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlIgnore]
    public bool IdSpecified { get; set; }
}