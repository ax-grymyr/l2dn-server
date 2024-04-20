using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlOptionSkill 
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }
}