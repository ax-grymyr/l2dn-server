using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Categories;

[XmlRoot("list")]
public class XmlCategoryList
{
    [XmlElement("category")]
    public List<XmlCategory> Categories { get; set; } = [];
}