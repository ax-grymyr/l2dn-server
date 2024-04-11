using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BuildDataPackDb.Db;

public class DbItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ItemId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string AdditionalName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public ItemType Type { get; set; }
    
    public int KeepType { get; set; } // figure it
    public int NameClass { get; set; } // figure it
    public int Color { get; set; } // figure it
    public int Popup { get; set; } // figure it
    public int EnchantBless { get; set; } // figure it
    
    public ItemDefaultAction DefaultAction { get; set; }
    
    public int UseOrder { get; set; }
    public int SortOrder { get; set; }
    public int AuctionCategory { get; set; }
    
    public ItemFlags Flags { get; set; }

    public int? Icon1Id { get; set; }
    public int? Icon2Id { get; set; }
    public int? Icon3Id { get; set; }
    public int? Icon4Id { get; set; }
    public int? Icon5Id { get; set; }
    public int? PanelIconId { get; set; }

    public int? Durability { get; set; }
    public int? Weight { get; set; }
    public ItemInventoryType InventoryType { get; set; }
    public ItemBodyPart? BodyPart { get; set; }
    public ItemMaterialType? MaterialType { get; set; }
    public ItemArmorType ArmorType { get; set; }
    public ItemWeaponType WeaponType { get; set; }
    public ItemGrade Grade { get; set; }
    public ItemConsumeType ConsumeType { get; set; }
    public ItemEtcType EtcItemType { get; set; }
    public int ScrollSetId { get; set; }
    public int MpBonus { get; set; }
    public int MpConsume { get; set; }
    public int Handness { get; set; } // weapon: figure it  
    public int RandomDamage { get; set; } // weapon: figure it  
    public int SoulshotCount { get; set; }
    public int SpiritshotCount { get; set; }  
    public int FullArmorEnchantEffectType { get; set; }
    public int NormalEnsoulCount { get; set; }
    public int SpecialEnsoulCount { get; set; }
}

public class DbItemCreateList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ListId { get; set; }

    public int BoxItemId { get; set; }
    public int Index { get; set; }
    public ItemCreateListType Type { get; set; }
}

[PrimaryKey(nameof(ListId), nameof(Index))]
public class DbItemCreateListItem
{
    public int ListId { get; set; }
    public int Index { get; set; } 
    public int ItemId { get; set; }
    public long Count { get; set; }
    public int EnchantValue { get; set; }
}

[PrimaryKey(nameof(ItemId), nameof(QuestId))]
public class DbItemRelatedQuest
{
    public int ItemId { get; set; }
    public int QuestId { get; set; }
}

public enum ItemCreateListType
{
    All = 0,
    AnyLowProbability = 1,
    AnyRandom = 2,
}

[Flags]
public enum ItemFlags
{
    None = 0,
    
    // Exchange and trade
    Trade = 1 << 0,
    Drop = 1 << 1,
    Destruct = 1 << 2,
    PrivateStore = 1 << 3,
    NpcTrade = 1 << 4,
    CommissionStore = 1 << 5,
    
    Crystallizable = 1 << 10,
    Attribution = 1 << 11,

    HeroWeapon = 1 << 20,
    MagicWeapon = 1 << 21,
}

public enum ItemInventoryType
{
    None = 0,
    Equipment = 1,
    Consumable = 2,
    Material = 3,
    Etc = 4,
    Quest = 5,
    Artifact = 6,
}

public enum ItemBodyPart
{
    Underwear = 0,
    REar = 1,
    Lear = 2,
    Neck = 3,
    RFinger = 4,
    LFinger = 5,
    Head = 6,
    LrHand = 7,
    OnePiece = 8,
    AllDress = 9,
    HairAll = 10,
    RBracelet = 11,
    LBracelet = 12,
    Deco1 = 13,
    Deco2 = 14,
    Deco3 = 15,
    Deco4 = 16,
    Deco5 = 17,
    Deco6 = 18,
    Waist = 19,
    Brooch = 20,
    Jewel1 = 21,
    Jewel2 = 22,
    Jewel3 = 23,
    Jewel4 = 24,
    Jewel5 = 25,
    Jewel6 = 26,
    AgathionMain = 27,
    AgathionSub1 = 28,
    AgathionSub2 = 29,
    AgathionSub3 = 30,
    AgathionSub4 = 31,
    Artifactbook = 32,
    ArtifactA1 = 33,
    ArtifactA2 = 34,
    ArtifactA3 = 35,
    ArtifactA4 = 36,
    ArtifactA5 = 37,
    ArtifactA6 = 38,
    ArtifactA7 = 39,
    ArtifactA8 = 40,
    ArtifactA9 = 41,
    ArtifactA10 = 42,
    ArtifactA11 = 43,
    ArtifactA12 = 44,
    ArtifactB1 = 45,
    ArtifactB2 = 46,
    ArtifactB3 = 47,
    ArtifactC1 = 48,
    ArtifactC2 = 49,
    ArtifactC3 = 50,
    ArtifactD1 = 51,
    ArtifactD2 = 52,
    ArtifactD3 = 53,
    Gloves = 54,
    Chest = 55,
    Legs = 56,
    Feet = 57,
    Back = 58,
    HairOld = 59, // ?
    Hair = 60,
    RHand1 = 61, // ?
    RHand = 62,
    LHand = 63,
}

public enum ItemMaterialType
{
    Fish = 0,
    Oriharukon = 1,
    Mithril = 2,
    Gold = 3,
    Silver = 4,
    Brass = 5,
    Bronze = 6,
    Diamond = 7,
    Steel = 8,
    DamascusSteel = 9,
    BoneOfDragon = 10,
    Air = 11,
    Water = 12,
    Wood = 13,
    Bone = 14,
    BranchOfOrbisArbor = 15,
    BranchOfWorldTree = 16,
    Cloth = 17,
    Paper = 18,
    Leather = 19,
    Skull = 20,
    Feather = 21,
    Sand = 22,
    Crystal = 23,
    Fur = 24,
    Parchment = 25,
    Eyeball = 26,
    Insect = 27,
    Corpse = 28,
    Crow = 29,
    HatchingEgg = 30,
    SkullOfWyvern = 31,
    Drug = 32,
    Cotton = 33,
    Silk = 34,
    Wool = 35,
    Flax = 36,
    Cobweb = 37,
    Dyestuff = 38,
    LeatherOfPuma = 39,
    LeatherOfLion = 40,
    LeatherOfManticore = 41,
    LeatherOfDrake = 42,
    LeatherOfSalamander = 43,
    LeatherOfUnicorn = 44,
    LeatherOfDragon = 45,
    ScaleOfDragon = 46,
    Adamantaite = 47,
    BloodSteel = 48,
    Chrysolite = 49,
    Damascus = 50,
    FineSteel = 51,
    Horn = 52,
    Liquid = 53,
    RuneXp = 54,
    RuneSp = 55,
    RuneRemovePenalty = 56,
}

public enum ItemArmorType
{
    None = 0,
    Light = 1,
    Heavy = 2,
    Magic = 3,
    Sigil = 4,
}

public enum ItemGrade
{
    None = 0,
    D = 1,
    C = 2,
    B = 3,
    A = 4,
    S = 5,
    S80 = 6,
    S84 = 7,
    R = 8,
    R95 = 9,
    R99 = 10,
    R110 = 11,
    L = 12,
    Event = 13,
    CrystalFree = 14,
}

public enum ItemDefaultAction
{
    None = 0,
    None2 = 1, // ?
    BlessSpiritshot = 2,
    Calc = 3,
    CallSkill = 4,
    Capsule = 5,
    CreateMpcc = 6,
    Dice = 7,
    Equip = 8,
    Fishingshot = 9,
    Harvest = 10,
    None3 = 11, // ?
    HideName = 12,
    Peel = 13,
    KeepExp = 14,
    NickColor = 15,
    Peel2 = 16, // again?
    Recipe = 17,
    Seed = 18,
    ShowHtml = 19,
    ShowSsqStatus = 20,
    ShowTutorial = 21,
    SkillMaintain = 22,
    SkillReduce = 23,
    SkillReduceOnSkillSuccess = 24,
    Soulshot = 25,
    Spiritshot = 26,
    StartQuest = 27,
    SummonSoulshot = 28,
    SummonSpiritshot = 29,
    UseCount = 30,
    VariationStone = 31,
    XmasOpen = 32,
    SkillReduceOnSkillSuccess2 = 33, // again?
    None4 = 34, // multisell selection item
}

public enum ItemType
{
    Unknown,
    Armor,
    Weapon,
    Etc
}

public enum ItemWeaponType
{
    Shield = 0,
    Sword = 1,
    TwoHandSword = 2,
    Buster = 3,
    Blunt = 4,
    TwoHandBlunt = 5,
    Staff = 6,
    TwoHandStaff = 7,
    Dagger = 8,
    Pole = 9,
    DualFist = 10,
    Bow = 11,
    WeaponEtc = 12,
    Dual = 13,
    Fist = 14,
    FishingRod = 15,
    Rapier = 16,
    Crossbow = 17,
    AncientSword = 18,
    DualDagger = 20,
    TwoHandCrossbow = 22,
    DualBlunt = 23,
    Pistols = 25,
}

public enum ItemConsumeType
{
    Normal = 0,
    Charge = 1,
    Stackable = 2,
    Asset = 3,
}

public enum ItemEtcType
{
	None = 0,
	Scroll = 1,
	Arrow = 2,
	Potion = 3,
	Spellbook = 4,
	Recipe = 5,
	Material = 6,
	PetCollar = 7,
	CastleGuard = 8,
	Dye = 9,
	Seed = 10,
	Seed2 = 11,
	Harvest = 12,
	Lotto = 13,
	RaceTicket = 14,
	TicketOfLord = 15,
	Lure = 16,
	Crop = 17,
	Maturecrop = 18,
	Coupon = 23,
	Elixir = 24,
	Rune = 34,
	RuneSelect = 35,
	ScrlEnchantAm = 20,
	ScrlEnchantWp = 19,
	BlessScrlEnchantAm = 22,
	BlessScrlEnchantWp = 21,
	ScrlIncEnchantPropAm = 29,
	ScrlIncEnchantPropWp = 28,
	BlessScrlIncEnchantPropWp = 52,
	BlessScrlIncEnchantPropAm = 53,
	CrystalEnchantAm = 30,
	CrystalEnchantWp = 31,
	AncientCrystalEnchantAm = 32,
	AncientCrystalEnchantWp = 33,
	ScrlEnchantAttr = 25,
	ScrlEnchantAttrCursed = 26,
	ScrlChangeAttr = 37,
	CardEvent = 54,
	Teleportbookmark = 36,
	SoulShot = 38,
	Bolt = 27,
	ScrlShapeShiftingWp = 39,
	BlessScrlShapeShiftingWp = 40,
	ScrlShapeShiftingWpFixed = 41,
	ScrlShapeShiftingAm = 42,
	BlessScrlShapeShiftingAm = 43,
	ScrlShapeShiftingAmFixed = 44,
	ScrlShapeShiftingHairAcc = 45,
	BlessScrlShapeShiftingHairAcc = 46,
	ScrlShapeShiftingHairAccFixed = 47,
	ScrlRestoreShapeShiftingWp = 48,
	ScrlRestoreShapeShiftingAm = 49,
	ScrlRestoreShapeShiftingHairAcc = 50,
	ScrlRestoreShapeShiftingAllItem = 51,
	ScrlShapeShiftingAllitemFixed = 55,
	MultiEnchtWp = 56,
	MultiEnchtAm = 57,
	MultiIncProbEnchtWp = 58,
	MultiIncProbEnchtAm = 59,
	EnsoulStone = 60,
	NickColorOld = 61,
	NickColorNew = 62,
	ScrlEnchantAg = 63,
	BlessScrlEnchantAg = 64,
	ScrlMultiEnchantAg = 65,
	AncientCrystalEnchantAg = 66,
	ScrlIncEnchantPropAg = 67,
	BlessScrlIncEnchantPropAg = 68,
	ScrlMultiIncEnchantProbAg = 69,
	ScrlLockItem = 70,
	ScrlUnlockItem = 71,
	Bullet = 72,
	CostumeBook = 73,
	CostumeBookRdAll = 74,
	CostumeBookRdPart = 75,
	CostumeBook1 = 76,
	CostumeBook2 = 77,
	CostumeBook3 = 78,
	CostumeBook4 = 79,
	CostumeBook5 = 80,
	MagicLamp = 81,
	PolyEnchantWp = 82,
	PolyEnchantAm = 83,
	PolyIncEnchantPropWp = 84,
	PolyIncEnchantPropAm = 85,
	CursedEnchantWp = 86,
	CursedEnchantAm = 87,
	VitalLegacyItem1D = 88,
	VitalLegacyItem7D = 89,
	VitalLegacyItem30D = 90,
	BlessUpgradeWp = 91,
	Orb = 92,
	ItemRestoreCoin = 93,
	SpecialEnchantWeapon = 94,
	SpecialEnchantArmor = 95,
	NickColrIcon = 96,
}