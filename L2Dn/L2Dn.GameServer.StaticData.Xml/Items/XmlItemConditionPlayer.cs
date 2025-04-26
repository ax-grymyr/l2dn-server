using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemConditionPlayer
{
    [XmlAttribute("castle")]
    public int Castle { get; set; }

    [XmlIgnore]
    public bool CastleSpecified { get; set; }

    [XmlAttribute("isOnSide")]
    public CastleSide CastleSide { get; set; }

    [XmlIgnore]
    public bool CastleSideSpecified { get; set; }

    [XmlAttribute("clanHall")]
    public string? ClanHall { get; set; }

    [XmlAttribute("class_id_restriction")]
    public string? ClassIdRestriction { get; set; }

    [XmlAttribute("cloakStatus")]
    public bool CloakStatus { get; set; }

    [XmlIgnore]
    public bool CloakStatusSpecified { get; set; }

    [XmlAttribute("hasSummon")]
    public bool HasSummon { get; set; }

    [XmlIgnore]
    public bool HasSummonSpecified { get; set; }

    [XmlAttribute("isHero")]
    public bool IsHero { get; set; }

    [XmlIgnore]
    public bool IsHeroSpecified { get; set; }

    [XmlAttribute("isPvpFlagged")]
    public bool IsPvpFlagged { get; set; }

    [XmlIgnore]
    public bool IsPvpFlaggedSpecified { get; set; }

    [XmlAttribute("insideZoneId")]
    public string? InsideZoneId { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlIgnore]
    public bool LevelSpecified { get; set; }

    [XmlAttribute("pledgeClass")]
    public int PledgeClass { get; set; }

    [XmlIgnore]
    public bool PledgeClassSpecified { get; set; }

    [XmlAttribute("levelRange")]
    public string? LevelRange { get; set; }

    [XmlAttribute("races")]
    public string? Races { get; set; }

    [XmlAttribute("sex")]
    public int Sex { get; set; }

    [XmlIgnore]
    public bool SexSpecified { get; set; }

    [XmlAttribute("fort")]
    public int Fort { get; set; }

    [XmlIgnore]
    public bool FortSpecified { get; set; }

    [XmlAttribute("chaotic")]
    public bool Chaotic { get; set; }

    [XmlIgnore]
    public bool ChaoticSpecified { get; set; }

    [XmlAttribute("subclass")]
    public bool Subclass { get; set; }

    [XmlIgnore]
    public bool SubclassSpecified { get; set; }

    [XmlAttribute("dualclass")]
    public bool DualClass { get; set; }

    [XmlIgnore]
    public bool DualClassSpecified { get; set; }

    [XmlAttribute("SiegeZone")]
    public int SiegeZone { get; set; }

    [XmlIgnore]
    public bool SiegeZoneSpecified { get; set; }

    [XmlAttribute("flyMounted")]
    public bool FlyMounted { get; set; }

    [XmlIgnore]
    public bool FlyMountedSpecified { get; set; }

    [XmlAttribute("instanceId")]
    public string? InstanceId { get; set; }

    [XmlAttribute("categoryType")]
    public string? CategoryType { get; set; }

    [XmlAttribute("pkCount")]
    public int PkCount { get; set; }

    [XmlIgnore]
    public bool PkCountSpecified { get; set; }

    [XmlAttribute("vehicleMounted")]
    public bool VehicleMounted { get; set; }

    [XmlIgnore]
    public bool VehicleMountedSpecified { get; set; }

    [XmlAttribute("MinimumVitalityPoints")]
    public int MinimumVitalityPoints { get; set; }

    [XmlIgnore]
    public bool MinimumVitalityPointsSpecified { get; set; }

    [XmlAttribute("resting")]
    public bool Resting { get; set; }

    [XmlIgnore]
    public bool RestingSpecified { get; set; }

    [XmlAttribute("flying")]
    public bool Flying { get; set; }

    [XmlIgnore]
    public bool FlyingSpecified { get; set; }

    [XmlAttribute("moving")]
    public bool Moving { get; set; }

    [XmlIgnore]
    public bool MovingSpecified { get; set; }

    [XmlAttribute("running")]
    public bool Running { get; set; }

    [XmlIgnore]
    public bool RunningSpecified { get; set; }

    [XmlAttribute("standing")]
    public bool Standing { get; set; }

    [XmlIgnore]
    public bool StandingSpecified { get; set; }

    [XmlAttribute("behind")]
    public bool Behind { get; set; }

    [XmlIgnore]
    public bool BehindSpecified { get; set; }

    [XmlAttribute("front")]
    public bool Front { get; set; }

    [XmlIgnore]
    public bool FrontSpecified { get; set; }

    [XmlAttribute("olympiad")]
    public bool Olympiad { get; set; }

    [XmlIgnore]
    public bool OlympiadSpecified { get; set; }

    [XmlAttribute("transformationId")]
    public int TransformationId { get; set; }

    [XmlIgnore]
    public bool TransformationIdSpecified { get; set; }

    [XmlAttribute("hp")]
    public int Hp { get; set; }

    [XmlIgnore]
    public bool HpSpecified { get; set; }

    [XmlAttribute("mp")]
    public int Mp { get; set; }

    [XmlIgnore]
    public bool MpSpecified { get; set; }

    [XmlAttribute("cp")]
    public int Cp { get; set; }

    [XmlIgnore]
    public bool CpSpecified { get; set; }

    [XmlAttribute("siegeSide")]
    public int SiegeSide { get; set; }

    [XmlIgnore]
    public bool SiegeSideSpecified { get; set; }

    [XmlAttribute("charges")]
    public int Charges { get; set; }

    [XmlIgnore]
    public bool ChargesSpecified { get; set; }

    [XmlAttribute("souls")]
    public int Souls { get; set; }

    [XmlIgnore]
    public bool SoulsSpecified { get; set; }

    [XmlAttribute("weight")]
    public int Weight { get; set; }

    [XmlIgnore]
    public bool WeightSpecified { get; set; }

    [XmlAttribute("invSize")]
    public int InventorySize { get; set; }

    [XmlIgnore]
    public bool InventorySizeSpecified { get; set; }

    [XmlAttribute("isClanLeader")]
    public bool IsClanLeader { get; set; }

    [XmlIgnore]
    public bool IsClanLeaderSpecified { get; set; }

    [XmlAttribute("landingZone")]
    public bool LandingZone { get; set; }

    [XmlIgnore]
    public bool LandingZoneSpecified { get; set; }

    [XmlAttribute("active_effect_id")]
    public int ActiveEffectId { get; set; }

    [XmlIgnore]
    public bool ActiveEffectIdSpecified { get; set; }

    [XmlAttribute("active_effect_id_lvl")]
    public string? ActiveEffectIdLevel { get; set; }

    [XmlAttribute("active_skill_id")]
    public int ActiveSkillId { get; set; }

    [XmlIgnore]
    public bool ActiveSkillIdSpecified { get; set; }

    [XmlAttribute("active_skill_id_lvl")]
    public string? ActiveSkillIdLevel { get; set; }

    [XmlAttribute("canSwitchSubclass")]
    public int CanSwitchSubclass { get; set; }

    [XmlIgnore]
    public bool CanSwitchSubclassSpecified { get; set; }

    [XmlAttribute("agathionId")]
    public int AgathionId { get; set; }

    [XmlIgnore]
    public bool AgathionIdSpecified { get; set; }

    [XmlAttribute("hasPet")]
    public string? HasPet { get; set; }

    [XmlAttribute("servitorNpcId")]
    public string? ServitorNpcId { get; set; }

    [XmlAttribute("npcIdRadius")]
    public string? NpcIdRadius { get; set; }

    [XmlAttribute("summonedNpcIdRadius")]
    public string? SummonedNpcIdRadius { get; set; }

    [XmlAttribute("checkAbnormal")]
    public string? CheckAbnormal { get; set; }

    [XmlAttribute("callPc")]
    public bool CallPc { get; set; }

    [XmlIgnore]
    public bool CallPcSpecified { get; set; }

    [XmlAttribute("canCreateBase")]
    public bool CanCreateBase { get; set; }

    [XmlIgnore]
    public bool CanCreateBaseSpecified { get; set; }

    [XmlAttribute("canEscape")]
    public bool CanEscape { get; set; }

    [XmlIgnore]
    public bool CanEscapeSpecified { get; set; }

    [XmlAttribute("canResurrect")]
    public bool CanResurrect { get; set; }

    [XmlIgnore]
    public bool CanResurrectSpecified { get; set; }

    [XmlAttribute("canSummonPet")]
    public bool CanSummonPet { get; set; }

    [XmlIgnore]
    public bool CanSummonPetSpecified { get; set; }

    [XmlAttribute("canSummonServitor")]
    public bool CanSummonServitor { get; set; }

    [XmlIgnore]
    public bool CanSummonServitorSpecified { get; set; }

    [XmlAttribute("canSummonSiegeGolem")]
    public bool CanSummonSiegeGolem { get; set; }

    [XmlIgnore]
    public bool CanSummonSiegeGolemSpecified { get; set; }

    [XmlAttribute("canSweep")]
    public bool CanSweep { get; set; }

    [XmlIgnore]
    public bool CanSweepSpecified { get; set; }

    [XmlAttribute("canTakeCastle")]
    public bool CanTakeCastle { get; set; }

    [XmlIgnore]
    public bool CanTakeCastleSpecified { get; set; }

    [XmlAttribute("canTakeFort")]
    public bool CanTakeFort { get; set; }

    [XmlIgnore]
    public bool CanTakeFortSpecified { get; set; }

    [XmlAttribute("canTransform")]
    public bool CanTransform { get; set; }

    [XmlIgnore]
    public bool CanTransformSpecified { get; set; }

    [XmlAttribute("canUntransform")]
    public bool CanUntransform { get; set; }

    [XmlIgnore]
    public bool CanUntransformSpecified { get; set; }

    [XmlAttribute("immobile")]
    public bool Immobile { get; set; }

    [XmlIgnore]
    public bool ImmobileSpecified { get; set; }

    [XmlAttribute("inCombat")]
    public bool InCombat { get; set; }

    [XmlIgnore]
    public bool InCombatSpecified { get; set; }

    [XmlAttribute("inInstance")]
    public bool InInstance { get; set; }

    [XmlIgnore]
    public bool InInstanceSpecified { get; set; }

    [XmlAttribute("canRefuelAirShip")]
    public int CanRefuelAirShip { get; set; }

    [XmlIgnore]
    public bool CanRefuelAirShipSpecified { get; set; }

    [XmlAttribute("hasFreeSummonPoints")]
    public int HasFreeSummonPoints { get; set; }

    [XmlIgnore]
    public bool HasFreeSummonPointsSpecified { get; set; }

    [XmlAttribute("hasFreeTeleportBookmarkSlots")]
    public int HasFreeTeleportBookmarkSlots { get; set; }

    [XmlIgnore]
    public bool HasFreeTeleportBookmarkSlotsSpecified { get; set; }
}