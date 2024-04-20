using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlArmorSetItem 
{
    [XmlAttribute("id")]
    public int Id { get; set; }
}