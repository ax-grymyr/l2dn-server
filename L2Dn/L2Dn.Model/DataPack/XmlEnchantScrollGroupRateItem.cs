using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnchantScrollGroupRateItem
{
    [XmlAttribute("slot")]
    public string Slot { get; set; } = string.Empty;

    [XmlAttribute("magicWeapon")]
    public bool MagicWeapon { get; set; }

    [XmlAttribute("itemId")]
    public int ItemId { get; set; }
}