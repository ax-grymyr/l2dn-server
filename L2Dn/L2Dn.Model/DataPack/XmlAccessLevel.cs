using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlAccessLevel
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("nameColor")]
    public string NameColor { get; set; } = "FFFFFF"; // White color

    [XmlAttribute("titleColor")]
    public string TitleColor { get; set; } = "FFFFFF"; // White color

    [XmlAttribute("isGM")]
    public bool IsGm { get; set; }

    [XmlAttribute("allowPeaceAttack")]
    public bool AllowPeaceAttack { get; set; }

    [XmlAttribute("allowFixedRes")]
    public bool AllowFixedRes { get; set; }

    [XmlAttribute("allowTransaction")]
    public bool AllowTransaction { get; set; } = true;

    [XmlAttribute("allowAltg")]
    public bool AllowAltG { get; set; }

    [XmlAttribute("giveDamage")]
    public bool GiveDamage { get; set; } = true;

    [XmlAttribute("takeAggro")]
    public bool TakeAggro { get; set; } = true;

    [XmlAttribute("gainExp")]
    public bool GainExp { get; set; } = true;

    [XmlAttribute("childAccess")]
    public int ChildAccess { get; set; }
}