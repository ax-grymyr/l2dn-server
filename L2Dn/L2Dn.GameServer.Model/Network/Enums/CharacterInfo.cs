using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Network.Enums;

public sealed class CharacterInfo: IHasId<int>
{
	private CharacterClass _baseClass;
	
	public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public Sex Sex { get; set; }
    public Race Race { get; set; }

    public CharacterClass BaseClass
    {
	    get => _baseClass;
	    set => _baseClass = ValidateBaseClass(value);
    }

    public CharacterClass Class { get; set; }
    public int HairStyle { get; set; }
    public int HairColor { get; set; }
    public int Face { get; set; }
    public bool HairAccessoryEnabled { get; set; }
    public bool IsNoble { get; set; }
    
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    
    public double CurrentHp { get; set; }
    public double CurrentMp { get; set; }
    public double MaxHp { get; set; }
    public double MaxMp { get; set; }
    
    public int Level { get; set; }
    public long Exp { get; set; }
    public long Sp { get; set; }
    
    public int Reputation { get; set; }
    public int PkKills { get; set; }
    public int PvpKills { get; set; }
    public int VitalityPoints { get; set; }
    public int VitalityItemsUsed { get; set; }
    
    public DateTime Created { get; set; }
    public DateTime? LastAccessTime { get; set; }
    public DateTime? DeleteTime { get; set; }
    public int AccessLevel { get; set; }
    public int? ClanId { get; set; }
    public bool IsGood { get; set; }
    public bool IsEvil { get; set; }

    public CharacterPaperdollSlotInfo[] Paperdoll { get; } =
        new CharacterPaperdollSlotInfo[Inventory.PAPERDOLL_TOTALSLOTS];
    
    public int ChestEnchantLevel { get; set; }
    public int LegsEnchantLevel { get; set; }
    public int HeadEnchantLevel { get; set; }
    public int GlovesEnchantLevel { get; set; }
    public int BootsEnchantLevel { get; set; }
    public int WeaponEnchantLevel { get; set; }
    public int WeaponAugmentationOption1Id { get; set; }
    public int WeaponAugmentationOption2Id { get; set; }

    public void LoadFrom(Character character)
    {
        Id = character.Id;
        Name = character.Name;
        Sex = character.Sex;
        
        // Check if the base class is set to zero and also doesn't match with the current active class,
        // otherwise send the base class ID. This prevents chars created before
        // base class was introduced from being displayed incorrectly.
        BaseClass = character is { BaseClass: 0, Class: > 0 } ? character.Class : character.BaseClass;

        Race = BaseClass.GetRace();
        Class = character.Class;
        HairStyle = character.HairStyle;
        HairColor = character.HairColor;
        Face = character.Face;
        IsNoble = character.IsNobless;
        
        IsGood = character.Faction == 1;
        IsEvil = character.Faction == 2;

        X = character.X;
        Y = character.Y;
        Z = character.Z;

        CurrentHp = character.CurrentHp;
        CurrentMp = character.CurrentMp;
        MaxHp = character.MaxHp;
        MaxMp = character.MaxMp;

        Level = character.Level;
        Exp = character.Exp;
        Sp = character.Sp;

        Reputation = character.Reputation;
        PkKills = character.PkKills;
        PvpKills = character.PvpKills;
        VitalityPoints = character.VitalityPoints;

        Created = character.Created;
        LastAccessTime = character.LastAccess;
        DeleteTime = character.DeleteTime;
        AccessLevel = character.AccessLevel;
        ClanId = character.ClanId;
    }

    public void LoadFrom(Player player)
    {
	    WeaponEnchantLevel = 0;
	    ChestEnchantLevel = 0;
	    LegsEnchantLevel = 0;
	    HeadEnchantLevel = 0;
	    GlovesEnchantLevel = 0;
	    BootsEnchantLevel = 0;
	    
	    PlayerInventory inventory = player.getInventory();
	    for (int i = 0; i < Paperdoll.Length; i++)
	    {
		    Item? item = inventory.getPaperdollItem(i);
		    if (item is null)
		    {
			    Paperdoll[i] = default;
			    continue;
		    }

		    Paperdoll[i] = new CharacterPaperdollSlotInfo(item.getId(), item.getVisualId());
		    switch (i)
		    {
			    case Inventory.PAPERDOLL_RHAND:
				    WeaponEnchantLevel = item.getEnchantLevel();
				    break;

			    case Inventory.PAPERDOLL_CHEST:
				    ChestEnchantLevel = item.getEnchantLevel();
				    break;

			    case Inventory.PAPERDOLL_LEGS:
				    LegsEnchantLevel = item.getEnchantLevel();
				    break;

			    case Inventory.PAPERDOLL_HEAD:
				    HeadEnchantLevel = item.getEnchantLevel();
				    break;

			    case Inventory.PAPERDOLL_GLOVES:
				    GlovesEnchantLevel = item.getEnchantLevel();
				    break;

			    case Inventory.PAPERDOLL_FEET:
				    BootsEnchantLevel = item.getEnchantLevel();
				    break;
		    }
	    }

	    Id = player.getObjectId();
	    Name = player.getName();
	    AccessLevel = player.getAccessLevel()?.getLevel() ?? 0;
	    Level = player.getLevel();
	    MaxHp = player.getMaxHp();
	    CurrentHp = player.getCurrentHp();
	    MaxMp = player.getMaxMp();
	    CurrentMp = player.getCurrentMp();
	    Reputation = player.getReputation();
	    PkKills = player.getPkKills();
	    PvpKills = player.getPvpKills();

	    Face = player.getVisualFace();
	    HairStyle = player.getVisualHair();
	    HairColor = player.getVisualHairColor();
	    HairAccessoryEnabled = player.isHairAccessoryEnabled();
	    VitalityItemsUsed = player.getVitalityItemsUsed();

	    Sex = player.getAppearance().getSex();
	    Exp = player.getExp();
	    Sp = player.getSp();
	    VitalityPoints = player.getVitalityPoints();
	    ClanId = player.getClanId();
	    Race = player.getClassId().GetRace();
	    IsNoble = player.isNoble();
	    CharacterClass baseClassId = player.getBaseClass();
	    CharacterClass activeClassId = player.getClassId();
	    X = player.getX();
	    Y = player.getY();
	    Z = player.getZ();

	    IsGood = player.isGood();
	    IsEvil = player.isEvil();

	    Class = activeClassId;

	    // Get the augmentation id for equipped weapon
	    VariationInstance variationInstance = inventory.getPaperdollAugmentation(Inventory.PAPERDOLL_RHAND);
	    if (variationInstance != null)
	    {
		    WeaponAugmentationOption1Id = variationInstance.getOption1Id();
		    WeaponAugmentationOption2Id = variationInstance.getOption2Id();
	    }

	    // Check if the base class is set to zero and also doesn't match with the current active class,
	    // otherwise send the base class ID. This prevents chars created before base class was introduced
	    // from being displayed incorrectly.
	    BaseClass = baseClassId == 0 && activeClassId > 0 ? activeClassId : baseClassId;
	    Race = BaseClass.GetRace();

	    DeleteTime = player.getDeleteTime();
	    LastAccessTime = player.getLastAccess();
    }

    private static CharacterClass ValidateBaseClass(CharacterClass baseClassId)
    {
	    // DK Human
	    if (baseClassId >= (CharacterClass)196 && baseClassId <= (CharacterClass)199)
		    return (CharacterClass)196;
	    
	    // DK Elf
	    if (baseClassId >= (CharacterClass)200 && baseClassId <= (CharacterClass)203)
		    return (CharacterClass)200;

	    // DK Dark Elf
	    if (baseClassId >= (CharacterClass)204 && baseClassId <= (CharacterClass)207)
		    return (CharacterClass)204;
	    
	    // Vanguard
	    if (baseClassId >= (CharacterClass)217 && baseClassId <= (CharacterClass)220)
		    return (CharacterClass)217;
	    
	    // Assassin Male
	    if (baseClassId >= (CharacterClass)221 && baseClassId <= (CharacterClass)224)
		    return (CharacterClass)221;
	    
	    // Assassin Female
	    if (baseClassId >= (CharacterClass)225 && baseClassId <= (CharacterClass)228)
		    return (CharacterClass)225;

	    // Other Classes
	    return baseClassId;
    }
}