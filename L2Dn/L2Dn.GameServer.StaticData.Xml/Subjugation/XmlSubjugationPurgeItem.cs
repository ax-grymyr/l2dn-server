using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Subjugation;

public class XmlSubjugationPurgeItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("rate")]
    public double Rate { get; set; }
}