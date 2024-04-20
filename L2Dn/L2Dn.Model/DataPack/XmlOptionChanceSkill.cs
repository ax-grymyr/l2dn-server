using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlOptionChanceSkill: XmlOptionSkill 
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }
}