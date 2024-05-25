using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlEquipmentUpgradeData: XmlBase
{
    [XmlElement("upgrade")]
    public List<XmlEquipmentUpgrade> Upgrades { get; set; } = [];
}