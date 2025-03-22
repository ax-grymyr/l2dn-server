using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Subjugation;

public sealed class XmlSubjugationPurgeNpc
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("points")]
    public int Points { get; set; }
}