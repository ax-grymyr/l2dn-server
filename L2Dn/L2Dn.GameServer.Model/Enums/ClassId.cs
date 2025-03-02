using L2Dn.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Enums;

public static class CharacterClassUtil
{
	public static CharacterClassInfo GetClassInfo(this CharacterClass cId) => CharacterClassInfo.GetClassInfo(cId);
	public static CharacterClass? GetParentClass(this CharacterClass cId) => CharacterClassInfo.GetClassInfo(cId).getParent()?.getId();
	public static CharacterClass GetRootClass(this CharacterClass cId) => CharacterClassInfo.GetClassInfo(cId).getRootClassId().getId();
	public static Race GetRace(this CharacterClass cId) => CharacterClassInfo.GetClassInfo(cId).getRace();
	public static int GetLevel(this CharacterClass cId) => CharacterClassInfo.GetClassInfo(cId).level();

	public static bool EqualsOrChildOf(this CharacterClass cId, CharacterClass other)
	{
		if (cId == other)
			return true;

		CharacterClass? parent = other.GetParentClass();
		while (parent != null)
		{
			if (parent == cId)
				return true;

			parent = parent.Value.GetParentClass();
		}

		return false;
	}
}

public class CharacterClassInfo
{
	private static readonly Dictionary<CharacterClass, CharacterClassInfo> _classIdMap = new();

	public static readonly CharacterClassInfo FIGHTER = new(CharacterClass.FIGHTER, false, Race.HUMAN, null);

	public static readonly CharacterClassInfo WARRIOR = new(CharacterClass.WARRIOR, false, Race.HUMAN, CharacterClass.FIGHTER);
	public static readonly CharacterClassInfo GLADIATOR = new(CharacterClass.GLADIATOR, false, Race.HUMAN, CharacterClass.WARRIOR);
	public static readonly CharacterClassInfo WARLORD = new(CharacterClass.WARLORD, false, Race.HUMAN, CharacterClass.WARRIOR);
	public static readonly CharacterClassInfo KNIGHT = new(CharacterClass.KNIGHT, false, Race.HUMAN, CharacterClass.FIGHTER);
	public static readonly CharacterClassInfo PALADIN = new(CharacterClass.PALADIN, false, Race.HUMAN, CharacterClass.KNIGHT);
	public static readonly CharacterClassInfo DARK_AVENGER = new(CharacterClass.DARK_AVENGER, false, Race.HUMAN, CharacterClass.KNIGHT);
	public static readonly CharacterClassInfo ROGUE = new(CharacterClass.ROGUE, false, Race.HUMAN, CharacterClass.FIGHTER);
	public static readonly CharacterClassInfo TREASURE_HUNTER = new(CharacterClass.TREASURE_HUNTER, false, Race.HUMAN, CharacterClass.ROGUE);
	public static readonly CharacterClassInfo HAWKEYE = new(CharacterClass.HAWKEYE, false, Race.HUMAN, CharacterClass.ROGUE);

	public static readonly CharacterClassInfo MAGE = new(CharacterClass.MAGE, true, Race.HUMAN, null);
	public static readonly CharacterClassInfo WIZARD = new(CharacterClass.WIZARD, true, Race.HUMAN, CharacterClass.MAGE);
	public static readonly CharacterClassInfo SORCERER = new(CharacterClass.SORCERER, true, Race.HUMAN, CharacterClass.WIZARD);
	public static readonly CharacterClassInfo NECROMANCER = new(CharacterClass.NECROMANCER, true, Race.HUMAN, CharacterClass.WIZARD);
	public static readonly CharacterClassInfo WARLOCK = new(CharacterClass.WARLOCK, true, true, Race.HUMAN, CharacterClass.WIZARD);
	public static readonly CharacterClassInfo CLERIC = new(CharacterClass.CLERIC, true, Race.HUMAN, CharacterClass.MAGE);
	public static readonly CharacterClassInfo BISHOP = new(CharacterClass.BISHOP, true, Race.HUMAN, CharacterClass.CLERIC);
	public static readonly CharacterClassInfo PROPHET = new(CharacterClass.PROPHET, true, Race.HUMAN, CharacterClass.CLERIC);

	public static readonly CharacterClassInfo ELVEN_FIGHTER = new(CharacterClass.ELVEN_FIGHTER, false, Race.ELF, null);
	public static readonly CharacterClassInfo ELVEN_KNIGHT = new(CharacterClass.ELVEN_KNIGHT, false, Race.ELF, CharacterClass.ELVEN_FIGHTER);
	public static readonly CharacterClassInfo TEMPLE_KNIGHT = new(CharacterClass.TEMPLE_KNIGHT, false, Race.ELF, CharacterClass.ELVEN_KNIGHT);
	public static readonly CharacterClassInfo SWORDSINGER = new(CharacterClass.SWORDSINGER, false, Race.ELF, CharacterClass.ELVEN_KNIGHT);
	public static readonly CharacterClassInfo ELVEN_SCOUT = new(CharacterClass.ELVEN_SCOUT, false, Race.ELF, CharacterClass.ELVEN_FIGHTER);
	public static readonly CharacterClassInfo PLAINS_WALKER = new(CharacterClass.PLAINS_WALKER, false, Race.ELF, CharacterClass.ELVEN_SCOUT);
	public static readonly CharacterClassInfo SILVER_RANGER = new(CharacterClass.SILVER_RANGER, false, Race.ELF, CharacterClass.ELVEN_SCOUT);

	public static readonly CharacterClassInfo ELVEN_MAGE = new(CharacterClass.ELVEN_MAGE, true, Race.ELF, null);
	public static readonly CharacterClassInfo ELVEN_WIZARD = new(CharacterClass.ELVEN_WIZARD, true, Race.ELF, CharacterClass.ELVEN_MAGE);
	public static readonly CharacterClassInfo SPELLSINGER = new(CharacterClass.SPELLSINGER, true, Race.ELF, CharacterClass.ELVEN_WIZARD);

	public static readonly CharacterClassInfo ELEMENTAL_SUMMONER = new(CharacterClass.ELEMENTAL_SUMMONER, true, true, Race.ELF, CharacterClass.ELVEN_WIZARD);
	public static readonly CharacterClassInfo ORACLE = new(CharacterClass.ORACLE, true, Race.ELF, CharacterClass.ELVEN_MAGE);
	public static readonly CharacterClassInfo ELDER = new(CharacterClass.ELDER, true, Race.ELF, CharacterClass.ORACLE);

	public static readonly CharacterClassInfo DARK_FIGHTER = new(CharacterClass.DARK_FIGHTER, false, Race.DARK_ELF, null);
	public static readonly CharacterClassInfo PALUS_KNIGHT = new(CharacterClass.PALUS_KNIGHT, false, Race.DARK_ELF, CharacterClass.DARK_FIGHTER);
	public static readonly CharacterClassInfo SHILLIEN_KNIGHT = new(CharacterClass.SHILLIEN_KNIGHT, false, Race.DARK_ELF, CharacterClass.PALUS_KNIGHT);
	public static readonly CharacterClassInfo BLADEDANCER = new(CharacterClass.BLADEDANCER, false, Race.DARK_ELF, CharacterClass.PALUS_KNIGHT);
	public static readonly CharacterClassInfo ASSASSIN = new(CharacterClass.ASSASSIN, false, Race.DARK_ELF, CharacterClass.DARK_FIGHTER);
	public static readonly CharacterClassInfo ABYSS_WALKER = new(CharacterClass.ABYSS_WALKER, false, Race.DARK_ELF, CharacterClass.ASSASSIN);
	public static readonly CharacterClassInfo PHANTOM_RANGER = new(CharacterClass.PHANTOM_RANGER, false, Race.DARK_ELF, CharacterClass.ASSASSIN);

	public static readonly CharacterClassInfo DARK_MAGE = new(CharacterClass.DARK_MAGE, true, Race.DARK_ELF, null);
	public static readonly CharacterClassInfo DARK_WIZARD = new(CharacterClass.DARK_WIZARD, true, Race.DARK_ELF, CharacterClass.DARK_MAGE);
	public static readonly CharacterClassInfo SPELLHOWLER = new(CharacterClass.SPELLHOWLER, true, Race.DARK_ELF, CharacterClass.DARK_WIZARD);

	public static readonly CharacterClassInfo PHANTOM_SUMMONER = new(CharacterClass.PHANTOM_SUMMONER, true, true, Race.DARK_ELF, CharacterClass.DARK_WIZARD);
	public static readonly CharacterClassInfo SHILLIEN_ORACLE = new(CharacterClass.SHILLIEN_ORACLE, true, Race.DARK_ELF, CharacterClass.DARK_MAGE);
	public static readonly CharacterClassInfo SHILLIEN_ELDER = new(CharacterClass.SHILLIEN_ELDER, true, Race.DARK_ELF, CharacterClass.SHILLIEN_ORACLE);

	public static readonly CharacterClassInfo ORC_FIGHTER = new(CharacterClass.ORC_FIGHTER, false, Race.ORC, null);
	public static readonly CharacterClassInfo ORC_RAIDER = new(CharacterClass.ORC_RAIDER, false, Race.ORC, CharacterClass.ORC_FIGHTER);
	public static readonly CharacterClassInfo DESTROYER = new(CharacterClass.DESTROYER, false, Race.ORC, CharacterClass.ORC_RAIDER);
	public static readonly CharacterClassInfo ORC_MONK = new(CharacterClass.ORC_MONK, false, Race.ORC, CharacterClass.ORC_FIGHTER);
	public static readonly CharacterClassInfo TYRANT = new(CharacterClass.TYRANT, false, Race.ORC, CharacterClass.ORC_MONK);

	public static readonly CharacterClassInfo ORC_MAGE = new(CharacterClass.ORC_MAGE, true, Race.ORC, null);
	public static readonly CharacterClassInfo ORC_SHAMAN = new(CharacterClass.ORC_SHAMAN, true, Race.ORC, CharacterClass.ORC_MAGE);
	public static readonly CharacterClassInfo OVERLORD = new(CharacterClass.OVERLORD, true, Race.ORC, CharacterClass.ORC_SHAMAN);
	public static readonly CharacterClassInfo WARCRYER = new(CharacterClass.WARCRYER, true, Race.ORC, CharacterClass.ORC_SHAMAN);

	public static readonly CharacterClassInfo DWARVEN_FIGHTER = new(CharacterClass.DWARVEN_FIGHTER, false, Race.DWARF, null);
	public static readonly CharacterClassInfo SCAVENGER = new(CharacterClass.SCAVENGER, false, Race.DWARF, CharacterClass.DWARVEN_FIGHTER);
	public static readonly CharacterClassInfo BOUNTY_HUNTER = new(CharacterClass.BOUNTY_HUNTER, false, Race.DWARF, CharacterClass.SCAVENGER);
	public static readonly CharacterClassInfo ARTISAN = new(CharacterClass.ARTISAN, false, Race.DWARF, CharacterClass.DWARVEN_FIGHTER);
	public static readonly CharacterClassInfo WARSMITH = new(CharacterClass.WARSMITH, false, Race.DWARF, CharacterClass.ARTISAN);

	public static readonly CharacterClassInfo DUELIST = new(CharacterClass.DUELIST, false, Race.HUMAN, CharacterClass.GLADIATOR);
	public static readonly CharacterClassInfo DREADNOUGHT = new(CharacterClass.DREADNOUGHT, false, Race.HUMAN, CharacterClass.WARLORD);
	public static readonly CharacterClassInfo PHOENIX_KNIGHT = new(CharacterClass.PHOENIX_KNIGHT, false, Race.HUMAN, CharacterClass.PALADIN);
	public static readonly CharacterClassInfo HELL_KNIGHT = new(CharacterClass.HELL_KNIGHT, false, Race.HUMAN, CharacterClass.DARK_AVENGER);
	public static readonly CharacterClassInfo SAGITTARIUS = new(CharacterClass.SAGITTARIUS, false, Race.HUMAN, CharacterClass.HAWKEYE);
	public static readonly CharacterClassInfo ADVENTURER = new(CharacterClass.ADVENTURER, false, Race.HUMAN, CharacterClass.TREASURE_HUNTER);
	public static readonly CharacterClassInfo ARCHMAGE = new(CharacterClass.ARCHMAGE, true, Race.HUMAN, CharacterClass.SORCERER);
	public static readonly CharacterClassInfo SOULTAKER = new(CharacterClass.SOULTAKER, true, Race.HUMAN, CharacterClass.NECROMANCER);
	public static readonly CharacterClassInfo ARCANA_LORD = new(CharacterClass.ARCANA_LORD, true, true, Race.HUMAN, CharacterClass.WARLOCK);
	public static readonly CharacterClassInfo CARDINAL = new(CharacterClass.CARDINAL, true, Race.HUMAN, CharacterClass.BISHOP);
	public static readonly CharacterClassInfo HIEROPHANT = new(CharacterClass.HIEROPHANT, true, Race.HUMAN, CharacterClass.PROPHET);

	public static readonly CharacterClassInfo EVA_TEMPLAR = new(CharacterClass.EVA_TEMPLAR, false, Race.ELF, CharacterClass.TEMPLE_KNIGHT);
	public static readonly CharacterClassInfo SWORD_MUSE = new(CharacterClass.SWORD_MUSE, false, Race.ELF, CharacterClass.SWORDSINGER);
	public static readonly CharacterClassInfo WIND_RIDER = new(CharacterClass.WIND_RIDER, false, Race.ELF, CharacterClass.PLAINS_WALKER);
	public static readonly CharacterClassInfo MOONLIGHT_SENTINEL = new(CharacterClass.MOONLIGHT_SENTINEL, false, Race.ELF, CharacterClass.SILVER_RANGER);
	public static readonly CharacterClassInfo MYSTIC_MUSE = new(CharacterClass.MYSTIC_MUSE, true, Race.ELF, CharacterClass.SPELLSINGER);

	public static readonly CharacterClassInfo ELEMENTAL_MASTER = new(CharacterClass.ELEMENTAL_MASTER, true, true, Race.ELF, CharacterClass.ELEMENTAL_SUMMONER);
	public static readonly CharacterClassInfo EVA_SAINT = new(CharacterClass.EVA_SAINT, true, Race.ELF, CharacterClass.ELDER);

	public static readonly CharacterClassInfo SHILLIEN_TEMPLAR = new(CharacterClass.SHILLIEN_TEMPLAR, false, Race.DARK_ELF, CharacterClass.SHILLIEN_KNIGHT);
	public static readonly CharacterClassInfo SPECTRAL_DANCER = new(CharacterClass.SPECTRAL_DANCER, false, Race.DARK_ELF, CharacterClass.BLADEDANCER);
	public static readonly CharacterClassInfo GHOST_HUNTER = new(CharacterClass.GHOST_HUNTER, false, Race.DARK_ELF, CharacterClass.ABYSS_WALKER);
	public static readonly CharacterClassInfo GHOST_SENTINEL = new(CharacterClass.GHOST_SENTINEL, false, Race.DARK_ELF, CharacterClass.PHANTOM_RANGER);
	public static readonly CharacterClassInfo STORM_SCREAMER = new(CharacterClass.STORM_SCREAMER, true, Race.DARK_ELF, CharacterClass.SPELLHOWLER);

	public static readonly CharacterClassInfo SPECTRAL_MASTER = new(CharacterClass.SPECTRAL_MASTER, true, true, Race.DARK_ELF, CharacterClass.PHANTOM_SUMMONER);
	public static readonly CharacterClassInfo SHILLIEN_SAINT = new(CharacterClass.SHILLIEN_SAINT, true, Race.DARK_ELF, CharacterClass.SHILLIEN_ELDER);

	public static readonly CharacterClassInfo TITAN = new(CharacterClass.TITAN, false, Race.ORC, CharacterClass.DESTROYER);
	public static readonly CharacterClassInfo GRAND_KHAVATARI = new(CharacterClass.GRAND_KHAVATARI, false, Race.ORC, CharacterClass.TYRANT);
	public static readonly CharacterClassInfo DOMINATOR = new(CharacterClass.DOMINATOR, true, Race.ORC, CharacterClass.OVERLORD);
	public static readonly CharacterClassInfo DOOMCRYER = new(CharacterClass.DOOMCRYER, true, Race.ORC, CharacterClass.WARCRYER);

	public static readonly CharacterClassInfo FORTUNE_SEEKER = new(CharacterClass.FORTUNE_SEEKER, false, Race.DWARF, CharacterClass.BOUNTY_HUNTER);
	public static readonly CharacterClassInfo MAESTRO = new(CharacterClass.MAESTRO, false, Race.DWARF, CharacterClass.WARSMITH);

	public static readonly CharacterClassInfo KAMAEL_SOLDIER = new(CharacterClass.KAMAEL_SOLDIER, false, Race.KAMAEL, null);

	public static readonly CharacterClassInfo TROOPER = new(CharacterClass.TROOPER, false, Race.KAMAEL, CharacterClass.KAMAEL_SOLDIER);
	public static readonly CharacterClassInfo SOUL_FINDER = new(CharacterClass.SOUL_FINDER, false, Race.KAMAEL, CharacterClass.KAMAEL_SOLDIER);
	public static readonly CharacterClassInfo WARDER = new(CharacterClass.WARDER, false, Race.KAMAEL, CharacterClass.KAMAEL_SOLDIER);

	public static readonly CharacterClassInfo BERSERKER = new(CharacterClass.BERSERKER, false, Race.KAMAEL, CharacterClass.TROOPER);
	public static readonly CharacterClassInfo SOUL_BREAKER = new(CharacterClass.SOUL_BREAKER, false, Race.KAMAEL, CharacterClass.SOUL_FINDER);
	public static readonly CharacterClassInfo SOUL_RANGER = new(CharacterClass.SOUL_RANGER, false, Race.KAMAEL, CharacterClass.WARDER);

	public static readonly CharacterClassInfo DOOMBRINGER = new(CharacterClass.DOOMBRINGER, false, Race.KAMAEL, CharacterClass.BERSERKER);
	public static readonly CharacterClassInfo SOUL_HOUND = new(CharacterClass.SOUL_HOUND, false, Race.KAMAEL, CharacterClass.SOUL_BREAKER);
	public static readonly CharacterClassInfo TRICKSTER = new(CharacterClass.TRICKSTER, false, Race.KAMAEL, CharacterClass.SOUL_RANGER);

	public static readonly CharacterClassInfo DEATH_PILGRIM_HUMAN = new(CharacterClass.DEATH_PILGRIM_HUMAN, false, Race.HUMAN, null);
	public static readonly CharacterClassInfo DEATH_BLADE_HUMAN = new(CharacterClass.DEATH_BLADE_HUMAN, false, Race.HUMAN, CharacterClass.DEATH_PILGRIM_HUMAN);
	public static readonly CharacterClassInfo DEATH_MESSENGER_HUMAN = new(CharacterClass.DEATH_MESSENGER_HUMAN, false, Race.HUMAN, CharacterClass.DEATH_BLADE_HUMAN);
	public static readonly CharacterClassInfo DEATH_KIGHT_HUMAN = new(CharacterClass.DEATH_KIGHT_HUMAN, false, Race.HUMAN, CharacterClass.DEATH_MESSENGER_HUMAN);

	public static readonly CharacterClassInfo DEATH_PILGRIM_ELF = new(CharacterClass.DEATH_PILGRIM_ELF, false, Race.ELF, null);
	public static readonly CharacterClassInfo DEATH_BLADE_ELF = new(CharacterClass.DEATH_BLADE_ELF, false, Race.ELF, CharacterClass.DEATH_PILGRIM_ELF);
	public static readonly CharacterClassInfo DEATH_MESSENGER_ELF = new(CharacterClass.DEATH_MESSENGER_ELF, false, Race.ELF, CharacterClass.DEATH_BLADE_ELF);
	public static readonly CharacterClassInfo DEATH_KIGHT_ELF = new(CharacterClass.DEATH_KIGHT_ELF, false, Race.ELF, CharacterClass.DEATH_MESSENGER_ELF);

	public static readonly CharacterClassInfo DEATH_PILGRIM_DARK_ELF = new(CharacterClass.DEATH_PILGRIM_DARK_ELF, false, Race.DARK_ELF, null);
	public static readonly CharacterClassInfo DEATH_BLADE_DARK_ELF = new(CharacterClass.DEATH_BLADE_DARK_ELF, false, Race.DARK_ELF, CharacterClass.DEATH_PILGRIM_DARK_ELF);
	public static readonly CharacterClassInfo DEATH_MESSENGER_DARK_ELF = new(CharacterClass.DEATH_MESSENGER_DARK_ELF, false, Race.DARK_ELF, CharacterClass.DEATH_BLADE_DARK_ELF);
	public static readonly CharacterClassInfo DEATH_KIGHT_DARK_ELF = new(CharacterClass.DEATH_KIGHT_DARK_ELF, false, Race.DARK_ELF, CharacterClass.DEATH_MESSENGER_DARK_ELF);

	public static readonly CharacterClassInfo SYLPH_GUNNER = new(CharacterClass.SYLPH_GUNNER, false, Race.SYLPH, null);
	public static readonly CharacterClassInfo SHARPSHOOTER = new(CharacterClass.SHARPSHOOTER, false, Race.SYLPH, CharacterClass.SYLPH_GUNNER);
	public static readonly CharacterClassInfo WIND_SNIPER = new(CharacterClass.WIND_SNIPER, false, Race.SYLPH, CharacterClass.SHARPSHOOTER);
	public static readonly CharacterClassInfo STORM_BLASTER = new(CharacterClass.STORM_BLASTER, false, Race.SYLPH, CharacterClass.WIND_SNIPER);

	public static readonly CharacterClassInfo ORC_LANCER = new(CharacterClass.ORC_LANCER, false, Race.ORC, null);
	public static readonly CharacterClassInfo RIDER = new(CharacterClass.RIDER, false, Race.ORC, CharacterClass.ORC_LANCER);
	public static readonly CharacterClassInfo DRAGOON = new(CharacterClass.DRAGOON, false, Race.ORC, CharacterClass.RIDER);
	public static readonly CharacterClassInfo VANGUARD_RIDER = new(CharacterClass.VANGUARD_RIDER, false, Race.ORC, CharacterClass.DRAGOON);

	public static readonly CharacterClassInfo ASSASSIN_MALE_0 = new(CharacterClass.ASSASSIN_MALE_0, false, Race.HUMAN, null);
	public static readonly CharacterClassInfo ASSASSIN_MALE_1 = new(CharacterClass.ASSASSIN_MALE_1, false, Race.HUMAN, CharacterClass.ASSASSIN_MALE_0);
	public static readonly CharacterClassInfo ASSASSIN_MALE_2 = new(CharacterClass.ASSASSIN_MALE_2, false, Race.HUMAN, CharacterClass.ASSASSIN_MALE_1);
	public static readonly CharacterClassInfo ASSASSIN_MALE_3 = new(CharacterClass.ASSASSIN_MALE_3, false, Race.HUMAN, CharacterClass.ASSASSIN_MALE_2);
	public static readonly CharacterClassInfo ASSASSIN_FEMALE_0 = new(CharacterClass.ASSASSIN_FEMALE_0, false, Race.DARK_ELF, null);
	public static readonly CharacterClassInfo ASSASSIN_FEMALE_1 = new(CharacterClass.ASSASSIN_FEMALE_1, false, Race.DARK_ELF, CharacterClass.ASSASSIN_FEMALE_0);
	public static readonly CharacterClassInfo ASSASSIN_FEMALE_2 = new(CharacterClass.ASSASSIN_FEMALE_2, false, Race.DARK_ELF, CharacterClass.ASSASSIN_FEMALE_1);
	public static readonly CharacterClassInfo ASSASSIN_FEMALE_3 = new(CharacterClass.ASSASSIN_FEMALE_3, false, Race.DARK_ELF, CharacterClass.ASSASSIN_FEMALE_2);

    public static readonly CharacterClassInfo ELEMENT_WEAVER_0 = new(CharacterClass.ELEMENT_WEAVER_0, true, Race.HIGH_ELF, null);
    public static readonly CharacterClassInfo ELEMENT_WEAVER_1 = new(CharacterClass.ELEMENT_WEAVER_1, true, Race.HIGH_ELF, CharacterClass.ELEMENT_WEAVER_0);
    public static readonly CharacterClassInfo ELEMENT_WEAVER_2 = new(CharacterClass.ELEMENT_WEAVER_2, true, Race.HIGH_ELF, CharacterClass.ELEMENT_WEAVER_1);
    public static readonly CharacterClassInfo ELEMENT_WEAVER_3 = new(CharacterClass.ELEMENT_WEAVER_3, true, Race.HIGH_ELF, CharacterClass.ELEMENT_WEAVER_2);
    public static readonly CharacterClassInfo DIVINE_TEMPLAR_0 = new(CharacterClass.DIVINE_TEMPLAR_0, false, Race.HIGH_ELF, null);
    public static readonly CharacterClassInfo DIVINE_TEMPLAR_1 = new(CharacterClass.DIVINE_TEMPLAR_1, false, Race.HIGH_ELF, CharacterClass.DIVINE_TEMPLAR_0);
    public static readonly CharacterClassInfo DIVINE_TEMPLAR_2 = new(CharacterClass.DIVINE_TEMPLAR_2, false, Race.HIGH_ELF, CharacterClass.DIVINE_TEMPLAR_1);
    public static readonly CharacterClassInfo DIVINE_TEMPLAR_3 = new(CharacterClass.DIVINE_TEMPLAR_3, false, Race.HIGH_ELF, CharacterClass.DIVINE_TEMPLAR_2);


	/** The Identifier of the Class */
	private readonly CharacterClass _id;

	/** True if the class is a mage class */
	private readonly bool _isMage;

	/** True if the class is a summoner class */
	private readonly bool _isSummoner;

	/** The Race object of the class */
	private readonly Race _race;

	/** The parent CharacterClass or null if this class is a root */
	private readonly CharacterClass? _parent;

	/** List of available Class for next transfer **/
	private readonly List<CharacterClassInfo> _nextClassIds = new();

	public static CharacterClassInfo GetClassInfo(CharacterClass cId)
	{
		return _classIdMap[cId];
	}

	/**
	 * Class constructor.
	 * @param pId the class Id.
	 * @param pIsMage {code true} if the class is mage class.
	 * @param race the race related to the class.
	 * @param pParent the parent class Id.
	 */
	private CharacterClassInfo(CharacterClass pId, bool pIsMage, Race race, CharacterClass? pParent)
		: this(pId, pIsMage, false, race, pParent)
	{
	}

	/**
	 * Class constructor.
	 * @param pId the class Id.
	 * @param pIsMage {code true} if the class is mage class.
	 * @param pIsSummoner {code true} if the class is summoner class.
	 * @param race the race related to the class.
	 * @param pParent the parent class Id.
	 */
	private CharacterClassInfo(CharacterClass pId, bool pIsMage, bool pIsSummoner, Race race, CharacterClass? pParent)
	{
		_id = pId;
		_isMage = pIsMage;
		_isSummoner = pIsSummoner;
		_race = race;
		_parent = pParent;

		_classIdMap[pId] = this;

		if (_parent != null)
		{
			_classIdMap[_parent.Value]._nextClassIds.Add(this);
		}
	}

	/**
	 * Gets the ID of the class.
	 * @return the ID of the class
	 */
	public CharacterClass getId()
	{
		return _id;
	}

	/**
	 * @return {code true} if the class is a mage class.
	 */
	public bool isMage()
	{
		return _isMage;
	}

	/**
	 * @return {code true} if the class is a summoner class.
	 */
	public bool isSummoner()
	{
		return _isSummoner;
	}

	/**
	 * @return the Race object of the class.
	 */
	public Race getRace()
	{
		return _race;
	}

	/**
	 * @param cid the parent CharacterClass to check.
	 * @return {code true} if this Class is a child of the selected CharacterClass.
	 */
	public bool childOf(CharacterClass cid)
	{
		if (_parent == null)
		{
			return false;
		}

		if (_parent == cid)
		{
			return true;
		}

		return _classIdMap[_parent.Value].childOf(cid);
	}

	/**
	 * @param cid the parent CharacterClass to check.
	 * @return {code true} if this Class is equal to the selected CharacterClass or a child of the selected CharacterClass.
	 */
	public bool equalsOrChildOf(CharacterClass cid)
	{
		return _id == cid || childOf(cid);
	}

	/**
	 * @return the child level of this Class (0=root, 1=child leve 1...)
	 */
	public int level()
	{
		if (_parent == null)
		{
			return 0;
		}

		return 1 + _classIdMap[_parent.Value].level();
	}

	/**
	 * @return its parent Class Id
	 */
	public CharacterClassInfo? getParent()
	{
		return _parent is null ? null : _classIdMap[_parent.Value];
	}

	public CharacterClassInfo getRootClassId()
	{
		if (_parent != null)
		{
			return _classIdMap[_parent.Value].getRootClassId();
		}

		return this;
	}

	/**
	 * @return list of possible class transfer for this class
	 */
	public IReadOnlyList<CharacterClassInfo> getNextClassIds()
	{
		return _nextClassIds;
	}
}