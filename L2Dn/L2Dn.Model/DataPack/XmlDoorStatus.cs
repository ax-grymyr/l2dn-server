using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlDoorStatus
{
    [XmlAttribute("targetable")]
    public bool Targetable { get; set; } = true;

    [XmlIgnore]
    public bool TargetableSpecified { get; set; }

    [XmlAttribute("attackable")]
    public bool Attackable { get; set; }

    [XmlAttribute("showHp")]
    public bool ShowHp { get; set; } = true;

    [XmlIgnore]
    public bool ShowHpSpecified { get; set; }

    [XmlAttribute("isWall")]
    public bool IsWall { get; set; }

    [XmlAttribute("isStealth")]
    public bool IsStealth { get; set; }

    [XmlAttribute("isCheckCollision")]
    public bool IsCheckCollision { get; set; } = true;

    [XmlIgnore]
    public bool IsCheckCollisionSpecified { get; set; }
}