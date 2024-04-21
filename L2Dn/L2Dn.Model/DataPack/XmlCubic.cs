using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlCubic 
{
    [XmlElement("conditions")]
    public XmlCubicBaseConditions? Conditions { get; set; }

    [XmlArray("skills")]
    [XmlArrayItem("skill")]
    public List<XmlCubicSkill> Skills { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("slot")]
    public int Slot { get; set; }

    [XmlAttribute("duration")]
    public int Duration { get; set; }

    [XmlAttribute("delay")]
    public int Delay { get; set; }

    [XmlAttribute("maxCount")]
    public int MaxCount { get; set; }

    [XmlAttribute("useUp")]
    public int UseUp { get; set; }

    [XmlAttribute("power")]
    public double Power { get; set; }

    [XmlAttribute("targetType")]
    public CubicTargetType TargetType { get; set; }
}