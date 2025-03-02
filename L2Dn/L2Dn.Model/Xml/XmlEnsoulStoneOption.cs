using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnsoulStoneOption
{
    [XmlAttribute("id")]
    public int Id { get; set; }
}