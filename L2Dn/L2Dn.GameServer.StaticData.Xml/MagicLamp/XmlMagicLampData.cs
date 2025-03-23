using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.MagicLamp;

[XmlRoot("list")]
public sealed class XmlMagicLampData
{
    [XmlElement("levelRange")]
    public List<XmlMagicLampLevelRange> LevelRanges { get; set; } = [];
}