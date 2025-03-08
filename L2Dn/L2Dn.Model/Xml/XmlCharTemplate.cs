using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCharTemplate
{
    [XmlElement("classId")]
    public int ClassId { get; set; }

    [XmlElement("staticData")]
    public XmlCharTemplateStaticData? StaticData { get; set; }

    [XmlArray("lvlUpgainData")]
    [XmlArrayItem("level")]
    public List<XmlCharTemplateLevel> Levels { get; set; } = [];
}

public class XmlCharTemplateStaticData
{
    [XmlElement("baseINT")]
    public int BaseInt { get; set; }

    [XmlElement("baseSTR")]
    public int BaseStr { get; set; }

    [XmlElement("baseCON")]
    public int BaseCon { get; set; }

    [XmlElement("baseMEN")]
    public int BaseMen { get; set; }


    [XmlElement("baseDEX")]
    public int BaseDex { get; set; }

    [XmlElement("baseWIT")]
    public int BaseWit { get; set; }

    [XmlElement("physicalAbnormalResist")]
    public int PhysicalAbnormalResist { get; set; }

    [XmlElement("magicAbnormalResist")]
    public int MagicAbnormalResist { get; set; }

    [XmlArray("creationPoints")]
    [XmlArrayItem("node")]
    public List<XmlLocation> CreationPoints { get; set; } = [];

    [XmlElement("basePAtk")]
    public int BasePAtk { get; set; }

    [XmlElement("baseCritRate")]
    public int BaseCritRate { get; set; }

    [XmlElement("baseMCritRate")]
    public int BaseMCritRate { get; set; }

    [XmlElement("baseAtkType")]
    public XmlCharTemplateBaseAtkType BaseAtkType { get; set; }

    [XmlElement("basePAtkSpd")]
    public int BasePAtkSpd { get; set; }

    [XmlElement("baseMAtkSpd")]
    public int BaseMAtkSpd { get; set; }

    [XmlElement("basePDef")]
    public XmlCharTemplateBasePDef BasePDef { get; set; } = new();

    [XmlElement("baseMAtk")]
    public int BaseMAtk { get; set; }

    [XmlElement("baseMDef")]
    public XmlCharTemplateBaseMDef BaseMDef { get; set; } = new();

    [XmlElement("baseCanPenetrate")]
    public int BaseCanPenetrate { get; set; }

    [XmlElement("baseAtkRange")]
    public int BaseAtkRange { get; set; }

    [XmlElement("baseDamRange")]
    public XmlCharTemplateBaseDamRange BaseDamRange { get; set; } = new();

    [XmlElement("baseRndDam")]
    public int BaseRndDam { get; set; }

    [XmlElement("baseMoveSpd")]
    public XmlCharTemplateBaseMoveSpd BaseMoveSpd { get; set; } = new();

    [XmlElement("baseBreath")]
    public int BaseBreath { get; set; }

    [XmlElement("baseSafeFall")]
    public int BaseSafeFall { get; set; }

    [XmlElement("collisionMale")]
    public XmlCharTemplateCollision CollisionMale { get; set; } = new();

    [XmlElement("collisionFemale")]
    public XmlCharTemplateCollision CollisionFemale { get; set; } = new();
}

public enum XmlCharTemplateBaseAtkType
{
    FIST,
}

public class XmlCharTemplateBasePDef
{
    [XmlElement("chest")]
    public int Chest { get; set; }

    [XmlElement("legs")]
    public int Legs { get; set; }

    [XmlElement("head")]
    public int Head { get; set; }

    [XmlElement("feet")]
    public int Feet { get; set; }

    [XmlElement("gloves")]
    public int Gloves { get; set; }

    [XmlElement("underwear")]
    public int Underwear { get; set; }

    [XmlElement("cloak")]
    public int Cloak { get; set; }
}

public class XmlCharTemplateBaseMDef
{
    [XmlElement("rear")]
    public int RightEar { get; set; }


    [XmlElement("lear")]
    public int LeftEar { get; set; }


    [XmlElement("rfinger")]
    public int RightFinger { get; set; }


    [XmlElement("lfinger")]
    public int LeftFinger { get; set; }


    [XmlElement("neck")]
    public int Neck { get; set; }
}

public class XmlCharTemplateBaseDamRange
{
    [XmlElement("verticalDirection")]
    public int VerticalDirection { get; set; }

    [XmlElement("horizontalDirection")]
    public int HorizontalDirection { get; set; }

    [XmlElement("distance")]
    public int Distance { get; set; }

    [XmlElement("width")]
    public int Width { get; set; }
}

public class XmlCharTemplateBaseMoveSpd
{
    [XmlElement("walk")]
    public int Walk { get; set; }

    [XmlElement("run")]
    public int Run { get; set; }

    [XmlElement("slowSwim")]
    public int SlowSwim { get; set; }

    [XmlElement("fastSwim")]
    public int FastSwim { get; set; }
}

public class XmlCharTemplateCollision
{
    [XmlElement("radius")]
    public double Radius { get; set; }

    [XmlElement("height")]
    public double Height { get; set; }
}

public class XmlCharTemplateLevel
{
    [XmlElement("hp")]
    public double Hp { get; set; }

    [XmlElement("mp")]
    public double Mp { get; set; }

    [XmlElement("cp")]
    public double Cp { get; set; }

    [XmlElement("hpRegen")]
    public double HpRegen { get; set; }

    [XmlElement("mpRegen")]
    public double MpRegen { get; set; }

    [XmlElement("cpRegen")]
    public double CpRegen { get; set; }

    [XmlAttribute("val")]
    public int Level { get; set; }
}