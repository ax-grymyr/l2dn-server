using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlExperience
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("tolevel")]
    public long ToLevel { get; set; }

    [XmlAttribute("trainingRate")]
    public double TrainingRate { get; set; }
}