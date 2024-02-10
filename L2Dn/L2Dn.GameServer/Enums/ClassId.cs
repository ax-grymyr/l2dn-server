namespace L2Dn.GameServer.Enums;

/**
 * This class defines all classes (ex : human fighter, darkFighter...) that a player can chose.<br>
 * Data:
 * <ul>
 * <li>id : The Identifier of the class</li>
 * <li>isMage : True if the class is a mage class</li>
 * <li>race : The race of this class</li>
 * <li>parent : The parent ClassId or null if this class is the root</li>
 * </ul>
 * @version $Revision: 1.4.4.4 $ $Date: 2005/03/27 15:29:33 $
 */
public enum ClassId
{
	FIGHTER = 0,
	
	WARRIOR = 1,
	GLADIATOR = 2,
	WARLORD=3,  
	KNIGHT=4,   
	PALADIN=5,  
	DARK_AVENGER=6,   
	ROGUE=7,   
	TREASURE_HUNTER=8,   
	HAWKEYE=9,   
	
	MAGE=10,   
	WIZARD=11,   
	SORCERER=12,   
	NECROMANCER=13,   
	WARLOCK=14,    
	CLERIC=15,   
	BISHOP=16,   
	PROPHET=17,   
	
	ELVEN_FIGHTER=18,   
	ELVEN_KNIGHT=19,   
	TEMPLE_KNIGHT=20,   
	SWORDSINGER=21,   
	ELVEN_SCOUT=22,   
	PLAINS_WALKER=23,   
	SILVER_RANGER=24,   
	
	ELVEN_MAGE=25,   
	ELVEN_WIZARD=26,   
	SPELLSINGER=27,   
	ELEMENTAL_SUMMONER=28,    
	ORACLE=29,   
	ELDER=30,   
	
	DARK_FIGHTER=31,   
	PALUS_KNIGHT=32,   
	SHILLIEN_KNIGHT=33,   
	BLADEDANCER=34,   
	ASSASSIN=35,   
	ABYSS_WALKER=36,   
	PHANTOM_RANGER=37,   
	
	DARK_MAGE=38,   
	DARK_WIZARD=39,   
	SPELLHOWLER=40,   
	PHANTOM_SUMMONER=41,    
	SHILLIEN_ORACLE=42,   
	SHILLIEN_ELDER=43,   
	
	ORC_FIGHTER=44,   
	ORC_RAIDER=45,   
	DESTROYER=46,   
	ORC_MONK=47,   
	TYRANT=48,   
	
	ORC_MAGE=49,   
	ORC_SHAMAN=50,   
	OVERLORD=51,   
	WARCRYER=52,   
	
	DWARVEN_FIGHTER=53,   
	SCAVENGER=54,   
	BOUNTY_HUNTER=55,   
	ARTISAN=56,   
	WARSMITH=57,   
	
	DUELIST=88,   
	DREADNOUGHT=89,   
	PHOENIX_KNIGHT=90,   
	HELL_KNIGHT=91,   
	SAGITTARIUS=92,   
	ADVENTURER=93,   
	ARCHMAGE=94,   
	SOULTAKER=95,   
	ARCANA_LORD=96,    
	CARDINAL=97,   
	HIEROPHANT=98,   
	
	EVA_TEMPLAR=99,   
	SWORD_MUSE=100,   
	WIND_RIDER=101,   
	MOONLIGHT_SENTINEL=102,   
	MYSTIC_MUSE=103,   
	ELEMENTAL_MASTER=104,    
	EVA_SAINT=105,   
	
	SHILLIEN_TEMPLAR=106,   
	SPECTRAL_DANCER=107,   
	GHOST_HUNTER=108,   
	GHOST_SENTINEL=109,   
	STORM_SCREAMER=110,   
	SPECTRAL_MASTER=111,    
	SHILLIEN_SAINT=112,   
	
	TITAN=113,   
	GRAND_KHAVATARI=114,   
	DOMINATOR=115,   
	DOOMCRYER=116,   
	
	FORTUNE_SEEKER=117,   
	MAESTRO=118,   
	
	KAMAEL_SOLDIER=192,   
	
	TROOPER=125,   
	SOUL_FINDER=193,   
	WARDER=126,   
	
	BERSERKER=127,   
	SOUL_BREAKER=194,   
	SOUL_RANGER=130,   
	
	DOOMBRINGER=131,   
	SOUL_HOUND=195,   
	TRICKSTER=134,   
	
	DEATH_PILGRIM_HUMAN=196,   
	DEATH_BLADE_HUMAN=197,   
	DEATH_MESSENGER_HUMAN=198,   
	DEATH_KIGHT_HUMAN=199,   
	
	DEATH_PILGRIM_ELF=200,   
	DEATH_BLADE_ELF=201,   
	DEATH_MESSENGER_ELF=202,   
	DEATH_KIGHT_ELF=203,   
	
	DEATH_PILGRIM_DARK_ELF=204,   
	DEATH_BLADE_DARK_ELF=205,   
	DEATH_MESSENGER_DARK_ELF=206,   
	DEATH_KIGHT_DARK_ELF=207,   
	
	SYLPH_GUNNER=208,  
	SHARPSHOOTER=209,
	WIND_SNIPER=210, 
	STORM_BLASTER=211,
	
	ORC_LANCER=217,   
	RIDER=218,   
	DRAGOON=219,   
	VANGUARD_RIDER=220,   
	
	ASSASSIN_MALE_0=221,   
	ASSASSIN_MALE_1=222,   
	ASSASSIN_MALE_2=223,   
	ASSASSIN_MALE_3=224,   
	ASSASSIN_FEMALE_0=225,   
	ASSASSIN_FEMALE_1=226,   
	ASSASSIN_FEMALE_2=227,   
	ASSASSIN_FEMALE_3=228,
}


public class ClassIdInfo
{
	public static readonly ClassIdInfo FIGHTER = new(ClassId.FIGHTER, false, Race.HUMAN, null);
	
	public static readonly ClassIdInfo WARRIOR = new(ClassId.WARRIOR, false, Race.HUMAN, ClassId.FIGHTER);
	public static readonly ClassIdInfo GLADIATOR = new(ClassId.GLADIATOR, false, Race.HUMAN, ClassId.WARRIOR);
	public static readonly ClassIdInfo WARLORD = new(ClassId.WARLORD, false, Race.HUMAN, ClassId.WARRIOR);
	public static readonly ClassIdInfo KNIGHT = new(ClassId.KNIGHT, false, Race.HUMAN, ClassId.FIGHTER);
	public static readonly ClassIdInfo PALADIN = new(ClassId.PALADIN, false, Race.HUMAN, ClassId.KNIGHT);
	public static readonly ClassIdInfo DARK_AVENGER = new(ClassId.DARK_AVENGER, false, Race.HUMAN, ClassId.KNIGHT);
	public static readonly ClassIdInfo ROGUE = new(ClassId.ROGUE, false, Race.HUMAN, ClassId.FIGHTER);
	public static readonly ClassIdInfo TREASURE_HUNTER = new(ClassId.TREASURE_HUNTER, false, Race.HUMAN, ClassId.ROGUE);
	public static readonly ClassIdInfo HAWKEYE = new(ClassId.HAWKEYE, false, Race.HUMAN, ClassId.ROGUE);
	
	public static readonly ClassIdInfo MAGE = new(ClassId.MAGE, true, Race.HUMAN, null);
	public static readonly ClassIdInfo WIZARD = new(ClassId.WIZARD, true, Race.HUMAN, ClassId.MAGE);
	public static readonly ClassIdInfo SORCERER = new(ClassId.SORCERER, true, Race.HUMAN, ClassId.WIZARD);
	public static readonly ClassIdInfo NECROMANCER = new(ClassId.NECROMANCER, true, Race.HUMAN, ClassId.WIZARD);
	public static readonly ClassIdInfo WARLOCK = new(ClassId.WARLOCK, true, true, Race.HUMAN, ClassId.WIZARD);
	public static readonly ClassIdInfo CLERIC = new(ClassId.CLERIC, true, Race.HUMAN, ClassId.MAGE);
	public static readonly ClassIdInfo BISHOP = new(ClassId.BISHOP, true, Race.HUMAN, ClassId.CLERIC);
	public static readonly ClassIdInfo PROPHET = new(ClassId.PROPHET, true, Race.HUMAN, ClassId.CLERIC);
	
	public static readonly ClassIdInfo ELVEN_FIGHTER = new(ClassId.ELVEN_FIGHTER, false, Race.ELF, null);
	public static readonly ClassIdInfo ELVEN_KNIGHT = new(ClassId.ELVEN_KNIGHT, false, Race.ELF, ClassId.ELVEN_FIGHTER);
	public static readonly ClassIdInfo TEMPLE_KNIGHT = new(ClassId.TEMPLE_KNIGHT, false, Race.ELF, ClassId.ELVEN_KNIGHT);
	public static readonly ClassIdInfo SWORDSINGER = new(ClassId.SWORDSINGER, false, Race.ELF, ClassId.ELVEN_KNIGHT);
	public static readonly ClassIdInfo ELVEN_SCOUT = new(ClassId.ELVEN_SCOUT, false, Race.ELF, ClassId.ELVEN_FIGHTER);
	public static readonly ClassIdInfo PLAINS_WALKER = new(ClassId.PLAINS_WALKER, false, Race.ELF, ClassId.ELVEN_SCOUT);
	public static readonly ClassIdInfo SILVER_RANGER = new(ClassId.SILVER_RANGER, false, Race.ELF, ClassId.ELVEN_SCOUT);
	
	public static readonly ClassIdInfo ELVEN_MAGE = new(ClassId.ELVEN_MAGE, true, Race.ELF, null);
	public static readonly ClassIdInfo ELVEN_WIZARD = new(ClassId.ELVEN_WIZARD, true, Race.ELF, ClassId.ELVEN_MAGE);
	public static readonly ClassIdInfo SPELLSINGER = new(ClassId.SPELLSINGER, true, Race.ELF, ClassId.ELVEN_WIZARD);

	public static readonly ClassIdInfo ELEMENTAL_SUMMONER =
		new(ClassId.ELEMENTAL_SUMMONER, true, true, Race.ELF, ClassId.ELVEN_WIZARD);
	public static readonly ClassIdInfo ORACLE = new(ClassId.ORACLE, true, Race.ELF, ClassId.ELVEN_MAGE);
	public static readonly ClassIdInfo ELDER = new(ClassId.ELDER, true, Race.ELF, ClassId.ORACLE);
	
	public static readonly ClassIdInfo DARK_FIGHTER = new(ClassId.DARK_FIGHTER, false, Race.DARK_ELF, null);
	public static readonly ClassIdInfo PALUS_KNIGHT = new(ClassId.PALUS_KNIGHT, false, Race.DARK_ELF, ClassId.DARK_FIGHTER);
	public static readonly ClassIdInfo SHILLIEN_KNIGHT = new(ClassId.SHILLIEN_KNIGHT, false, Race.DARK_ELF, ClassId.PALUS_KNIGHT);
	public static readonly ClassIdInfo BLADEDANCER = new(ClassId.BLADEDANCER, false, Race.DARK_ELF, ClassId.PALUS_KNIGHT);
	public static readonly ClassIdInfo ASSASSIN = new(ClassId.ASSASSIN, false, Race.DARK_ELF, ClassId.DARK_FIGHTER);
	public static readonly ClassIdInfo ABYSS_WALKER = new(ClassId.ABYSS_WALKER, false, Race.DARK_ELF, ClassId.ASSASSIN);
	public static readonly ClassIdInfo PHANTOM_RANGER = new(ClassId.PHANTOM_RANGER, false, Race.DARK_ELF, ClassId.ASSASSIN);
	
	public static readonly ClassIdInfo DARK_MAGE = new(ClassId.DARK_MAGE, true, Race.DARK_ELF, null);
	public static readonly ClassIdInfo DARK_WIZARD = new(ClassId.DARK_WIZARD, true, Race.DARK_ELF, ClassId.DARK_MAGE);
	public static readonly ClassIdInfo SPELLHOWLER = new(ClassId.SPELLHOWLER, true, Race.DARK_ELF, ClassId.DARK_WIZARD);

	public static readonly ClassIdInfo PHANTOM_SUMMONER =
		new(ClassId.PHANTOM_SUMMONER, true, true, Race.DARK_ELF, ClassId.DARK_WIZARD);
	public static readonly ClassIdInfo SHILLIEN_ORACLE = new(ClassId.SHILLIEN_ORACLE, true, Race.DARK_ELF, ClassId.DARK_MAGE);
	public static readonly ClassIdInfo SHILLIEN_ELDER = new(ClassId.SHILLIEN_ELDER, true, Race.DARK_ELF, ClassId.SHILLIEN_ORACLE);
	
	public static readonly ClassIdInfo ORC_FIGHTER = new(ClassId.ORC_FIGHTER, false, Race.ORC, null);
	public static readonly ClassIdInfo ORC_RAIDER = new(ClassId.ORC_RAIDER, false, Race.ORC, ClassId.ORC_FIGHTER);
	public static readonly ClassIdInfo DESTROYER = new(ClassId.DESTROYER, false, Race.ORC, ClassId.ORC_RAIDER);
	public static readonly ClassIdInfo ORC_MONK = new(ClassId.ORC_MONK, false, Race.ORC, ClassId.ORC_FIGHTER);
	public static readonly ClassIdInfo TYRANT = new(ClassId.TYRANT, false, Race.ORC, ClassId.ORC_MONK);
	
	public static readonly ClassIdInfo ORC_MAGE = new(ClassId.ORC_MAGE, true, Race.ORC, null);
	public static readonly ClassIdInfo ORC_SHAMAN = new(ClassId.ORC_SHAMAN, true, Race.ORC, ClassId.ORC_MAGE);
	public static readonly ClassIdInfo OVERLORD = new(ClassId.OVERLORD, true, Race.ORC, ClassId.ORC_SHAMAN);
	public static readonly ClassIdInfo WARCRYER = new(ClassId.WARCRYER, true, Race.ORC, ClassId.ORC_SHAMAN);
	
	public static readonly ClassIdInfo DWARVEN_FIGHTER = new(ClassId.DWARVEN_FIGHTER, false, Race.DWARF, null);
	public static readonly ClassIdInfo SCAVENGER = new(ClassId.SCAVENGER, false, Race.DWARF, ClassId.DWARVEN_FIGHTER);
	public static readonly ClassIdInfo BOUNTY_HUNTER = new(ClassId.BOUNTY_HUNTER, false, Race.DWARF, ClassId.SCAVENGER);
	public static readonly ClassIdInfo ARTISAN = new(ClassId.ARTISAN, false, Race.DWARF, ClassId.DWARVEN_FIGHTER);
	public static readonly ClassIdInfo WARSMITH = new(ClassId.WARSMITH, false, Race.DWARF, ClassId.ARTISAN);
	
	public static readonly ClassIdInfo DUELIST = new(ClassId.DUELIST, false, Race.HUMAN, ClassId.GLADIATOR);
	public static readonly ClassIdInfo DREADNOUGHT = new(ClassId.DREADNOUGHT, false, Race.HUMAN, ClassId.WARLORD);
	public static readonly ClassIdInfo PHOENIX_KNIGHT = new(ClassId.PHOENIX_KNIGHT, false, Race.HUMAN, ClassId.PALADIN);
	public static readonly ClassIdInfo HELL_KNIGHT = new(ClassId.HELL_KNIGHT, false, Race.HUMAN, ClassId.DARK_AVENGER);
	public static readonly ClassIdInfo SAGITTARIUS = new(ClassId.SAGITTARIUS, false, Race.HUMAN, ClassId.HAWKEYE);
	public static readonly ClassIdInfo ADVENTURER = new(ClassId.ADVENTURER, false, Race.HUMAN, ClassId.TREASURE_HUNTER);
	public static readonly ClassIdInfo ARCHMAGE = new(ClassId.ARCHMAGE, true, Race.HUMAN, ClassId.SORCERER);
	public static readonly ClassIdInfo SOULTAKER = new(ClassId.SOULTAKER, true, Race.HUMAN, ClassId.NECROMANCER);
	public static readonly ClassIdInfo ARCANA_LORD = new(ClassId.ARCANA_LORD, true, true, Race.HUMAN, ClassId.WARLOCK);
	public static readonly ClassIdInfo CARDINAL = new(ClassId.CARDINAL, true, Race.HUMAN, ClassId.BISHOP);
	public static readonly ClassIdInfo HIEROPHANT = new(ClassId.HIEROPHANT, true, Race.HUMAN, ClassId.PROPHET);
	
	public static readonly ClassIdInfo EVA_TEMPLAR = new(ClassId.EVA_TEMPLAR, false, Race.ELF, ClassId.TEMPLE_KNIGHT);
	public static readonly ClassIdInfo SWORD_MUSE = new(ClassId.SWORD_MUSE, false, Race.ELF, ClassId.SWORDSINGER);
	public static readonly ClassIdInfo WIND_RIDER = new(ClassId.WIND_RIDER, false, Race.ELF, ClassId.PLAINS_WALKER);
	public static readonly ClassIdInfo MOONLIGHT_SENTINEL = new(ClassId.MOONLIGHT_SENTINEL, false, Race.ELF, ClassId.SILVER_RANGER);
	public static readonly ClassIdInfo MYSTIC_MUSE = new(ClassId.MYSTIC_MUSE, true, Race.ELF, ClassId.SPELLSINGER);

	public static readonly ClassIdInfo ELEMENTAL_MASTER =
		new(ClassId.ELEMENTAL_MASTER, true, true, Race.ELF, ClassId.ELEMENTAL_SUMMONER);
	public static readonly ClassIdInfo EVA_SAINT = new(ClassId.EVA_SAINT, true, Race.ELF, ClassId.ELDER);
	
	public static readonly ClassIdInfo SHILLIEN_TEMPLAR = new(ClassId.SHILLIEN_TEMPLAR, false, Race.DARK_ELF, ClassId.SHILLIEN_KNIGHT);
	public static readonly ClassIdInfo SPECTRAL_DANCER = new(ClassId.SPECTRAL_DANCER, false, Race.DARK_ELF, ClassId.BLADEDANCER);
	public static readonly ClassIdInfo GHOST_HUNTER = new(ClassId.GHOST_HUNTER, false, Race.DARK_ELF, ClassId.ABYSS_WALKER);
	public static readonly ClassIdInfo GHOST_SENTINEL = new(ClassId.GHOST_SENTINEL, false, Race.DARK_ELF, ClassId.PHANTOM_RANGER);
	public static readonly ClassIdInfo STORM_SCREAMER = new(ClassId.STORM_SCREAMER, true, Race.DARK_ELF, ClassId.SPELLHOWLER);

	public static readonly ClassIdInfo SPECTRAL_MASTER =
		new(ClassId.SPECTRAL_MASTER, true, true, Race.DARK_ELF, ClassId.PHANTOM_SUMMONER);
	public static readonly ClassIdInfo SHILLIEN_SAINT = new(ClassId.SHILLIEN_SAINT, true, Race.DARK_ELF, ClassId.SHILLIEN_ELDER);
	
	public static readonly ClassIdInfo TITAN = new(ClassId.TITAN, false, Race.ORC, ClassId.DESTROYER);
	public static readonly ClassIdInfo GRAND_KHAVATARI = new(ClassId.GRAND_KHAVATARI, false, Race.ORC, ClassId.TYRANT);
	public static readonly ClassIdInfo DOMINATOR = new(ClassId.DOMINATOR, true, Race.ORC, ClassId.OVERLORD);
	public static readonly ClassIdInfo DOOMCRYER = new(ClassId.DOOMCRYER, true, Race.ORC, ClassId.WARCRYER);
	
	public static readonly ClassIdInfo FORTUNE_SEEKER = new(ClassId.FORTUNE_SEEKER, false, Race.DWARF, ClassId.BOUNTY_HUNTER);
	public static readonly ClassIdInfo MAESTRO = new(ClassId.MAESTRO, false, Race.DWARF, ClassId.WARSMITH);
	
	public static readonly ClassIdInfo KAMAEL_SOLDIER = new(ClassId.KAMAEL_SOLDIER, false, Race.KAMAEL, null);
	
	public static readonly ClassIdInfo TROOPER = new(ClassId.TROOPER, false, Race.KAMAEL, ClassId.KAMAEL_SOLDIER);
	public static readonly ClassIdInfo SOUL_FINDER = new(ClassId.SOUL_FINDER, false, Race.KAMAEL, ClassId.KAMAEL_SOLDIER);
	public static readonly ClassIdInfo WARDER = new(ClassId.WARDER, false, Race.KAMAEL, ClassId.KAMAEL_SOLDIER);
	
	public static readonly ClassIdInfo BERSERKER = new(ClassId.BERSERKER, false, Race.KAMAEL, ClassId.TROOPER);
	public static readonly ClassIdInfo SOUL_BREAKER = new(ClassId.SOUL_BREAKER, false, Race.KAMAEL, ClassId.SOUL_FINDER);
	public static readonly ClassIdInfo SOUL_RANGER = new(ClassId.SOUL_RANGER, false, Race.KAMAEL, ClassId.WARDER);
	
	public static readonly ClassIdInfo DOOMBRINGER = new(ClassId.DOOMBRINGER, false, Race.KAMAEL, ClassId.BERSERKER);
	public static readonly ClassIdInfo SOUL_HOUND = new(ClassId.SOUL_HOUND, false, Race.KAMAEL, ClassId.SOUL_BREAKER);
	public static readonly ClassIdInfo TRICKSTER = new(ClassId.TRICKSTER, false, Race.KAMAEL, ClassId.SOUL_RANGER);
	
	public static readonly ClassIdInfo DEATH_PILGRIM_HUMAN = new(ClassId.DEATH_PILGRIM_HUMAN, false, Race.HUMAN, null);
	public static readonly ClassIdInfo DEATH_BLADE_HUMAN = new(ClassId.DEATH_BLADE_HUMAN, false, Race.HUMAN, ClassId.DEATH_PILGRIM_HUMAN);
	public static readonly ClassIdInfo DEATH_MESSENGER_HUMAN = new(ClassId.DEATH_MESSENGER_HUMAN, false, Race.HUMAN, ClassId.DEATH_BLADE_HUMAN);
	public static readonly ClassIdInfo DEATH_KIGHT_HUMAN = new(ClassId.DEATH_KIGHT_HUMAN, false, Race.HUMAN, ClassId.DEATH_MESSENGER_HUMAN);
	
	public static readonly ClassIdInfo DEATH_PILGRIM_ELF = new(ClassId.DEATH_PILGRIM_ELF, false, Race.ELF, null);
	public static readonly ClassIdInfo DEATH_BLADE_ELF = new(ClassId.DEATH_BLADE_ELF, false, Race.ELF, ClassId.DEATH_PILGRIM_ELF);
	public static readonly ClassIdInfo DEATH_MESSENGER_ELF = new(ClassId.DEATH_MESSENGER_ELF, false, Race.ELF, ClassId.DEATH_BLADE_ELF);
	public static readonly ClassIdInfo DEATH_KIGHT_ELF = new(ClassId.DEATH_KIGHT_ELF, false, Race.ELF, ClassId.DEATH_MESSENGER_ELF);
	
	public static readonly ClassIdInfo DEATH_PILGRIM_DARK_ELF = new(ClassId.DEATH_PILGRIM_DARK_ELF, false, Race.DARK_ELF, null);
	public static readonly ClassIdInfo DEATH_BLADE_DARK_ELF = new(ClassId.DEATH_BLADE_DARK_ELF, false, Race.DARK_ELF, ClassId.DEATH_PILGRIM_DARK_ELF);
	public static readonly ClassIdInfo DEATH_MESSENGER_DARK_ELF = new(ClassId.DEATH_MESSENGER_DARK_ELF, false, Race.DARK_ELF, ClassId.DEATH_BLADE_DARK_ELF);
	public static readonly ClassIdInfo DEATH_KIGHT_DARK_ELF = new(ClassId.DEATH_KIGHT_DARK_ELF, false, Race.DARK_ELF, ClassId.DEATH_MESSENGER_DARK_ELF);
	
	public static readonly ClassIdInfo SYLPH_GUNNER = new(ClassId.SYLPH_GUNNER, false, Race.SYLPH, null);
	public static readonly ClassIdInfo SHARPSHOOTER = new(ClassId.SHARPSHOOTER, false, Race.SYLPH, ClassId.SYLPH_GUNNER);
	public static readonly ClassIdInfo WIND_SNIPER = new(ClassId.WIND_SNIPER, false, Race.SYLPH, ClassId.SHARPSHOOTER);
	public static readonly ClassIdInfo STORM_BLASTER = new(ClassId.STORM_BLASTER, false, Race.SYLPH, ClassId.WIND_SNIPER);
	
	public static readonly ClassIdInfo ORC_LANCER = new(ClassId.ORC_LANCER, false, Race.ORC, null);
	public static readonly ClassIdInfo RIDER = new(ClassId.RIDER, false, Race.ORC, ClassId.ORC_LANCER);
	public static readonly ClassIdInfo DRAGOON = new(ClassId.DRAGOON, false, Race.ORC, ClassId.RIDER);
	public static readonly ClassIdInfo VANGUARD_RIDER = new(ClassId.VANGUARD_RIDER, false, Race.ORC, ClassId.DRAGOON);
	
	public static readonly ClassIdInfo ASSASSIN_MALE_0 = new(ClassId.ASSASSIN_MALE_0, false, Race.HUMAN, null);
	public static readonly ClassIdInfo ASSASSIN_MALE_1 = new(ClassId.ASSASSIN_MALE_1, false, Race.HUMAN, ClassId.ASSASSIN_MALE_0);
	public static readonly ClassIdInfo ASSASSIN_MALE_2 = new(ClassId.ASSASSIN_MALE_2, false, Race.HUMAN, ClassId.ASSASSIN_MALE_1);
	public static readonly ClassIdInfo ASSASSIN_MALE_3 = new(ClassId.ASSASSIN_MALE_3, false, Race.HUMAN, ClassId.ASSASSIN_MALE_2);
	public static readonly ClassIdInfo ASSASSIN_FEMALE_0 = new(ClassId.ASSASSIN_FEMALE_0, false, Race.DARK_ELF, null);
	public static readonly ClassIdInfo ASSASSIN_FEMALE_1 = new(ClassId.ASSASSIN_FEMALE_1, false, Race.DARK_ELF, ClassId.ASSASSIN_FEMALE_0);
	public static readonly ClassIdInfo ASSASSIN_FEMALE_2 = new(ClassId.ASSASSIN_FEMALE_2, false, Race.DARK_ELF, ClassId.ASSASSIN_FEMALE_1);
	public static readonly ClassIdInfo ASSASSIN_FEMALE_3=new(ClassId.ASSASSIN_FEMALE_3, false, Race.DARK_ELF, ClassId.ASSASSIN_FEMALE_2);
	
	/** The Identifier of the Class */
	private readonly ClassId _id;
	
	/** True if the class is a mage class */
	private readonly bool _isMage;
	
	/** True if the class is a summoner class */
	private readonly bool _isSummoner;
	
	/** The Race object of the class */
	private readonly Race _race;
	
	/** The parent ClassId or null if this class is a root */
	private readonly ClassId? _parent;
	
	/** List of available Class for next transfer **/
	private readonly List<ClassIdInfo> _nextClassIds = new();
	
	private static Dictionary<ClassId, ClassIdInfo> _classIdMap = new();
	
	public static ClassIdInfo getClassId(ClassId cId)
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
	private ClassIdInfo(ClassId pId, bool pIsMage, Race race, ClassId? pParent)
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
	private ClassIdInfo(ClassId pId, bool pIsMage, bool pIsSummoner, Race race, ClassId? pParent)
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
	public ClassId getId()
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
	 * @param cid the parent ClassId to check.
	 * @return {code true} if this Class is a child of the selected ClassId.
	 */
	public bool childOf(ClassId cid)
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
	 * @param cid the parent ClassId to check.
	 * @return {code true} if this Class is equal to the selected ClassId or a child of the selected ClassId.
	 */
	public bool equalsOrChildOf(ClassId cid)
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
	public ClassIdInfo? getParent()
	{
		return _parent is null ? null : _classIdMap[_parent.Value];
	}
	
	public ClassIdInfo getRootClassId()
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
	public IReadOnlyList<ClassIdInfo> getNextClassIds()
	{
		return _nextClassIds;
	}
}