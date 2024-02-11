using System.Runtime.CompilerServices;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.CommunityBbs.BB;
using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Appearance;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Status;
using L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Cubics;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Events.Returns;
using L2Dn.GameServer.Model.Events.Timers;
using L2Dn.GameServer.Model.Fishings;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Model.Vips;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using FortManager = L2Dn.GameServer.Model.Actor.Instances.FortManager;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Actor;

/**
 * This class represents all player characters in the world.<br>
 * There is always a client-thread connected to this (except if a player-store is activated upon logout).
 */
public class Player: Playable
{
	// Character Skill SQL String Definitions:
	private const string RESTORE_SKILLS_FOR_CHAR = "SELECT skill_id,skill_level,skill_sub_level FROM character_skills WHERE charId=? AND class_index=?";
	private const string UPDATE_CHARACTER_SKILL_LEVEL = "UPDATE character_skills SET skill_level=?, skill_sub_level=?  WHERE skill_id=? AND charId=? AND class_index=?";
	private const string ADD_NEW_SKILLS = "REPLACE INTO character_skills (charId,skill_id,skill_level,skill_sub_level,class_index) VALUES (?,?,?,?,?)";
	private const string DELETE_SKILL_FROM_CHAR = "DELETE FROM character_skills WHERE skill_id=? AND charId=? AND class_index=?";
	private const string DELETE_CHAR_SKILLS = "DELETE FROM character_skills WHERE charId=? AND class_index=?";
	
	// Character Skill Save SQL String Definitions:
	private const string ADD_SKILL_SAVE = "REPLACE INTO character_skills_save (charId,skill_id,skill_level,skill_sub_level,remaining_time,reuse_delay,systime,restore_type,class_index,buff_index) VALUES (?,?,?,?,?,?,?,?,?,?)";
	private const string RESTORE_SKILL_SAVE = "SELECT skill_id,skill_level,skill_sub_level,remaining_time, reuse_delay, systime, restore_type FROM character_skills_save WHERE charId=? AND class_index=? ORDER BY buff_index ASC";
	private const string DELETE_SKILL_SAVE = "DELETE FROM character_skills_save WHERE charId=? AND class_index=?";
	
	// Character Item Reuse Time String Definition:
	private const string ADD_ITEM_REUSE_SAVE = "INSERT INTO character_item_reuse_save (charId,itemId,itemObjId,reuseDelay,systime) VALUES (?,?,?,?,?)";
	private const string RESTORE_ITEM_REUSE_SAVE = "SELECT charId,itemId,itemObjId,reuseDelay,systime FROM character_item_reuse_save WHERE charId=?";
	private const string DELETE_ITEM_REUSE_SAVE = "DELETE FROM character_item_reuse_save WHERE charId=?";
	
	// Character Character SQL String Definitions:
	private const string INSERT_CHARACTER = "INSERT INTO characters (account_name,charId,char_name,level,maxHp,curHp,maxCp,curCp,maxMp,curMp,face,hairStyle,hairColor,sex,exp,sp,reputation,fame,raidbossPoints,pvpkills,pkkills,clanid,race,classid,deletetime,cancraft,title,title_color,online,clan_privs,wantspeace,base_class,nobless,power_grade,vitality_points,createDate,kills,deaths) values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
	private const string UPDATE_CHARACTER = "UPDATE characters SET level=?,maxHp=?,curHp=?,maxCp=?,curCp=?,maxMp=?,curMp=?,face=?,hairStyle=?,hairColor=?,sex=?,heading=?,x=?,y=?,z=?,exp=?,expBeforeDeath=?,sp=?,reputation=?,fame=?,raidbossPoints=?,pvpkills=?,pkkills=?,clanid=?,race=?,classid=?,deletetime=?,title=?,title_color=?,online=?,clan_privs=?,wantspeace=?,base_class=?,onlinetime=?,nobless=?,power_grade=?,subpledge=?,lvl_joined_academy=?,apprentice=?,sponsor=?,clan_join_expiry_time=?,clan_create_expiry_time=?,char_name=?,bookmarkslot=?,vitality_points=?,language=?,faction=?,pccafe_points=?,kills=?,deaths=? WHERE charId=?";
	private const string UPDATE_CHARACTER_ACCESS = "UPDATE characters SET accesslevel = ? WHERE charId = ?";
	private const string RESTORE_CHARACTER = "SELECT * FROM characters WHERE charId=?";
	
	// Character Teleport Bookmark:
	private const string INSERT_TP_BOOKMARK = "INSERT INTO character_tpbookmark (charId,Id,x,y,z,icon,tag,name) values (?,?,?,?,?,?,?,?)";
	private const string UPDATE_TP_BOOKMARK = "UPDATE character_tpbookmark SET icon=?,tag=?,name=? where charId=? AND Id=?";
	private const string RESTORE_TP_BOOKMARK = "SELECT Id,x,y,z,icon,tag,name FROM character_tpbookmark WHERE charId=?";
	private const string DELETE_TP_BOOKMARK = "DELETE FROM character_tpbookmark WHERE charId=? AND Id=?";
	
	// Character Subclass SQL String Definitions:
	private const string RESTORE_CHAR_SUBCLASSES = "SELECT class_id,exp,sp,level,vitality_points,class_index,dual_class FROM character_subclasses WHERE charId=? ORDER BY class_index ASC";
	private const string ADD_CHAR_SUBCLASS = "INSERT INTO character_subclasses (charId,class_id,exp,sp,level,vitality_points,class_index,dual_class) VALUES (?,?,?,?,?,?,?,?)";
	private const string UPDATE_CHAR_SUBCLASS = "UPDATE character_subclasses SET exp=?,sp=?,level=?,vitality_points=?,class_id=?,dual_class=? WHERE charId=? AND class_index =?";
	private const string DELETE_CHAR_SUBCLASS = "DELETE FROM character_subclasses WHERE charId=? AND class_index=?";
	
	// Character Henna SQL String Definitions:
	private const string RESTORE_CHAR_HENNAS = "SELECT slot,symbol_id FROM character_hennas WHERE charId=? AND class_index=?";
	private const string ADD_CHAR_HENNA = "INSERT INTO character_hennas (charId,symbol_id,slot,class_index) VALUES (?,?,?,?)";
	private const string DELETE_CHAR_HENNA = "DELETE FROM character_hennas WHERE charId=? AND slot=? AND class_index=?";
	private const string ADD_CHAR_HENNA_POTENS = "REPLACE INTO character_potens (charId,slot_position,poten_id,enchant_level,enchant_exp) VALUES (?,?,?,?,?)";
	private const string RESTORE_CHAR_HENNA_POTENS = "SELECT slot_position,poten_id,enchant_level,enchant_exp FROM character_potens WHERE charId=?";
	
	// Character Shortcut SQL String Definitions:
	private const string DELETE_CHAR_SHORTCUTS = "DELETE FROM character_shortcuts WHERE charId=? AND class_index=?";
	
	// Character Collections list:
	private const string INSERT_COLLECTION = "REPLACE INTO collections (`accountName`, `itemId`, `collectionId`, `index`) VALUES (?, ?, ?, ?)";
	private const string RESTORE_COLLECTION = "SELECT * FROM collections WHERE accountName=? ORDER BY `index`";
	private const string DELETE_COLLECTION_FAVORITE = "DELETE FROM collection_favorites WHERE accountName=?";
	private const string INSERT_COLLECTION_FAVORITE = "REPLACE INTO collection_favorites (`accountName`, `collectionId`) VALUES (?, ?)";
	private const string RESTORE_COLLECTION_FAVORITE = "SELECT * FROM collection_favorites WHERE accountName=?";
	
	// Character Recipe List Save:
	private const string DELETE_CHAR_RECIPE_SHOP = "DELETE FROM character_recipeshoplist WHERE charId=?";
	private const string INSERT_CHAR_RECIPE_SHOP = "REPLACE INTO character_recipeshoplist (`charId`, `recipeId`, `price`, `index`) VALUES (?, ?, ?, ?)";
	private const string RESTORE_CHAR_RECIPE_SHOP = "SELECT * FROM character_recipeshoplist WHERE charId=? ORDER BY `index`";
	
	// Purge list:
	private const string DELETE_SUBJUGATION = "DELETE FROM character_purge WHERE charId=?";
	private const string INSERT_SUBJUGATION = "REPLACE INTO character_purge (`charId`, `category`, `points`, `keys`, `remainingKeys`) VALUES (?, ?, ?, ?, ?)";
	private const string RESTORE_SUBJUGATION = "SELECT * FROM character_purge WHERE charId=?";
	
	// Elemental Spirits:
	private const string RESTORE_ELEMENTAL_SPIRITS = "SELECT * FROM character_spirits WHERE charId=?";
	
	private const string COND_OVERRIDE_KEY = "cond_override";
	
	public const String NEWBIE_KEY = "NEWBIE";
	
	public const int ID_NONE = -1;
	
	public const int REQUEST_TIMEOUT = 15;
	
	private int _pcCafePoints = 0;
	
	private GameClient _client;
	private String _ip = "N/A";
	
	private readonly String _accountName;
	private long _deleteTimer;
	private DateTime _createDate = DateTime.Now;
	
	private String _lang = null;
	private String _htmlPrefix = "";
	
	private volatile bool _isOnline = false;
	private bool _offlinePlay = false;
	private bool _enteredWorld = false;
	private long _onlineTime;
	private long _onlineBeginTime;
	private long _lastAccess;
	private long _uptime;
	
	private ScheduledFuture _itemListTask;
	private ScheduledFuture _skillListTask;
	private ScheduledFuture _storageCountTask;
	private ScheduledFuture _userBoostStatTask;
	private ScheduledFuture _abnormalVisualEffectTask;
	private ScheduledFuture _updateAndBroadcastStatusTask;
	private ScheduledFuture _broadcastCharInfoTask;
	
	private bool _subclassLock = false;
	protected int _baseClass;
	protected int _activeClass;
	protected int _classIndex = 0;
	private bool _isDeathKnight = false;
	private bool _isVanguard = false;
	private bool _isAssassin = false;
	
	/** data for mounted pets */
	private int _controlItemId;
	private PetData _data;
	private PetLevelData _leveldata;
	private int _curFeed;
	protected ScheduledFuture _mountFeedTask;
	private ScheduledFuture _dismountTask;
	private bool _petItems = false;
	
	/** The list of sub-classes this character has. */
	private readonly Map<int, SubClassHolder> _subClasses = new();
	
	private readonly PlayerAppearance _appearance;
	
	/** The Experience of the Player before the last Death Penalty */
	private long _expBeforeDeath;
	
	/** The number of player killed during a PvP (the player killed was PvP Flagged) */
	private int _pvpKills;
	
	/** The PK counter of the Player (= Number of non PvP Flagged player killed) */
	private int _pkKills;
	
	/** The total kill counter of the Player */
	private int _totalKills = 0;
	
	/** The total death counter of the Player */
	private int _totalDeaths = 0;
	
	/** The PvP Flag state of the Player (0=White, 1=Purple) */
	private byte _pvpFlag;
	
	private int _einhasadOverseeingLevel = 0;
	
	private readonly List<DamageTakenHolder> _lastDamageTaken = new();
	
	/** The Fame of this Player */
	private int _fame;
	private ScheduledFuture _fameTask;
	
	/** The Raidboss points of this Player */
	private int _raidbossPoints;
	
	private ScheduledFuture _teleportWatchdog;
	
	/** The Siege state of the Player */
	private byte _siegeState = 0;
	
	/** The id of castle/fort which the Player is registered for siege */
	private int _siegeSide = 0;
	
	private int _curWeightPenalty = 0;
	
	private int _lastCompassZone; // the last compass zone update send to the client
	
	private readonly ContactList _contactList = new ContactList(this);
	
	private int _bookmarkslot = 0; // The Teleport Bookmark Slot
	
	private readonly Map<int, TeleportBookmark> _tpbookmarks = new();
	
	private bool _canFeed;
	private bool _isInSiege;
	private bool _isInHideoutSiege = false;
	
	/** Olympiad */
	private bool _inOlympiadMode = false;
	private bool _olympiadStart = false;
	private int _olympiadGameId = -1;
	private int _olympiadSide = -1;
	
	/** Duel */
	private bool _isInDuel = false;
	private bool _startingDuel = false;
	private int _duelState = Duel.DUELSTATE_NODUEL;
	private int _duelId = 0;
	private SystemMessageId _noDuelReason = SystemMessageId.THERE_IS_NO_OPPONENT_TO_RECEIVE_YOUR_CHALLENGE_FOR_A_DUEL;
	
	/** Boat and AirShip */
	private Vehicle _vehicle = null;
	private Location _inVehiclePosition;
	
	private MountType _mountType = MountType.NONE;
	private int _mountNpcId;
	private int _mountLevel;
	/** Store object used to summon the strider you are mounting **/
	private int _mountObjectID = 0;
	
	private AdminTeleportType _teleportType = AdminTeleportType.NORMAL;
	
	private bool _inCrystallize;
	private bool _isCrafting;
	
	private long _offlineShopStart = 0;
	
	/** The table containing all RecipeList of the Player */
	private readonly Map<int, RecipeList> _dwarvenRecipeBook = new();
	private readonly Map<int, RecipeList> _commonRecipeBook = new();
	
	/** Premium Items */
	private readonly Map<int, PremiumItem> _premiumItems = new();
	
	/** True if the Player is sitting */
	private bool _waitTypeSitting = false;
	
	/** Location before entering Observer Mode */
	private Location _lastLoc;
	private bool _observerMode = false;
	
	private Location _teleportLocation;
	
	/** Stored from last ValidatePosition **/
	private readonly Location _lastServerPosition = new Location(0, 0, 0);
	
	private readonly AtomicBoolean _blinkActive = new AtomicBoolean();
	
	/** The number of recommendation obtained by the Player */
	private int _recomHave; // how much I was recommended by others
	/** The number of recommendation that the Player can give */
	private int _recomLeft; // how many recommendations I can give to others
	/** Recommendation task **/
	private ScheduledFuture _recoGiveTask;
	/** Recommendation Two Hours bonus **/
	protected bool _recoTwoHoursGiven = false;
	
	private ScheduledFuture _onlineTimeUpdateTask;
	
	private readonly PlayerInventory _inventory = new PlayerInventory(this);
	private readonly PlayerFreight _freight = new PlayerFreight(this);
	private readonly PlayerWarehouse _warehouse = new PlayerWarehouse(this);
	private PlayerRefund _refund;
	private PrivateStoreType _privateStoreType = PrivateStoreType.NONE;
	private TradeList _activeTradeList;
	private ItemContainer _activeWarehouse;
	private Map<int, ManufactureItem> _manufactureItems;
	private String _storeName = "";
	private TradeList _sellList;
	private TradeList _buyList;
	
	// Multisell
	private PreparedMultisellListHolder _currentMultiSell = null;
	
	private bool _noble = false;
	private bool _hero = false;
	private bool _trueHero = false;
	
	/** Premium System */
	private bool _premiumStatus = false;
	
	/** Faction System */
	private bool _isGood = false;
	private bool _isEvil = false;
	
	/** The Npc corresponding to the last Folk which one the player talked. */
	private Npc _lastFolkNpc = null;
	
	/** Last NPC Id talked on a quest */
	private int _questNpcObject = 0;
	
	/** Used for simulating Quest onTalk */
	private bool _simulatedTalking = false;
	
	/** The table containing all Quests began by the Player */
	private readonly Map<String, QuestState> _quests = new();
	
	/** The list containing all shortCuts of this player. */
	private readonly ShortCuts _shortCuts = new ShortCuts(this);
	
	/** The list containing all macros of this player. */
	private readonly MacroList _macros = new MacroList(this);
	
	private readonly Set<Player> _snoopListener = new();
	private readonly Set<Player> _snoopedPlayer = new();
	
	/** Hennas */
	private readonly HennaPoten[] _hennaPoten = new HennaPoten[4];
	private readonly Map<BaseStat, int> _hennaBaseStats = new();
	private readonly Map<int, ScheduledFuture> _hennaRemoveSchedules = new();
	
	/** Hennas Potential */
	
	/** The Pet of the Player */
	private Pet _pet = null;
	/** Servitors of the Player */
	private readonly Map<int, Summon> _servitors = new();
	/** The Agathion of the Player */
	private int _agathionId = 0;
	// apparently, a Player CAN have both a summon AND a tamed beast at the same time!!
	// after Freya players can control more than one tamed beast
	private readonly Set<TamedBeast> _tamedBeast = new();
	
	private bool _minimapAllowed = false;
	
	// client radar
	// TODO: This needs to be better integrated and saved/loaded
	private readonly Radar _radar;
	
	private MatchingRoom _matchingRoom;
	
	private ScheduledFuture _taskWarnUserTakeBreak;
	
	// Clan related attributes
	/** The Clan Identifier of the Player */
	private int _clanId;
	
	/** The Clan object of the Player */
	private Clan _clan;
	
	/** Apprentice and Sponsor IDs */
	private int _apprentice = 0;
	private int _sponsor = 0;
	
	private long _clanJoinExpiryTime;
	private long _clanCreateExpiryTime;
	
	private int _powerGrade = 0;
	private EnumIntBitmask<ClanPrivilege> _clanPrivileges = new();
	
	/** Player's pledge class (knight, Baron, etc.) */
	private int _pledgeClass = 0;
	private int _pledgeType = 0;
	
	/** Level at which the player joined the clan as an academy member */
	private int _lvlJoinedAcademy = 0;
	
	private int _wantsPeace = 0;
	
	// charges
	private readonly AtomicInteger _charges = new AtomicInteger();
	private ScheduledFuture _chargeTask = null;
	
	// Absorbed Souls
	private const int KAMAEL_LIGHT_MASTER = 45178;
	private const int KAMAEL_SHADOW_MASTER = 45179;
	private const int KAMAEL_LIGHT_TRANSFORMATION = 397;
	private const int KAMAEL_SHADOW_TRANSFORMATION = 398;
	private readonly Map<SoulType, int> _souls = new();
	private ScheduledFuture _soulTask = null;
	
	// Death Points
	private const int DEATH_POINTS_PASSIVE = 45352;
	private const int DEVASTATING_MIND = 45300;
	private int _deathPoints = 0;
	private int _maxDeathPoints = 0;
	
	// Beast points
	private int _beastPoints = 0;
	private int _maxBeastPoints = 0;
	
	// Assasination points
	private int _assassinationPoints = 0;
	private readonly int _maxAssassinationPoints = 100000;
	
	// WorldPosition used by TARGET_SIGNET_GROUND
	private Location _currentSkillWorldPosition;
	
	private AccessLevel _accessLevel;
	
	private bool _messageRefusal = false; // message refusal mode
	
	private bool _silenceMode = false; // silence mode
	private List<int> _silenceModeExcluded; // silence mode
	private bool _dietMode = false; // ignore weight penalty
	private bool _tradeRefusal = false; // Trade refusal
	private bool _exchangeRefusal = false; // Exchange refusal
	
	private Party _party;
	
	// this is needed to find the inviting player for Party response
	// there can only be one active party request at once
	private Player _activeRequester;
	private long _requestExpireTime = 0;
	private readonly Request _request = new Request(this);
	
	// Used for protection after teleport
	private long _spawnProtectEndTime = 0;
	private long _teleportProtectEndTime = 0;
	
	private readonly Map<int, ExResponseCommissionInfo> _lastCommissionInfos = new();
	
	// protects a char from aggro mobs when getting up from fake death
	private long _recentFakeDeathEndTime = 0;
	
	/** The fists Weapon of the Player (used when no weapon is equipped) */
	private Weapon _fistsWeaponItem;
	
	private readonly Map<int, String> _chars = new();
	
	private readonly Map<Type, AbstractRequest> _requests = new();
	
	protected bool _inventoryDisable = false;
	/** Player's cubics. */
	private readonly Map<int, Cubic> _cubics = new();
	/** Active shots. */
	protected Set<int> _activeSoulShots = new();
	/** Active Brooch Jewels **/
	private BroochJewel _activeRubyJewel = BroochJewel.None;
	private BroochJewel _activeShappireJewel = BroochJewel.None;
	
	private int _lastAmmunitionId = 0;
	
	/** Event parameters */
	private bool _isRegisteredOnEvent = false;
	private bool _isOnSoloEvent = false;
	private bool _isOnEvent = false;
	
	/** new race ticket **/
	private readonly int[] _raceTickets = new int[2];
	
	private readonly BlockList _blockList = new BlockList(this);
	
	private readonly Map<int, Skill> _transformSkills = new();
	private ScheduledFuture _taskRentPet;
	private ScheduledFuture _taskWater;
	
	/** Last Html Npcs, 0 = last html was not bound to an npc */
	private readonly int[] _htmlActionOriginObjectIds = new int[Enum.GetValues<HtmlActionScope>().Length];
	/**
	 * Origin of the last incoming html action request.<br>
	 * This can be used for htmls continuing the conversation with an npc.
	 */
	private int _lastHtmlActionOriginObjId;
	
	/** Bypass validations */
	private readonly List<String>[] _htmlActionCaches = new List<String>[];
	
	private Forum _forumMail;
	private Forum _forumMemo;
	
	/** Skills queued because a skill is already in progress */
	private SkillUseHolder _queuedSkill;
	
	private int _cursedWeaponEquippedId = 0;
	private bool _combatFlagEquippedId = false;
	
	private bool _canRevive = true;
	private int _reviveRequested = 0;
	private double _revivePower = 0;
	private int _reviveHpPercent = 0;
	private int _reviveMpPercent = 0;
	private int _reviveCpPercent = 0;
	private bool _revivePet = false;
	
	private double _cpUpdateIncCheck = .0;
	private double _cpUpdateDecCheck = .0;
	private double _cpUpdateInterval = .0;
	private double _mpUpdateIncCheck = .0;
	private double _mpUpdateDecCheck = .0;
	private double _mpUpdateInterval = .0;
	
	private double _originalCp = .0;
	private double _originalHp = .0;
	private double _originalMp = .0;
	
	/** Char Coords from Client */
	private int _clientX;
	private int _clientY;
	private int _clientZ;
	private int _clientHeading;
	
	// during fall validations will be disabled for 1000 ms.
	private const int FALLING_VALIDATION_DELAY = 1000;
	private volatile long _fallingTimestamp = 0;
	private volatile int _fallingDamage = 0;
	private ScheduledFuture _fallingDamageTask = null;
	
	private int _multiSocialTarget = 0;
	private int _multiSociaAction = 0;
	
	private MovieHolder _movieHolder = null;
	
	private String _adminConfirmCmd = null;
	
	private volatile long _lastItemAuctionInfoRequest = 0;
	
	private long _pvpFlagLasts;
	
	private long _notMoveUntil = 0;
	
	/** Map containing all custom skills of this player. */
	private Map<int, Skill> _customSkills = null;
	
	private volatile int _actionMask;
	
	private int _questZoneId = -1;
	
	private readonly Fishing _fishing = new Fishing(this);
	
	public void setPvpFlagLasts(long time)
	{
		_pvpFlagLasts = time;
	}
	
	public long getPvpFlagLasts()
	{
		return _pvpFlagLasts;
	}
	
	public void startPvPFlag()
	{
		updatePvPFlag(1);
		PvpFlagTaskManager.getInstance().add(this);
	}
	
	public void stopPvpRegTask()
	{
		PvpFlagTaskManager.getInstance().remove(this);
	}
	
	public void stopPvPFlag()
	{
		stopPvpRegTask();
		updatePvPFlag(0);
	}
	
	// Training Camp
	private const string TRAINING_CAMP_VAR = "TRAINING_CAMP";
	private const string TRAINING_CAMP_DURATION = "TRAINING_CAMP_DURATION";
	
	// Save responder name for log it
	private String _lastPetitionGmName = null;
	
	private bool _hasCharmOfCourage = false;
	
	private readonly Set<int> _whisperers = new();
	
	private ElementalSpirit[] _spirits;
	private ElementalType _activeElementalSpiritType;
	
	private byte _vipTier = 0;
	
	private long _attendanceDelay;
	
	private readonly AutoPlaySettingsHolder _autoPlaySettings = new AutoPlaySettingsHolder();
	private readonly AutoUseSettingsHolder _autoUseSettings = new AutoUseSettingsHolder();
	private readonly AtomicBoolean _autoPlaying = new AtomicBoolean();
	private bool _resumedAutoPlay = false;
	
	private ScheduledFuture _timedHuntingZoneTask = null;
	
	private PlayerRandomCraft _randomCraft = null;
	
	private ScheduledFuture _statIncreaseSkillTask;
	
	private readonly List<PlayerCollectionData> _collections = new();
	private readonly List<int> _collectionFavorites = new();
	
	private readonly Map<int, PurgePlayerHolder> _purgePoints = new();
	
	private readonly HuntPass _huntPass;
	private readonly AchievementBox _achivemenetBox;
	
	private readonly ChallengePoint _challengePoints;
	
	private readonly RankingHistory _rankingHistory;
	
	private readonly Map<int, PetEvolveHolder> _petEvolves = new();
	
	private MissionLevelPlayerDataHolder _missionLevelProgress = null;
	
	private int _dualInventorySlot = 0;
	private List<int> _dualInventorySetA;
	private List<int> _dualInventorySetB;
	
	private readonly List<QuestTimer> _questTimers = new();
	private readonly List<TimerHolder> _timerHolders = new();
	
	// Selling buffs system
	private bool _isSellingBuffs = false;
	private List<SellBuffHolder> _sellingBuffs = null;
	
	public bool isSellingBuffs()
	{
		return _isSellingBuffs;
	}
	
	public void setSellingBuffs(bool value)
	{
		_isSellingBuffs = value;
	}
	
	public List<SellBuffHolder> getSellingBuffs()
	{
		if (_sellingBuffs == null)
		{
			_sellingBuffs = new();
		}
		return _sellingBuffs;
	}
	
	// Player client settings
	private ClientSettings _clientSettings;
	
	public ClientSettings getClientSettings()
	{
		if (_clientSettings == null)
		{
			_clientSettings = new ClientSettings(this);
		}
		return _clientSettings;
	}
	
	/**
	 * Create a new Player and add it in the characters table of the database.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Create a new Player with an account name</li>
	 * <li>Set the name, the Hair Style, the Hair Color and the Face type of the Player</li>
	 * <li>Add the player in the characters table of the database</li>
	 * </ul>
	 * @param template The PlayerTemplate to apply to the Player
	 * @param accountName The name of the Player
	 * @param name The name of the Player
	 * @param app the player's appearance
	 * @return The Player added to the database or null
	 */
	public static Player create(PlayerTemplate template, String accountName, String name, PlayerAppearance app)
	{
		// Create a new Player with an account name
		Player player = new Player(template, accountName, app);
		// Set the name of the Player
		player.setName(name);
		// Set access level
		player.setAccessLevel(0, false, false);
		// Set Character's create time
		player.setCreateDate(DateTime.Now);
		// Set the base class ID to that of the actual class ID.
		player.setBaseClass(player.getClassId());
		// Give 20 recommendations
		player.setRecomLeft(20);
		// Add the player in the characters table of the database
		if (player.createDb())
		{
			CharInfoTable.getInstance().addName(player);
			return player;
		}
		return null;
	}
	
	public String getAccountName()
	{
		return _client == null ? _accountName : _client.getAccountName();
	}
	
	public String getAccountNamePlayer()
	{
		return _accountName;
	}
	
	public Map<int, String> getAccountChars()
	{
		return _chars;
	}
	
	public long getRelation(Player target)
	{
		Clan clan = getClan();
		Party party = getParty();
		Clan targetClan = target.getClan();
		long result = 0;
		if (clan != null)
		{
			result |= RelationChanged.RELATION_CLAN_MEMBER;
			if (clan == target.getClan())
			{
				result |= RelationChanged.RELATION_CLAN_MATE;
			}
			if (getAllyId() != 0)
			{
				result |= RelationChanged.RELATION_ALLY_MEMBER;
			}
		}
		if (isClanLeader())
		{
			result |= RelationChanged.RELATION_LEADER;
		}
		if ((party != null) && (party == target.getParty()))
		{
			result |= RelationChanged.RELATION_HAS_PARTY;
			for (int i = 0; i < party.getMembers().size(); i++)
			{
				if (party.getMembers().get(i) != this)
				{
					continue;
				}
				switch (i)
				{
					case 0:
					{
						result |= RelationChanged.RELATION_PARTYLEADER; // 0x10
						break;
					}
					case 1:
					{
						result |= RelationChanged.RELATION_PARTY4; // 0x8
						break;
					}
					case 2:
					{
						result |= RelationChanged.RELATION_PARTY3 + RelationChanged.RELATION_PARTY2 + RelationChanged.RELATION_PARTY1; // 0x7
						break;
					}
					case 3:
					{
						result |= RelationChanged.RELATION_PARTY3 + RelationChanged.RELATION_PARTY2; // 0x6
						break;
					}
					case 4:
					{
						result |= RelationChanged.RELATION_PARTY3 + RelationChanged.RELATION_PARTY1; // 0x5
						break;
					}
					case 5:
					{
						result |= RelationChanged.RELATION_PARTY3; // 0x4
						break;
					}
					case 6:
					{
						result |= RelationChanged.RELATION_PARTY2 + RelationChanged.RELATION_PARTY1; // 0x3
						break;
					}
					case 7:
					{
						result |= RelationChanged.RELATION_PARTY2; // 0x2
						break;
					}
					case 8:
					{
						result |= RelationChanged.RELATION_PARTY1; // 0x1
						break;
					}
				}
			}
		}
		if (_siegeState != 0)
		{
			result |= RelationChanged.RELATION_INSIEGE;
			if (getSiegeState() != target.getSiegeState())
			{
				result |= RelationChanged.RELATION_ENEMY;
			}
			else
			{
				result |= RelationChanged.RELATION_ALLY;
			}
			if (_siegeState == 1)
			{
				result |= RelationChanged.RELATION_ATTACKER;
			}
		}
		if ((clan != null) && (targetClan != null) && (target.getPledgeType() != Clan.SUBUNIT_ACADEMY) && (getPledgeType() != Clan.SUBUNIT_ACADEMY))
		{
			ClanWar war = clan.getWarWith(target.getClan().getId());
			if (war != null)
			{
				switch (war.getState())
				{
					case ClanWarState.DECLARATION:
					case ClanWarState.BLOOD_DECLARATION:
					{
						if (war.getAttackerClanId() != target.getClanId())
						{
							result |= RelationChanged.RELATION_DECLARED_WAR;
						}
						break;
					}
					case ClanWarState.MUTUAL:
					{
						result |= RelationChanged.RELATION_MUTUAL_WAR;
						break;
					}
				}
			}
		}
		if (target.getSurveillanceList().Contains(getObjectId()))
		{
			result |= RelationChanged.RELATION_SURVEILLANCE;
		}
		return result;
	}
	
	/**
	 * Retrieve a Player from the characters table of the database and add it in _allObjects of the world (call restore method).<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Retrieve the Player from the characters table of the database</li>
	 * <li>Add the Player object in _allObjects</li>
	 * <li>Set the x,y,z position of the Player and make it invisible</li>
	 * <li>Update the overloaded status of the Player</li>
	 * </ul>
	 * @param objectId Identifier of the object to initialized
	 * @return The Player loaded from the database
	 */
	public static Player load(int objectId)
	{
		return restore(objectId);
	}
	
	private void initPcStatusUpdateValues()
	{
		_cpUpdateInterval = getMaxCp() / 352.0;
		_cpUpdateIncCheck = getMaxCp();
		_cpUpdateDecCheck = getMaxCp() - _cpUpdateInterval;
		_mpUpdateInterval = getMaxMp() / 352.0;
		_mpUpdateIncCheck = getMaxMp();
		_mpUpdateDecCheck = getMaxMp() - _mpUpdateInterval;
	}
	
	/**
	 * Constructor of Player (use Creature constructor).<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Call the Creature constructor to create an empty _skills slot and copy basic Calculator set to this Player</li>
	 * <li>Set the name of the Player</li>
	 * </ul>
	 * <font color=#FF0000><b><u>Caution</u>: This method SET the level of the Player to 1</b></font>
	 * @param objectId Identifier of the object to initialized
	 * @param template The PlayerTemplate to apply to the Player
	 * @param accountName The name of the account including this Player
	 * @param app
	 */
	private Player(int objectId, PlayerTemplate template, String accountName, PlayerAppearance app): base(objectId, template)
	{
		setInstanceType(InstanceType.Player);
		base.initCharStatusUpdateValues();
		initPcStatusUpdateValues();
		
		for (int i = 0; i < _htmlActionCaches.Length; ++i)
		{
			_htmlActionCaches[i] = new();
		}
		
		_accountName = accountName;
		app.setOwner(this);
		_appearance = app;
		
		_huntPass = Config.ENABLE_HUNT_PASS ? new HuntPass(this) : null;
		_achivemenetBox = Config.ENABLE_ACHIEVEMENT_BOX ? new AchievementBox(this) : null;
		
		// Create an AI
		getAI();
		
		// Create a Radar object
		_radar = new Radar(this);
		_challengePoints = new ChallengePoint(this);
		_rankingHistory = new RankingHistory(this);
	}
	
	/**
	 * Creates a player.
	 * @param template the player template
	 * @param accountName the account name
	 * @param app the player appearance
	 */
	private Player(PlayerTemplate template, String accountName, PlayerAppearance app)
		: this(IdManager.getInstance().getNextId(), template, accountName, app)
	{
	}
	
	public override PlayerStat getStat()
	{
		return (PlayerStat) base.getStat();
	}
	
	public override void initCharStat()
	{
		setStat(new PlayerStat(this));
	}
	
	public override PlayerStatus getStatus()
	{
		return (PlayerStatus) base.getStatus();
	}
	
	public override void initCharStatus()
	{
		setStatus(new PlayerStatus(this));
	}
	
	public PlayerAppearance getAppearance()
	{
		return _appearance;
	}
	
	public bool isHairAccessoryEnabled()
	{
		return getVariables().getBoolean(PlayerVariables.HAIR_ACCESSORY_VARIABLE_NAME, true);
	}
	
	public void setHairAccessoryEnabled(bool enabled)
	{
		getVariables().set(PlayerVariables.HAIR_ACCESSORY_VARIABLE_NAME, enabled);
	}
	
	public int getLampExp()
	{
		return getVariables().getInt(PlayerVariables.MAGIC_LAMP_EXP, 0);
	}
	
	public void setLampExp(int exp)
	{
		getVariables().set(PlayerVariables.MAGIC_LAMP_EXP, exp);
	}
	
	/**
	 * @return the base PlayerTemplate link to the Player.
	 */
	public PlayerTemplate getBaseTemplate()
	{
		return PlayerTemplateData.getInstance().getTemplate(_baseClass);
	}
	
	public HuntPass getHuntPass()
	{
		return _huntPass;
	}
	
	public AchievementBox getAchievementBox()
	{
		return _achivemenetBox;
	}
	
	public ICollection<RankingHistoryDataHolder> getRankingHistoryData()
	{
		return _rankingHistory.getData();
	}
	
	public ChallengePoint getChallengeInfo()
	{
		return _challengePoints;
	}
	
	/**
	 * @return the PlayerTemplate link to the Player.
	 */
	public override PlayerTemplate getTemplate()
	{
		return (PlayerTemplate) base.getTemplate();
	}
	
	/**
	 * @param newclass
	 */
	public void setTemplate(ClassId newclass)
	{
		base.setTemplate(PlayerTemplateData.getInstance().getTemplate(newclass));
	}
	
	protected override CreatureAI initAI()
	{
		return new PlayerAI(this);
	}
	
	/** Return the Level of the Player. */
	public override int getLevel()
	{
		return getStat().getLevel();
	}
	
	public void setBaseClass(int baseClass)
	{
		_baseClass = baseClass;
	}
	
	public void setBaseClass(ClassId classId)
	{
		_baseClass = classId.getId();
	}
	
	public bool isInStoreMode()
	{
		return _privateStoreType != PrivateStoreType.NONE;
	}
	
	public bool isInStoreSellOrBuyMode()
	{
		return (_privateStoreType == PrivateStoreType.BUY) || (_privateStoreType == PrivateStoreType.SELL) || (_privateStoreType == PrivateStoreType.PACKAGE_SELL);
	}
	
	public bool isCrafting()
	{
		return _isCrafting;
	}
	
	public void setCrafting(bool isCrafting)
	{
		_isCrafting = isCrafting;
	}
	
	/**
	 * @return a collection containing all Common RecipeList of the Player.
	 */
	public ICollection<RecipeList> getCommonRecipeBook()
	{
		return _commonRecipeBook.values();
	}
	
	/**
	 * @return a collection containing all Dwarf RecipeList of the Player.
	 */
	public ICollection<RecipeList> getDwarvenRecipeBook()
	{
		return _dwarvenRecipeBook.values();
	}
	
	/**
	 * Add a new RecipList to the table _commonrecipebook containing all RecipeList of the Player
	 * @param recipe The RecipeList to add to the _recipebook
	 * @param saveToDb
	 */
	public void registerCommonRecipeList(RecipeList recipe, bool saveToDb)
	{
		_commonRecipeBook.put(recipe.getId(), recipe);
		
		if (saveToDb)
		{
			insertNewRecipeData(recipe.getId(), false);
		}
	}
	
	/**
	 * Add a new RecipList to the table _recipebook containing all RecipeList of the Player
	 * @param recipe The RecipeList to add to the _recipebook
	 * @param saveToDb
	 */
	public void registerDwarvenRecipeList(RecipeList recipe, bool saveToDb)
	{
		_dwarvenRecipeBook.put(recipe.getId(), recipe);
		
		if (saveToDb)
		{
			insertNewRecipeData(recipe.getId(), true);
		}
	}
	
	/**
	 * @param recipeId The Identifier of the RecipeList to check in the player's recipe books
	 * @return {@code true}if player has the recipe on Common or Dwarven Recipe book else returns {@code false}
	 */
	public bool hasRecipeList(int recipeId)
	{
		return _dwarvenRecipeBook.containsKey(recipeId) || _commonRecipeBook.containsKey(recipeId);
	}
	
	/**
	 * Tries to remove a RecipList from the table _DwarvenRecipeBook or from table _CommonRecipeBook, those table contain all RecipeList of the Player
	 * @param recipeId The Identifier of the RecipeList to remove from the _recipebook
	 */
	public void unregisterRecipeList(int recipeId)
	{
		if (_dwarvenRecipeBook.remove(recipeId) != null)
		{
			deleteRecipeData(recipeId, true);
		}
		else if (_commonRecipeBook.remove(recipeId) != null)
		{
			deleteRecipeData(recipeId, false);
		}
		else
		{
			LOGGER.Warn("Attempted to remove unknown RecipeList: " + recipeId);
		}
		
		foreach (Shortcut sc in _shortCuts.getAllShortCuts())
		{
			if ((sc != null) && (sc.getId() == recipeId) && (sc.getType() == ShortcutType.RECIPE))
			{
				deleteShortCut(sc.getSlot(), sc.getPage());
			}
		}
	}
	
	private void insertNewRecipeData(int recipeId, bool isDwarf)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement =
				con.prepareStatement("INSERT INTO character_recipebook (charId, id, classIndex, type) values(?,?,?,?)");
			statement.setInt(1, getObjectId());
			statement.setInt(2, recipeId);
			statement.setInt(3, isDwarf ? _classIndex : 0);
			statement.setInt(4, isDwarf ? 1 : 0);
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("SQL exception while inserting recipe: " + recipeId + " from character " + getObjectId() + ": "+ e);
		}
	}
	
	private void deleteRecipeData(int recipeId, bool isDwarf)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement =
				con.prepareStatement("DELETE FROM character_recipebook WHERE charId=? AND id=? AND classIndex=?");
			statement.setInt(1, getObjectId());
			statement.setInt(2, recipeId);
			statement.setInt(3, isDwarf ? _classIndex : 0);
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("SQL exception while deleting recipe: " + recipeId + " from character " + getObjectId(), e);
		}
	}
	
	/**
	 * @return the Id for the last talked quest NPC.
	 */
	public int getLastQuestNpcObject()
	{
		return _questNpcObject;
	}
	
	public void setLastQuestNpcObject(int npcId)
	{
		_questNpcObject = npcId;
	}
	
	public bool isSimulatingTalking()
	{
		return _simulatedTalking;
	}
	
	public void setSimulatedTalking(bool value)
	{
		_simulatedTalking = value;
	}
	
	/**
	 * @param quest The name of the quest
	 * @return the QuestState object corresponding to the quest name.
	 */
	public QuestState getQuestState(String quest)
	{
		return _quests.get(quest);
	}
	
	/**
	 * Add a QuestState to the table _quest containing all quests began by the Player.
	 * @param qs The QuestState to add to _quest
	 */
	public void setQuestState(QuestState qs)
	{
		_quests.put(qs.getQuestName(), qs);
	}
	
	/**
	 * Verify if the player has the quest state.
	 * @param quest the quest state to check
	 * @return {@code true} if the player has the quest state, {@code false} otherwise
	 */
	public bool hasQuestState(String quest)
	{
		return _quests.containsKey(quest);
	}
	
	public bool hasAnyCompletedQuestStates(List<int> questIds)
	{
		foreach (QuestState questState in _quests.values())
		{
			if (questIds.Contains(questState.getQuest().getId()) && questState.isCompleted())
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Remove a QuestState from the table _quest containing all quests began by the Player.
	 * @param quest The name of the quest
	 */
	public void delQuestState(String quest)
	{
		_quests.remove(quest);
	}
	
	/**
	 * @return List of {@link QuestState}s of the current player.
	 */
	public ICollection<QuestState> getAllQuestStates()
	{
		return _quests.values();
	}
	
	/**
	 * @return a table containing all Quest in progress from the table _quests.
	 */
	public ICollection<Quest> getAllActiveQuests()
	{
		List<Quest> activeQuests = new();
		foreach (QuestState questState in _quests.values())
		{
			if (!questState.isStarted())
			{
				continue;
			}
			
			Quest quest = questState.getQuest();
			if ((quest == null) || (quest.getId() <= 1))
			{
				continue;
			}
			
			activeQuests.add(quest);
		}
		return activeQuests;
	}
	
	public void processQuestEvent(String questName, String ev)
	{
		Quest quest = QuestManager.getInstance().getQuest(questName);
		if ((quest == null) || (ev == null) || ev.isEmpty())
		{
			return;
		}
		
		Npc target = _lastFolkNpc;
		if ((target != null) && isInsideRadius2D(target, Npc.INTERACTION_DISTANCE))
		{
			quest.notifyEvent(ev, target, this);
		}
		else if (_questNpcObject > 0)
		{
			WorldObject obj = World.getInstance().findObject(getLastQuestNpcObject());
			if ((obj != null) && obj.isNpc() && isInsideRadius2D(obj, Npc.INTERACTION_DISTANCE))
			{
				Npc npc = (Npc) obj;
				quest.notifyEvent(ev, npc, this);
			}
		}
	}
	
	/** List of all QuestState instance that needs to be notified of this Player's or its pet's death */
	private Set<QuestState> _notifyQuestOfDeathList;
	
	/**
	 * Add QuestState instance that is to be notified of Player's death.
	 * @param qs The QuestState that subscribe to this event
	 */
	public void addNotifyQuestOfDeath(QuestState qs)
	{
		if (qs == null)
		{
			return;
		}
		
		if (!getNotifyQuestOfDeath().Contains(qs))
		{
			getNotifyQuestOfDeath().add(qs);
		}
	}
	
	/**
	 * Remove QuestState instance that is to be notified of Player's death.
	 * @param qs The QuestState that subscribe to this event
	 */
	public void removeNotifyQuestOfDeath(QuestState qs)
	{
		if ((qs == null) || (_notifyQuestOfDeathList == null))
		{
			return;
		}
		
		_notifyQuestOfDeathList.remove(qs);
	}
	
	/**
	 * @return a list of QuestStates which registered for notify of death of this Player.
	 */
	public Set<QuestState> getNotifyQuestOfDeath()
	{
		if (_notifyQuestOfDeathList == null)
		{
			lock (this)
			{
				if (_notifyQuestOfDeathList == null)
				{
					_notifyQuestOfDeathList = new();
				}
			}
		}
		return _notifyQuestOfDeathList;
	}
	
	public bool isNotifyQuestOfDeathEmpty()
	{
		return (_notifyQuestOfDeathList == null) || _notifyQuestOfDeathList.isEmpty();
	}
	
	/**
	 * @return a collection containing all ShortCut of the Player.
	 */
	public ICollection<Shortcut> getAllShortCuts()
	{
		return _shortCuts.getAllShortCuts();
	}
	
	/**
	 * @param slot The slot in which the shortCuts is equipped
	 * @param page The page of shortCuts containing the slot
	 * @return the ShortCut of the Player corresponding to the position (page-slot).
	 */
	public Shortcut getShortCut(int slot, int page)
	{
		return _shortCuts.getShortCut(slot, page);
	}
	
	/**
	 * Add a L2shortCut to the Player _shortCuts
	 * @param shortcut
	 */
	public void registerShortCut(Shortcut shortcut)
	{
		_shortCuts.registerShortCut(shortcut);
	}
	
	/**
	 * Updates the shortcut bars with the new skill.
	 * @param skillId the skill Id to search and update.
	 * @param skillLevel the skill level to update.
	 * @param skillSubLevel the skill sub level to update.
	 */
	public void updateShortCuts(int skillId, int skillLevel, int skillSubLevel)
	{
		_shortCuts.updateShortCuts(skillId, skillLevel, skillSubLevel);
	}
	
	/**
	 * Delete the ShortCut corresponding to the position (page-slot) from the Player _shortCuts.
	 * @param slot
	 * @param page
	 */
	public void deleteShortCut(int slot, int page)
	{
		_shortCuts.deleteShortCut(slot, page);
	}
	
	/**
	 * @param macro the macro to add to this Player.
	 */
	public void registerMacro(Macro macro)
	{
		_macros.registerMacro(macro);
	}
	
	/**
	 * @param id the macro Id to delete.
	 */
	public void deleteMacro(int id)
	{
		_macros.deleteMacro(id);
	}
	
	/**
	 * @return all Macro of the Player.
	 */
	public MacroList getMacros()
	{
		return _macros;
	}
	
	/**
	 * Set the siege state of the Player.
	 * @param siegeState 1 = attacker, 2 = defender, 0 = not involved
	 */
	public void setSiegeState(byte siegeState)
	{
		_siegeState = siegeState;
	}
	
	/**
	 * Get the siege state of the Player.
	 * @return 1 = attacker, 2 = defender, 0 = not involved
	 */
	public byte getSiegeState()
	{
		return _siegeState;
	}
	
	/**
	 * Set the siege Side of the Player.
	 * @param value
	 */
	public void setSiegeSide(int value)
	{
		_siegeSide = value;
	}
	
	public bool isRegisteredOnThisSiegeField(int value)
	{
		return (_siegeSide == value) || ((_siegeSide >= 81) && (_siegeSide <= 89));
	}
	
	public int getSiegeSide()
	{
		return _siegeSide;
	}
	
	public bool isSiegeFriend(WorldObject target)
	{
		// If i'm natural or not in siege zone, not friends.
		if ((_siegeState == 0) || !isInsideZone(ZoneId.SIEGE))
		{
			return false;
		}
		
		// Check first castle mid victory.
		Castle castle = CastleManager.getInstance().getCastleById(_siegeSide);
		Player targetPlayer = target.getActingPlayer();
		if ((castle != null) && (targetPlayer != null) && !castle.isFirstMidVictory())
		{
			return true;
		}
		
		// If target isn't a player, is self, isn't on same siege or not on same state, not friends.
		if ((targetPlayer == null) || (targetPlayer == this) || (targetPlayer.getSiegeSide() != _siegeSide) || (_siegeState != targetPlayer.getSiegeState()))
		{
			return false;
		}
		
		// Attackers are considered friends only if castle has no owner.
		if (_siegeState == 1)
		{
			if (castle == null)
			{
				return false;
			}
			
			return castle.getOwner() == null;
		}
		
		// Both are defenders, friends.
		return true;
	}
	
	/**
	 * Set the PvP Flag of the Player.
	 * @param pvpFlag
	 */
	public void setPvpFlag(int pvpFlag)
	{
		_pvpFlag = (byte) pvpFlag;
	}
	
	public bool getPvpFlag()
	{
		return _pvpFlag;
	}
	
	public void updatePvPFlag(int value)
	{
		if (_pvpFlag == value)
		{
			return;
		}
		setPvpFlag(value);
		
		StatusUpdate su = new StatusUpdate(this);
		computeStatusUpdate(su, StatusUpdateType.PVP_FLAG);
		if (su.hasUpdates())
		{
			broadcastPacket(su);
			sendPacket(su);
		}
		
		// If this player has a pet update the pets pvp flag as well
		if (hasSummon())
		{
			RelationChanged rc = new RelationChanged();
			Summon pet = _pet;
			if (pet != null)
			{
				rc.addRelation(pet, getRelation(this), false);
			}
			if (hasServitors())
			{
				getServitors().values().forEach(s => rc.addRelation(s, getRelation(this), false));
			}
			sendPacket(rc);
		}
		
		World.getInstance().forEachVisibleObject<Player>(this, player =>
		{
			if (!isVisibleFor(player))
			{
				return;
			}
			
			long relation = getRelation(player);
			bool isAutoAttackable = isAutoAttackable(player);
			RelationCache oldrelation = getKnownRelations().get(player.getObjectId());
			if ((oldrelation == null) || (oldrelation.getRelation() != relation) || (oldrelation.isAutoAttackable() != isAutoAttackable))
			{
				RelationChanged rc = new RelationChanged();
				rc.addRelation(this, relation, isAutoAttackable);
				if (hasSummon())
				{
					Summon pet = _pet;
					if (pet != null)
					{
						rc.addRelation(pet, relation, isAutoAttackable);
					}
					if (hasServitors())
					{
						getServitors().values().forEach(s => rc.addRelation(s, relation, isAutoAttackable));
					}
				}
				player.sendPacket(rc);
				getKnownRelations().put(player.getObjectId(), new RelationCache(relation, isAutoAttackable));
			}
		});
	}
	
	public void revalidateZone(bool force)
	{
		// Cannot validate if not in a world region (happens during teleport)
		if (getWorldRegion() == null)
		{
			return;
		}
		
		// This function is called too often from movement code.
		if (!force && (calculateDistance3D(_lastZoneValidateLocation) < 100))
		{
			return;
		}
		_lastZoneValidateLocation.setXYZ(this);
		
		ZoneManager.getInstance().getRegion(this).revalidateZones(this);
		
		if (Config.ALLOW_WATER)
		{
			checkWaterState();
		}
		
		if (!isInsideZone(ZoneId.PEACE) && !_autoUseSettings.isEmpty())
		{
			AutoUseTaskManager.getInstance().startAutoUseTask(this);
		}
		
		if (isInsideZone(ZoneId.ALTERED))
		{
			if (_lastCompassZone == ExSetCompassZoneCode.ALTEREDZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCode.ALTEREDZONE;
			sendPacket(new ExSetCompassZoneCode(ExSetCompassZoneCode.ALTEREDZONE));
		}
		else if (isInsideZone(ZoneId.SIEGE))
		{
			if (_lastCompassZone == ExSetCompassZoneCode.SIEGEWARZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCode.SIEGEWARZONE;
			sendPacket(new ExSetCompassZoneCode(ExSetCompassZoneCode.SIEGEWARZONE));
		}
		else if (isInsideZone(ZoneId.PVP))
		{
			if (_lastCompassZone == ExSetCompassZoneCode.PVPZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCode.PVPZONE;
			sendPacket(new ExSetCompassZoneCode(ExSetCompassZoneCode.PVPZONE));
		}
		else if (isInsideZone(ZoneId.PEACE))
		{
			if (_lastCompassZone == ExSetCompassZoneCode.PEACEZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCode.PEACEZONE;
			sendPacket(new ExSetCompassZoneCode(ExSetCompassZoneCode.PEACEZONE));
		}
		else if (isInsideZone(ZoneId.NO_PVP))
		{
			if (_lastCompassZone == ExSetCompassZoneCode.NOPVPZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCode.NOPVPZONE;
			sendPacket(new ExSetCompassZoneCode(ExSetCompassZoneCode.NOPVPZONE));
		}
		else
		{
			if (_lastCompassZone == ExSetCompassZoneCode.GENERALZONE)
			{
				return;
			}
			if (_lastCompassZone == ExSetCompassZoneCode.SIEGEWARZONE)
			{
				updatePvPStatus();
			}
			_lastCompassZone = ExSetCompassZoneCode.GENERALZONE;
			sendPacket(new ExSetCompassZoneCode(ExSetCompassZoneCode.GENERALZONE));
		}
	}
	
	/**
	 * @return True if the Player can Craft Dwarven Recipes.
	 */
	public bool hasDwarvenCraft()
	{
		return getSkillLevel((int)CommonSkill.CREATE_DWARVEN) >= 1;
	}
	
	public int getDwarvenCraft()
	{
		return getSkillLevel((int)CommonSkill.CREATE_DWARVEN);
	}
	
	/**
	 * @return True if the Player can Craft Dwarven Recipes.
	 */
	public bool hasCommonCraft()
	{
		return getSkillLevel((int)CommonSkill.CREATE_COMMON) >= 1;
	}
	
	public int getCommonCraft()
	{
		return getSkillLevel((int)CommonSkill.CREATE_COMMON);
	}
	
	/**
	 * @return the PK counter of the Player.
	 */
	public int getPkKills()
	{
		return _pkKills;
	}
	
	/**
	 * Set the PK counter of the Player.
	 * @param pkKills
	 */
	public void setPkKills(int pkKills)
	{
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_PK_CHANGED, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerPKChanged(this, _pkKills, pkKills), this);
		}
		
		_pkKills = pkKills;
	}
	
	public int getTotalKills()
	{
		return _totalKills;
	}
	
	public int getTotalDeaths()
	{
		return _totalDeaths;
	}
	
	public void setTotalKills(int value)
	{
		_totalKills = value;
	}
	
	public void setTotalDeaths(int value)
	{
		_totalDeaths = value;
	}
	
	/**
	 * @return the _deleteTimer of the Player.
	 */
	public long getDeleteTimer()
	{
		return _deleteTimer;
	}
	
	/**
	 * Set the _deleteTimer of the Player.
	 * @param deleteTimer
	 */
	public void setDeleteTimer(long deleteTimer)
	{
		_deleteTimer = deleteTimer;
	}
	
	/**
	 * @return the number of recommendation obtained by the Player.
	 */
	public int getRecomHave()
	{
		return _recomHave;
	}
	
	/**
	 * Increment the number of recommendation obtained by the Player (Max : 255).
	 */
	protected void incRecomHave()
	{
		if (_recomHave < 255)
		{
			_recomHave++;
		}
	}
	
	/**
	 * Set the number of recommendation obtained by the Player (Max : 255).
	 * @param value
	 */
	public void setRecomHave(int value)
	{
		_recomHave = Math.Min(Math.Max(value, 0), 255);
	}
	
	/**
	 * Set the number of recommendation obtained by the Player (Max : 255).
	 * @param value
	 */
	public void setRecomLeft(int value)
	{
		_recomLeft = Math.Min(Math.Max(value, 0), 255);
	}
	
	/**
	 * @return the number of recommendation that the Player can give.
	 */
	public int getRecomLeft()
	{
		return _recomLeft;
	}
	
	/**
	 * Increment the number of recommendation that the Player can give.
	 */
	protected void decRecomLeft()
	{
		if (_recomLeft > 0)
		{
			_recomLeft--;
		}
	}
	
	public void giveRecom(Player target)
	{
		target.incRecomHave();
		decRecomLeft();
	}
	
	/**
	 * Set the exp of the Player before a death
	 * @param exp
	 */
	public void setExpBeforeDeath(long exp)
	{
		_expBeforeDeath = exp;
	}
	
	public long getExpBeforeDeath()
	{
		return _expBeforeDeath;
	}
	
	public void setInitialReputation(int reputation)
	{
		base.setReputation(reputation);
	}
	
	/**
	 * Set the reputation of the Player and send a Server=>Client packet StatusUpdate (broadcast).
	 * @param value
	 */
	public void setReputation(int value)
	{
		// Notify to scripts.
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_REPUTATION_CHANGED, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerReputationChanged(this, getReputation(), value), this);
		}
		
		int reputation = value;
		if (reputation > Config.MAX_REPUTATION) // Max count of positive reputation
		{
			reputation = Config.MAX_REPUTATION;
		}
		
		if (getReputation() == reputation)
		{
			return;
		}
		
		if ((getReputation() >= 0) && (reputation < 0))
		{
			World.getInstance().forEachVisibleObject<Guard>(this, obj =>
			{
				if (obj.getAI().getIntention() == CtrlIntention.AI_INTENTION_IDLE)
				{
					obj.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
				}
			});
		}
		
		base.setReputation(reputation);
		
		sendPacket(new SystemMessage(SystemMessageId.YOUR_REPUTATION_HAS_BEEN_CHANGED_TO_S1).addInt(getReputation()));
		broadcastReputation();
		
		applyKarmaPenalty();
	}
	
	public void applyKarmaPenalty()
	{
		int expectedLevel;
		if (getReputation() < -288000)
		{
			expectedLevel = 10;
		}
		else if (getReputation() < -216000)
		{
			expectedLevel = 9;
		}
		else if (getReputation() < -144000)
		{
			expectedLevel = 8;
		}
		else if (getReputation() < -72000)
		{
			expectedLevel = 7;
		}
		else if (getReputation() < -36000)
		{
			expectedLevel = 6;
		}
		else if (getReputation() < -33840)
		{
			expectedLevel = 5;
		}
		else if (getReputation() < -30240)
		{
			expectedLevel = 4;
		}
		else if (getReputation() < -27000)
		{
			expectedLevel = 3;
		}
		else if (getReputation() < -18000)
		{
			expectedLevel = 2;
		}
		else if (getReputation() < 0)
		{
			expectedLevel = 1;
		}
		else
		{
			expectedLevel = 0;
		}
		
		if (expectedLevel > 0)
		{
			if (_einhasadOverseeingLevel != expectedLevel)
			{
				getEffectList().stopSkillEffects(SkillFinishType.REMOVED, (int)CommonSkill.EINHASAD_OVERSEEING);
				SkillData.getInstance().getSkill((int)CommonSkill.EINHASAD_OVERSEEING, expectedLevel).applyEffects(this, this);
			}
		}
		else
		{
			getEffectList().stopSkillEffects(SkillFinishType.REMOVED, (int)CommonSkill.EINHASAD_OVERSEEING);
			getServitors().values().forEach(s => s.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, (int)CommonSkill.EINHASAD_OVERSEEING));
			if (getPet() != null)
			{
				getPet().getEffectList().stopSkillEffects(SkillFinishType.REMOVED, (int)CommonSkill.EINHASAD_OVERSEEING);
			}
		}
		
		_einhasadOverseeingLevel = expectedLevel;
	}
	
	public int getEinhasadOverseeingLevel()
	{
		return _einhasadOverseeingLevel;
	}
	
	public void setEinhasadOverseeingLevel(int level)
	{
		_einhasadOverseeingLevel = level;
	}
	
	public int getWeightPenalty()
	{
		return _dietMode ? 0 : _curWeightPenalty;
	}
	
	/**
	 * Update the overloaded status of the Player.
	 * @param broadcast
	 */
	public void refreshOverloaded(bool broadcast)
	{
		int maxLoad = getMaxLoad();
		if (maxLoad > 0)
		{
			long weightproc = (((getCurrentLoad() - getBonusWeightPenalty()) * 1000) / getMaxLoad());
			int newWeightPenalty;
			if ((weightproc < 500) || _dietMode)
			{
				newWeightPenalty = 0;
			}
			else if (weightproc < 666)
			{
				newWeightPenalty = 1;
			}
			else if (weightproc < 800)
			{
				newWeightPenalty = 2;
			}
			else if (weightproc < 1000)
			{
				newWeightPenalty = 3;
			}
			else
			{
				newWeightPenalty = 4;
			}
			
			if (_curWeightPenalty != newWeightPenalty)
			{
				_curWeightPenalty = newWeightPenalty;
				if ((newWeightPenalty > 0) && !_dietMode)
				{
					addSkill(SkillData.getInstance().getSkill((int)CommonSkill.WEIGHT_PENALTY, newWeightPenalty));
					setOverloaded(getCurrentLoad() > maxLoad);
				}
				else
				{
					removeSkill(getKnownSkill(4270), false, true);
					setOverloaded(false);
				}
				if (broadcast)
				{
					sendPacket(new EtcStatusUpdate(this));
					broadcastUserInfo();
				}
			}
		}
	}
	
	public void useEquippableItem(Item item, bool abortAttack)
	{
		// Check if the item is null.
		if (item == null)
		{
			return;
		}
		
		// Check if the item is owned by this player.
		if (item.getOwnerId() != getObjectId())
		{
			return;
		}
		
		// Check if the item is in the inventory.
		ItemLocation itemLocation = item.getItemLocation();
		if ((itemLocation != ItemLocation.INVENTORY) && (itemLocation != ItemLocation.PAPERDOLL))
		{
			return;
		}
		
		// Equip or unEquip
		List<Item> items = null;
		bool isEquiped = item.isEquipped();
		int oldInvLimit = getInventoryLimit();
		SystemMessage sm = null;
		if (isEquiped)
		{
			getDualInventorySet().set(item.getLocationSlot(), 0);
			
			if (item.getEnchantLevel() > 0)
			{
				sm = new SystemMessage(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.addInt(item.getEnchantLevel());
				sm.addItemName(item);
			}
			else
			{
				sm = new SystemMessage(SystemMessageId.S1_UNEQUIPPED);
				sm.addItemName(item);
			}
			sendPacket(sm);
			
			long slot = _inventory.getSlotFromItem(item);
			// we can't unequip talisman by body slot
			if ((slot == ItemTemplate.SLOT_DECO) || (slot == ItemTemplate.SLOT_BROOCH_JEWEL) || (slot == ItemTemplate.SLOT_AGATHION) || (slot == ItemTemplate.SLOT_ARTIFACT))
			{
				items = _inventory.unEquipItemInSlotAndRecord(item.getLocationSlot());
			}
			else
			{
				items = _inventory.unEquipItemInBodySlotAndRecord(slot);
			}
		}
		else
		{
			items = _inventory.equipItemAndRecord(item);
			if (item.isEquipped())
			{
				if (item.getEnchantLevel() > 0)
				{
					sm = new SystemMessage(SystemMessageId.S1_S2_EQUIPPED);
					sm.addInt(item.getEnchantLevel());
					sm.addItemName(item);
				}
				else
				{
					sm = new SystemMessage(SystemMessageId.S1_EQUIPPED);
					sm.addItemName(item);
				}
				sendPacket(sm);
				
				// Consume mana - will start a task if required; returns if item is not a shadow item
				item.decreaseMana(false);
				
				if ((item.getTemplate().getBodyPart() & ItemTemplate.SLOT_MULTI_ALLWEAPON) != 0)
				{
					rechargeShots(true, true, false);
				}
				
				// Notify to scripts
				if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_ITEM_EQUIP, item.getTemplate()))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnPlayerItemEquip(this, item), item.getTemplate());
				}
				
				getDualInventorySet().set(item.getLocationSlot(), item.getObjectId());
			}
			else
			{
				sendPacket(SystemMessageId.NO_EQUIPMENT_SLOT_AVAILABLE);
			}
		}
		
		broadcastUserInfo();
		ThreadPool.schedule(() => sendPacket(new ExUserInfoEquipSlot(this)), 100);
		
		InventoryUpdate iu = new InventoryUpdate();
		iu.addItems(items);
		sendInventoryUpdate(iu);
		
		if (abortAttack)
		{
			abortAttack();
		}
		
		if (getInventoryLimit() != oldInvLimit)
		{
			sendStorageMaxCount();
		}
	}
	
	/**
	 * @return the the PvP Kills of the Player (Number of player killed during a PvP).
	 */
	public int getPvpKills()
	{
		return _pvpKills;
	}
	
	/**
	 * Set the the PvP Kills of the Player (Number of player killed during a PvP).
	 * @param pvpKills
	 */
	public void setPvpKills(int pvpKills)
	{
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_PVP_CHANGED, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerPvPChanged(this, _pvpKills, pvpKills), this);
		}
		
		_pvpKills = pvpKills;
	}
	
	/**
	 * @return the Fame of this Player
	 */
	public int getFame()
	{
		return _fame;
	}
	
	/**
	 * Set the Fame of this PlayerInstane
	 * @param fame
	 */
	public void setFame(int fame)
	{
		int newFame = fame;
		if (fame > Config.MAX_PERSONAL_FAME_POINTS)
		{
			newFame = Config.MAX_PERSONAL_FAME_POINTS;
		}
		else if (fame < 0)
		{
			newFame = 0;
		}
		
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_FAME_CHANGED, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerFameChanged(this, _fame, newFame), this);
		}
		
		_fame = newFame;
	}
	
	/**
	 * @return the Raidboss points of this Player
	 */
	public int getRaidbossPoints()
	{
		return _raidbossPoints;
	}
	
	/**
	 * Set the Raidboss points of this Player
	 * @param points
	 */
	public void setRaidbossPoints(int points)
	{
		int value = points;
		if (points > 2000000000) // Close to integer max value (2147483647).
		{
			value = 2000000000;
		}
		else if (points < 0)
		{
			value = 0;
		}
		
		_raidbossPoints = value;
	}
	
	/**
	 * Increase the Raidboss points of this Player
	 * @param increasePoints
	 */
	public void increaseRaidbossPoints(int increasePoints)
	{
		setRaidbossPoints(_raidbossPoints + increasePoints);
	}
	
	/**
	 * @return the ClassId object of the Player contained in PlayerTemplate.
	 */
	public ClassId getClassId()
	{
		return getTemplate().getClassId();
	}
	
	/**
	 * Set the template of the Player.
	 * @param id The Identifier of the PlayerTemplate to set to the Player
	 */
	public void setClassId(int id)
	{
		if (_subclassLock)
		{
			return;
		}
		_subclassLock = true;
		
		try
		{
			if ((_lvlJoinedAcademy != 0) && (_clan != null) && CategoryData.getInstance().isInCategory(CategoryType.THIRD_CLASS_GROUP, id))
			{
				if (_lvlJoinedAcademy <= 16)
				{
					_clan.addReputationScore(Config.JOIN_ACADEMY_MAX_REP_SCORE);
				}
				else if (_lvlJoinedAcademy >= 39)
				{
					_clan.addReputationScore(Config.JOIN_ACADEMY_MIN_REP_SCORE);
				}
				else
				{
					_clan.addReputationScore((Config.JOIN_ACADEMY_MAX_REP_SCORE - ((_lvlJoinedAcademy - 16) * 20)));
				}
				setLvlJoinedAcademy(0);
				// oust pledge member from the academy, cuz he has finished his 2nd class transfer
				SystemMessage msg = new SystemMessage(SystemMessageId.S1_IS_DISMISSED_FROM_THE_CLAN);
				msg.addPcName(this);
				_clan.broadcastToOnlineMembers(msg);
				_clan.broadcastToOnlineMembers(new PledgeShowMemberListDelete(getName()));
				_clan.removeClanMember(getObjectId(), 0);
				sendPacket(SystemMessageId.CONGRATULATIONS_YOU_WILL_NOW_GRADUATE_FROM_THE_CLAN_ACADEMY_AND_LEAVE_YOUR_CURRENT_CLAN_YOU_CAN_NOW_JOIN_A_CLAN_WITHOUT_BEING_SUBJECT_TO_ANY_PENALTIES);
				
				// receive graduation gift
				_inventory.addItem("Gift", 8181, 1, this, null); // give academy circlet
			}
			if (isSubClassActive())
			{
				getSubClasses().get(_classIndex).setClassId(id);
			}
			setTarget(this);
			broadcastPacket(new MagicSkillUse(this, 5103, 1, 0, 0));
			setClassTemplate(id);
			if (getClassId().level() == 3)
			{
				sendPacket(SystemMessageId.CONGRATULATIONS_YOU_VE_COMPLETED_YOUR_THIRD_CLASS_TRANSFER_QUEST);
				initElementalSpirits();
			}
			else
			{
				sendPacket(SystemMessageId.CONGRATULATIONS_YOU_VE_COMPLETED_THE_CLASS_CHANGE);
			}
			
			// Remove class permitted hennas.
			for (int slot = 1; slot < 4; slot++)
			{
				Henna henna = getHenna(slot);
				if ((henna != null) && !henna.isAllowedClass(getActingPlayer()))
				{
					removeHenna(slot);
				}
			}
			
			// Update class icon in party and clan
			if (isInParty())
			{
				_party.broadcastPacket(new PartySmallWindowUpdate(this, true));
			}
			
			if (_clan != null)
			{
				_clan.broadcastToOnlineMembers(new PledgeShowMemberListUpdate(this));
			}
			
			sendPacket(new ExSubjobInfo(this, SubclassInfoType.CLASS_CHANGED));
			
			// Add AutoGet skills and normal skills and/or learnByFS depending on configurations.
			rewardSkills();
			
			if (!canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS) && Config.DECREASE_SKILL_LEVEL)
			{
				checkPlayerSkills();
			}
			
			notifyFriends(FriendStatus.MODE_CLASS);
		}
		finally
		{
			_subclassLock = false;
			getStat().recalculateStats(false);
			updateAbnormalVisualEffects();
			sendSkillList();
			
			CharInfoTable.getInstance().setClassId(getObjectId(), id);
		}
	}
	
	public bool isChangingClass()
	{
		return _subclassLock;
	}
	
	/**
	 * @return the Experience of the Player.
	 */
	public long getExp()
	{
		return getStat().getExp();
	}
	
	/**
	 * Set the fists weapon of the Player (used when no weapon is equiped).
	 * @param weaponItem The fists Weapon to set to the Player
	 */
	public void setFistsWeaponItem(Weapon weaponItem)
	{
		_fistsWeaponItem = weaponItem;
	}
	
	/**
	 * @return the fists weapon of the Player (used when no weapon is equipped).
	 */
	public Weapon getFistsWeaponItem()
	{
		return _fistsWeaponItem;
	}
	
	/**
	 * @param classId
	 * @return the fists weapon of the Player Class (used when no weapon is equipped).
	 */
	public Weapon findFistsWeaponItem(int classId)
	{
		Weapon weaponItem = null;
		if ((classId >= 0x00) && (classId <= 0x09))
		{
			// human fighter fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(246);
			weaponItem = (Weapon) temp;
		}
		else if ((classId >= 0x0a) && (classId <= 0x11))
		{
			// human mage fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(251);
			weaponItem = (Weapon) temp;
		}
		else if ((classId >= 0x12) && (classId <= 0x18))
		{
			// elven fighter fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(244);
			weaponItem = (Weapon) temp;
		}
		else if ((classId >= 0x19) && (classId <= 0x1e))
		{
			// elven mage fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(249);
			weaponItem = (Weapon) temp;
		}
		else if ((classId >= 0x1f) && (classId <= 0x25))
		{
			// dark elven fighter fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(245);
			weaponItem = (Weapon) temp;
		}
		else if ((classId >= 0x26) && (classId <= 0x2b))
		{
			// dark elven mage fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(250);
			weaponItem = (Weapon) temp;
		}
		else if ((classId >= 0x2c) && (classId <= 0x30))
		{
			// orc fighter fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(248);
			weaponItem = (Weapon) temp;
		}
		else if ((classId >= 0x31) && (classId <= 0x34))
		{
			// orc mage fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(252);
			weaponItem = (Weapon) temp;
		}
		else if ((classId >= 0x35) && (classId <= 0x39))
		{
			// dwarven fists
			ItemTemplate temp = ItemData.getInstance().getTemplate(247);
			weaponItem = (Weapon) temp;
		}
		return weaponItem;
	}
	
	/**
	 * This method reward all AutoGet skills and Normal skills if Auto-Learn configuration is true.
	 */
	public void rewardSkills()
	{
		// Give all normal skills if activated Auto-Learn is activated, included AutoGet skills.
		if (Config.AUTO_LEARN_SKILLS)
		{
			giveAvailableSkills(Config.AUTO_LEARN_FS_SKILLS, true, Config.AUTO_LEARN_SKILLS_WITHOUT_ITEMS);
		}
		else
		{
			giveAvailableAutoGetSkills();
		}
		
		if (Config.DECREASE_SKILL_LEVEL && !canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS))
		{
			checkPlayerSkills();
		}
		
		foreach (SkillLearn skill in SkillTreeData.getInstance().getRaceSkillTree(getRace()))
		{
			addSkill(SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel()), true);
		}
		
		checkItemRestriction();
		sendSkillList();
		restoreAutoShortcutVisual();
	}
	
	/**
	 * Re-give all skills which aren't saved to database, like Noble, Hero, Clan Skills.
	 */
	public void regiveTemporarySkills()
	{
		// Do not call this on enterworld or char load
		
		// Add noble skills if noble
		if (isNoble())
		{
			setNoble(true);
		}
		
		// Add Hero skills if hero
		if (_hero)
		{
			setHero(true);
		}
		
		// Add clan skills
		if (_clan != null)
		{
			_clan.addSkillEffects(this);
			
			if ((_clan.getLevel() >= SiegeManager.getInstance().getSiegeClanMinLevel()) && isClanLeader())
			{
				SiegeManager.getInstance().addSiegeSkills(this);
			}
			if (_clan.getCastleId() > 0)
			{
				Castle castle = CastleManager.getInstance().getCastleByOwner(_clan);
				if (castle != null)
				{
					castle.giveResidentialSkills(this);
				}
			}
			if (_clan.getFortId() > 0)
			{
				Fort fort = FortManager.getInstance().getFortByOwner(_clan);
				if (fort != null)
				{
					fort.giveResidentialSkills(this);
				}
			}
		}
		
		// Reload passive skills from armors / jewels / weapons
		_inventory.reloadEquippedItems();
	}
	
	/**
	 * Give all available skills to the player.
	 * @param includeByFs if {@code true} forgotten scroll skills present in the skill tree will be added
	 * @param includeAutoGet if {@code true} auto-get skills present in the skill tree will be added
	 * @param includeRequiredItems if {@code true} skills that have required items will be added
	 * @return the amount of new skills earned
	 */
	public int giveAvailableSkills(bool includeByFs, bool includeAutoGet, bool includeRequiredItems)
	{
		int skillCounter = 0;
		// Get available skills
		ICollection<Skill> skills = SkillTreeData.getInstance().getAllAvailableSkills(this, getTemplate().getClassId(), includeByFs, includeAutoGet, includeRequiredItems);
		List<Skill> skillsForStore = new();
		foreach (Skill skill in skills)
		{
			int skillId = skill.getId();
			Skill oldSkill = getKnownSkill(skillId);
			if (oldSkill == skill)
			{
				continue;
			}
			
			if (getReplacementSkill(skillId) != skillId)
			{
				continue;
			}
			
			if (getSkillLevel(skillId) == 0)
			{
				skillCounter++;
			}
			
			// fix when learning toggle skills
			if (skill.isToggle() && !skill.isNecessaryToggle() && isAffectedBySkill(skillId))
			{
				stopSkillEffects(SkillFinishType.REMOVED, skillId);
			}
			
			// Mobius: Keep sublevel on skill level increase.
			int skillLevel = skill.getLevel();
			if ((oldSkill != null) && (oldSkill.getSubLevel() > 0) && (skill.getSubLevel() == 0) && (oldSkill.getLevel() < skillLevel))
			{
				skill = SkillData.getInstance().getSkill(skillId, skillLevel, oldSkill.getSubLevel());
			}
			
			addSkill(skill, false);
			skillsForStore.add(skill);
			
			if (Config.AUTO_LEARN_SKILLS)
			{
				updateShortCuts(skillId, skillLevel, skill.getSubLevel());
			}
		}
		
		storeSkills(skillsForStore, -1);
		
		if (Config.AUTO_LEARN_SKILLS && (skillCounter > 0))
		{
			// Sending ShortCutInit breaks auto use shortcuts.
			// sendPacket(new ShortCutInit(this));
			
			sendMessage("You have learned " + skillCounter + " new skills.");
		}
		restoreAutoShortcutVisual();
		
		return skillCounter;
	}
	
	/**
	 * Give all available auto-get skills to the player.
	 */
	public void giveAvailableAutoGetSkills()
	{
		// Get available skills
		List<SkillLearn> autoGetSkills = SkillTreeData.getInstance().getAvailableAutoGetSkills(this);
		SkillData st = SkillData.getInstance();
		Skill skill;
		foreach (SkillLearn s in autoGetSkills)
		{
			skill = st.getSkill(s.getSkillId(), s.getSkillLevel());
			if (skill != null)
			{
				addSkill(skill, true);
			}
			else
			{
				LOGGER.Warn("Skipping null auto-get skill for " + this);
			}
		}
	}
	
	/**
	 * Set the Experience value of the Player.
	 * @param exp
	 */
	public void setExp(long exp)
	{
		getStat().setExp(Math.Max(0, exp));
	}
	
	/**
	 * @return the Race object of the Player.
	 */
	public override Race getRace()
	{
		if (!isSubClassActive())
		{
			return getTemplate().getRace();
		}
		return PlayerTemplateData.getInstance().getTemplate(_baseClass).getRace();
	}
	
	public Radar getRadar()
	{
		return _radar;
	}
	
	/* Return true if Hellbound minimap allowed */
	public bool isMinimapAllowed()
	{
		return _minimapAllowed;
	}
	
	/* Enable or disable minimap on Hellbound */
	public void setMinimapAllowed(bool value)
	{
		_minimapAllowed = value;
	}
	
	/**
	 * @return the SP amount of the Player.
	 */
	public long getSp()
	{
		return getStat().getSp();
	}
	
	/**
	 * Set the SP amount of the Player.
	 * @param sp
	 */
	public void setSp(long sp)
	{
		base.getStat().setSp(Math.Max(0, sp));
	}
	
	/**
	 * @param castleId
	 * @return true if this Player is a clan leader in ownership of the passed castle
	 */
	public bool isCastleLord(int castleId)
	{
		// player has clan and is the clan leader, check the castle info
		if ((_clan != null) && (_clan.getLeader().getPlayer() == this))
		{
			// if the clan has a castle and it is actually the queried castle, return true
			Castle castle = CastleManager.getInstance().getCastleByOwner(_clan);
			if ((castle != null) && (castle == CastleManager.getInstance().getCastleById(castleId)))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * @return the Clan Identifier of the Player.
	 */
	public override int getClanId()
	{
		return _clanId;
	}
	
	/**
	 * @return the Clan Crest Identifier of the Player or 0.
	 */
	public int getClanCrestId()
	{
		if (_clan != null)
		{
			return _clan.getCrestId();
		}
		return 0;
	}
	
	/**
	 * @return The Clan CrestLarge Identifier or 0
	 */
	public int getClanCrestLargeId()
	{
		if ((_clan != null) && ((_clan.getCastleId() != 0) || (_clan.getHideoutId() != 0)))
		{
			return _clan.getCrestLargeId();
		}
		return 0;
	}
	
	public long getClanJoinExpiryTime()
	{
		return _clanJoinExpiryTime;
	}
	
	public void setClanJoinExpiryTime(long time)
	{
		_clanJoinExpiryTime = time;
	}
	
	public DateTime getClanJoinTime()
	{
		return getVariables().getLong(PlayerVariables.CLAN_JOIN_TIME, 0L);
	}
	
	public void setClanJoinTime(DateTime time)
	{
		getVariables().set(PlayerVariables.CLAN_JOIN_TIME, time);
	}
	
	public DateTime getClanCreateExpiryTime()
	{
		return _clanCreateExpiryTime;
	}
	
	public void setClanCreateExpiryTime(DateTime time)
	{
		_clanCreateExpiryTime = time;
	}
	
	public void setOnlineTime(long time)
	{
		_onlineTime = time;
		_onlineBeginTime = DateTime.Now;
	}
	
	/**
	 * Return the PcInventory Inventory of the Player contained in _inventory.
	 */
	public override PlayerInventory getInventory()
	{
		return _inventory;
	}
	
	/**
	 * Delete a ShortCut of the Player _shortCuts.
	 * @param objectId
	 */
	public void removeItemFromShortCut(int objectId)
	{
		_shortCuts.deleteShortCutByObjectId(objectId);
	}
	
	/**
	 * @return True if the Player is sitting.
	 */
	public bool isSitting()
	{
		return _waitTypeSitting;
	}
	
	/**
	 * Set _waitTypeSitting to given value
	 * @param value
	 */
	public void setSitting(bool value)
	{
		_waitTypeSitting = value;
	}
	
	/**
	 * Sit down the Player, set the AI Intention to AI_INTENTION_REST and send a Server=>Client ChangeWaitType packet (broadcast)
	 */
	public void sitDown()
	{
		sitDown(true);
	}
	
	public void sitDown(bool checkCast)
	{
		if (checkCast && isCastingNow())
		{
			sendMessage("Cannot sit while casting.");
			return;
		}
		
		if (!_waitTypeSitting && !isAttackDisabled() && !isControlBlocked() && !isImmobilized() && !isFishing())
		{
			breakAttack();
			setSitting(true);
			getAI().setIntention(CtrlIntention.AI_INTENTION_REST);
			broadcastPacket(new ChangeWaitType(this, ChangeWaitType.WT_SITTING));
			// Schedule a sit down task to wait for the animation to finish
			ThreadPool.schedule(new SitDownTask(this), 2500);
			setBlockActions(true);
		}
	}
	
	/**
	 * Stand up the Player, set the AI Intention to AI_INTENTION_IDLE and send a Server=>Client ChangeWaitType packet (broadcast)
	 */
	public void standUp()
	{
		if (_waitTypeSitting && !isInStoreMode() && !isAlikeDead())
		{
			if (getEffectList().isAffected(EffectFlag.RELAXING))
			{
				stopEffects(EffectFlag.RELAXING);
			}
			
			broadcastPacket(new ChangeWaitType(this, ChangeWaitType.WT_STANDING));
			// Schedule a stand up task to wait for the animation to finish
			ThreadPool.schedule(new StandUpTask(this), 2500);
		}
	}
	
	/**
	 * @return the PlayerWarehouse object of the Player.
	 */
	public PlayerWarehouse getWarehouse()
	{
		return _warehouse;
	}
	
	/**
	 * @return the PlayerFreight object of the Player.
	 */
	public PlayerFreight getFreight()
	{
		return _freight;
	}
	
	/**
	 * @return true if refund list is not empty
	 */
	public bool hasRefund()
	{
		return (_refund != null) && (_refund.getSize() > 0) && Config.ALLOW_REFUND;
	}
	
	/**
	 * @return refund object or create new if not exist
	 */
	public PlayerRefund getRefund()
	{
		if (_refund == null)
		{
			_refund = new PlayerRefund(this);
		}
		return _refund;
	}
	
	/**
	 * Clear refund
	 */
	public void clearRefund()
	{
		if (_refund != null)
		{
			_refund.deleteMe();
		}
		_refund = null;
	}
	
	/**
	 * @return the Adena amount of the Player.
	 */
	public long getAdena()
	{
		return _inventory.getAdena();
	}
	
	/**
	 * @return the Ancient Adena amount of the Player.
	 */
	public long getAncientAdena()
	{
		return _inventory.getAncientAdena();
	}
	
	/**
	 * @return the Beauty Tickets of the Player.
	 */
	public long getBeautyTickets()
	{
		return _inventory.getBeautyTickets();
	}
	
	/**
	 * Add adena to Inventory of the Player and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param count : int Quantity of adena to be added
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 */
	public void addAdena(String process, long count, WorldObject reference, bool sendMessage)
	{
		if (sendMessage)
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_OBTAINED_S1_ADENA_2);
			sm.addLong(count);
			sendPacket(sm);
		}
		
		if (count > 0)
		{
			_inventory.addAdena(process, count, this, reference);
			
			// Send update packet
			if (count == getAdena())
			{
				sendItemList();
			}
			else
			{
				InventoryUpdate iu = new InventoryUpdate();
				iu.addModifiedItem(_inventory.getAdenaInstance());
				sendInventoryUpdate(iu);
			}
		}
	}
	
	/**
	 * Reduce adena in Inventory of the Player and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param count : long Quantity of adena to be reduced
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successful
	 */
	public bool reduceAdena(String process, long count, WorldObject reference, bool sendMessage)
	{
		if (count > _inventory.getAdena())
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			}
			return false;
		}
		
		if (count > 0)
		{
			Item adenaItem = _inventory.getAdenaInstance();
			if (!_inventory.reduceAdena(process, count, this, reference))
			{
				return false;
			}
			
			// Send update packet
			InventoryUpdate iu = new InventoryUpdate();
			iu.addItem(adenaItem);
			sendInventoryUpdate(iu);
			
			if (sendMessage)
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.S1_ADENA_DISAPPEARED);
				sm.addLong(count);
				sendPacket(sm);
			}
		}
		
		return true;
	}
	
	/**
	 * Reduce Beauty Tickets in Inventory of the Player and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param count : long Quantity of Beauty Tickets to be reduced
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successful
	 */
	public bool reduceBeautyTickets(String process, long count, WorldObject reference, bool sendMessage)
	{
		if (count > _inventory.getBeautyTickets())
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return false;
		}
		
		if (count > 0)
		{
			Item beautyTickets = _inventory.getBeautyTicketsInstance();
			if (!_inventory.reduceBeautyTickets(process, count, this, reference))
			{
				return false;
			}
			
			// Send update packet
			InventoryUpdate iu = new InventoryUpdate();
			iu.addItem(beautyTickets);
			sendInventoryUpdate(iu);
			
			if (sendMessage)
			{
				if (count > 1)
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.S1_X_S2_DISAPPEARED);
					sm.addItemName(Inventory.BEAUTY_TICKET_ID);
					sm.addLong(count);
					sendPacket(sm);
				}
				else
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.S1_DISAPPEARED);
					sm.addItemName(Inventory.BEAUTY_TICKET_ID);
					sendPacket(sm);
				}
			}
		}
		
		return true;
	}
	
	/**
	 * Add ancient adena to Inventory of the Player and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param count : int Quantity of ancient adena to be added
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 */
	public void addAncientAdena(String process, long count, WorldObject reference, bool sendMessage)
	{
		if (sendMessage)
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
			sm.addItemName(Inventory.ANCIENT_ADENA_ID);
			sm.addLong(count);
			sendPacket(sm);
		}
		
		if (count > 0)
		{
			_inventory.addAncientAdena(process, count, this, reference);
			
			InventoryUpdate iu = new InventoryUpdate();
			iu.addItem(_inventory.getAncientAdenaInstance());
			sendInventoryUpdate(iu);
		}
	}
	
	/**
	 * Reduce ancient adena in Inventory of the Player and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param count : long Quantity of ancient adena to be reduced
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successful
	 */
	public bool reduceAncientAdena(String process, long count, WorldObject reference, bool sendMessage)
	{
		if (count > _inventory.getAncientAdena())
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			}
			return false;
		}
		
		if (count > 0)
		{
			Item ancientAdenaItem = _inventory.getAncientAdenaInstance();
			if (!_inventory.reduceAncientAdena(process, count, this, reference))
			{
				return false;
			}
			
			InventoryUpdate iu = new InventoryUpdate();
			iu.addItem(ancientAdenaItem);
			sendInventoryUpdate(iu);
			
			if (sendMessage)
			{
				if (count > 1)
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.S1_X_S2_DISAPPEARED);
					sm.addItemName(Inventory.ANCIENT_ADENA_ID);
					sm.addLong(count);
					sendPacket(sm);
				}
				else
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.S1_DISAPPEARED);
					sm.addItemName(Inventory.ANCIENT_ADENA_ID);
					sendPacket(sm);
				}
			}
		}
		
		return true;
	}
	
	/**
	 * Adds item to inventory and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be added
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 */
	public void addItem(String process, Item item, WorldObject reference, bool sendMessage)
	{
		if (item.getCount() > 0)
		{
			// Sends message to client if requested
			if (sendMessage)
			{
				if (item.getCount() > 1)
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
					sm.addItemName(item);
					sm.addLong(item.getCount());
					sendPacket(sm);
				}
				else if (item.getEnchantLevel() > 0)
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.YOU_VE_OBTAINED_S1_S2);
					sm.addInt(item.getEnchantLevel());
					sm.addItemName(item);
					sendPacket(sm);
				}
				else
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_OBTAINED_S1);
					sm.addItemName(item);
					sendPacket(sm);
				}
			}
			
			// Add the item to inventory
			Item newitem = _inventory.addItem(process, item, this, reference);
			
			// If over capacity, drop the item
			if (!canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !_inventory.validateCapacity(0, item.isQuestItem()) && newitem.isDropable() && (!newitem.isStackable() || (newitem.getLastChange() != Item.MODIFIED)))
			{
				dropItem("InvDrop", newitem, null, true, true);
			}
			else if (CursedWeaponsManager.getInstance().isCursed(newitem.getId()))
			{
				CursedWeaponsManager.getInstance().activate(this, newitem);
			}
			else if (FortSiegeManager.getInstance().isCombat(item.getId()) && FortSiegeManager.getInstance().activateCombatFlag(this, item))
			{
				Fort fort = FortManager.getInstance().getFort(this);
				fort.getSiege().announceToPlayer(new SystemMessage(SystemMessageId.C1_HAS_ACQUIRED_THE_FLAG), getName());
			}
		}
	}
	
	/**
	 * Adds item to Inventory and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item Identifier of the item to be added
	 * @param count : long Quantity of items to be added
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return
	 */
	public Item addItem(String process, int itemId, long count, WorldObject reference, bool sendMessage)
	{
		return addItem(process, itemId, count, -1, reference, sendMessage);
	}
	
	/**
	 * Adds item to Inventory and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item Identifier of the item to be added
	 * @param count : long Quantity of items to be added
	 * @param enchantLevel : int EnchantLevel of the item to be added
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return
	 */
	public Item addItem(String process, int itemId, long count, int enchantLevel, WorldObject reference, bool sendMessage)
	{
		if (count > 0)
		{
			ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
			if (item == null)
			{
				LOGGER.Error("Item doesn't exist so cannot be added. Item ID: " + itemId);
				return null;
			}
			
			// Sends message to client if requested
			if (sendMessage && ((!isCastingNow() && item.hasExImmediateEffect()) || !item.hasExImmediateEffect()))
			{
				if (count > 1)
				{
					if (process.equalsIgnoreCase("Sweeper") || process.equalsIgnoreCase("Quest"))
					{
						SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
						sm.addItemName(itemId);
						sm.addLong(count);
						sendPacket(sm);
					}
					else
					{
						SystemMessage sm;
						if (enchantLevel > 0)
						{
							sm = new SystemMessage(SystemMessageId.YOU_VE_OBTAINED_S1_S2_X_S3);
							sm.addInt(enchantLevel);
						}
						else
						{
							sm = new SystemMessage(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
						}
						sm.addItemName(itemId);
						sm.addLong(count);
						sendPacket(sm);
					}
				}
				else if (process.equalsIgnoreCase("Sweeper") || process.equalsIgnoreCase("Quest"))
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_ACQUIRED_S1);
					sm.addItemName(itemId);
					sendPacket(sm);
				}
				else
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_OBTAINED_S1);
					sm.addItemName(itemId);
					sendPacket(sm);
				}
			}
			
			// Auto-use herbs.
			if (item.hasExImmediateEffect() && item.isEtcItem())
			{
				foreach (SkillHolder skillHolder in item.getAllSkills())
				{
					SkillCaster.triggerCast(this, null, skillHolder.getSkill(), null, false);
				}
				broadcastInfo();
			}
			else
			{
				// Add the item to inventory
				Item createdItem = _inventory.addItem(process, itemId, count, this, reference);
				if (enchantLevel > -1)
				{
					createdItem.setEnchantLevel(enchantLevel);
				}
				
				// If over capacity, drop the item
				if (!canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !_inventory.validateCapacity(0, item.isQuestItem()) && createdItem.isDropable() && (!createdItem.isStackable() || (createdItem.getLastChange() != Item.MODIFIED)))
				{
					dropItem("InvDrop", createdItem, null, true);
				}
				else if (CursedWeaponsManager.getInstance().isCursed(createdItem.getId()))
				{
					CursedWeaponsManager.getInstance().activate(this, createdItem);
				}
				return createdItem;
			}
		}
		return null;
	}
	
	/**
	 * @param process the process name
	 * @param item the item holder
	 * @param reference the reference object
	 * @param sendMessage if {@code true} a system message will be sent
	 */
	public void addItem(String process, ItemHolder item, WorldObject reference, bool sendMessage)
	{
		addItem(process, item.getId(), item.getCount(), reference, sendMessage);
	}
	
	/**
	 * Destroy item from inventory and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be destroyed
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successful
	 */
	public bool destroyItem(String process, Item item, WorldObject reference, bool sendMessage)
	{
		return destroyItem(process, item, item.getCount(), reference, sendMessage);
	}
	
	/**
	 * Destroy item from inventory and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be destroyed
	 * @param count
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successful
	 */
	public bool destroyItem(String process, Item item, long count, WorldObject reference, bool sendMessage)
	{
		Item destoyedItem = _inventory.destroyItem(process, item, count, this, reference);
		if (destoyedItem == null)
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return false;
		}
		
		// Send inventory update packet
		InventoryUpdate playerIU = new InventoryUpdate();
		if (destoyedItem.isStackable() && (destoyedItem.getCount() > 0))
		{
			playerIU.addModifiedItem(destoyedItem);
		}
		else
		{
			playerIU.addRemovedItem(destoyedItem);
		}
		sendInventoryUpdate(playerIU);
		
		// Sends message to client if requested
		if (sendMessage)
		{
			if (count > 1)
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.S1_X_S2_DISAPPEARED);
				sm.addItemName(destoyedItem);
				sm.addLong(count);
				sendPacket(sm);
			}
			else
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.S1_DISAPPEARED);
				sm.addItemName(destoyedItem);
				sendPacket(sm);
			}
		}
		
		return true;
	}
	
	/**
	 * Destroys item from inventory and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Instance identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successful
	 */
	public override bool destroyItem(String process, int objectId, long count, WorldObject reference, bool sendMessage)
	{
		Item item = _inventory.getItemByObjectId(objectId);
		if (item == null)
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return false;
		}
		return destroyItem(process, item, count, reference, sendMessage);
	}
	
	/**
	 * Destroys shots from inventory without logging and only occasional saving to database. Sends a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Instance identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successful
	 */
	public bool destroyItemWithoutTrace(String process, int objectId, long count, WorldObject reference, bool sendMessage)
	{
		Item item = _inventory.getItemByObjectId(objectId);
		if ((item == null) || (item.getCount() < count))
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return false;
		}
		
		return destroyItem(null, item, count, reference, sendMessage);
	}
	
	/**
	 * Destroy item from inventory by using its <b>itemId</b> and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successful
	 */
	public override bool destroyItemByItemId(String process, int itemId, long count, WorldObject reference, bool sendMessage)
	{
		if (itemId == Inventory.ADENA_ID)
		{
			return reduceAdena(process, count, reference, sendMessage);
		}
		
		Item item = _inventory.getItemByItemId(itemId);
		long quantity = (count < 0) && (item != null) ? item.getCount() : count;
		if ((item == null) || (item.getCount() < quantity) || (quantity <= 0) || (_inventory.destroyItemByItemId(process, itemId, quantity, this, reference) == null))
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return false;
		}
		
		// Send inventory update packet
		InventoryUpdate playerIU = new InventoryUpdate();
		if (item.isStackable() && (item.getCount() > 0))
		{
			playerIU.addModifiedItem(item);
		}
		else
		{
			playerIU.addRemovedItem(item);
		}
		sendInventoryUpdate(playerIU);
		
		// Sends message to client if requested
		if (sendMessage)
		{
			if (quantity > 1)
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.S1_X_S2_DISAPPEARED);
				sm.addItemName(itemId);
				sm.addLong(quantity);
				sendPacket(sm);
			}
			else
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.S1_DISAPPEARED);
				sm.addItemName(itemId);
				sendPacket(sm);
			}
		}
		
		return true;
	}
	
	/**
	 * Transfers item to another ItemContainer and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Identifier of the item to be transfered
	 * @param count : long Quantity of items to be transfered
	 * @param target
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public Item transferItem(String process, int objectId, long count, Inventory target, WorldObject reference)
	{
		Item oldItem = checkItemManipulation(objectId, count, "transfer");
		if (oldItem == null)
		{
			return null;
		}
		Item newItem = _inventory.transferItem(process, objectId, count, target, this, reference);
		if (newItem == null)
		{
			return null;
		}
		
		// Send inventory update packet
		InventoryUpdate playerIU = new InventoryUpdate();
		if ((oldItem.getCount() > 0) && (oldItem != newItem))
		{
			playerIU.addModifiedItem(oldItem);
		}
		else
		{
			playerIU.addRemovedItem(oldItem);
		}
		sendInventoryUpdate(playerIU);
		
		// Send target update packet
		if (target is PlayerInventory)
		{
			Player targetPlayer = ((PlayerInventory) target).getOwner();
			InventoryUpdate targetIU = new InventoryUpdate();
			if (newItem.getCount() > count)
			{
				targetIU.addModifiedItem(newItem);
			}
			else
			{
				targetIU.addNewItem(newItem);
			}
			targetPlayer.sendPacket(targetIU);
		}
		
		// LCoin UI update.
		if (newItem.getId() == Inventory.LCOIN_ID)
		{
			sendPacket(new ExBloodyCoinCount(this));
		}
		
		return newItem;
	}
	
	/**
	 * Use instead of calling {@link #addItem(String, Item, WorldObject, bool)} and {@link #destroyItemByItemId(String, int, long, WorldObject, bool)}<br>
	 * This method validates slots and weight limit, for stackable and non-stackable items.
	 * @param process a generic string representing the process that is exchanging this items
	 * @param reference the (probably NPC) reference, could be null
	 * @param coinId the item Id of the item given on the exchange
	 * @param cost the amount of items given on the exchange
	 * @param rewardId the item received on the exchange
	 * @param count the amount of items received on the exchange
	 * @param sendMessage if {@code true} it will send messages to the acting player
	 * @return {@code true} if the player successfully exchanged the items, {@code false} otherwise
	 */
	public bool exchangeItemsById(String process, WorldObject reference, int coinId, long cost, int rewardId, long count, bool sendMessage)
	{
		if (!_inventory.validateCapacityByItemId(rewardId, count))
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			}
			return false;
		}
		
		if (!_inventory.validateWeightByItemId(rewardId, count))
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
			}
			return false;
		}
		
		if (destroyItemByItemId(process, coinId, cost, reference, sendMessage))
		{
			addItem(process, rewardId, count, reference, sendMessage);
			return true;
		}
		return false;
	}
	
	/**
	 * Drop item from inventory and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process String Identifier of process triggering this action
	 * @param item Item to be dropped
	 * @param reference WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage bool Specifies whether to send message to Client about this action
	 * @param protectItem whether or not dropped item must be protected temporary against other players
	 * @return bool informing if the action was successful
	 */
	public bool dropItem(String process, Item item, WorldObject reference, bool sendMessage, bool protectItem)
	{
		Item droppedItem = _inventory.dropItem(process, item, this, reference);
		if (droppedItem == null)
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return false;
		}
		
		droppedItem.dropMe(this, (getX() + Rnd.get(50)) - 25, (getY() + Rnd.get(50)) - 25, getZ() + 20);
		if ((Config.AUTODESTROY_ITEM_AFTER > 0) && Config.DESTROY_DROPPED_PLAYER_ITEM && !Config.LIST_PROTECTED_ITEMS.Contains(droppedItem.getId()) && ((droppedItem.isEquipable() && Config.DESTROY_EQUIPABLE_PLAYER_ITEM) || !droppedItem.isEquipable()))
		{
			ItemsAutoDestroyTaskManager.getInstance().addItem(droppedItem);
		}
		
		// protection against auto destroy dropped item
		if (Config.DESTROY_DROPPED_PLAYER_ITEM)
		{
			droppedItem.setProtected(droppedItem.isEquipable() && (!droppedItem.isEquipable() || !Config.DESTROY_EQUIPABLE_PLAYER_ITEM));
		}
		else
		{
			droppedItem.setProtected(true);
		}
		
		// retail drop protection
		if (protectItem)
		{
			droppedItem.getDropProtection().protect(this);
		}
		
		// Send inventory update packet
		InventoryUpdate playerIU = new InventoryUpdate();
		playerIU.addItem(droppedItem);
		sendInventoryUpdate(playerIU);
		
		// Sends message to client if requested
		if (sendMessage)
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_DROPPED_S1);
			sm.addItemName(droppedItem);
			sendPacket(sm);
		}
		
		// LCoin UI update.
		if (item.getId() == Inventory.LCOIN_ID)
		{
			sendPacket(new ExBloodyCoinCount(this));
		}
		
		return true;
	}
	
	public bool dropItem(String process, Item item, WorldObject reference, bool sendMessage)
	{
		return dropItem(process, item, reference, sendMessage, false);
	}
	
	/**
	 * Drop item from inventory by using its <b>objectID</b> and send a Server=>Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Instance identifier of the item to be dropped
	 * @param count : long Quantity of items to be dropped
	 * @param x : int coordinate for drop X
	 * @param y : int coordinate for drop Y
	 * @param z : int coordinate for drop Z
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @param protectItem
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public Item dropItem(String process, int objectId, long count, int x, int y, int z, WorldObject reference, bool sendMessage, bool protectItem)
	{
		Item invitem = _inventory.getItemByObjectId(objectId);
		Item item = _inventory.dropItem(process, objectId, count, this, reference);
		if (item == null)
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return null;
		}
		
		item.dropMe(this, x, y, z);
		if ((Config.AUTODESTROY_ITEM_AFTER > 0) && Config.DESTROY_DROPPED_PLAYER_ITEM && !Config.LIST_PROTECTED_ITEMS.contains(item.getId()) && ((item.isEquipable() && Config.DESTROY_EQUIPABLE_PLAYER_ITEM) || !item.isEquipable()))
		{
			ItemsAutoDestroyTaskManager.getInstance().addItem(item);
		}
		if (Config.DESTROY_DROPPED_PLAYER_ITEM)
		{
			item.setProtected(item.isEquipable() && (!item.isEquipable() || !Config.DESTROY_EQUIPABLE_PLAYER_ITEM));
		}
		else
		{
			item.setProtected(true);
		}
		
		// retail drop protection
		if (protectItem)
		{
			item.getDropProtection().protect(this);
		}
		
		// Send inventory update packet
		InventoryUpdate playerIU = new InventoryUpdate();
		playerIU.addItem(invitem);
		sendInventoryUpdate(playerIU);
		
		// Sends message to client if requested
		if (sendMessage)
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_DROPPED_S1);
			sm.addItemName(item);
			sendPacket(sm);
		}
		
		// LCoin UI update.
		if (item.getId() == Inventory.LCOIN_ID)
		{
			sendPacket(new ExBloodyCoinCount(this));
		}
		
		return item;
	}
	
	public Item checkItemManipulation(int objectId, long count, String action)
	{
		// TODO: if we remove objects that are not visible from the World, we'll have to remove this check
		if (World.getInstance().findObject(objectId) == null)
		{
			LOGGER.Info(getObjectId() + ": player tried to " + action + " item not available in World");
			return null;
		}
		
		Item item = _inventory.getItemByObjectId(objectId);
		if ((item == null) || (item.getOwnerId() != getObjectId()))
		{
			LOGGER.Info(getObjectId() + ": player tried to " + action + " item he is not owner of");
			return null;
		}
		
		if ((count < 0) || ((count > 1) && !item.isStackable()))
		{
			LOGGER.Info(getObjectId() + ": player tried to " + action + " item with invalid count: " + count);
			return null;
		}
		
		if (count > item.getCount())
		{
			LOGGER.Info(getObjectId() + ": player tried to " + action + " more items than he owns");
			return null;
		}
		
		// Pet is summoned and not the item that summoned the pet AND not the buggle from strider you're mounting
		if (((_pet != null) && (_pet.getControlObjectId() == objectId)) || (_mountObjectID == objectId))
		{
			return null;
		}
		
		if (isProcessingItem(objectId))
		{
			return null;
		}
		
		// We cannot put a Weapon with Augmentation in WH while casting (Possible Exploit)
		if (item.isAugmented() && isCastingNow())
		{
			return null;
		}
		
		return item;
	}
	
	public bool isSpawnProtected()
	{
		return (_spawnProtectEndTime != 0) && (_spawnProtectEndTime > DateTime.Now);
	}
	
	public bool isTeleportProtected()
	{
		return (_teleportProtectEndTime != 0) && (_teleportProtectEndTime > DateTime.Now);
	}
	
	public void setSpawnProtection(bool protect)
	{
		_spawnProtectEndTime = protect ? DateTime.Now.AddMilliseconds(Config.PLAYER_SPAWN_PROTECTION * 1000) : 0;
	}
	
	public void setTeleportProtection(bool protect)
	{
		_teleportProtectEndTime = protect ? DateTime.Now.AddMilliseconds(Config.PLAYER_TELEPORT_PROTECTION * 1000) : 0;
	}
	
	/**
	 * Set protection from aggro mobs when getting up from fake death, according settings.
	 * @param protect
	 */
	public void setRecentFakeDeath(bool protect)
	{
		_recentFakeDeathEndTime = protect ? GameTimeTaskManager.getInstance().getGameTicks() + (Config.PLAYER_FAKEDEATH_UP_PROTECTION * GameTimeTaskManager.TICKS_PER_SECOND) : 0;
	}
	
	public bool isRecentFakeDeath()
	{
		return _recentFakeDeathEndTime > GameTimeTaskManager.getInstance().getGameTicks();
	}
	
	public bool isFakeDeath()
	{
		return isAffected(EffectFlag.FAKE_DEATH);
	}
	
	public override bool isAlikeDead()
	{
		return base.isAlikeDead() || isFakeDeath();
	}
	
	/**
	 * @return the client owner of this char.
	 */
	public GameClient getClient()
	{
		return _client;
	}
	
	public void setClient(GameClient client)
	{
		_client = client;
		if ((_client != null) && (_client.getIp() != null))
		{
			_ip = _client.getIp();
		}
	}
	
	public String getIPAddress()
	{
		return _ip;
	}
	
	public Location getCurrentSkillWorldPosition()
	{
		return _currentSkillWorldPosition;
	}
	
	public void setCurrentSkillWorldPosition(Location worldPosition)
	{
		_currentSkillWorldPosition = worldPosition;
	}
	
	public void enableSkill(Skill skill, bool removeTimeStamp)
	{
		base.enableSkill(skill);
		if (removeTimeStamp)
		{
			this.removeTimeStamp(skill);
		}
	}
	
	public override void enableSkill(Skill skill)
	{
		enableSkill(skill, true);
	}
	
	/**
	 * Returns true if cp update should be done, false if not.
	 * @return bool
	 */
	private bool needCpUpdate()
	{
		double currentCp = getCurrentCp();
		if ((currentCp <= 1.0) || (getMaxCp() < MAX_HP_BAR_PX))
		{
			return true;
		}
		
		if ((currentCp <= _cpUpdateDecCheck) || (currentCp >= _cpUpdateIncCheck))
		{
			if (currentCp == getMaxCp())
			{
				_cpUpdateIncCheck = currentCp + 1;
				_cpUpdateDecCheck = currentCp - _cpUpdateInterval;
			}
			else
			{
				double doubleMulti = currentCp / _cpUpdateInterval;
				int intMulti = (int) doubleMulti;
				_cpUpdateDecCheck = _cpUpdateInterval * (doubleMulti < intMulti ? intMulti - 1 : intMulti);
				_cpUpdateIncCheck = _cpUpdateDecCheck + _cpUpdateInterval;
			}
			
			return true;
		}
		
		return false;
	}
	
	/**
	 * Returns true if mp update should be done, false if not.
	 * @return bool
	 */
	private bool needMpUpdate()
	{
		double currentMp = getCurrentMp();
		if ((currentMp <= 1.0) || (getMaxMp() < MAX_HP_BAR_PX))
		{
			return true;
		}
		
		if ((currentMp <= _mpUpdateDecCheck) || (currentMp >= _mpUpdateIncCheck))
		{
			if (currentMp == getMaxMp())
			{
				_mpUpdateIncCheck = currentMp + 1;
				_mpUpdateDecCheck = currentMp - _mpUpdateInterval;
			}
			else
			{
				double doubleMulti = currentMp / _mpUpdateInterval;
				int intMulti = (int) doubleMulti;
				_mpUpdateDecCheck = _mpUpdateInterval * (doubleMulti < intMulti ? intMulti - 1 : intMulti);
				_mpUpdateIncCheck = _mpUpdateDecCheck + _mpUpdateInterval;
			}
			
			return true;
		}
		
		return false;
	}
	
	/**
	 * Send packet StatusUpdate with current HP,MP and CP to the Player and only current HP, MP and Level to all other Player of the Party. <b><u>Actions</u>:</b>
	 * <li>Send the Server=>Client packet StatusUpdate with current HP, MP and CP to this Player</li>
	 * <li>Send the Server=>Client packet PartySmallWindowUpdate with current HP, MP and Level to all other Player of the Party</li> <font color=#FF0000><b><u>Caution</u>: This method DOESN'T SEND current HP and MP to all Player of the _statusListener</b></font>
	 */
	public override void broadcastStatusUpdate(Creature caster)
	{
		StatusUpdate su = new StatusUpdate(this);
		if (caster != null)
		{
			su.addCaster(caster);
		}
		
		computeStatusUpdate(su, StatusUpdateType.LEVEL);
		computeStatusUpdate(su, StatusUpdateType.MAX_HP);
		computeStatusUpdate(su, StatusUpdateType.CUR_HP);
		computeStatusUpdate(su, StatusUpdateType.MAX_MP);
		computeStatusUpdate(su, StatusUpdateType.CUR_MP);
		computeStatusUpdate(su, StatusUpdateType.MAX_CP);
		computeStatusUpdate(su, StatusUpdateType.CUR_CP);
		if (su.hasUpdates())
		{
			broadcastPacket(su);
		}
		
		bool needCpUpdate = needCpUpdate();
		bool needHpUpdate = needHpUpdate();
		bool needMpUpdate = needMpUpdate();
		Party party = getParty();
		
		// Check if a party is in progress and party window update is usefull
		if ((_party != null) && (needCpUpdate || needHpUpdate || needMpUpdate))
		{
			PartySmallWindowUpdate partyWindow = new PartySmallWindowUpdate(this, false);
			if (needCpUpdate)
			{
				partyWindow.addComponentType(PartySmallWindowUpdateType.CURRENT_CP);
				partyWindow.addComponentType(PartySmallWindowUpdateType.MAX_CP);
			}
			if (needHpUpdate)
			{
				partyWindow.addComponentType(PartySmallWindowUpdateType.CURRENT_HP);
				partyWindow.addComponentType(PartySmallWindowUpdateType.MAX_HP);
			}
			if (needMpUpdate)
			{
				partyWindow.addComponentType(PartySmallWindowUpdateType.CURRENT_MP);
				partyWindow.addComponentType(PartySmallWindowUpdateType.MAX_MP);
			}
			party.broadcastToPartyMembers(this, partyWindow);
		}
		
		if (_inOlympiadMode && _olympiadStart && (needCpUpdate || needHpUpdate))
		{
			OlympiadGameTask game = OlympiadGameManager.getInstance().getOlympiadTask(getOlympiadGameId());
			if ((game != null) && game.isBattleStarted())
			{
				game.getStadium().broadcastStatusUpdate(this);
			}
		}
		
		// In duel MP updated only with CP or HP
		if (_isInDuel && (needCpUpdate || needHpUpdate))
		{
			DuelManager.getInstance().broadcastToOppositTeam(this, new ExDuelUpdateUserInfo(this));
		}
	}
	
	/**
	 * Send a Server=>Client packet UserInfo to this Player and CharInfo to all known players.<br>
	 * <font color=#FF0000><b><u>Caution</u>: DON'T SEND UserInfo packet to other players instead of CharInfo packet.<br>
	 * UserInfo packet contains PRIVATE DATA as MaxHP, STR, DEX...</b></font>
	 */
	public void broadcastUserInfo()
	{
		// Send user info to the current player.
		updateUserInfo();
		
		// Broadcast char info to known players.
		broadcastCharInfo();
	}
	
	public void updateUserInfo()
	{
		sendPacket(new UserInfo(this));
		sendPacket(new ExUserViewInfoParameter(this));
	}
	
	public void broadcastUserInfo(params UserInfoType[] types)
	{
		// Send user info to the current player
		UserInfo ui = new UserInfo(this, false);
		ui.addComponentType(types);
		sendPacket(ui);
		
		// Broadcast char info to all known players
		broadcastCharInfo();
	}
	
	public void broadcastCharInfo()
	{
		// Client is disconnected.
		if (isOnlineInt() == 0)
		{
			return;
		}
		
		if (_broadcastCharInfoTask == null)
		{
			_broadcastCharInfoTask = ThreadPool.schedule(() =>
			{
				CharInfo charInfo = new CharInfo(this, false);
				World.getInstance().forEachVisibleObject<Player>(this, player =>
				{
					if (isVisibleFor(player))
					{
						if (isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS))
						{
							player.sendPacket(new CharInfo(this, true));
						}
						else
						{
							player.sendPacket(charInfo);
						}
						
						// Update relation.
						long relation = getRelation(player);
						bool isAutoAttackable = this.isAutoAttackable(player);
						RelationCache oldrelation = getKnownRelations().get(player.getObjectId());
						if ((oldrelation == null) || (oldrelation.getRelation() != relation) || (oldrelation.isAutoAttackable() != isAutoAttackable))
						{
							RelationChanged rc = new RelationChanged();
							rc.addRelation(this, relation, isAutoAttackable);
							if (hasSummon())
							{
								Summon pet = getPet();
								if (pet != null)
								{
									rc.addRelation(pet, relation, isAutoAttackable);
								}
								if (hasServitors())
								{
									getServitors().values().forEach(s => rc.addRelation(s, relation, isAutoAttackable));
								}
							}
							player.sendPacket(rc);
							getKnownRelations().put(player.getObjectId(), new RelationCache(relation, isAutoAttackable));
						}
					}
				});
				_broadcastCharInfoTask = null;
			}, 50);
		}
	}
	
	public void broadcastTitleInfo()
	{
		// Send a Server=>Client packet UserInfo to this Player.
		broadcastUserInfo(UserInfoType.CLAN);
		
		// Send a Server=>Client packet TitleUpdate to all known players.
		broadcastPacket(new NicknameChanged(this));
	}
	
	public override void broadcastPacket<TPacket>(TPacket packet, bool includeSelf)
	{
		if (packet is CharInfo)
		{
			throw new ArgumentException("CharInfo is being send via broadcastPacket. Do NOT do that! Use broadcastCharInfo() instead.");
		}
		
		if (includeSelf)
		{
			sendPacket(packet);
		}
		
		World.getInstance().forEachVisibleObject<Player>(this, player =>
		{
			if (!isVisibleFor(player))
			{
				return;
			}
			
			player.sendPacket(packet);
		});
	}
	
	public override void broadcastPacket<TPacket>(TPacket packet, int radiusInKnownlist)
	{
		if (packet is CharInfo)
		{
			throw new ArgumentException("CharInfo is being send via broadcastPacket. Do NOT do that! Use broadcastCharInfo() instead.");
		}
		
		sendPacket(packet);
		
		World.getInstance().forEachVisibleObject<Player>(this, player =>
		{
			if (!isVisibleFor(player) || (calculateDistance3D(player) >= radiusInKnownlist))
			{
				return;
			}
			player.sendPacket(packet);
		});
	}
	
	/**
	 * @return the Alliance Identifier of the Player.
	 */
	public override int getAllyId()
	{
		return _clan == null ? 0 : _clan.getAllyId();
	}
	
	public int getAllyCrestId()
	{
		return getAllyId() == 0 ? 0 : _clan.getAllyCrestId();
	}
	
	public override void sendPacket<TPacket>(TPacket packet)
	{
		if (_client != null)
		{
			_client.sendPacket(packet);
		}
	}
	
	/**
	 * Send SystemMessage packet.
	 * @param id SystemMessageId
	 */
	public override void sendPacket(SystemMessageId id)
	{
		sendPacket(new SystemMessage(id));
	}
	
	/**
	 * Manage Interact Task with another Player. <b><u>Actions</u>:</b>
	 * <li>If the private store is a STORE_PRIVATE_SELL, send a Server=>Client PrivateBuyListSell packet to the Player</li>
	 * <li>If the private store is a STORE_PRIVATE_BUY, send a Server=>Client PrivateBuyListBuy packet to the Player</li>
	 * <li>If the private store is a STORE_PRIVATE_MANUFACTURE, send a Server=>Client RecipeShopSellList packet to the Player</li><br>
	 * @param target The Creature targeted
	 */
	public void doInteract(Creature target)
	{
		if (target == null)
		{
			return;
		}
		
		if (target.isPlayer())
		{
			Player targetPlayer = (Player) target;
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			
			if ((targetPlayer.getPrivateStoreType() == PrivateStoreType.SELL) || (targetPlayer.getPrivateStoreType() == PrivateStoreType.PACKAGE_SELL))
			{
				if (_isSellingBuffs)
				{
					SellBuffsManager.getInstance().sendBuffMenu(this, targetPlayer, 0);
				}
				else
				{
					sendPacket(new PrivateStoreListSell(this, targetPlayer));
				}
			}
			else if (targetPlayer.getPrivateStoreType() == PrivateStoreType.BUY)
			{
				sendPacket(new PrivateStoreListBuy(this, targetPlayer));
			}
			else if (targetPlayer.getPrivateStoreType() == PrivateStoreType.MANUFACTURE)
			{
				sendPacket(new RecipeShopSellList(this, targetPlayer));
			}
		}
		else // _interactTarget=null should never happen but one never knows ^^;
		{
			target.onAction(this);
		}
	}
	
	/**
	 * Manages AutoLoot Task.<br>
	 * <ul>
	 * <li>Send a system message to the player.</li>
	 * <li>Add the item to the player's inventory.</li>
	 * <li>Send a Server=>Client packet InventoryUpdate to this player with NewItem (use a new slot) or ModifiedItem (increase amount).</li>
	 * <li>Send a Server=>Client packet StatusUpdate to this player with current weight.</li>
	 * </ul>
	 * <font color=#FF0000><b><u>Caution</u>: If a party is in progress, distribute the items between the party members!</b></font>
	 * @param target the NPC dropping the item
	 * @param itemId the item ID
	 * @param itemCount the item count
	 */
	public void doAutoLoot(Attackable target, int itemId, long itemCount)
	{
		if (isInParty() && !ItemData.getInstance().getTemplate(itemId).hasExImmediateEffect())
		{
			_party.distributeItem(this, itemId, itemCount, false, target);
		}
		else if (itemId == Inventory.ADENA_ID)
		{
			addAdena("Loot", itemCount, target, true);
		}
		else
		{
			addItem("Loot", itemId, itemCount, target, true);
		}
	}
	
	/**
	 * Method overload for {@link Player#doAutoLoot(Attackable, int, long)}
	 * @param target the NPC dropping the item
	 * @param item the item holder
	 */
	public void doAutoLoot(Attackable target, ItemHolder item)
	{
		doAutoLoot(target, item.getId(), item.getCount());
	}
	
	/**
	 * Manage Pickup Task. <b><u>Actions</u>:</b>
	 * <li>Send a Server=>Client packet StopMove to this Player</li>
	 * <li>Remove the Item from the world and send server=>client GetItem packets</li>
	 * <li>Send a System Message to the Player : YOU_PICKED_UP_S1_ADENA or YOU_PICKED_UP_S1_S2</li>
	 * <li>Add the Item to the Player inventory</li>
	 * <li>Send a Server=>Client packet InventoryUpdate to this Player with NewItem (use a new slot) or ModifiedItem (increase amount)</li>
	 * <li>Send a Server=>Client packet StatusUpdate to this Player with current weight</li> <font color=#FF0000><b><u>Caution</u>: If a Party is in progress, distribute Items between party members</b></font>
	 * @param object The Item to pick up
	 */
	public override void doPickupItem(WorldObject obj)
	{
		if (isAlikeDead() || isFakeDeath())
		{
			return;
		}
		
		// Set the AI Intention to AI_INTENTION_IDLE
		getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		
		// Check if the WorldObject to pick up is a Item
		if (!obj.isItem())
		{
			// dont try to pickup anything that is not an item :)
			LOGGER.Warn(this + " trying to pickup wrong target." + getTarget());
			return;
		}
		
		Item target = (Item)obj;
		
		// Send a Server=>Client packet ActionFailed to this Player
		sendPacket(ActionFailedPacket.STATIC_PACKET);
		
		// Send a Server=>Client packet StopMove to this Player
		sendPacket(new StopMovePacket(this));
		SystemMessage smsg = null;
		lock (target)
		{
			// Check if the target to pick up is visible
			if (!target.isSpawned())
			{
				// Send a Server=>Client packet ActionFailed to this Player
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				return;
			}
			
			if (!target.getDropProtection().tryPickUp(this))
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				smsg = new SystemMessage(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
				smsg.addItemName(target);
				sendPacket(smsg);
				return;
			}
			
			if (((isInParty() && (_party.getDistributionType() == PartyDistributionType.FINDERS_KEEPERS)) || !isInParty()) && !_inventory.validateCapacity(target))
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
				return;
			}
			
			if (isInvul() && !canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS))
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				smsg = new SystemMessage(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
				smsg.addItemName(target);
				sendPacket(smsg);
				return;
			}
			
			if ((target.getOwnerId() != 0) && (target.getOwnerId() != getObjectId()) && !isInLooterParty(target.getOwnerId()))
			{
				if (target.getId() == Inventory.ADENA_ID)
				{
					smsg = new SystemMessage(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1_ADENA);
					smsg.addLong(target.getCount());
				}
				else if (target.getCount() > 1)
				{
					smsg = new SystemMessage(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S2_S1_S);
					smsg.addItemName(target);
					smsg.addLong(target.getCount());
				}
				else
				{
					smsg = new SystemMessage(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
					smsg.addItemName(target);
				}
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(smsg);
				return;
			}
			
			// You can pickup only 1 combat flag
			if (FortSiegeManager.getInstance().isCombat(target.getId()) && !FortSiegeManager.getInstance().checkIfCanPickup(this))
			{
				return;
			}
			
			if ((target.getItemLootShedule() != null) && ((target.getOwnerId() == getObjectId()) || isInLooterParty(target.getOwnerId())))
			{
				target.resetOwnerTimer();
			}
			
			// Remove the Item from the world and send server=>client GetItem packets
			target.pickupMe(this);
			if (Config.SAVE_DROPPED_ITEM)
			{
				ItemsOnGroundManager.getInstance().removeObject(target);
			}
		}
		
		// Auto use herbs - pick up
		if (target.getTemplate().hasExImmediateEffect())
		{
			IItemHandler handler = ItemHandler.getInstance().getHandler(target.getEtcItem());
			if (handler == null)
			{
				LOGGER.Warn("No item handler registered for item ID: " + target.getId() + ".");
			}
			else
			{
				handler.useItem(this, target, false);
			}
			ItemData.getInstance().destroyItem("Consume", target, this, null);
		}
		// Cursed Weapons are not distributed
		else if (CursedWeaponsManager.getInstance().isCursed(target.getId()))
		{
			addItem("Pickup", target, null, true);
		}
		else if (FortSiegeManager.getInstance().isCombat(target.getId()))
		{
			addItem("Pickup", target, null, true);
		}
		else
		{
			// if item is instance of ArmorType or WeaponType broadcast an "Attention" system message
			if ((target.getItemType() is ArmorType) || (target.getItemType() is WeaponType))
			{
				if (target.getEnchantLevel() > 0)
				{
					smsg = new SystemMessage(SystemMessageId.ATTENTION_C1_HAS_PICKED_UP_S2_S3);
					smsg.addPcName(this);
					smsg.addInt(target.getEnchantLevel());
					smsg.addItemName(target.getId());
					broadcastPacket(smsg, 1400);
				}
				else
				{
					smsg = new SystemMessage(SystemMessageId.ATTENTION_C1_HAS_PICKED_UP_S2);
					smsg.addPcName(this);
					smsg.addItemName(target.getId());
					broadcastPacket(smsg, 1400);
				}
			}
			
			// Check if a Party is in progress
			if (isInParty())
			{
				_party.distributeItem(this, target);
			}
			else if ((target.getId() == Inventory.ADENA_ID) && (_inventory.getAdenaInstance() != null))
			{
				addAdena("Pickup", target.getCount(), null, true);
				ItemData.getInstance().destroyItem("Pickup", target, this, null);
			}
			else
			{
				addItem("Pickup", target, null, true);
			}
		}
	}
	
	public override void doAutoAttack(Creature target)
	{
		base.doAutoAttack(target);
		setRecentFakeDeath(false);
		if (target.isFakePlayer() && !Config.FAKE_PLAYER_AUTO_ATTACKABLE)
		{
			updatePvPStatus();
		}
	}
	
	public override void doCast(Skill skill)
	{
		base.doCast(skill);
		setRecentFakeDeath(false);
	}
	
	public bool canOpenPrivateStore()
	{
		if ((Config.SHOP_MIN_RANGE_FROM_NPC > 0) || (Config.SHOP_MIN_RANGE_FROM_PLAYER > 0))
		{
			foreach (Creature creature in World.getInstance().getVisibleObjectsInRange<Creature>(this, 1000))
			{
				if ((creature.getMinShopDistance() > 0) && Util.checkIfInRange(creature.getMinShopDistance(), this, creature, true))
				{
					sendPacket(new SystemMessage(SystemMessageId.YOU_CANNOT_OPEN_A_PRIVATE_STORE_HERE));
					return false;
				}
			}
		}
		return !_isSellingBuffs && !isAlikeDead() && !_inOlympiadMode && !isMounted() && !isInsideZone(ZoneId.NO_STORE) && !isCastingNow();
	}
	
	public override int getMinShopDistance()
	{
		return _waitTypeSitting ? Config.SHOP_MIN_RANGE_FROM_PLAYER : 0;
	}
	
	public void tryOpenPrivateBuyStore()
	{
		// Player shouldn't be able to set stores if he/she is alike dead (dead or fake death)
		if (canOpenPrivateStore())
		{
			if ((_privateStoreType == PrivateStoreType.BUY) || (_privateStoreType == PrivateStoreType.BUY_MANAGE))
			{
				setPrivateStoreType(PrivateStoreType.NONE);
			}
			if (_privateStoreType == PrivateStoreType.NONE)
			{
				if (_waitTypeSitting)
				{
					standUp();
				}
				setPrivateStoreType(PrivateStoreType.BUY_MANAGE);
				sendPacket(new PrivateStoreManageListBuy(1, this));
				sendPacket(new PrivateStoreManageListBuy(2, this));
			}
		}
		else
		{
			if (isInsideZone(ZoneId.NO_STORE))
			{
				sendPacket(SystemMessageId.YOU_CANNOT_OPEN_A_PRIVATE_STORE_HERE);
			}
			sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
	}
	
	public PreparedMultisellListHolder getMultiSell()
	{
		return _currentMultiSell;
	}
	
	public void setMultiSell(PreparedMultisellListHolder list)
	{
		_currentMultiSell = list;
	}
	
	/**
	 * Set a target. <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Remove the Player from the _statusListener of the old target if it was a Creature</li>
	 * <li>Add the Player to the _statusListener of the new target if it's a Creature</li>
	 * <li>Target the new WorldObject (add the target to the Player _target, _knownObject and Player to _KnownObject of the WorldObject)</li>
	 * </ul>
	 * @param worldObject The WorldObject to target
	 */
	public override void setTarget(WorldObject worldObject)
	{
		WorldObject newTarget = worldObject;
		if (newTarget != null)
		{
			bool isInParty = (newTarget.isPlayer() && this.isInParty() && _party.containsPlayer(newTarget.getActingPlayer()));
			
			// Prevents /target exploiting
			if (!isInParty && (Math.Abs(newTarget.getZ() - getZ()) > 1000))
			{
				newTarget = null;
			}
			
			// Check if the new target is visible
			if ((newTarget != null) && !isInParty && !newTarget.isSpawned())
			{
				newTarget = null;
			}
			
			// vehicles cant be targeted
			if (!isGM() && (newTarget is Vehicle))
			{
				newTarget = null;
			}
		}
		
		// Get the current target
		WorldObject oldTarget = getTarget();
		if (oldTarget != null)
		{
			if (oldTarget.Equals(newTarget)) // no target change?
			{
				// Validate location of the target.
				if ((newTarget != null) && (newTarget.getObjectId() != getObjectId()))
				{
					sendPacket(new ValidateLocation(newTarget));
				}
				return;
			}
			
			// Remove the target from the status listener.
			oldTarget.removeStatusListener(this);
		}
		
		if ((newTarget != null) && newTarget.isCreature())
		{
			Creature target = (Creature) newTarget;
			
			// Validate location of the new target.
			if (newTarget.getObjectId() != getObjectId())
			{
				sendPacket(new ValidateLocation(target));
			}
			
			// Show the client his new target.
			sendPacket(new MyTargetSelected(this, target));
			
			// Register target to listen for hp changes.
			target.addStatusListener(this);
			
			// Send max/current hp.
			StatusUpdate su = new StatusUpdate(target);
			su.addUpdate(StatusUpdateType.MAX_HP, target.getMaxHp());
			su.addUpdate(StatusUpdateType.CUR_HP, (int) target.getCurrentHp());
			sendPacket(su);
			
			// To others the new target, and not yourself!
			Broadcast.toKnownPlayers(this, new TargetSelected(getObjectId(), newTarget.getObjectId(), getX(), getY(), getZ()));
			
			// Send buffs
			sendPacket(new ExAbnormalStatusUpdateFromTarget(target));
		}
		
		// Target was removed?
		if ((newTarget == null) && (getTarget() != null))
		{
			broadcastPacket(new TargetUnselected(this));
		}
		
		// Target the new WorldObject (add the target to the Player _target, _knownObject and Player to _KnownObject of the WorldObject)
		base.setTarget(newTarget);
	}
	
	/**
	 * Return the active weapon instance (always equipped in the right hand).
	 */
	public override Item getActiveWeaponInstance()
	{
		return _inventory.getPaperdollItem(Inventory.PAPERDOLL_RHAND);
	}
	
	/**
	 * Return the active weapon item (always equipped in the right hand).
	 */
	public override Weapon getActiveWeaponItem()
	{
		Item weapon = getActiveWeaponInstance();
		if (weapon == null)
		{
			return _fistsWeaponItem;
		}
		return (Weapon) weapon.getTemplate();
	}
	
	public Item getChestArmorInstance()
	{
		return _inventory.getPaperdollItem(Inventory.PAPERDOLL_CHEST);
	}
	
	public Item getLegsArmorInstance()
	{
		return _inventory.getPaperdollItem(Inventory.PAPERDOLL_LEGS);
	}
	
	public Armor getActiveChestArmorItem()
	{
		Item armor = getChestArmorInstance();
		if (armor == null)
		{
			return null;
		}
		return (Armor) armor.getTemplate();
	}
	
	public Armor getActiveLegsArmorItem()
	{
		Item legs = getLegsArmorInstance();
		if (legs == null)
		{
			return null;
		}
		return (Armor) legs.getTemplate();
	}
	
	public bool isWearingHeavyArmor()
	{
		Item legs = getLegsArmorInstance();
		Item armor = getChestArmorInstance();
		if ((armor != null) && (legs != null) && (legs.getItemType() == ArmorType.HEAVY) && (armor.getItemType() == ArmorType.HEAVY))
		{
			return true;
		}
		return (armor != null) && ((_inventory.getPaperdollItem(Inventory.PAPERDOLL_CHEST).getTemplate().getBodyPart() == ItemTemplate.SLOT_FULL_ARMOR) && (armor.getItemType() == ArmorType.HEAVY));
	}
	
	public bool isWearingLightArmor()
	{
		Item legs = getLegsArmorInstance();
		Item armor = getChestArmorInstance();
		if ((armor != null) && (legs != null) && (legs.getItemType() == ArmorType.LIGHT) && (armor.getItemType() == ArmorType.LIGHT))
		{
			return true;
		}
		return (armor != null) && ((_inventory.getPaperdollItem(Inventory.PAPERDOLL_CHEST).getTemplate().getBodyPart() == ItemTemplate.SLOT_FULL_ARMOR) && (armor.getItemType() == ArmorType.LIGHT));
	}
	
	public bool isWearingMagicArmor()
	{
		Item legs = getLegsArmorInstance();
		Item armor = getChestArmorInstance();
		if ((armor != null) && (legs != null) && (legs.getItemType() == ArmorType.MAGIC) && (armor.getItemType() == ArmorType.MAGIC))
		{
			return true;
		}
		return (armor != null) && ((_inventory.getPaperdollItem(Inventory.PAPERDOLL_CHEST).getTemplate().getBodyPart() == ItemTemplate.SLOT_FULL_ARMOR) && (armor.getItemType() == ArmorType.MAGIC));
	}
	
	/**
	 * Return the secondary weapon instance (always equipped in the left hand).
	 */
	public override Item getSecondaryWeaponInstance()
	{
		return _inventory.getPaperdollItem(Inventory.PAPERDOLL_LHAND);
	}
	
	/**
	 * Return the secondary Item item (always equipped in the left hand).<br>
	 * Arrows, Shield..
	 */
	public override ItemTemplate getSecondaryWeaponItem()
	{
		Item item = _inventory.getPaperdollItem(Inventory.PAPERDOLL_LHAND);
		if (item != null)
		{
			return item.getTemplate();
		}
		return null;
	}
	
	/**
	 * Kill the Creature, Apply Death Penalty, Manage gain/loss Karma and Item Drop. <b><u>Actions</u>:</b>
	 * <li>Reduce the Experience of the Player in function of the calculated Death Penalty</li>
	 * <li>If necessary, unsummon the Pet of the killed Player</li>
	 * <li>Manage Karma gain for attacker and Karam loss for the killed Player</li>
	 * <li>If the killed Player has Karma, manage Drop Item</li>
	 * <li>Kill the Player</li><br>
	 * @param killer
	 */
	public override bool doDie(Creature killer)
	{
		// Stop auto peel.
		if (hasRequest<AutoPeelRequest>())
		{
			sendPacket(new ExStopItemAutoPeel(true));
			sendPacket(new ExReadyItemAutoPeel(false, 0));
			removeRequest<AutoPeelRequest>();
		}
		
		ICollection<Item> droppedItems = null;
		
		if (killer != null)
		{
			Player pk = killer.getActingPlayer();
			bool fpcKill = killer.isFakePlayer();
			if ((pk != null) || fpcKill)
			{
				if (pk != null)
				{
					if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_PVP_KILL, this))
					{
						EventDispatcher.getInstance().notifyEventAsync(new OnPlayerPvPKill(pk, this), this);
					}
					
					setTotalDeaths(getTotalDeaths() + 1);
					
					if (pk != this)
					{
						RevengeHistoryManager.getInstance().addNewKill(this, pk);
					}
					
					// pvp/pk item rewards
					if (!(Config.DISABLE_REWARDS_IN_INSTANCES && (getInstanceId() != 0)) && //
						!(Config.DISABLE_REWARDS_IN_PVP_ZONES && isInsideZone(ZoneId.PVP)))
					{
						// pvp
						if (Config.REWARD_PVP_ITEM && (_pvpFlag != 0))
						{
							pk.addItem("PvP Item Reward", Config.REWARD_PVP_ITEM_ID, Config.REWARD_PVP_ITEM_AMOUNT, this, Config.REWARD_PVP_ITEM_MESSAGE);
						}
						// pk
						if (Config.REWARD_PK_ITEM && (_pvpFlag == 0))
						{
							pk.addItem("PK Item Reward", Config.REWARD_PK_ITEM_ID, Config.REWARD_PK_ITEM_AMOUNT, this, Config.REWARD_PK_ITEM_MESSAGE);
						}
					}
				}
				
				// announce pvp/pk
				if (Config.ANNOUNCE_PK_PVP && (((pk != null) && !pk.isGM()) || fpcKill))
				{
					String msg = "";
					if (_pvpFlag == 0)
					{
						msg = Config.ANNOUNCE_PK_MSG.Replace("$killer", killer.getName()).Replace("$target", getName());
						if (Config.ANNOUNCE_PK_PVP_NORMAL_MESSAGE)
						{
							SystemMessage sm = new SystemMessage(SystemMessageId.S1_3);
							sm.addString(msg);
							Broadcast.toAllOnlinePlayers(sm);
						}
						else
						{
							Broadcast.toAllOnlinePlayers(msg, false);
						}
					}
					else if (_pvpFlag != 0)
					{
						msg = Config.ANNOUNCE_PVP_MSG.Replace("$killer", killer.getName()).Replace("$target", getName());
						if (Config.ANNOUNCE_PK_PVP_NORMAL_MESSAGE)
						{
							SystemMessage sm = new SystemMessage(SystemMessageId.S1_3);
							sm.addString(msg);
							Broadcast.toAllOnlinePlayers(sm);
						}
						else
						{
							Broadcast.toAllOnlinePlayers(msg, false);
						}
					}
				}
				
				if (fpcKill && Config.FAKE_PLAYER_KILL_KARMA && (_pvpFlag == 0) && (getReputation() >= 0))
				{
					killer.setReputation(killer.getReputation() - 150);
				}
			}
			
			broadcastStatusUpdate();
			// Clear resurrect xp calculation
			setExpBeforeDeath(0);
			
			// Kill the Player
			if (!base.doDie(killer))
			{
				return false;
			}
			
			// Issues drop of Cursed Weapon.
			if (isCursedWeaponEquipped())
			{
				CursedWeaponsManager.getInstance().drop(_cursedWeaponEquippedId, killer);
			}
			else if (_combatFlagEquippedId)
			{
				Fort fort = FortManager.getInstance().getFort(this);
				if (fort != null)
				{
					FortSiegeManager.getInstance().dropCombatFlag(this, fort.getResidenceId());
				}
				else
				{
					long slot = _inventory.getSlotFromItem(_inventory.getItemByItemId(FortManager.ORC_FORTRESS_FLAG));
					_inventory.unEquipItemInBodySlot(slot);
					destroyItem("OrcFortress CombatFlag", _inventory.getItemByItemId(FortManager.ORC_FORTRESS_FLAG), null, true);
				}
			}
			else
			{
				bool insidePvpZone = isInsideZone(ZoneId.PVP) || isInsideZone(ZoneId.SIEGE);
				if ((pk == null) || !pk.isCursedWeaponEquipped())
				{
					droppedItems = onDieDropItem(killer); // Check if any item should be dropped
					if (!insidePvpZone && (pk != null))
					{
						Clan pkClan = pk.getClan();
						if ((pkClan != null) && (_clan != null) && !isAcademyMember() && !(pk.isAcademyMember()))
						{
							ClanWar clanWar = _clan.getWarWith(pkClan.getId());
							if ((clanWar != null) && AntiFeedManager.getInstance().check(killer, this))
							{
								clanWar.onKill(pk, this);
							}
						}
					}
					
					// Should not penalize player when lucky, in a PvP zone or event.
					if (!isLucky() && !insidePvpZone && !isOnEvent())
					{
						calculateDeathExpPenalty(killer);
					}
				}
			}
		}
		
		sendPacket(new ExDieInfo(droppedItems == null ? new() : droppedItems, _lastDamageTaken));
		
		if (isMounted())
		{
			stopFeed();
		}
		// synchronized (this)
		// {
		if (isFakeDeath())
		{
			stopFakeDeath(true);
		}
		// }
		
		// Unsummon Cubics
		if (!_cubics.isEmpty())
		{
			_cubics.values().forEach(x => x.deactivate());
			_cubics.clear();
		}
		
		if (isChannelized())
		{
			getSkillChannelized().abortChannelization();
		}
		
		if (_agathionId != 0)
		{
			setAgathionId(0);
		}
		
		if (hasServitors())
		{
			getServitors().values().forEach(servitor =>
			{
				if (servitor.isBetrayed())
				{
					sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
					return;
				}
				servitor.cancelAction();
			});
		}
		
		stopRentPet();
		stopWaterTask();
		
		AntiFeedManager.getInstance().setLastDeathTime(getObjectId());
		
		// FIXME: Karma reduction tempfix.
		if (getReputation() < 0)
		{
			int newRep = getReputation() - (getReputation() / 4);
			setReputation(newRep < -20 ? newRep : 0);
		}
		
		if (isInTimedHuntingZone())
		{
			DecayTaskManager.getInstance().add(this);
			sendPacket(new TimeRestrictFieldDieLimitTime());
		}
		else if (Config.DISCONNECT_AFTER_DEATH)
		{
			DecayTaskManager.getInstance().add(this);
		}
		
		return true;
	}
	
	public void addDamageTaken(Creature attacker, int skillId, double damage)
	{
		if (attacker == this)
		{
			return;
		}
		
		lock (_lastDamageTaken)
		{
			_lastDamageTaken.add(new DamageTakenHolder(attacker, skillId, damage));
			if (_lastDamageTaken.size() > 20)
			{
				_lastDamageTaken.RemoveAt(0);
			}
		}
	}
	
	public void clearDamageTaken()
	{
		lock (_lastDamageTaken)
		{
			_lastDamageTaken.Clear();
		}
	}
	
	private ICollection<Item> onDieDropItem(Creature killer)
	{
		List<Item> droppedItems = new();
		if (isOnEvent() || (killer == null))
		{
			return droppedItems;
		}
		
		Player pk = killer.getActingPlayer();
		if ((getReputation() >= 0) && (pk != null) && (pk.getClan() != null) && (getClan() != null) && (pk.getClan().isAtWarWith(_clanId)
		// || _clan.isAtWarWith(((Player)killer).getClanId())
		))
		{
			return droppedItems;
		}
		
		if ((!isInsideZone(ZoneId.PVP) || (pk == null)) && (!isGM() || Config.KARMA_DROP_GM))
		{
			bool isKarmaDrop = false;
			int dropEquip = 0;
			int dropEquipWeapon = 0;
			int dropItem = 0;
			int dropLimit = 0;
			int dropPercent = 0;
			
			// Classic calculation.
			if (killer.isPlayable() && (getReputation() < 0) && (_pkKills >= Config.KARMA_PK_LIMIT))
			{
				isKarmaDrop = true;
				dropPercent = Config.KARMA_RATE_DROP;
				dropEquip = Config.KARMA_RATE_DROP_EQUIP;
				dropEquipWeapon = Config.KARMA_RATE_DROP_EQUIP_WEAPON;
				dropItem = Config.KARMA_RATE_DROP_ITEM;
				dropLimit = Config.KARMA_DROP_LIMIT;
			}
			else if (killer.isNpc())
			{
				dropPercent = Config.PLAYER_RATE_DROP;
				dropEquip = Config.PLAYER_RATE_DROP_EQUIP;
				dropEquipWeapon = Config.PLAYER_RATE_DROP_EQUIP_WEAPON;
				dropItem = Config.PLAYER_RATE_DROP_ITEM;
				dropLimit = Config.PLAYER_DROP_LIMIT;
			}
			
			if ((dropPercent > 0) && (Rnd.get(100) < dropPercent))
			{
				int dropCount = 0;
				int itemDropPercent = 0;
				foreach (Item itemDrop in _inventory.getItems())
				{
					// Don't drop
					if (itemDrop.isShadowItem() || // Dont drop Shadow Items
						itemDrop.isTimeLimitedItem() || // Dont drop Time Limited Items
						!itemDrop.isDropable() || (itemDrop.getId() == Inventory.ADENA_ID) || // Adena
						(itemDrop.getTemplate().getType2() == ItemTemplate.TYPE2_QUEST) || // Quest Items
						((_pet != null) && (_pet.getControlObjectId() == itemDrop.getId())) || // Control Item of active pet
						(Arrays.binarySearch(Config.KARMA_LIST_NONDROPPABLE_ITEMS, itemDrop.getId()) >= 0) || // Item listed in the non droppable item list
						(Arrays.binarySearch(Config.KARMA_LIST_NONDROPPABLE_PET_ITEMS, itemDrop.getId()) >= 0 // Item listed in the non droppable pet item list
						))
					{
						continue;
					}
					
					if (itemDrop.isEquipped())
					{
						// Set proper chance according to Item type of equipped Item
						itemDropPercent = itemDrop.getTemplate().getType2() == ItemTemplate.TYPE2_WEAPON ? dropEquipWeapon : dropEquip;
						_inventory.unEquipItemInSlot(itemDrop.getLocationSlot());
					}
					else
					{
						itemDropPercent = dropItem; // Item in inventory
					}
					
					// NOTE: Each time an item is dropped, the chance of another item being dropped gets lesser (dropCount * 2)
					if (Rnd.get(100) < itemDropPercent)
					{
						dropItem("DieDrop", itemDrop, killer, true);
						droppedItems.add(itemDrop);
						
						if (isKarmaDrop)
						{
							LOGGER.Warn(getName() + " has karma and dropped id = " + itemDrop.getId() + ", count = " + itemDrop.getCount());
						}
						else
						{
							LOGGER.Warn(getName() + " dropped id = " + itemDrop.getId() + ", count = " + itemDrop.getCount());
						}
						
						if (++dropCount >= dropLimit)
						{
							break;
						}
					}
				}
			}
		}
		
		return droppedItems;
	}
	
	public void onPlayerKill(Playable target)
	{
		if ((target == null) || !target.isPlayable())
		{
			return;
		}
		
		// Avoid nulls && check if player != killedPlayer
		Player killedPlayer = target.getActingPlayer();
		if ((killedPlayer == null) || (this == killedPlayer))
		{
			return;
		}
		
		// Cursed weapons progress
		if (isCursedWeaponEquipped() && target.isPlayer())
		{
			CursedWeaponsManager.getInstance().increaseKills(_cursedWeaponEquippedId);
			return;
		}
		
		// Olympiad support
		if (isInOlympiadMode() || killedPlayer.isInOlympiadMode())
		{
			return;
		}
		
		// Duel support
		if (isInDuel() && killedPlayer.isInDuel())
		{
			return;
		}
		
		// If both players are in SIEGE zone just increase siege kills/deaths.
		if (target.isPlayer() && isInsideZone(ZoneId.SIEGE) && killedPlayer.isInsideZone(ZoneId.SIEGE))
		{
			if (!isSiegeFriend(killedPlayer))
			{
				Clan targetClan = killedPlayer.getClan();
				if ((_clan != null) && (targetClan != null))
				{
					_clan.addSiegeKill();
					targetClan.addSiegeDeath();
				}
			}
			return;
		}
		
		// Do nothing when in PVP zone.
		if (isInsideZone(ZoneId.PVP) || target.isInsideZone(ZoneId.PVP))
		{
			return;
		}
		
		if (checkIfPvP(killedPlayer))
		{
			// Check if player should get + rep.
			if (killedPlayer.getReputation() < 0)
			{
				int levelDiff = killedPlayer.getLevel() - getLevel();
				if ((getReputation() >= 0) && (levelDiff < 11) && (levelDiff > -11)) // TODO: Time check, same player can't be killed again in 8 hours
				{
					setReputation(getReputation() + Config.REPUTATION_INCREASE);
				}
			}
			
			if (target.isPlayer())
			{
				setPvpKills(_pvpKills + 1);
				setTotalKills(getTotalKills() + 1);
				updatePvpTitleAndColor(true);
				if (Config.ENABLE_ACHIEVEMENT_PVP)
				{
					getAchievementBox().addPvpPoints(1);
				}
			}
		}
		else if ((getReputation() > 0) && (_pkKills == 0))
		{
			setReputation(0);
			if (target.isPlayer())
			{
				setTotalKills(getTotalKills() + 1);
				setPkKills(getPkKills() + 1);
			}
		}
		else // Calculate new karma and increase pk count.
		{
			if (Config.FACTION_SYSTEM_ENABLED)
			{
				if ((_isGood && killedPlayer.isGood()) || (_isEvil && killedPlayer.isEvil()))
				{
					setReputation(getReputation() - Formulas.calculateKarmaGain(getPkKills(), target.isSummon()));
					if (target.isPlayer())
					{
						setPkKills(getPkKills() + 1);
						setTotalKills(getTotalKills() + 1);
					}
				}
			}
			else
			{
				setReputation(getReputation() - Formulas.calculateKarmaGain(getPkKills(), target.isSummon()));
				if (target.isPlayer())
				{
					setPkKills(getPkKills() + 1);
					setTotalKills(getTotalKills() + 1);
				}
			}
		}
		
		broadcastUserInfo(UserInfoType.SOCIAL);
		checkItemRestriction();
	}
	
	public void updatePvpTitleAndColor(bool broadcastInfo)
	{
		if (Config.PVP_COLOR_SYSTEM_ENABLED && !Config.FACTION_SYSTEM_ENABLED) // Faction system uses title colors.
		{
			if ((_pvpKills >= (Config.PVP_AMOUNT1)) && (_pvpKills < (Config.PVP_AMOUNT2)))
			{
				setTitle("\u00AE " + Config.TITLE_FOR_PVP_AMOUNT1 + " \u00AE");
				_appearance.setTitleColor(Config.NAME_COLOR_FOR_PVP_AMOUNT1);
			}
			else if ((_pvpKills >= (Config.PVP_AMOUNT2)) && (_pvpKills < (Config.PVP_AMOUNT3)))
			{
				setTitle("\u00AE " + Config.TITLE_FOR_PVP_AMOUNT2 + " \u00AE");
				_appearance.setTitleColor(Config.NAME_COLOR_FOR_PVP_AMOUNT2);
			}
			else if ((_pvpKills >= (Config.PVP_AMOUNT3)) && (_pvpKills < (Config.PVP_AMOUNT4)))
			{
				setTitle("\u00AE " + Config.TITLE_FOR_PVP_AMOUNT3 + " \u00AE");
				_appearance.setTitleColor(Config.NAME_COLOR_FOR_PVP_AMOUNT3);
			}
			else if ((_pvpKills >= (Config.PVP_AMOUNT4)) && (_pvpKills < (Config.PVP_AMOUNT5)))
			{
				setTitle("\u00AE " + Config.TITLE_FOR_PVP_AMOUNT4 + " \u00AE");
				_appearance.setTitleColor(Config.NAME_COLOR_FOR_PVP_AMOUNT4);
			}
			else if (_pvpKills >= (Config.PVP_AMOUNT5))
			{
				setTitle("\u00AE " + Config.TITLE_FOR_PVP_AMOUNT5 + " \u00AE");
				_appearance.setTitleColor(Config.NAME_COLOR_FOR_PVP_AMOUNT5);
			}
			
			if (broadcastInfo)
			{
				broadcastTitleInfo();
			}
		}
	}
	
	public void updatePvPStatus()
	{
		if (isInsideZone(ZoneId.PVP))
		{
			return;
		}
		
		setPvpFlagLasts(DateTime.Now.AddMilliseconds(Config.PVP_NORMAL_TIME));
		if (_pvpFlag == 0)
		{
			startPvPFlag();
		}
	}
	
	public void updatePvPStatus(Creature target)
	{
		Player targetPlayer = target.getActingPlayer();
		if (targetPlayer == null)
		{
			return;
		}
		
		if (this == targetPlayer)
		{
			return;
		}
		
		if (Config.FACTION_SYSTEM_ENABLED && target.isPlayer() && ((isGood() && targetPlayer.isEvil()) || (isEvil() && targetPlayer.isGood())))
		{
			return;
		}
		
		if (_isInDuel && (targetPlayer.getDuelId() == getDuelId()))
		{
			return;
		}
		
		if ((!isInsideZone(ZoneId.PVP) || !target.isInsideZone(ZoneId.PVP)) && (targetPlayer.getReputation() >= 0))
		{
			if (checkIfPvP(targetPlayer))
			{
				setPvpFlagLasts(DateTime.Now.AddMilliseconds(Config.PVP_PVP_TIME));
			}
			else
			{
				setPvpFlagLasts(DateTime.Now.AddMilliseconds(Config.PVP_NORMAL_TIME));
			}
			if (_pvpFlag == 0)
			{
				startPvPFlag();
			}
		}
	}
	
	/**
	 * @return {@code true} if player has Lucky effect and is level 9 or less
	 */
	public bool isLucky()
	{
		return (getLevel() <= 9) && isAffectedBySkill((int)CommonSkill.LUCKY);
	}
	
	/**
	 * Restore the specified % of experience this Player has lost and sends a Server=>Client StatusUpdate packet.
	 * @param restorePercent
	 */
	public void restoreExp(double restorePercent)
	{
		if (_expBeforeDeath > 0)
		{
			// Restore the specified % of lost experience.
			getStat().addExp((long)Math.Round(((_expBeforeDeath - getExp()) * restorePercent) / 100));
			setExpBeforeDeath(0);
		}
	}
	
	/**
	 * Reduce the Experience (and level if necessary) of the Player in function of the calculated Death Penalty.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <li>Calculate the Experience loss</li>
	 * <li>Set the value of _expBeforeDeath</li>
	 * <li>Set the new Experience value of the Player and Decrease its level if necessary</li>
	 * <li>Send a Server=>Client StatusUpdate packet with its new Experience</li><br>
	 * @param killer
	 */
	public void calculateDeathExpPenalty(Creature killer)
	{
		int lvl = getLevel();
		double percentLost = PlayerXpPercentLostData.getInstance().getXpPercent(getLevel());
		if (killer != null)
		{
			if (killer.isRaid())
			{
				percentLost *= getStat().getValue(Stat.REDUCE_EXP_LOST_BY_RAID, 1);
			}
			else if (killer.isMonster())
			{
				percentLost *= getStat().getValue(Stat.REDUCE_EXP_LOST_BY_MOB, 1);
			}
			else if (killer.isPlayable())
			{
				percentLost *= getStat().getValue(Stat.REDUCE_EXP_LOST_BY_PVP, 1);
			}
		}
		
		if (getReputation() < 0)
		{
			percentLost *= Config.RATE_KARMA_EXP_LOST;
		}
		
		// Calculate the Experience loss
		long lostExp = 0;
		if (!isOnEvent())
		{
			if (lvl < ExperienceData.getInstance().getMaxLevel())
			{
				lostExp = (long)Math.Round(((getStat().getExpForLevel(lvl + 1) - getStat().getExpForLevel(lvl)) * percentLost) / 100);
			}
			else
			{
				lostExp = (long)Math.Round(((getStat().getExpForLevel(ExperienceData.getInstance().getMaxLevel()) - getStat().getExpForLevel(ExperienceData.getInstance().getMaxLevel() - 1)) * percentLost) / 100);
			}
		}
		
		if ((killer != null) && killer.isPlayable() && atWarWith(killer.getActingPlayer()))
		{
			lostExp /= 4;
		}
		
		setExpBeforeDeath(getExp());
		getStat().removeExp(lostExp);
	}
	
	/**
	 * Stop the HP/MP/CP Regeneration task. <b><u>Actions</u>:</b>
	 * <li>Set the RegenActive flag to False</li>
	 * <li>Stop the HP/MP/CP Regeneration task</li>
	 */
	public void stopAllTimers()
	{
		stopHpMpRegeneration();
		stopWarnUserTakeBreak();
		stopWaterTask();
		stopFeed();
		clearPetData();
		storePetFood(_mountNpcId);
		stopRentPet();
		stopPvpRegTask();
		stopSoulTask();
		stopChargeTask();
		stopFameTask();
		stopRecoGiveTask();
		stopOnlineTimeUpdateTask();
	}
	
	public override Pet getPet()
	{
		return _pet;
	}
	
	public override Map<int, Summon> getServitors()
	{
		return _servitors;
	}
	
	public Summon getAnyServitor()
	{
		return getServitors().values().FirstOrDefault();
	}
	
	public Summon getFirstServitor()
	{
		if (getServitors().isEmpty())
		{
			return null;
		}
		
		return getServitors().values().FirstOrDefault();
	}
	
	public override Summon getServitor(int objectId)
	{
		return getServitors().get(objectId);
	}
	
	public List<Summon> getServitorsAndPets()
	{
		List<Summon> summons = new();
		summons.addAll(getServitors().values());
		if (_pet != null)
		{
			summons.add(_pet);
		}
		return summons;
	}
	
	/**
	 * @return any summoned trap by this player or null.
	 */
	public Trap getTrap()
	{
		foreach (Npc npc in getSummonedNpcs())
		{
			if (npc.isTrap())
			{
				return (Trap) npc;
			}
		}
		return null;
	}
	
	/**
	 * Set the summoned Pet of the Player.
	 * @param pet
	 */
	public void setPet(Pet pet)
	{
		_pet = pet;
	}
	
	public void addServitor(Summon servitor)
	{
		_servitors.put(servitor.getObjectId(), servitor);
	}
	
	/**
	 * @return the Summon of the Player or null.
	 */
	public Set<TamedBeast> getTrainedBeasts()
	{
		return _tamedBeast;
	}
	
	/**
	 * Set the Summon of the Player.
	 * @param tamedBeast
	 */
	public void addTrainedBeast(TamedBeast tamedBeast)
	{
		_tamedBeast.add(tamedBeast);
	}
	
	/**
	 * @return the Player requester of a transaction (ex : FriendInvite, JoinAlly, JoinParty...).
	 */
	public Request getRequest()
	{
		return _request;
	}
	
	/**
	 * Set the Player requester of a transaction (ex : FriendInvite, JoinAlly, JoinParty...).
	 * @param requester
	 */
	public void setActiveRequester(Player requester)
	{
		_activeRequester = requester;
	}
	
	/**
	 * @return the Player requester of a transaction (ex : FriendInvite, JoinAlly, JoinParty...).
	 */
	public Player getActiveRequester()
	{
		Player requester = _activeRequester;
		if ((requester != null) && requester.isRequestExpired() && (_activeTradeList == null))
		{
			_activeRequester = null;
		}
		return _activeRequester;
	}
	
	/**
	 * @return True if a transaction is in progress.
	 */
	public bool isProcessingRequest()
	{
		return (getActiveRequester() != null) || (_requestExpireTime > GameTimeTaskManager.getInstance().getGameTicks());
	}
	
	/**
	 * @return True if a transaction is in progress.
	 */
	public bool isProcessingTransaction()
	{
		return (getActiveRequester() != null) || (_activeTradeList != null) || (_requestExpireTime > GameTimeTaskManager.getInstance().getGameTicks());
	}
	
	/**
	 * Used by fake players to emulate proper behavior.
	 */
	public void blockRequest()
	{
		_requestExpireTime = GameTimeTaskManager.getInstance().getGameTicks() + (REQUEST_TIMEOUT * GameTimeTaskManager.TICKS_PER_SECOND);
	}
	
	/**
	 * Select the Warehouse to be used in next activity.
	 * @param partner
	 */
	public void onTransactionRequest(Player partner)
	{
		_requestExpireTime = GameTimeTaskManager.getInstance().getGameTicks() + (REQUEST_TIMEOUT * GameTimeTaskManager.TICKS_PER_SECOND);
		partner.setActiveRequester(this);
	}
	
	/**
	 * Return true if last request is expired.
	 * @return
	 */
	public bool isRequestExpired()
	{
		return _requestExpireTime <= GameTimeTaskManager.getInstance().getGameTicks();
	}
	
	/**
	 * Select the Warehouse to be used in next activity.
	 */
	public void onTransactionResponse()
	{
		_requestExpireTime = 0;
	}
	
	/**
	 * Select the Warehouse to be used in next activity.
	 * @param warehouse
	 */
	public void setActiveWarehouse(ItemContainer warehouse)
	{
		_activeWarehouse = warehouse;
	}
	
	/**
	 * @return active Warehouse.
	 */
	public ItemContainer getActiveWarehouse()
	{
		return _activeWarehouse;
	}
	
	/**
	 * Select the TradeList to be used in next activity.
	 * @param tradeList
	 */
	public void setActiveTradeList(TradeList tradeList)
	{
		_activeTradeList = tradeList;
	}
	
	/**
	 * @return active TradeList.
	 */
	public TradeList getActiveTradeList()
	{
		return _activeTradeList;
	}
	
	public void onTradeStart(Player partner)
	{
		_activeTradeList = new TradeList(this);
		_activeTradeList.setPartner(partner);
		
		SystemMessage msg = new SystemMessage(SystemMessageId.YOU_BEGIN_TRADING_WITH_C1);
		msg.addPcName(partner);
		sendPacket(msg);
		sendPacket(new TradeStart(1, this));
		sendPacket(new TradeStart(2, this));
	}
	
	public void onTradeConfirm(Player partner)
	{
		SystemMessage msg = new SystemMessage(SystemMessageId.C1_HAS_CONFIRMED_THE_TRADE);
		msg.addPcName(partner);
		sendPacket(msg);
		sendPacket(TradeOtherDone.STATIC_PACKET);
	}
	
	public void onTradeCancel(Player partner)
	{
		if (_activeTradeList == null)
		{
			return;
		}
		
		_activeTradeList.@lock();
		_activeTradeList = null;
		sendPacket(new TradeDone(0));
		SystemMessage msg = new SystemMessage(SystemMessageId.C1_HAS_CANCELLED_THE_TRADE);
		msg.addPcName(partner);
		sendPacket(msg);
	}
	
	public void onTradeFinish(bool successfull)
	{
		_activeTradeList = null;
		sendPacket(new TradeDone(1));
		if (successfull)
		{
			sendPacket(SystemMessageId.YOUR_TRADE_WAS_SUCCESSFUL);
		}
	}
	
	public void startTrade(Player partner)
	{
		onTradeStart(partner);
		partner.onTradeStart(this);
	}
	
	public void cancelActiveTrade()
	{
		if (_activeTradeList == null)
		{
			return;
		}
		
		Player partner = _activeTradeList.getPartner();
		if (partner != null)
		{
			partner.onTradeCancel(this);
		}
		onTradeCancel(this);
	}
	
	public bool hasManufactureShop()
	{
		return (_manufactureItems != null) && !_manufactureItems.isEmpty();
	}
	
	/**
	 * Get the manufacture items map of this player.
	 * @return the the manufacture items map
	 */
	public Map<int, ManufactureItem> getManufactureItems()
	{
		if (_manufactureItems == null)
		{
			lock (this)
			{
				if (_manufactureItems == null)
				{
					_manufactureItems = new();
				}
			}
		}
		return _manufactureItems;
	}
	
	/**
	 * Get the store name, if any.
	 * @return the store name
	 */
	public String getStoreName()
	{
		return _storeName;
	}
	
	/**
	 * Set the store name.
	 * @param name the store name to set
	 */
	public void setStoreName(String name)
	{
		_storeName = name == null ? "" : name;
	}
	
	/**
	 * @return the _buyList object of the Player.
	 */
	public TradeList getSellList()
	{
		if (_sellList == null)
		{
			_sellList = new TradeList(this);
		}
		return _sellList;
	}
	
	/**
	 * @return the _buyList object of the Player.
	 */
	public TradeList getBuyList()
	{
		if (_buyList == null)
		{
			_buyList = new TradeList(this);
		}
		return _buyList;
	}
	
	/**
	 * Set the Private Store type of the Player. <b><u>Values</u>:</b>
	 * <li>0 : STORE_PRIVATE_NONE</li>
	 * <li>1 : STORE_PRIVATE_SELL</li>
	 * <li>2 : sellmanage</li>
	 * <li>3 : STORE_PRIVATE_BUY</li>
	 * <li>4 : buymanage</li>
	 * <li>5 : STORE_PRIVATE_MANUFACTURE</li><br>
	 * @param privateStoreType
	 */
	public void setPrivateStoreType(PrivateStoreType privateStoreType)
	{
		_privateStoreType = privateStoreType;
		if (Config.OFFLINE_DISCONNECT_FINISHED && (privateStoreType == PrivateStoreType.NONE) && ((_client == null) || _client.isDetached()))
		{
			OfflineTraderTable.getInstance().removeTrader(getObjectId());
			Disconnection.of(this).storeMe().deleteMe();
		}
	}
	
	/**
	 * <b><u>Values</u>:</b>
	 * <li>0 : STORE_PRIVATE_NONE</li>
	 * <li>1 : STORE_PRIVATE_SELL</li>
	 * <li>2 : sellmanage</li>
	 * <li>3 : STORE_PRIVATE_BUY</li>
	 * <li>4 : buymanage</li>
	 * <li>5 : STORE_PRIVATE_MANUFACTURE</li>
	 * @return the Private Store type of the Player.
	 */
	public PrivateStoreType getPrivateStoreType()
	{
		return _privateStoreType;
	}
	
	/**
	 * Set the _clan object, _clanId, _clanLeader Flag and title of the Player.
	 * @param clan
	 */
	public void setClan(Clan clan)
	{
		_clan = clan;
		if (clan == null)
		{
			setTitle("");
			_clanId = 0;
			_clanPrivileges = new EnumIntBitmask<ClanPrivilege>();
			_pledgeType = 0;
			_powerGrade = 0;
			_lvlJoinedAcademy = 0;
			_apprentice = 0;
			_sponsor = 0;
			_activeWarehouse = null;
			CharInfoTable.getInstance().removeClanId(getObjectId());
			return;
		}
		
		if (!clan.isMember(getObjectId()))
		{
			// char has been kicked from clan
			setClan(null);
			return;
		}
		
		_clanId = clan.getId();
		CharInfoTable.getInstance().setClanId(getObjectId(), _clanId);
	}
	
	/**
	 * @return the _clan object of the Player.
	 */
	public override Clan getClan()
	{
		return _clan;
	}
	
	/**
	 * @return True if the Player is the leader of its clan.
	 */
	public bool isClanLeader()
	{
		if (_clan == null)
		{
			return false;
		}
		return getObjectId() == _clan.getLeaderId();
	}
	
	/**
	 * Equip arrows needed in left hand and send a Server=>Client packet ItemList to the Player then return True.
	 * @param type
	 */
	protected override bool checkAndEquipAmmunition(EtcItemType type)
	{
		Item ammunition = null;
		Weapon weapon = getActiveWeaponItem();
		if (type == EtcItemType.ARROW)
		{
			ammunition = _inventory.findArrowForBow(weapon);
		}
		else if (type == EtcItemType.BOLT)
		{
			ammunition = _inventory.findBoltForCrossBow(weapon);
		}
		else if (type == EtcItemType.ELEMENTAL_ORB)
		{
			ammunition = _inventory.findElementalOrbForPistols(weapon);
		}
		
		if (ammunition != null)
		{
			addAmmunitionSkills(ammunition);
			InventoryUpdate iu = new InventoryUpdate();
			iu.addModifiedItem(ammunition);
			sendInventoryUpdate(iu);
			return true;
		}
		
		removeAmmunitionSkills();
		return false;
	}
	
	private void addAmmunitionSkills(Item ammunition)
	{
		int currentAmmunitionId = ammunition.getId();
		if (_lastAmmunitionId == currentAmmunitionId)
		{
			return;
		}
		removeAmmunitionSkills();
		_lastAmmunitionId = currentAmmunitionId;
		
		List<ItemSkillHolder> skills = ammunition.getTemplate().getAllSkills();
		if (skills == null)
		{
			return;
		}
		
		bool sendSkillList = false;
		foreach (ItemSkillHolder holder in skills)
		{
			if (!isAffectedBySkill(holder))
			{
				Skill skill = holder.getSkill();
				if (skill.isPassive())
				{
					addSkill(skill);
					sendSkillList = true;
				}
			}
		}
		if (sendSkillList)
		{
			this.sendSkillList();
		}
	}
	
	public void removeAmmunitionSkills()
	{
		if (_lastAmmunitionId == 0)
		{
			return;
		}
		_lastAmmunitionId = 0;
		
		bool sendSkillList = false;
		foreach (int skillId in AmmunitionSkillList.values())
		{
			if (removeSkill(skillId, true) != null)
			{
				sendSkillList = true;
			}
		}
		if (sendSkillList)
		{
			this.sendSkillList();
		}
	}
	
	/**
	 * Disarm the player's weapon.
	 * @return {@code true} if the player was disarmed or doesn't have a weapon to disarm, {@code false} otherwise.
	 */
	public bool disarmWeapons()
	{
		// If there is no weapon to disarm then return true.
		Item wpn = _inventory.getPaperdollItem(Inventory.PAPERDOLL_RHAND);
		if (wpn == null)
		{
			return true;
		}
		
		// Don't allow disarming a cursed weapon
		if (isCursedWeaponEquipped())
		{
			return false;
		}
		
		// Don't allow disarming a Combat Flag or Territory Ward.
		if (_combatFlagEquippedId)
		{
			return false;
		}
		
		// Don't allow disarming if the weapon is force equip.
		if (wpn.getWeaponItem().isForceEquip())
		{
			return false;
		}
		
		List<Item> unequipped = _inventory.unEquipItemInBodySlotAndRecord(wpn.getTemplate().getBodyPart());
		InventoryUpdate iu = new InventoryUpdate();
		foreach (Item itm in unequipped)
		{
			iu.addModifiedItem(itm);
		}
		
		sendInventoryUpdate(iu);
		abortAttack();
		broadcastUserInfo();
		
		// This can be 0 if the user pressed the right mousebutton twice very fast.
		if (!unequipped.isEmpty())
		{
			SystemMessage sm;
			Item unequippedItem = unequipped.get(0);
			if (unequippedItem.getEnchantLevel() > 0)
			{
				sm = new SystemMessage(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.addInt(unequippedItem.getEnchantLevel());
				sm.addItemName(unequippedItem);
			}
			else
			{
				sm = new SystemMessage(SystemMessageId.S1_UNEQUIPPED);
				sm.addItemName(unequippedItem);
			}
			sendPacket(sm);
		}
		return true;
	}
	
	/**
	 * Disarm the player's shield.
	 * @return {@code true}.
	 */
	public bool disarmShield()
	{
		Item sld = _inventory.getPaperdollItem(Inventory.PAPERDOLL_LHAND);
		if (sld != null)
		{
			List<Item> unequipped = _inventory.unEquipItemInBodySlotAndRecord(sld.getTemplate().getBodyPart());
			InventoryUpdate iu = new InventoryUpdate();
			foreach (Item itm in unequipped)
			{
				iu.addModifiedItem(itm);
			}
			sendInventoryUpdate(iu);
			
			abortAttack();
			broadcastUserInfo();
			
			// this can be 0 if the user pressed the right mousebutton twice very fast
			if (!unequipped.isEmpty())
			{
				SystemMessage sm = null;
				Item unequippedItem = unequipped.get(0);
				if (unequippedItem.getEnchantLevel() > 0)
				{
					sm = new SystemMessage(SystemMessageId.S1_S2_UNEQUIPPED);
					sm.addInt(unequippedItem.getEnchantLevel());
					sm.addItemName(unequippedItem);
				}
				else
				{
					sm = new SystemMessage(SystemMessageId.S1_UNEQUIPPED);
					sm.addItemName(unequippedItem);
				}
				sendPacket(sm);
			}
		}
		return true;
	}
	
	public bool mount(Summon pet)
	{
		if (!Config.ALLOW_MOUNTS_DURING_SIEGE && isInsideZone(ZoneId.SIEGE))
		{
			return false;
		}
		
		if (!disarmWeapons() || !disarmShield() || isTransformed())
		{
			return false;
		}
		
		getEffectList().stopAllToggles();
		setMount(pet.getId(), pet.getLevel());
		setMountObjectID(pet.getControlObjectId());
		clearPetData();
		startFeed(pet.getId());
		broadcastPacket(new Ride(this));
		
		// Notify self and others about speed change
		broadcastUserInfo();
		
		pet.unSummon(this);
		return true;
	}
	
	public bool mount(int npcId, int controlItemObjId, bool useFood)
	{
		if (!disarmWeapons() || !disarmShield() || isTransformed())
		{
			return false;
		}
		
		getEffectList().stopAllToggles();
		setMount(npcId, getLevel());
		clearPetData();
		setMountObjectID(controlItemObjId);
		broadcastPacket(new Ride(this));
		
		// Notify self and others about speed change
		broadcastUserInfo();
		if (useFood)
		{
			startFeed(npcId);
		}
		return true;
	}
	
	public bool mountPlayer(Summon pet)
	{
		if ((pet != null) && pet.isMountable() && !isMounted() && !isBetrayed())
		{
			if (isDead())
			{
				// A strider cannot be ridden when dead
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.A_MOUNT_CANNOT_BE_RIDDEN_WHEN_DEAD);
				return false;
			}
			else if (pet.isDead())
			{
				// A dead strider cannot be ridden.
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.A_DEAD_MOUNT_CANNOT_BE_RIDDEN);
				return false;
			}
			else if (pet.isInCombat() || pet.isRooted())
			{
				// A strider in battle cannot be ridden
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.A_MOUNT_IN_BATTLE_CANNOT_BE_RIDDEN);
				return false;
			}
			else if (isInCombat())
			{
				// A strider cannot be ridden while in battle
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.A_MOUNT_CANNOT_BE_RIDDEN_WHILE_IN_BATTLE);
				return false;
			}
			else if (_waitTypeSitting)
			{
				// A strider can be ridden only when standing
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.A_MOUNT_CAN_BE_RIDDEN_ONLY_WHEN_STANDING);
				return false;
			}
			else if (isFishing())
			{
				// You can't mount, dismount, break and drop items while fishing
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_FISHING_2);
				return false;
			}
			else if (isTransformed() || isCursedWeaponEquipped())
			{
				// no message needed, player while transformed doesn't have mount action
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
			else if (_inventory.getItemByItemId(FortManager.ORC_FORTRESS_FLAG) != null)
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				// FIXME: Wrong Message
				sendMessage("You cannot mount a steed while holding a flag.");
				return false;
			}
			else if (pet.isHungry())
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.A_HUNGRY_MOUNT_CANNOT_BE_MOUNTED_OR_DISMOUNTED);
				return false;
			}
			else if (!Util.checkIfInRange(200, this, pet, true))
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.YOU_ARE_TOO_FAR_AWAY_FROM_YOUR_MOUNT_TO_RIDE);
				return false;
			}
			else if (!pet.isDead() && !isMounted())
			{
				mount(pet);
			}
		}
		else if (isRentedPet())
		{
			stopRentPet();
		}
		else if (isMounted())
		{
			if ((_mountType == MountType.WYVERN) && isInsideZone(ZoneId.NO_LANDING))
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.YOU_ARE_NOT_ALLOWED_TO_DISMOUNT_IN_THIS_LOCATION);
				return false;
			}
			else if (isHungry())
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.A_HUNGRY_MOUNT_CANNOT_BE_MOUNTED_OR_DISMOUNTED);
				return false;
			}
			else
			{
				dismount();
			}
		}
		return true;
	}
	
	public bool dismount()
	{
		if (ZoneManager.getInstance().getZone<WaterZone>(getX(), getY(), getZ() - 300) == null)
		{
			if (!isInWater() && (getZ() > 10000))
			{
				sendPacket(SystemMessageId.YOU_ARE_NOT_ALLOWED_TO_DISMOUNT_IN_THIS_LOCATION);
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
			if ((GeoEngine.getInstance().getHeight(getX(), getY(), getZ()) + 300) < getZ())
			{
				sendPacket(SystemMessageId.YOU_CANNOT_DISMOUNT_FROM_THIS_ELEVATION);
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
		}
		else
		{
			ThreadPool.schedule(() =>
			{
				if (isInWater())
				{
					broadcastUserInfo();
				}
			}, 1500);
		}
		
		bool wasFlying = isFlying();
		sendPacket(new SetupGauge(3, 0, 0));
		int petId = _mountNpcId;
		setMount(0, 0);
		stopFeed();
		clearPetData();
		if (wasFlying)
		{
			removeSkill((int)CommonSkill.WYVERN_BREATH);
		}
		broadcastPacket(new Ride(this));
		setMountObjectID(0);
		storePetFood(petId);
		
		// Notify self and others about speed change
		broadcastUserInfo();
		
		return true;
	}
	
	public void setUptime(long time)
	{
		_uptime = time;
	}
	
	public long getUptime()
	{
		return DateTime.Now - _uptime;
	}
	
	/**
	 * Return True if the Player is invulnerable.
	 */
	public override bool isInvul()
	{
		return base.isInvul() || isTeleportProtected();
	}
	
	/**
	 * Return True if the Player has a Party in progress.
	 */
	public override bool isInParty()
	{
		return _party != null;
	}
	
	/**
	 * Set the _party object of the Player (without joining it).
	 * @param party
	 */
	public void setParty(Party party)
	{
		_party = party;
	}
	
	/**
	 * Set the _party object of the Player AND join it.
	 * @param party
	 */
	public void joinParty(Party party)
	{
		if (party != null)
		{
			// First set the party otherwise this wouldn't be considered
			// as in a party into the Creature.updateEffectIcons() call.
			_party = party;
			party.addPartyMember(this);
		}
	}
	
	/**
	 * Manage the Leave Party task of the Player.
	 */
	public void leaveParty()
	{
		if (isInParty())
		{
			_party.removePartyMember(this, PartyMessageType.DISCONNECTED);
			_party = null;
		}
	}
	
	/**
	 * Return the _party object of the Player.
	 */
	public override Party getParty()
	{
		return _party;
	}
	
	public bool isInCommandChannel()
	{
		return isInParty() && _party.isInCommandChannel();
	}
	
	public CommandChannel getCommandChannel()
	{
		return (isInCommandChannel()) ? _party.getCommandChannel() : null;
	}
	
	/**
	 * Return True if the Player is a GM.
	 */
	public override bool isGM()
	{
		return _accessLevel.isGm();
	}
	
	/**
	 * Set the _accessLevel of the Player.
	 * @param level
	 * @param broadcast
	 * @param updateInDb
	 */
	public void setAccessLevel(int level, bool broadcast, bool updateInDb)
	{
		AccessLevel accessLevel = AdminData.getInstance().getAccessLevel(level);
		if (accessLevel == null)
		{
			LOGGER.Warn("Can't find access level " + level + " for " + this);
			accessLevel = AdminData.getInstance().getAccessLevel(0);
		}
		
		if ((accessLevel.getLevel() == 0) && (Config.DEFAULT_ACCESS_LEVEL > 0))
		{
			accessLevel = AdminData.getInstance().getAccessLevel(Config.DEFAULT_ACCESS_LEVEL);
			if (accessLevel == null)
			{
				LOGGER.Warn("Config's default access level (" + Config.DEFAULT_ACCESS_LEVEL + ") is not defined, defaulting to 0!");
				accessLevel = AdminData.getInstance().getAccessLevel(0);
				Config.DEFAULT_ACCESS_LEVEL = 0;
			}
		}
		
		_accessLevel = accessLevel;
		_appearance.setNameColor(_accessLevel.getNameColor());
		_appearance.setTitleColor(_accessLevel.getTitleColor());
		if (broadcast)
		{
			broadcastUserInfo();
		}
		
		if (updateInDb)
		{
			try 
			{
				using GameServerDbContext ctx = new();
				PreparedStatement ps = con.prepareStatement(UPDATE_CHARACTER_ACCESS);
				ps.setInt(1, accessLevel.getLevel());
				ps.setInt(2, getObjectId());
				ps.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn("Failed to update character's accesslevel in db: " + this, e);
			}
		}
		
		CharInfoTable.getInstance().addName(this);
		
		if (accessLevel == null)
		{
			LOGGER.Warn("Tryed to set unregistered access level " + level + " for " + this + ". Setting access level without privileges!");
		}
		else if (level > 0)
		{
			LOGGER.Warn(_accessLevel.getName() + " access level set for character " + getName() + "! Just a warning to be careful ;)");
		}
	}
	
	public void setAccountAccesslevel(int level)
	{
		LoginServerThread.getInstance().sendAccessLevel(getAccountName(), level);
	}
	
	/**
	 * @return the _accessLevel of the Player.
	 */
	public override AccessLevel getAccessLevel()
	{
		return _accessLevel;
	}
	
	/**
	 * Update Stats of the Player client side by sending Server=>Client packet UserInfo/StatusUpdate to this Player and CharInfo/StatusUpdate to all known players (broadcast).
	 */
	public void updateAndBroadcastStatus()
	{
		if (_updateAndBroadcastStatusTask == null)
		{
			_updateAndBroadcastStatusTask = ThreadPool.schedule(() =>
			{
				refreshOverloaded(true);
				
				// Send a Server=>Client packet UserInfo to this Player and CharInfo to all known players (broadcast)
				broadcastUserInfo();
				
				_updateAndBroadcastStatusTask = null;
			}, 50);
		}
	}
	
	/**
	 * Send a Server=>Client StatusUpdate packet with Karma to the Player and all Player to inform (broadcast).
	 */
	public void broadcastReputation()
	{
		broadcastUserInfo(UserInfoType.SOCIAL);
		
		World.getInstance().forEachVisibleObject<Player>(this, player =>
		{
			if (!isVisibleFor(player))
			{
				return;
			}
			
			long relation = getRelation(player);
			bool isAutoAttackable = this.isAutoAttackable(player);
			RelationCache oldrelation = getKnownRelations().get(player.getObjectId());
			if ((oldrelation == null) || (oldrelation.getRelation() != relation) || (oldrelation.isAutoAttackable() != isAutoAttackable))
			{
				RelationChanged rc = new RelationChanged();
				rc.addRelation(this, relation, isAutoAttackable);
				if (hasSummon())
				{
					if (_pet != null)
					{
						rc.addRelation(_pet, relation, isAutoAttackable);
					}
					if (hasServitors())
					{
						getServitors().values().forEach(s => rc.addRelation(s, relation, isAutoAttackable));
					}
				}
				player.sendPacket(rc);
				getKnownRelations().put(player.getObjectId(), new RelationCache(relation, isAutoAttackable));
			}
		});
	}
	
	/**
	 * Set the online Flag to True or False and update the characters table of the database with online status and lastAccess (called when login and logout).
	 * @param isOnline
	 * @param updateInDb
	 */
	public void setOnlineStatus(bool isOnline, bool updateInDb)
	{
		if (_isOnline != isOnline)
		{
			_isOnline = isOnline;
		}
		
		// Update the characters table of the database with online status and lastAccess (called when login and logout)
		if (updateInDb)
		{
			updateOnlineStatus();
		}
	}
	
	/**
	 * Update the characters table of the database with online status and lastAccess of this Player (called when login and logout).
	 */
	public void updateOnlineStatus()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement =
				con.prepareStatement("UPDATE characters SET online=?, lastAccess=? WHERE charId=?");
			statement.setInt(1, isOnlineInt());
			statement.setLong(2, System.currentTimeMillis());
			statement.setInt(3, getObjectId());
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed updating character online status." + e);
		}
	}
	
	/**
	 * Create a new player in the characters table of the database.
	 * @return
	 */
	private bool createDb()
	{
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(INSERT_CHARACTER);
			statement.setString(1, _accountName);
			statement.setInt(2, getObjectId());
			statement.setString(3, getName());
			statement.setInt(4, getLevel());
			statement.setInt(5, getMaxHp());
			statement.setDouble(6, getCurrentHp());
			statement.setInt(7, getMaxCp());
			statement.setDouble(8, getCurrentCp());
			statement.setInt(9, getMaxMp());
			statement.setDouble(10, getCurrentMp());
			statement.setInt(11, _appearance.getFace());
			statement.setInt(12, _appearance.getHairStyle());
			statement.setInt(13, _appearance.getHairColor());
			statement.setInt(14, _appearance.isFemale() ? 1 : 0);
			statement.setLong(15, getExp());
			statement.setLong(16, getSp());
			statement.setInt(17, getReputation());
			statement.setInt(18, _fame);
			statement.setInt(19, _raidbossPoints);
			statement.setInt(20, _pvpKills);
			statement.setInt(21, _pkKills);
			statement.setInt(22, _clanId);
			statement.setInt(23, getRace().ordinal());
			statement.setInt(24, getClassId().getId());
			statement.setLong(25, _deleteTimer);
			statement.setInt(26, hasDwarvenCraft() ? 1 : 0);
			statement.setString(27, getTitle());
			statement.setInt(28, _appearance.getTitleColor());
			statement.setInt(29, isOnlineInt());
			statement.setInt(30, _clanPrivileges.getBitmask());
			statement.setInt(31, _wantsPeace);
			statement.setInt(32, _baseClass);
			statement.setInt(33, isNoble() ? 1 : 0);
			statement.setLong(34, 0);
			statement.setInt(35, PlayerStat.MIN_VITALITY_POINTS);
			statement.setDate(36, new Date(_createDate.getTimeInMillis()));
			statement.setInt(37, getTotalKills());
			statement.setInt(38, getTotalDeaths());
			statement.executeUpdate();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not insert char data: " + e);
			return false;
		}
		return true;
	}
	
	/**
	 * Retrieve a Player from the characters table of the database and add it in _allObjects of the L2world. <b><u>Actions</u>:</b>
	 * <li>Retrieve the Player from the characters table of the database</li>
	 * <li>Add the Player object in _allObjects</li>
	 * <li>Set the x,y,z position of the Player and make it invisible</li>
	 * <li>Update the overloaded status of the Player</li><br>
	 * @param objectId Identifier of the object to initialized
	 * @return The Player loaded from the database
	 */
	private static Player restore(int objectId)
	{
		Player player = null;
		double currentCp = 0;
		double currentHp = 0;
		double currentMp = 0;
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(RESTORE_CHARACTER);
			// Retrieve the Player from the characters table of the database
			statement.setInt(1, objectId);
			{
                ResultSet rset = statement.executeQuery();
				if (rset.next())
				{
					int activeClassId = rset.getInt("classid");
					bool female = rset.getInt("sex") != Sex.MALE.ordinal();
					PlayerTemplate template = PlayerTemplateData.getInstance().getTemplate(activeClassId);
					PlayerAppearance app = new PlayerAppearance(rset.getByte("face"), rset.getByte("hairColor"), rset.getByte("hairStyle"), female);
					player = new Player(objectId, template, rset.getString("account_name"), app);
					player.setName(rset.getString("char_name"));
					player.setLastAccess(rset.getLong("lastAccess"));
					
					player.getStat().setExp(rset.getLong("exp"));
					player.setExpBeforeDeath(rset.getLong("expBeforeDeath"));
					player.getStat().setLevel(rset.getInt("level"));
					player.getStat().setSp(rset.getLong("sp"));
					
					player.setWantsPeace(rset.getInt("wantspeace"));
					
					player.setHeading(rset.getInt("heading"));
					
					player.setInitialReputation(rset.getInt("reputation"));
					player.setFame(rset.getInt("fame"));
					player.setRaidbossPoints(rset.getInt("raidbossPoints"));
					player.setPvpKills(rset.getInt("pvpkills"));
					player.setPkKills(rset.getInt("pkkills"));
					player.setOnlineTime(rset.getLong("onlinetime"));
					player.setNoble(rset.getInt("nobless") == 1);
					
					int factionId = rset.getInt("faction");
					if (factionId == 1)
					{
						player.setGood();
					}
					if (factionId == 2)
					{
						player.setEvil();
					}
					
					player.setClanJoinExpiryTime(rset.getLong("clan_join_expiry_time"));
					if (player.getClanJoinExpiryTime() < System.currentTimeMillis())
					{
						player.setClanJoinExpiryTime(0);
					}
					player.setClanCreateExpiryTime(rset.getLong("clan_create_expiry_time"));
					if (player.getClanCreateExpiryTime() < System.currentTimeMillis())
					{
						player.setClanCreateExpiryTime(0);
					}
					
					player.setPcCafePoints(rset.getInt("pccafe_points"));
					
					int clanId = rset.getInt("clanid");
					player.setPowerGrade(rset.getInt("power_grade"));
					player.getStat().setVitalityPoints(rset.getInt("vitality_points"));
					player.setPledgeType(rset.getInt("subpledge"));
					// player.setApprentice(rset.getInt("apprentice"));
					
					if (clanId > 0)
					{
						player.setClan(ClanTable.getInstance().getClan(clanId));
					}
					
					if (player.getClan() != null)
					{
						if (player.getClan().getLeaderId() != player.getObjectId())
						{
							if (player.getPowerGrade() == 0)
							{
								player.setPowerGrade(5);
							}
							player.setClanPrivileges(player.getClan().getRankPrivs(player.getPowerGrade()));
						}
						else
						{
							player.getClanPrivileges().setAll();
							player.setPowerGrade(1);
						}
						player.setPledgeClass(ClanMember.calculatePledgeClass(player));
					}
					else
					{
						if (player.isNoble())
						{
							player.setPledgeClass(5);
						}
						
						if (player.isHero())
						{
							player.setPledgeClass(8);
						}
						
						player.getClanPrivileges().clear();
					}
					player.setTotalDeaths(rset.getInt("deaths"));
					player.setTotalKills(rset.getInt("kills"));
					player.setDeleteTimer(rset.getLong("deletetime"));
					player.setTitle(rset.getString("title"));
					player.setAccessLevel(rset.getInt("accesslevel"), false, false);
					int titleColor = rset.getInt("title_color");
					if (titleColor != PlayerAppearance.DEFAULT_TITLE_COLOR)
					{
						player.getAppearance().setTitleColor(titleColor);
					}
					player.setFistsWeaponItem(player.findFistsWeaponItem(activeClassId));
					player.setUptime(System.currentTimeMillis());
					
					currentHp = rset.getDouble("curHp");
					currentCp = rset.getDouble("curCp");
					currentMp = rset.getDouble("curMp");
					player.setClassIndex(0);
					try
					{
						player.setBaseClass(rset.getInt("base_class"));
					}
					catch (Exception e)
					{
						player.setBaseClass(activeClassId);
						LOGGER.Warn("Exception during player.setBaseClass for player: " + player + " base class: " + rset.getInt("base_class"), e);
					}
					
					// Restore Subclass Data (cannot be done earlier in function)
					if (restoreSubClassData(player) && (activeClassId != player.getBaseClass()))
					{
						foreach (SubClassHolder subClass in player.getSubClasses().values())
						{
							if (subClass.getClassId() == activeClassId)
							{
								player.setClassIndex(subClass.getClassIndex());
							}
						}
					}
					if ((player.getClassIndex() == 0) && (activeClassId != player.getBaseClass()))
					{
						// Subclass in use but doesn't exist in DB -
						// a possible restart-while-modifysubclass cheat has been attempted.
						// Switching to use base class
						player.setClassId(player.getBaseClass());
						LOGGER.Warn(player + " reverted to base class. Possibly has tried a relogin exploit while subclassing.");
					}
					else
					{
						player._activeClass = activeClassId;
					}
					if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, player.getBaseTemplate().getClassId()))
					{
						player._isDeathKnight = true;
					}
					else if (CategoryData.getInstance().isInCategory(CategoryType.VANGUARD_ALL_CLASS, player.getBaseTemplate().getClassId()))
					{
						player._isVanguard = true;
					}
					else if (CategoryData.getInstance().isInCategory(CategoryType.ASSASSIN_ALL_CLASS, player.getBaseTemplate().getClassId()))
					{
						player._isAssassin = true;
					}
					
					player.setApprentice(rset.getInt("apprentice"));
					player.setSponsor(rset.getInt("sponsor"));
					player.setLvlJoinedAcademy(rset.getInt("lvl_joined_academy"));
					
					// Set Hero status if it applies.
					player.setHero(Hero.getInstance().isHero(objectId));
					
					CursedWeaponsManager.getInstance().checkPlayer(player);
					
					// Set the x,y,z position of the Player and make it invisible
					int x = rset.getInt("x");
					int y = rset.getInt("y");
					int z = rset.getInt("z");
					player.setXYZInvisible(x, y, z);
					player.setLastServerPosition(x, y, z);
					
					// Set Teleport Bookmark Slot
					player.setBookMarkSlot(rset.getInt("BookmarkSlot"));
					
					// character creation Time
					player.getCreateDate().setTime(rset.getDate("createDate"));
					
					// Language
					player.setLang(rset.getString("language"));
					
					// Retrieve the name and ID of the other characters assigned to this account.
					try
					{
						PreparedStatement stmt =
							con.prepareStatement(
								"SELECT charId, char_name FROM characters WHERE account_name=? AND charId<>?");
						stmt.setString(1, player._accountName);
						stmt.setInt(2, objectId);
						{
							ResultSet chars = stmt.executeQuery();
							while (chars.next())
							{
								player._chars.put(chars.getInt("charId"), chars.getString("char_name"));
							}
						}
					}
				}
			}
			
			if (player == null)
			{
				return null;
			}
			
			if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_LOAD, player))
			{
				EventDispatcher.getInstance().notifyEventAsync(new OnPlayerLoad(player), player);
			}
			
			if (player.isGM())
			{
				long masks = player.getVariables().getLong(COND_OVERRIDE_KEY, PlayerCondOverride.getAllExceptionsMask());
				player.setOverrideCond(masks);
			}
			
			// Retrieve from the database all items of this Player and add them to _inventory
			player.getInventory().restore();
			player.getWarehouse().restore();
			player.getFreight().restore();
			player.restoreItemReuse();
			
			// Retrieve from the database all secondary data of this Player
			// Note that Clan, Noblesse and Hero skills are given separately and not here.
			// Retrieve from the database all skills of this Player and add them to _skills
			player.restoreCharData();
			
			// Reward auto-get skills and all available skills if auto-learn skills is true.
			player.rewardSkills();
			
			// Restore player shortcuts
			player.restoreShortCuts();
			
			player.restorePetEvolvesByItem();
			
			// Initialize status update cache
			player.initStatusUpdateCache();
			
			// Restore current Cp, HP and MP values
			player.setCurrentCp(currentCp);
			player.setCurrentHp(currentHp);
			player.setCurrentMp(currentMp);
			
			player.setOriginalCpHpMp(currentCp, currentHp, currentMp);
			if (currentHp < 0.5)
			{
				player.setDead(true);
				player.stopHpMpRegeneration();
			}
			
			// Restore pet if exists in the world
			player.setPet(World.getInstance().getPet(player.getObjectId()));
			Summon pet = player.getPet();
			if (pet != null)
			{
				pet.setOwner(player);
			}
			
			if (player.hasServitors())
			{
				foreach (Summon summon in player.getServitors().values())
				{
					summon.setOwner(player);
				}
			}
			
			// Recalculate all stats
			player.getStat().recalculateStats(false);
			
			// Update the overloaded status of the Player
			player.refreshOverloaded(false);
			
			player.restoreFriendList();
			
			player.restoreRandomCraft();
			player.restoreSurveillanceList();
			
			player.loadRecommendations();
			player.startRecoGiveTask();
			player.startOnlineTimeUpdateTask();
			
			player.setOnlineStatus(true, false);
			
			PlayerAutoSaveTaskManager.getInstance().add(player);
			
			if (Config.ENABLE_ACHIEVEMENT_BOX)
			{
				player.getAchievementBox().restore();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed loading character: "+e);
		}
		return player;
	}
	
	/**
	 * @return
	 */
	public Forum getMail()
	{
		if (_forumMail == null)
		{
			setMail(ForumsBBSManager.getInstance().getForumByName("MailRoot").getChildByName(getName()));
			
			if (_forumMail == null)
			{
				ForumsBBSManager.getInstance().createNewForum(getName(), ForumsBBSManager.getInstance().getForumByName("MailRoot"), Forum.MAIL, Forum.OWNERONLY, getObjectId());
				setMail(ForumsBBSManager.getInstance().getForumByName("MailRoot").getChildByName(getName()));
			}
		}
		return _forumMail;
	}
	
	public void setMail(Forum forum)
	{
		_forumMail = forum;
	}
	
	public Forum getMemo()
	{
		if (_forumMemo == null)
		{
			setMemo(ForumsBBSManager.getInstance().getForumByName("MemoRoot").getChildByName(_accountName));
			
			if (_forumMemo == null)
			{
				ForumsBBSManager.getInstance().createNewForum(_accountName, ForumsBBSManager.getInstance().getForumByName("MemoRoot"), Forum.MEMO, Forum.OWNERONLY, getObjectId());
				setMemo(ForumsBBSManager.getInstance().getForumByName("MemoRoot").getChildByName(_accountName));
			}
		}
		return _forumMemo;
	}
	
	public void setMemo(Forum forum)
	{
		_forumMemo = forum;
	}
	
	/**
	 * Restores sub-class data for the Player, used to check the current class index for the character.
	 * @param player
	 * @return
	 */
	private static bool restoreSubClassData(Player player)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(RESTORE_CHAR_SUBCLASSES);
			statement.setInt(1, player.getObjectId());
			{
				ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					SubClassHolder subClass = new SubClassHolder();
					subClass.setClassId(rset.getInt("class_id"));
					subClass.setDualClassActive(rset.getBoolean("dual_class"));
					subClass.setVitalityPoints(rset.getInt("vitality_points"));
					subClass.setLevel(rset.getInt("level"));
					subClass.setExp(rset.getLong("exp"));
					subClass.setSp(rset.getLong("sp"));
					subClass.setClassIndex(rset.getInt("class_index"));
					
					// Enforce the correct indexing of _subClasses against their class indexes.
					player.getSubClasses().put(subClass.getClassIndex(), subClass);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore classes for " + player.getName() + ": " + e);
		}
		return true;
	}
	
	/**
	 * Restores:
	 * <ul>
	 * <li>Skills</li>
	 * <li>Macros</li>
	 * <li>Henna</li>
	 * <li>Teleport Bookmark</li>
	 * <li>Recipe Book</li>
	 * <li>Recipe Shop List (If configuration enabled)</li>
	 * <li>Premium Item List</li>
	 * <li>Pet Inventory Items</li>
	 * </ul>
	 */
	private void restoreCharData()
	{
		// Retrieve from the database all skills of this Player and add them to _skills.
		restoreSkills();
		
		// Retrieve from the database all macroses of this Player and add them to _macros.
		_macros.restoreMe();
		
		// Retrieve from the database all henna of this Player and add them to _henna.
		restoreHenna();
		
		// Retrieve from the database all teleport bookmark of this Player and add them to _tpbookmark.
		restoreTeleportBookmark();
		
		// Retrieve from the database the recipe book of this Player.
		restoreRecipeBook(true);
		
		// Restore Recipe Shop list.
		if (Config.STORE_RECIPE_SHOPLIST)
		{
			restoreRecipeShopList();
		}
		
		// Restore collections.
		restoreCollections();
		restoreCollectionBonuses();
		restoreCollectionFavorites();
		
		_challengePoints.restoreChallengePoints();
		
		// Purge.
		restoreSubjugation();
		
		// Load Premium Item List.
		loadPremiumItemList();
		
		// Restore items in pet inventory.
		restorePetInventoryItems();
	}
	
	/**
	 * Restores:
	 * <ul>
	 * <li>Short-cuts</li>
	 * </ul>
	 */
	private void restoreShortCuts()
	{
		// Retrieve from the database all shortCuts of this Player and add them to _shortCuts.
		_shortCuts.restoreMe();
	}
	
	/**
	 * Restore recipe book data for this Player.
	 * @param loadCommon
	 */
	private void restoreRecipeBook(bool loadCommon)
	{
		String sql = loadCommon ? "SELECT id, type, classIndex FROM character_recipebook WHERE charId=?" : "SELECT id FROM character_recipebook WHERE charId=? AND classIndex=? AND type = 1";
		try 
		{
            using GameServerDbContext ctx = new();
            PreparedStatement statement = con.prepareStatement(sql);
			statement.setInt(1, getObjectId());
			if (!loadCommon)
			{
				statement.setInt(2, _classIndex);
			}


			{
				ResultSet rset = statement.executeQuery();
				_dwarvenRecipeBook.clear();
				
				RecipeList recipe;
				RecipeData rd = RecipeData.getInstance();
				while (rset.next())
				{
					recipe = rd.getRecipeList(rset.getInt("id"));
					if (loadCommon)
					{
						if (rset.getInt(2) == 1)
						{
							if (rset.getInt(3) == _classIndex)
							{
								registerDwarvenRecipeList(recipe, false);
							}
						}
						else
						{
							registerCommonRecipeList(recipe, false);
						}
					}
					else
					{
						registerDwarvenRecipeList(recipe, false);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore recipe book data:" + e);
		}
	}
	
	public Map<int, PremiumItem> getPremiumItemList()
	{
		return _premiumItems;
	}
	
	private void loadPremiumItemList()
	{
		String sql = "SELECT itemNum, itemId, itemCount, itemSender FROM character_premium_items WHERE charId=?";
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(sql);
			statement.setInt(1, getObjectId());
					{
                        ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					int itemNum = rset.getInt("itemNum");
					int itemId = rset.getInt("itemId");
					long itemCount = rset.getLong("itemCount");
					String itemSender = rset.getString("itemSender");
					_premiumItems.put(itemNum, new PremiumItem(itemId, itemCount, itemSender));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore premium items: " + e);
		}
	}
	
	public void updatePremiumItem(int itemNum, long newcount)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement =
				con.prepareStatement("UPDATE character_premium_items SET itemCount=? WHERE charId=? AND itemNum=? ");
			statement.setLong(1, newcount);
			statement.setInt(2, getObjectId());
			statement.setInt(3, itemNum);
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not update premium items: " + e);
		}
	}
	
	public void deletePremiumItem(int itemNum)
	{
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement("DELETE FROM character_premium_items WHERE charId=? AND itemNum=? ");
			statement.setInt(1, getObjectId());
			statement.setInt(2, itemNum);
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not delete premium item: " + e);
		}
	}
	
	/**
	 * Update Player stats in the characters table of the database.
	 * @param storeActiveEffects
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void store(bool storeActiveEffects)
	{
		storeCharBase();
		storeCharSub();
		storeEffect(storeActiveEffects);
		storeItemReuseDelay();
		storeDyePoten();
		
		if (Config.STORE_RECIPE_SHOPLIST)
		{
			storeRecipeShopList();
		}
		
		_rankingHistory.store();
		
		// Store collections.
		storeCollections();
		storeCollectionFavorites();
		
		// Purge.
		storeSubjugation();
		
		_challengePoints.storeChallengePoints();
		
		storeDualInventory();
		
		PlayerVariables vars = getScript<PlayerVariables>();
		if (vars != null)
		{
			vars.storeMe();
		}
		
		AccountVariables aVars = getScript<AccountVariables>();
		if (aVars != null)
		{
			aVars.storeMe();
		}
		
		getInventory().updateDatabase();
		getWarehouse().updateDatabase();
		getFreight().updateDatabase();
		
		if (_spirits != null)
		{
			foreach (ElementalSpirit spirit in _spirits)
			{
				if (spirit != null)
				{
					spirit.save();
				}
			}
		}
		
		if (_randomCraft != null)
		{
			_randomCraft.store();
		}
		
		if (_huntPass != null)
		{
			_huntPass.store();
		}
		
		if (_achivemenetBox != null)
		{
			_achivemenetBox.store();
		}
	}
	
	public override void storeMe()
	{
		store(true);
	}
	
	private void storeCharBase()
	{
		// Get the exp, level, and sp of base class to store in base table
		long exp = getStat().getBaseExp();
		int level = getStat().getBaseLevel();
		long sp = getStat().getBaseSp();
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(UPDATE_CHARACTER);
			statement.setInt(1, level);
			statement.setInt(2, getMaxHp());
			statement.setDouble(3, getCurrentHp());
			statement.setInt(4, getMaxCp());
			statement.setDouble(5, getCurrentCp());
			statement.setInt(6, getMaxMp());
			statement.setDouble(7, getCurrentMp());
			statement.setInt(8, _appearance.getFace());
			statement.setInt(9, _appearance.getHairStyle());
			statement.setInt(10, _appearance.getHairColor());
			statement.setInt(11, _appearance.isFemale() ? 1 : 0);
			statement.setInt(12, getHeading());
			statement.setInt(13, _lastLoc != null ? _lastLoc.getX() : getX());
			statement.setInt(14, _lastLoc != null ? _lastLoc.getY() : getY());
			statement.setInt(15, _lastLoc != null ? _lastLoc.getZ() : getZ());
			statement.setLong(16, exp);
			statement.setLong(17, _expBeforeDeath);
			statement.setLong(18, sp);
			statement.setInt(19, getReputation());
			statement.setInt(20, _fame);
			statement.setInt(21, _raidbossPoints);
			statement.setInt(22, _pvpKills);
			statement.setInt(23, _pkKills);
			statement.setInt(24, _clanId);
			statement.setInt(25, getRace().ordinal());
			statement.setInt(26, getClassId().getId());
			statement.setLong(27, _deleteTimer);
			statement.setString(28, getTitle());
			statement.setInt(29, _appearance.getTitleColor());
			statement.setInt(30, isOnlineInt());
			statement.setInt(31, _clanPrivileges.getBitmask());
			statement.setInt(32, _wantsPeace);
			statement.setInt(33, _baseClass);
			long totalOnlineTime = _onlineTime;
			if (_onlineBeginTime > 0)
			{
				totalOnlineTime += (System.currentTimeMillis() - _onlineBeginTime) / 1000;
			}
			statement.setLong(34, _offlineShopStart > 0 ? _onlineTime : totalOnlineTime);
			statement.setInt(35, isNoble() ? 1 : 0);
			statement.setInt(36, _powerGrade);
			statement.setInt(37, _pledgeType);
			statement.setInt(38, _lvlJoinedAcademy);
			statement.setLong(39, _apprentice);
			statement.setLong(40, _sponsor);
			statement.setLong(41, _clanJoinExpiryTime);
			statement.setLong(42, _clanCreateExpiryTime);
			statement.setString(43, getName());
			statement.setInt(44, _bookmarkslot);
			statement.setInt(45, getStat().getBaseVitalityPoints());
			statement.setString(46, _lang);
			int factionId = 0;
			if (_isGood)
			{
				factionId = 1;
			}
			if (_isEvil)
			{
				factionId = 2;
			}
			statement.setInt(47, factionId);
			statement.setInt(48, _pcCafePoints);
			statement.setInt(49, getTotalKills());
			statement.setInt(50, getTotalDeaths());
			statement.setInt(51, getObjectId());
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store char base data: " + this + " - " + e);
		}
	}
	
	private void storeCharSub()
	{
		if (getTotalSubClasses() <= 0)
		{
			return;
		}
		
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(UPDATE_CHAR_SUBCLASS);
			foreach (SubClassHolder subClass in getSubClasses().values())
			{
				statement.setLong(1, subClass.getExp());
				statement.setLong(2, subClass.getSp());
				statement.setInt(3, subClass.getLevel());
				statement.setInt(4, subClass.getVitalityPoints());
				statement.setInt(5, subClass.getClassId());
				statement.setBoolean(6, subClass.isDualClass());
				statement.setInt(7, getObjectId());
				statement.setInt(8, subClass.getClassIndex());
				statement.addBatch();
			}
			statement.executeBatch();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store sub class data for " + getName() + ": " + e);
		}
	}
	
	public override void storeEffect(bool storeEffects)
	{
		if (!Config.STORE_SKILL_COOLTIME)
		{
			return;
		}
		
		try
		{
			using GameServerDbContext ctx = new();
			// Delete all current stored effects for char to avoid dupe

			{
				PreparedStatement delete = con.prepareStatement(DELETE_SKILL_SAVE);
				delete.setInt(1, getObjectId());
				delete.setInt(2, _classIndex);
				delete.execute();
			}
			
			int buffIndex = 0;
			List<long> storedSkills = new();
			long currentTime = System.currentTimeMillis();
			
			// Store all effect data along with calulated remaining
			// reuse delays for matching skills. 'restore_type'= 0.
			{
				PreparedStatement statement = con.prepareStatement(ADD_SKILL_SAVE);
				if (storeEffects)
				{
					foreach (BuffInfo info in getEffectList().getEffects())
					{
						if (info == null)
						{
							continue;
						}
						
						Skill skill = info.getSkill();
						
						// Do not store those effects.
						if (skill.isDeleteAbnormalOnLeave())
						{
							continue;
						}
						
						// Do not save heals.
						if (skill.getAbnormalType() == AbnormalType.LIFE_FORCE_OTHERS)
						{
							continue;
						}
						
						// Toggles are skipped, unless they are necessary to be always on.
						if ((skill.isToggle() && !skill.isNecessaryToggle()))
						{
							continue;
						}
						
						if (skill.isMentoring())
						{
							continue;
						}
						
						// Dances and songs are not kept in retail.
						if (skill.isDance() && !Config.ALT_STORE_DANCES)
						{
							continue;
						}
						
						if (storedSkills.Contains(skill.getReuseHashCode()))
						{
							continue;
						}
						
						storedSkills.add(skill.getReuseHashCode());
						
						statement.setInt(1, getObjectId());
						statement.setInt(2, skill.getId());
						statement.setInt(3, skill.getLevel());
						statement.setInt(4, skill.getSubLevel());
						statement.setInt(5, info.getTime());
						
						TimeStamp t = getSkillReuseTimeStamp(skill.getReuseHashCode());
						statement.setLong(6, (t != null) && (currentTime < t.getStamp()) ? t.getReuse() : 0);
						statement.setDouble(7, (t != null) && (currentTime < t.getStamp()) ? t.getStamp() : 0);
						statement.setInt(8, 0); // Store type 0, active buffs/debuffs.
						statement.setInt(9, _classIndex);
						statement.setInt(10, ++buffIndex);
						statement.addBatch();
					}
				}
				
				// Skills under reuse.
				foreach (var ts in getSkillReuseTimeStamps())
				{
					long hash = ts.Key;
					if (storedSkills.Contains(hash))
					{
						continue;
					}
					
					TimeStamp t = ts.Value;
					if ((t != null) && (currentTime < t.getStamp()))
					{
						storedSkills.add(hash);
						
						statement.setInt(1, getObjectId());
						statement.setInt(2, t.getSkillId());
						statement.setInt(3, t.getSkillLevel());
						statement.setInt(4, t.getSkillSubLevel());
						statement.setInt(5, -1);
						statement.setLong(6, t.getReuse());
						statement.setDouble(7, t.getStamp());
						statement.setInt(8, 1); // Restore type 1, skill reuse.
						statement.setInt(9, _classIndex);
						statement.setInt(10, ++buffIndex);
						statement.addBatch();
					}
				}
				
				statement.executeBatch();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store char effect data: ", e);
		}
	}
	
	private void storeItemReuseDelay()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps1 = con.prepareStatement(DELETE_ITEM_REUSE_SAVE);
			PreparedStatement ps2 = con.prepareStatement(ADD_ITEM_REUSE_SAVE);
			ps1.setInt(1, getObjectId());
			ps1.execute();
			
			long currentTime = System.currentTimeMillis();
			foreach (TimeStamp ts in getItemReuseTimeStamps().values())
			{
				if ((ts != null) && (currentTime < ts.getStamp()))
				{
					ps2.setInt(1, getObjectId());
					ps2.setInt(2, ts.getItemId());
					ps2.setInt(3, ts.getItemObjectId());
					ps2.setLong(4, ts.getReuse());
					ps2.setDouble(5, ts.getStamp());
					ps2.addBatch();
				}
			}
			ps2.executeBatch();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store char item reuse data: ", e);
		}
	}
	
	/**
	 * @return True if the Player is online.
	 */
	public bool isOnline()
	{
		return _isOnline;
	}
	
	public int isOnlineInt()
	{
		if (_isOnline && (_client != null))
		{
			return _client.isDetached() ? 2 : 1;
		}
		return 0;
	}
	
	public void startOfflinePlay()
	{
		sendPacket(LeaveWorld.STATIC_PACKET);
		
		if (Config.OFFLINE_PLAY_SET_NAME_COLOR)
		{
			getAppearance().setNameColor(Config.OFFLINE_NAME_COLOR);
		}
		if (!Config.OFFLINE_PLAY_ABNORMAL_EFFECTS.isEmpty())
		{
			getEffectList().startAbnormalVisualEffect(Config.OFFLINE_PLAY_ABNORMAL_EFFECTS[(Rnd.get(Config.OFFLINE_PLAY_ABNORMAL_EFFECTS.Length))]);
		}
		broadcastUserInfo();
		
		_offlinePlay = true;
		_client.setDetached(true);
	}
	
	public bool isOfflinePlay()
	{
		return _offlinePlay;
	}
	
	public void setEnteredWorld()
	{
		_enteredWorld = true;
	}
	
	public bool hasEnteredWorld()
	{
		return _enteredWorld;
	}
	
	/**
	 * Verifies if the player is in offline mode.<br>
	 * The offline mode may happen for different reasons:<br>
	 * Abnormally: Player gets abruptly disconnected from server.<br>
	 * Normally: The player gets into offline shop mode, only available by enabling the offline shop mod.
	 * @return {@code true} if the player is in offline mode, {@code false} otherwise
	 */
	public bool isInOfflineMode()
	{
		return (_client == null) || _client.isDetached();
	}
	
	public override Skill addSkill(Skill newSkill)
	{
		addCustomSkill(newSkill);
		return base.addSkill(newSkill);
	}
	
	/**
	 * Add a skill to the Player _skills and its Func objects to the calculator set of the Player and save update in the character_skills table of the database. <b><u>Concept</u>:</b> All skills own by a Player are identified in <b>_skills</b> <b><u> Actions</u>:</b>
	 * <li>Replace oldSkill by newSkill or Add the newSkill</li>
	 * <li>If an old skill has been replaced, remove all its Func objects of Creature calculator set</li>
	 * <li>Add Func objects of newSkill to the calculator set of the Creature</li><br>
	 * @param newSkill The Skill to add to the Creature
	 * @param store
	 * @return The Skill replaced or null if just added a new Skill
	 */
	public Skill addSkill(Skill newSkill, bool store)
	{
		// Add a skill to the Player _skills and its Func objects to the calculator set of the Player
		Skill oldSkill = addSkill(newSkill);
		// Add or update a Player skill in the character_skills table of the database
		if (store)
		{
			storeSkill(newSkill, oldSkill, -1);
		}
		return oldSkill;
	}
	
	public override Skill removeSkill(Skill skill, bool store)
	{
		removeCustomSkill(skill);
		return store ? removeSkill(skill) : base.removeSkill(skill, true);
	}
	
	public Skill removeSkill(Skill skill, bool store, bool cancelEffect)
	{
		removeCustomSkill(skill);
		return store ? removeSkill(skill) : base.removeSkill(skill, cancelEffect);
	}
	
	/**
	 * Remove a skill from the Creature and its Func objects from calculator set of the Creature and save update in the character_skills table of the database. <b><u>Concept</u>:</b> All skills own by a Creature are identified in <b>_skills</b> <b><u> Actions</u>:</b>
	 * <li>Remove the skill from the Creature _skills</li>
	 * <li>Remove all its Func objects from the Creature calculator set</li> <b><u> Overridden in</u>:</b>
	 * <li>Player : Save update in the character_skills table of the database</li><br>
	 * @param skill The Skill to remove from the Creature
	 * @return The Skill removed
	 */
	public Skill removeSkill(Skill skill)
	{
		removeCustomSkill(skill);
		
		// Remove a skill from the Creature and its stats
		Skill oldSkill = base.removeSkill(skill, true);
		if (oldSkill != null)
		{
			try 
			{
				using GameServerDbContext ctx = new();
				PreparedStatement statement = con.prepareStatement(DELETE_SKILL_FROM_CHAR);
				// Remove or update a Player skill from the character_skills table of the database
				statement.setInt(1, oldSkill.getId());
				statement.setInt(2, getObjectId());
				statement.setInt(3, _classIndex);
				statement.execute();
			}
			catch (Exception e)
			{
				LOGGER.Warn("Error could not delete skill: " + e);
			}
		}
		
		if ((getTransformationId() > 0) || isCursedWeaponEquipped())
		{
			return oldSkill;
		}
		
		if (skill != null)
		{
			foreach (Shortcut sc in _shortCuts.getAllShortCuts())
			{
				if ((sc != null) && (sc.getId() == skill.getId()) && (sc.getType() == ShortcutType.SKILL) && !((skill.getId() >= 3080) && (skill.getId() <= 3259)))
				{
					deleteShortCut(sc.getSlot(), sc.getPage());
				}
			}
		}
		return oldSkill;
	}
	
	/**
	 * Add or update a Player skill in the character_skills table of the database.<br>
	 * If newClassIndex > -1, the skill will be stored with that class index, not the current one.
	 * @param newSkill
	 * @param oldSkill
	 * @param newClassIndex
	 */
	private void storeSkill(Skill newSkill, Skill oldSkill, int newClassIndex)
	{
		int classIndex = (newClassIndex > -1) ? newClassIndex : _classIndex;
		try
		{
			using GameServerDbContext ctx = new();
			if ((oldSkill != null) && (newSkill != null))
			{
				
				{
                    PreparedStatement ps = con.prepareStatement(UPDATE_CHARACTER_SKILL_LEVEL);
					ps.setInt(1, newSkill.getLevel());
					ps.setInt(2, newSkill.getSubLevel());
					ps.setInt(3, oldSkill.getId());
					ps.setInt(4, getObjectId());
					ps.setInt(5, classIndex);
					ps.execute();
				}
			}
			else if (newSkill != null)
			{

				{
					PreparedStatement ps = con.prepareStatement(ADD_NEW_SKILLS);
					ps.setInt(1, getObjectId());
					ps.setInt(2, newSkill.getId());
					ps.setInt(3, newSkill.getLevel());
					ps.setInt(4, newSkill.getSubLevel());
					ps.setInt(5, classIndex);
					ps.execute();
				}
			}
			// else
			// {
			// LOGGER.warning("Could not store new skill, it's null!");
			// }
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error could not store char skills: " + e);
		}
	}
	
	/**
	 * Adds or updates player's skills in the database.
	 * @param newSkills the list of skills to store
	 * @param newClassIndex if newClassIndex > -1, the skills will be stored for that class index, not the current one
	 */
	private void storeSkills(List<Skill> newSkills, int newClassIndex)
	{
		if (newSkills.isEmpty())
		{
			return;
		}
		
		int classIndex = (newClassIndex > -1) ? newClassIndex : _classIndex;
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(ADD_NEW_SKILLS);
			foreach (Skill addSkill in newSkills)
			{
				ps.setInt(1, getObjectId());
				ps.setInt(2, addSkill.getId());
				ps.setInt(3, addSkill.getLevel());
				ps.setInt(4, addSkill.getSubLevel());
				ps.setInt(5, classIndex);
				ps.addBatch();
			}
			ps.executeBatch();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error could not store char skills: " + e);
		}
	}
	
	/**
	 * Retrieve from the database all skills of this Player and add them to _skills.
	 */
	private void restoreSkills()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(RESTORE_SKILLS_FOR_CHAR);
			// Retrieve all skills of this Player from the database
			statement.setInt(1, getObjectId());
			statement.setInt(2, _classIndex);

			{
				ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					int id = rset.getInt("skill_id");
					int level = rset.getInt("skill_level");
					int subLevel = rset.getInt("skill_sub_level");
					
					// Create a Skill object for each record
					Skill skill = SkillData.getInstance().getSkill(id, level, subLevel);
					if (skill == null)
					{
						LOGGER.Warn("Skipped null skill Id: " + id + " Level: " + level + " while restoring player skills for playerObjId: " + getObjectId());
						continue;
					}
					
					// Add the Skill object to the Creature _skills and its Func objects to the calculator set of the Creature
					addSkill(skill);
					
					if (Config.SKILL_CHECK_ENABLE && (!canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS) || Config.SKILL_CHECK_GM) && !SkillTreeData.getInstance().isSkillAllowed(this, skill))
					{
						Util.handleIllegalPlayerAction(this, "Player " + getName() + " has invalid skill " + skill.getName() + " (" + skill.getId() + "/" + skill.getLevel() + "), class:" + ClassListData.getInstance().getClass(getClassId()).getClassName(), IllegalActionPunishmentType.BROADCAST);
						if (Config.SKILL_CHECK_REMOVE)
						{
							removeSkill(skill);
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore character " + this + " skills: " + e);
		}
	}
	
	/**
	 * Retrieve from the database all skill effects of this Player and add them to the player.
	 */
	public override void restoreEffects()
	{
		try
		{
            using GameServerDbContext ctx = new();
            PreparedStatement statement = con.prepareStatement(RESTORE_SKILL_SAVE);
			statement.setInt(1, getObjectId());
			statement.setInt(2, _classIndex);

			{
				ResultSet rset = statement.executeQuery();
				long currentTime = System.currentTimeMillis();
				while (rset.next())
				{
					int remainingTime = rset.getInt("remaining_time");
					long reuseDelay = rset.getLong("reuse_delay");
					long systime = rset.getLong("systime");
					int restoreType = rset.getInt("restore_type");
					Skill skill = SkillData.getInstance().getSkill(rset.getInt("skill_id"), rset.getInt("skill_level"), rset.getInt("skill_sub_level"));
					if (skill == null)
					{
						continue;
					}
					
					long time = systime - currentTime;
					if (time > 10)
					{
						disableSkill(skill, time);
						addTimeStamp(skill, reuseDelay, systime);
					}
					
					// Restore Type 1 The remaning skills lost effect upon logout but were still under a high reuse delay.
					if (restoreType > 0)
					{
						continue;
					}
					
					// Restore Type 0 These skill were still in effect on the character upon logout.
					// Some of which were self casted and might still have had a long reuse delay which also is restored.
					skill.applyEffects(this, this, false, remainingTime);
				}
			}
			// Remove previously restored skills

			{
				PreparedStatement delete = con.prepareStatement(DELETE_SKILL_SAVE);
				delete.setInt(1, getObjectId());
				delete.setInt(2, _classIndex);
				delete.executeUpdate();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore " + this + " active effect data: " + e);
		}
	}
	
	/**
	 * Retrieve from the database all Item Reuse Time of this Player and add them to the player.
	 */
	private void restoreItemReuse()
	{
		try 
        {
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(RESTORE_ITEM_REUSE_SAVE);
            			PreparedStatement delete = con.prepareStatement(DELETE_ITEM_REUSE_SAVE);
			statement.setInt(1, getObjectId());

			{
				ResultSet rset = statement.executeQuery();
				int itemId;
				long reuseDelay;
				long systime;
				bool isInInventory;
				long remainingTime;
				long currentTime = System.currentTimeMillis();
				while (rset.next())
				{
					itemId = rset.getInt("itemId");
					reuseDelay = rset.getLong("reuseDelay");
					systime = rset.getLong("systime");
					isInInventory = true;
					
					// Using item Id
					Item item = _inventory.getItemByItemId(itemId);
					if (item == null)
					{
						item = getWarehouse().getItemByItemId(itemId);
						isInInventory = false;
					}
					
					if ((item != null) && (item.getId() == itemId) && (item.getReuseDelay() > 0))
					{
						remainingTime = systime - currentTime;
						if (remainingTime > 10)
						{
							addTimeStampItem(item, reuseDelay, systime);
							if (isInInventory && item.isEtcItem())
							{
								int group = item.getSharedReuseGroup();
								if (group > 0)
								{
									sendPacket(new ExUseSharedGroupItem(itemId, group, (int) remainingTime, (int) reuseDelay));
								}
							}
						}
					}
				}
			}
			
			// Delete item reuse.
			delete.setInt(1, getObjectId());
			delete.executeUpdate();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore " + this + " Item Reuse data: " + e);
		}
	}
	
	/**
	 * Retrieve from the database all Henna of this Player, add them to _henna and calculate stats of the Player.
	 */
	private void restoreHenna()
	{
		restoreDyePoten();
		
		// Cancel and remove existing running tasks.
		foreach (var entry in _hennaRemoveSchedules)
		{
			ScheduledFuture task = entry.Value;
			if ((task != null) && !task.isCancelled() && !task.isDone())
			{
				task.cancel(true);
			}
			_hennaRemoveSchedules.remove(entry.Key);
		}
		
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(RESTORE_CHAR_HENNAS);
			statement.setInt(1, getObjectId());
			statement.setInt(2, _classIndex);
			
			{
                ResultSet rset = statement.executeQuery();
				int slot;
				int symbolId;
				long currentTime = System.currentTimeMillis();
				while (rset.next())
				{
					slot = rset.getInt("slot");
					if ((slot < 1) || (slot > getAvailableHennaSlots()))
					{
						continue;
					}
					
					symbolId = rset.getInt("symbol_id");
					if (symbolId == 0)
					{
						continue;
					}
					
					Henna henna = HennaData.getInstance().getHennaByDyeId(symbolId);
					if (henna == null)
					{
						continue;
					}
					
					// Task for henna duration
					if (henna.getDuration() > 0)
					{
						long remainingTime = getVariables().getLong("HennaDuration" + slot, currentTime) - currentTime;
						if (remainingTime < 0)
						{
							removeHenna(slot);
							continue;
						}
						
						// Add the new task.
						_hennaRemoveSchedules.put(slot, ThreadPool.schedule(new HennaDurationTask(this, slot), currentTime + remainingTime));
					}
					
					_hennaPoten[slot - 1].setHenna(henna);
					
					// Reward henna skills
					foreach (Skill skill in henna.getSkills())
					{
						addSkill(skill, false);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed restoing character " + this + " hennas: " + e);
		}
		
		// Calculate henna modifiers of this player.
		recalcHennaStats();
		applyDyePotenSkills();
	}
	
	/**
	 * @return the number of Henna empty slot of the Player.
	 */
	public int getHennaEmptySlots()
	{
		int totalSlots = 0;
		if (getClassId().level() == 1)
		{
			totalSlots = 2;
		}
		else if (getClassId().level() > 1)
		{
			totalSlots = getAvailableHennaSlots();
		}
		
		for (int i = 0; i < _hennaPoten.Length; i++)
		{
			if (_hennaPoten[i].getHenna() != null)
			{
				totalSlots--;
			}
		}
		
		if (totalSlots <= 0)
		{
			return 0;
		}
		
		return totalSlots;
	}
	
	/**
	 * Remove a Henna of the Player, save update in the character_hennas table of the database and send Server=>Client HennaInfo/UserInfo packet to this Player.
	 * @param slot
	 * @return
	 */
	public bool removeHenna(int slot)
	{
		return removeHenna(slot, true);
	}
	
	/**
	 * Remove a Henna of the Player, save update in the character_hennas table of the database and send Server=>Client HennaInfo/UserInfo packet to this Player.
	 * @param slot
	 * @param restoreDye
	 * @return
	 */
	public bool removeHenna(int slot, bool restoreDye)
	{
		if ((slot < 1) || (slot > _hennaPoten.Length))
		{
			return false;
		}
		
		Henna henna = _hennaPoten[slot - 1].getHenna();
		if (henna == null)
		{
			return false;
		}
		
		_hennaPoten[slot - 1].setHenna(null);
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(DELETE_CHAR_HENNA);
			statement.setInt(1, getObjectId());
			statement.setInt(2, slot);
			statement.setInt(3, _classIndex);
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed removing character henna: " + e);
		}
		
		// Calculate Henna modifiers of this Player
		recalcHennaStats();
		
		// Send Server=>Client UserInfo packet to this Player
		broadcastUserInfo(UserInfoType.BASE_STATS, UserInfoType.STAT_POINTS, UserInfoType.STAT_ABILITIES, UserInfoType.MAX_HPCPMP, UserInfoType.STATS, UserInfoType.SPEED);
		
		if ((henna.getCancelCount() > 0) && restoreDye)
		{
			long remainingTime = getVariables().getLong("HennaDuration" + slot, 0) - System.currentTimeMillis();
			if ((remainingTime > 0) || (henna.getDuration() < 0))
			{
				_inventory.addItem("Henna", henna.getDyeItemId(), henna.getCancelCount(), this, null);
				SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
				sm.addItemName(henna.getDyeItemId());
				sm.addLong(henna.getCancelCount());
				sendPacket(sm);
			}
		}
		sendPacket(SystemMessageId.PATTERN_WAS_DELETED);
		
		// Remove henna duration task
		if (henna.getDuration() > 0)
		{
			getVariables().remove("HennaDuration" + slot);
			if (_hennaRemoveSchedules.get(slot) != null)
			{
				_hennaRemoveSchedules.get(slot).cancel(false);
				_hennaRemoveSchedules.remove(slot);
			}
		}
		
		// Remove henna skills
		foreach (Skill skill in henna.getSkills())
		{
			removeSkill(skill, false);
		}
		
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_HENNA_REMOVE, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerHennaRemove(this, henna), this);
		}
		
		return true;
	}
	
	/**
	 * Add a Henna to the Player, save update in the character_hennas table of the database and send Server=>Client HennaInfo/UserInfo packet to this Player.
	 * @param slotId
	 * @param henna the henna to add to the player.
	 * @return {@code true} if the henna is added to the player, {@code false} otherwise.
	 */
	public bool addHenna(int slotId, Henna henna)
	{
		if (slotId > getAvailableHennaSlots())
		{
			return false;
		}
		{
			if (_hennaPoten[slotId - 1].getHenna() == null)
			{
				_hennaPoten[slotId - 1].setHenna(henna);
				
				// Calculate Henna modifiers of this Player
				recalcHennaStats();
				
				try 
				{
					using GameServerDbContext ctx = new();
					PreparedStatement statement = con.prepareStatement(ADD_CHAR_HENNA);
					statement.setInt(1, getObjectId());
					statement.setInt(2, henna.getDyeId());
					statement.setInt(3, slotId);
					statement.setInt(4, _classIndex);
					statement.execute();
				}
				catch (Exception e)
				{
					LOGGER.Error("Failed saving character henna: " + e);
				}
				
				// Task for henna duration
				if (henna.getDuration() > 0)
				{
					getVariables().set("HennaDuration" + slotId, System.currentTimeMillis() + (henna.getDuration() * 60000));
					_hennaRemoveSchedules.put(slotId, ThreadPool.schedule(new HennaDurationTask(this, slotId), System.currentTimeMillis() + (henna.getDuration() * 60000)));
				}
				
				// Reward henna skills
				foreach (Skill skill in henna.getSkills())
				{
					if (skill.getLevel() > getSkillLevel(skill.getId()))
					
					{
						addSkill(skill, false);
					}
				}
				
				// Send Server=>Client UserInfo packet to this Player
				broadcastUserInfo(UserInfoType.BASE_STATS, UserInfoType.STAT_ABILITIES, UserInfoType.STAT_POINTS, UserInfoType.MAX_HPCPMP, UserInfoType.STATS, UserInfoType.SPEED);
				
				// Notify to scripts
				if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_HENNA_ADD, this))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnPlayerHennaAdd(this, henna), this);
				}
				
				return true;
			}
		}
		
		return false;
	}
	
	/**
	 * Calculate Henna modifiers of this Player.
	 */
	private void recalcHennaStats()
	{
		_hennaBaseStats.clear();
		foreach (HennaPoten hennaPoten in _hennaPoten)
		{
			Henna henna = hennaPoten.getHenna();
			if (henna == null)
			{
				continue;
			}
			
			foreach (var entry in henna.getBaseStats())
			{
				_hennaBaseStats.merge(entry.Key, entry.Value, int::sum);
			}
		}
	}
	
	private void restoreDyePoten()
	{
		int pos = 0;
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(RESTORE_CHAR_HENNA_POTENS);
			statement.setInt(1, getObjectId());

			{
				ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					_hennaPoten[pos] = new HennaPoten();
					_hennaPoten[pos].setSlotPosition(rset.getInt("slot_position"));
					_hennaPoten[pos].setEnchantLevel(rset.getInt("enchant_level"));
					_hennaPoten[pos].setEnchantExp(rset.getInt("enchant_exp"));
					_hennaPoten[pos].setPotenId(rset.getInt("poten_id"));
					pos++;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed restoring character " + this + " henna potential: " + e);
		}
		
		for (int i = pos; i < 4; i++)
		{
			_hennaPoten[i] = new HennaPoten();
		}
		
		applyDyePotenSkills();
	}
	
	private void storeDyePoten()
	{
		for (int i = 0; i < 4; i++)
		{
			if ((_hennaPoten[i] != null) && (_hennaPoten[i].getSlotPosition() > 0))
			{
				try 
				{
                    using GameServerDbContext ctx = new();
                    					PreparedStatement statement = con.prepareStatement(ADD_CHAR_HENNA_POTENS);
					statement.setInt(1, getObjectId());
					statement.setInt(2, _hennaPoten[i].getSlotPosition());
					statement.setInt(3, _hennaPoten[i].getPotenId());
					statement.setInt(4, _hennaPoten[i].getEnchantLevel());
					statement.setInt(5, _hennaPoten[i].getEnchantExp());
					statement.execute();
				}
				catch (Exception e)
				{
					LOGGER.Error("Failed saving character " + this + " henna potential: " + e);
				}
			}
		}
	}
	
	public void applyDyePotenSkills()
	{
		for (int i = 1; i <= _hennaPoten.Length; i++)
		{
			foreach (int skillId in HennaPatternPotentialData.getInstance().getSkillIdsBySlotId(i))
			{
				removeSkill(skillId);
			}
			
			HennaPoten hennaPoten = _hennaPoten[i - 1];
			if ((hennaPoten != null) && (hennaPoten.getPotenId() > 0) && hennaPoten.isPotentialAvailable() && (hennaPoten.getActiveStep() > 0))
			{
				Skill hennaSkill = HennaPatternPotentialData.getInstance().getPotentialSkill(hennaPoten.getPotenId(), i, hennaPoten.getActiveStep());
				if ((hennaSkill != null) && (hennaSkill.getLevel() > getSkillLevel(hennaSkill.getId())))
				{
					addSkill(hennaSkill, false);
				}
			}
		}
	}
	
	public HennaPoten getHennaPoten(int slot)
	{
		if ((slot < 1) || (slot > _hennaPoten.Length))
		{
			return null;
		}
		
		return _hennaPoten[slot - 1];
	}
	
	/**
	 * @param slot the character inventory henna slot.
	 * @return the Henna of this Player corresponding to the selected slot.
	 */
	public Henna getHenna(int slot)
	{
		if ((slot < 1) || (slot > getAvailableHennaSlots()))
		{
			return null;
		}
		
		HennaPoten poten = _hennaPoten[slot - 1];
		if (poten == null)
		{
			return null;
		}
		
		return poten.getHenna();
	}
	
	/**
	 * @return {@code true} if player has at least 1 henna symbol, {@code false} otherwise.
	 */
	public bool hasHennas()
	{
		foreach (HennaPoten hennaPoten in _hennaPoten)
		{
			Henna henna = hennaPoten.getHenna();
			if (henna != null)
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * @return the henna holder for this player.
	 */
	public HennaPoten[] getHennaPotenList()
	{
		return _hennaPoten;
	}
	
	/**
	 * @param stat
	 * @return the henna bonus of specified base stat
	 */
	public int getHennaValue(BaseStat stat)
	{
		return _hennaBaseStats.getOrDefault(stat, 0);
	}
	
	public int getAvailableHennaSlots()
	{
		return (int) getStat().getValue(Stat.HENNA_SLOTS_AVAILABLE, 4);
	}
	
	public void setDyePotentialDailyStep(int dailyStep)
	{
		getVariables().set(PlayerVariables.DYE_POTENTIAL_DAILY_STEP, dailyStep);
	}
	
	public void setDyePotentialDailyCount(int dailyCount)
	{
		getVariables().set(PlayerVariables.DYE_POTENTIAL_DAILY_COUNT, dailyCount);
	}
	
	public int getDyePotentialDailyStep()
	{
		return getVariables().getInt(PlayerVariables.DYE_POTENTIAL_DAILY_STEP, 1);
	}
	
	public int getDyePotentialDailyCount()
	{
		return getVariables().getInt(PlayerVariables.DYE_POTENTIAL_DAILY_COUNT, 20);
	}
	
	public int getDyePotentialDailyEnchantReset()
	{
		return getVariables().getInt(PlayerVariables.DYE_POTENTIAL_DAILY_COUNT_ENCHANT_RESET, 0);
	}
	
	public void setDyePotentialDailyEnchantReset(int val)
	{
		getVariables().set(PlayerVariables.DYE_POTENTIAL_DAILY_COUNT_ENCHANT_RESET, val);
	}
	
	/**
	 * @return map of all henna base stats bonus
	 */
	public Map<BaseStat, int> getHennaBaseStats()
	{
		return _hennaBaseStats;
	}
	
	/**
	 * Checks if the player has basic property resist towards mesmerizing debuffs.
	 * @return {@code true} if the player has resist towards mesmerizing debuffs, {@code false} otherwise
	 */
	public override bool hasBasicPropertyResist()
	{
		// return isInCategory(CategoryType.SIXTH_CLASS_GROUP);
		return false;
	}
	
	public void autoSave()
	{
		storeMe();
		storeRecommendations();
		
		if (Config.UPDATE_ITEMS_ON_CHAR_STORE)
		{
			getInventory().updateDatabase();
			getWarehouse().updateDatabase();
			getFreight().updateDatabase();
		}
	}
	
	public bool canLogout()
	{
		if (hasItemRequest())
		{
			return false;
		}
		
		if (isSubclassLocked())
		{
			LOGGER.Warn("Player " + getName() + " tried to restart/logout during class change.");
			return false;
		}
		
		if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(this) && !(isGM() && Config.GM_RESTART_FIGHTING))
		{
			return false;
		}
		
		if (isRegisteredOnEvent())
		{
			return false;
		}
		
		return true;
	}
	
	/**
	 * Return True if the Player is autoAttackable.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Check if the attacker isn't the Player Pet</li>
	 * <li>Check if the attacker is Monster</li>
	 * <li>If the attacker is a Player, check if it is not in the same party</li>
	 * <li>Check if the Player has Karma</li>
	 * <li>If the attacker is a Player, check if it is not in the same siege clan (Attacker, Defender)</li>
	 * </ul>
	 */
	public override bool isAutoAttackable(Creature attacker)
	{
		if (attacker == null)
		{
			return false;
		}
		
		// Invisible or untargetable players should not be attackable.
		if (isInvisible() || isAffected(EffectFlag.UNTARGETABLE))
		{
			return false;
		}
		
		// Check if the attacker isn't the Player Pet
		if ((attacker == this) || (attacker == _pet) || attacker.hasServitor(attacker.getObjectId()))
		{
			return false;
		}
		
		// Friendly mobs do not attack players
		if (attacker is FriendlyMob)
		{
			return false;
		}
		
		// Check if the attacker is a Monster
		if (attacker.isMonster())
		{
			return true;
		}
		
		// is AutoAttackable if both players are in the same duel and the duel is still going on
		if (attacker.isPlayable() && (_duelState == Duel.DUELSTATE_DUELLING) && (getDuelId() == attacker.getActingPlayer().getDuelId()))
		{
			return true;
		}
		
		// Check if the attacker is not in the same party. NOTE: Party checks goes before oly checks in order to prevent patry member autoattack at oly.
		if (isInParty() && _party.getMembers().Contains(attacker))
		{
			return false;
		}
		
		// Check if the attacker is in olympia and olympia start
		if (attacker.isPlayer() && attacker.getActingPlayer().isInOlympiadMode())
		{
			return _inOlympiadMode && _olympiadStart && (((Player) attacker).getOlympiadGameId() == getOlympiadGameId());
		}
		
		// Check if the attacker is in an event
		if (isOnEvent())
		{
			return isOnSoloEvent() || (getTeam() != attacker.getTeam());
		}
		
		// Check if the attacker is a Playable
		if (attacker.isPlayable())
		{
			if (isInsideZone(ZoneId.PEACE) || isInsideZone(ZoneId.NO_PVP))
			{
				return false;
			}
			
			// Same Command Channel are friends.
			if (Config.ALT_COMMAND_CHANNEL_FRIENDS && (isInParty() && (getParty().getCommandChannel() != null) && attacker.isInParty() && (attacker.getParty().getCommandChannel() != null) && (getParty().getCommandChannel() == attacker.getParty().getCommandChannel())))
			{
				return false;
			}
			
			// Get Player
			Player attackerPlayer = attacker.getActingPlayer();
			Clan clan = getClan();
			Clan attackerClan = attackerPlayer.getClan();
			if ((clan != null) && (attackerClan != null))
			{
				if (clan != attackerClan)
				{
					Siege siege = SiegeManager.getInstance().getSiege(getX(), getY(), getZ());
					if (siege != null)
					{
						// Check if a siege is in progress and if attacker and the Player aren't in the Defender clan.
						if (siege.checkIsDefender(attackerClan) && siege.checkIsDefender(clan))
						{
							return false;
						}
						
						// Check if a siege is in progress and if attacker and the Player aren't in the Attacker clan.
						if (siege.checkIsAttacker(attackerClan) && siege.checkIsAttacker(clan))
						{
							// If first mid victory is achieved, attackers can attack attackers.
							Castle castle = CastleManager.getInstance().getCastleById(_siegeSide);
							return (castle != null) && castle.isFirstMidVictory();
						}
					}
				}
				
				// Check if clan is at war
				if ((getWantsPeace() == 0) && (attackerPlayer.getWantsPeace() == 0) && !isAcademyMember())
				{
					ClanWar war = attackerClan.getWarWith(getClanId());
					if (war != null)
					{
						ClanWarState warState = war.getState();
						if ((warState == ClanWarState.MUTUAL) || (((warState == ClanWarState.BLOOD_DECLARATION) || (warState == ClanWarState.DECLARATION)) && (war.getAttackerClanId() == clan.getId())))
						{
							return true;
						}
					}
				}
			}
			
			// Check if the Player is in an arena, but NOT siege zone. NOTE: This check comes before clan/ally checks, but after party checks.
			// This is done because in arenas, clan/ally members can autoattack if they arent in party.
			if ((isInsideZone(ZoneId.PVP) && attackerPlayer.isInsideZone(ZoneId.PVP)) && !(isInsideZone(ZoneId.SIEGE) && attackerPlayer.isInsideZone(ZoneId.SIEGE)))
			{
				return true;
			}
			
			// Check if the attacker is not in the same clan
			if ((clan != null) && clan.isMember(attacker.getObjectId()))
			{
				return false;
			}
			
			// Check if the attacker is not in the same ally
			if (attacker.isPlayer() && (getAllyId() != 0) && (getAllyId() == attackerPlayer.getAllyId()))
			{
				return false;
			}
			
			// Now check again if the Player is in pvp zone, but this time at siege PvP zone, applying clan/ally checks
			if (isInsideZone(ZoneId.PVP) && attackerPlayer.isInsideZone(ZoneId.PVP) && isInsideZone(ZoneId.SIEGE) && attackerPlayer.isInsideZone(ZoneId.SIEGE))
			{
				return true;
			}
			
			if (Config.FACTION_SYSTEM_ENABLED && ((isGood() && attackerPlayer.isEvil()) || (isEvil() && attackerPlayer.isGood())))
			{
				return true;
			}
		}
		
		if ((attacker is Defender) && (_clan != null))
		{
			Siege siege = SiegeManager.getInstance().getSiege(this);
			return (siege != null) && siege.checkIsAttacker(_clan);
		}
		
		if (attacker is Guard)
		{
			if (Config.FACTION_SYSTEM_ENABLED && Config.FACTION_GUARDS_ENABLED && ((_isGood && ((Npc) attacker).getTemplate().isClan(Config.FACTION_EVIL_TEAM_NAME)) || (_isEvil && ((Npc) attacker).getTemplate().isClan(Config.FACTION_GOOD_TEAM_NAME))))
			{
				return true;
			}
			return (getReputation() < 0); // Guards attack only PK players.
		}
		
		// Check if the Player has Karma
		if ((getReputation() < 0) || (_pvpFlag > 0))
		{
			return true;
		}
		
		return false;
	}
	
	/**
	 * Check if the active Skill can be casted.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Check if the skill isn't toggle and is offensive</li>
	 * <li>Check if the target is in the skill cast range</li>
	 * <li>Check if the skill is Spoil type and if the target isn't already spoiled</li>
	 * <li>Check if the caster owns enought consummed Item, enough HP and MP to cast the skill</li>
	 * <li>Check if the caster isn't sitting</li>
	 * <li>Check if all skills are enabled and this skill is enabled</li>
	 * <li>Check if the caster own the weapon needed</li>
	 * <li>Check if the skill is active</li>
	 * <li>Check if all casting conditions are completed</li>
	 * <li>Notify the AI with AI_INTENTION_CAST and target</li>
	 * </ul>
	 * @param skill The Skill to use
	 * @param forceUse used to force ATTACK on players
	 * @param dontMove used to prevent movement, if not in range
	 */
	public override bool useMagic(Skill skill, Item item, bool forceUse, bool dontMove)
	{
		Skill usedSkill = skill;
		
		// Passive skills cannot be used.
		if (usedSkill.isPassive())
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// If Alternate rule Karma punishment is set to true, forbid skill Return to player with Karma
		if (!Config.ALT_GAME_KARMA_PLAYER_CAN_TELEPORT && (getReputation() < 0) && usedSkill.hasEffectType(EffectType.TELEPORT))
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// players mounted on pets cannot use any toggle skills
		if (usedSkill.isToggle() && isMounted())
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// Support for wizard skills with stances (Fire, Water, Wind, Earth)
		Skill attachedSkill = usedSkill.getAttachedSkill(this);
		if (attachedSkill != null)
		{
			usedSkill = attachedSkill;
		}
		
		// ************************************* Check Player State *******************************************
		
		// Abnormal effects(ex : Stun, Sleep...) are checked in Creature useMagic()
		if (!usedSkill.canCastWhileDisabled() && (isControlBlocked() || hasBlockActions()))
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// Check if the player is dead
		if (isDead())
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// Check if fishing and trying to use non-fishing skills.
		if (isFishing() && !usedSkill.hasEffectType(EffectType.FISHING, EffectType.FISHING_START))
		{
			sendPacket(SystemMessageId.ONLY_FISHING_SKILLS_MAY_BE_USED_AT_THIS_TIME);
			return false;
		}
		
		if (_observerMode)
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_FUNCTION_IN_THE_SPECTATOR_MODE);
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		if (isSkillDisabled(usedSkill))
		{
			SystemMessage sm;
			if (hasSkillReuse(usedSkill.getReuseHashCode()))
			{
				int remainingTime = (int) (getSkillRemainingReuseTime(usedSkill.getReuseHashCode()) / 1000);
				int hours = remainingTime / 3600;
				int minutes = (remainingTime % 3600) / 60;
				int seconds = (remainingTime % 60);
				if (hours > 0)
				{
					sm = new SystemMessage(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_H_S3_MIN_S4_SEC);
					sm.addSkillName(usedSkill);
					sm.addInt(hours);
					sm.addInt(minutes);
				}
				else if (minutes > 0)
				{
					sm = new SystemMessage(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_MIN_S3_SEC);
					sm.addSkillName(usedSkill);
					sm.addInt(minutes);
				}
				else
				{
					sm = new SystemMessage(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_SEC);
					sm.addSkillName(usedSkill);
				}
				
				sm.addInt(seconds);
			}
			else
			{
				sm = new SystemMessage(SystemMessageId.S1_IS_NOT_AVAILABLE_AT_THIS_TIME_BEING_PREPARED_FOR_REUSE);
				sm.addSkillName(usedSkill);
			}
			
			sendPacket(sm);
			return false;
		}
		
		// Check if the caster is sitting
		if (_waitTypeSitting)
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_ACTIONS_AND_SKILLS_WHILE_THE_CHARACTER_IS_SITTING);
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// Check if the skill type is toggle and disable it, unless the toggle is necessary to be on.
		if (usedSkill.isToggle())
		{
			if (isAffectedBySkill(usedSkill.getId()))
			{
				if (!usedSkill.isNecessaryToggle())
				{
					stopSkillEffects(SkillFinishType.REMOVED, usedSkill.getId());
				}
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
			else if (usedSkill.getToggleGroupId() > 0)
			{
				getEffectList().stopAllTogglesOfGroup(usedSkill.getToggleGroupId());
			}
		}
		
		// Check if the player uses "Fake Death" skill
		// Note: do not check this before TOGGLE reset
		if (isFakeDeath())
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// ************************************* Check Target *******************************************
		// Create and set a WorldObject containing the target of the skill
		WorldObject target = usedSkill.getTarget(this, forceUse, dontMove, true);
		Location worldPosition = _currentSkillWorldPosition;
		if ((usedSkill.getTargetType() == TargetType.GROUND) && (worldPosition == null))
		{
			if (usedSkill.getAffectScope() == AffectScope.FAN_PB)
			{
				if (isInsideZone(ZoneId.PEACE))
				{
					sendPacket(SystemMessageId.YOU_CANNOT_ATTACK_IN_A_PEACEFUL_ZONE);
				}
				else if (getCurrentMp() < usedSkill.getMpConsume())
				{
					sendPacket(SystemMessageId.NOT_ENOUGH_MP);
				}
				else if (usedSkill.checkCondition(this, target, true))
				{
					sendPacket(new MagicSkillUse(this, this, usedSkill.getDisplayId(), usedSkill.getDisplayLevel(), 0, 0, usedSkill.getReuseDelayGroup(), -1, SkillCastingType.NORMAL, true));
				}
			}
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// Check the validity of the target
		if (target == null)
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// Check if all casting conditions are completed
		if (!usedSkill.checkCondition(this, target, true))
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			
			// Upon failed conditions, next action is called.
			if ((usedSkill.getNextAction() != NextActionType.NONE) && (target != this) && target.isAutoAttackable(this))
			{
				CreatureAI.IntentionCommand nextIntention = getAI().getNextIntention();
				if ((nextIntention == null) || (nextIntention.getCtrlIntention() != CtrlIntention.AI_INTENTION_MOVE_TO))
				{
					if (usedSkill.getNextAction() == NextActionType.ATTACK)
					{
						getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
					}
					else if (usedSkill.getNextAction() == NextActionType.CAST)
					{
						getAI().setIntention(CtrlIntention.AI_INTENTION_CAST, usedSkill, target, item, false, false);
					}
				}
			}
			
			return false;
		}
		
		bool doubleCast = isAffected(EffectFlag.DOUBLE_CAST) && usedSkill.canDoubleCast();
		
		// If a skill is currently being used, queue this one if this is not the same
		// In case of double casting, check if both slots are occupied, then queue skill.
		if ((!doubleCast && (isAttackingNow() || isCastingNow(x => x.isAnyNormalType()))) || (isCastingNow(s => s.getCastingType() == SkillCastingType.NORMAL) && isCastingNow(s => s.getCastingType() == SkillCastingType.NORMAL_SECOND)))
		{
			// Do not queue skill if called by an item.
			if (item == null)
			{
				// Create a new SkillUseHolder object and queue it in the player _queuedSkill
				setQueuedSkill(usedSkill, item, forceUse, dontMove);
			}
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		if (_queuedSkill != null)
		{
			setQueuedSkill(null, null, false, false);
		}
		
		// Notify the AI with AI_INTENTION_CAST and target
		getAI().setIntention(CtrlIntention.AI_INTENTION_CAST, usedSkill, target, item, forceUse, dontMove);
		return true;
	}
	
	public bool isInLooterParty(int looterId)
	{
		Player looter = World.getInstance().getPlayer(looterId);
		
		// if Player is in a CommandChannel
		if (isInParty() && _party.isInCommandChannel() && (looter != null))
		{
			return _party.getCommandChannel().getMembers().Contains(looter);
		}
		
		if (isInParty() && (looter != null))
		{
			return _party.getMembers().Contains(looter);
		}
		
		return false;
	}
	
	/**
	 * @return True if the Player is a Mage.
	 */
	public bool isMageClass()
	{
		return getClassId().isMage();
	}
	
	/**
	 * @return True if the Player is a Death Knight.
	 */
	public bool isDeathKnight()
	{
		return _isDeathKnight;
	}
	
	public void setDeathKnight(bool value)
	{
		_isDeathKnight = value;
	}
	
	/**
	 * @return True if the Player is a Vanguard.
	 */
	public bool isVanguard()
	{
		return _isVanguard;
	}
	
	public void setVanguard(bool value)
	{
		_isVanguard = value;
	}
	
	/**
	 * @return True if the Player is a Assassin.
	 */
	public bool isAssassin()
	{
		return _isAssassin;
	}
	
	public void setAssassin(bool value)
	{
		_isAssassin = value;
	}
	
	public bool isMounted()
	{
		return _mountType != MountType.NONE;
	}
	
	public bool checkLandingState()
	{
		// Check if char is in a no landing zone
		if (isInsideZone(ZoneId.NO_LANDING))
		{
			return true;
		}
		else
		// if this is a castle that is currently being sieged, and the rider is NOT a castle owner
		// he cannot land.
		// castle owner is the leader of the clan that owns the castle where the pc is
		if (isInsideZone(ZoneId.SIEGE) && !((getClan() != null) && (CastleManager.getInstance().getCastle(this) == CastleManager.getInstance().getCastleByOwner(getClan())) && (this == getClan().getLeader().getPlayer())))
		{
			return true;
		}
		return false;
	}
	
	// returns false if the change of mount type fails.
	public void setMount(int npcId, int npcLevel)
	{
		MountType type = MountType.findByNpcId(npcId);
		switch (type)
		{
			case MountType.NONE: // None
			{
				setFlying(false);
				break;
			}
			case MountType.STRIDER: // Strider
			{
				if (isNoble())
				{
					addSkill((int)CommonSkill.STRIDER_SIEGE_ASSAULT, false);
				}
				break;
			}
			case MountType.WYVERN: // Wyvern
			{
				setFlying(true);
				break;
			}
		}
		
		_mountType = type;
		_mountNpcId = npcId;
		_mountLevel = npcLevel;
	}
	
	/**
	 * @return the type of Pet mounted (0 : none, 1 : Strider, 2 : Wyvern, 3: Wolf).
	 */
	public MountType getMountType()
	{
		return _mountType;
	}
	
	public override void stopAllEffects()
	{
		base.stopAllEffects();
		updateAndBroadcastStatus();
	}
	
	public override void stopAllEffectsExceptThoseThatLastThroughDeath()
	{
		base.stopAllEffectsExceptThoseThatLastThroughDeath();
		updateAndBroadcastStatus();
	}
	
	public void stopCubics()
	{
		if (!_cubics.isEmpty())
		{
			_cubics.values().forEach(x => x.deactivate());
			_cubics.clear();
		}
	}
	
	public void stopCubicsByOthers()
	{
		if (!_cubics.isEmpty())
		{
			bool broadcast = false;
			foreach (Cubic cubic in _cubics.values())
			{
				if (cubic.isGivenByOther())
				{
					cubic.deactivate();
					_cubics.remove(cubic.getTemplate().getId());
					broadcast = true;
				}
			}
			if (broadcast)
			{
				sendPacket(new ExUserInfoCubic(this));
				broadcastUserInfo();
			}
		}
	}
	
	/**
	 * Send a Server=>Client packet ExUserInfoAbnormalVisualEffect to this Player and broadcast char info.<br>
	 */
	public override void updateAbnormalVisualEffects()
	{
		if (_abnormalVisualEffectTask == null)
		{
			_abnormalVisualEffectTask = ThreadPool.schedule(() =>
			{
				sendPacket(new ExUserInfoAbnormalVisualEffect(this));
				broadcastCharInfo();
				_abnormalVisualEffectTask = null;
			}, 50);
		}
	}
	
	/**
	 * Disable the Inventory and create a new task to enable it after 1.5s.
	 * @param value
	 */
	public void setInventoryBlockingStatus(bool value)
	{
		_inventoryDisable = value;
		if (value)
		{
			ThreadPool.schedule(new InventoryEnableTask(this), 1500);
		}
	}
	
	/**
	 * @return True if the Inventory is disabled.
	 */
	public bool isInventoryDisabled()
	{
		return _inventoryDisable;
	}
	
	/**
	 * Add a cubic to this player.
	 * @param cubic
	 * @return the old cubic for this cubic ID if any, otherwise {@code null}
	 */
	public Cubic addCubic(Cubic cubic)
	{
		return _cubics.put(cubic.getTemplate().getId(), cubic);
	}
	
	/**
	 * Get the player's cubics.
	 * @return the cubics
	 */
	public Map<int, Cubic> getCubics()
	{
		return _cubics;
	}
	
	/**
	 * Get the player cubic by cubic ID, if any.
	 * @param cubicId the cubic ID
	 * @return the cubic with the given cubic ID, {@code null} otherwise
	 */
	public Cubic getCubicById(int cubicId)
	{
		return _cubics.get(cubicId);
	}
	
	/**
	 * @return the modifier corresponding to the Enchant Effect of the Active Weapon (Min : 127).
	 */
	public int getEnchantEffect()
	{
		Item wpn = getActiveWeaponInstance();
		if (wpn == null)
		{
			return 0;
		}
		return Math.Min(127, wpn.getEnchantLevel());
	}
	
	/**
	 * Set the _lastFolkNpc of the Player corresponding to the last Folk wich one the player talked.
	 * @param folkNpc
	 */
	public void setLastFolkNPC(Npc folkNpc)
	{
		_lastFolkNpc = folkNpc;
	}
	
	/**
	 * @return the _lastFolkNpc of the Player corresponding to the last Folk wich one the player talked.
	 */
	public Npc getLastFolkNPC()
	{
		return _lastFolkNpc;
	}
	
	public void addAutoSoulShot(int itemId)
	{
		_activeSoulShots.add(itemId);
	}
	
	public bool removeAutoSoulShot(int itemId)
	{
		return _activeSoulShots.remove(itemId);
	}
	
	public Set<int> getAutoSoulShot()
	{
		return _activeSoulShots;
	}
	
	public override void rechargeShots(bool physical, bool magic, bool fish)
	{
		foreach (int itemId in _activeSoulShots)
		{
			Item item = _inventory.getItemByItemId(itemId);
			if (item == null)
			{
				removeAutoSoulShot(itemId);
				continue;
			}
			
			IItemHandler handler = ItemHandler.getInstance().getHandler(item.getEtcItem());
			if (handler == null)
			{
				continue;
			}
			
			ActionType defaultAction = item.getTemplate().getDefaultAction();
			if ((magic && (defaultAction == ActionType.SPIRITSHOT)) || (physical && (defaultAction == ActionType.SOULSHOT)) || (fish && (defaultAction == ActionType.FISHINGSHOT)))
			{
				handler.useItem(this, item, false);
			}
		}
	}
	
	/**
	 * Cancel autoshot for all shots matching crystaltype {@link ItemTemplate#getCrystalType()}.
	 * @param crystalType int type to disable
	 */
	public void disableAutoShotByCrystalType(int crystalType)
	{
		foreach (int itemId in _activeSoulShots)
		{
			if (ItemData.getInstance().getTemplate(itemId).getCrystalType().getLevel() == crystalType)
			{
				disableAutoShot(itemId);
			}
		}
	}
	
	/**
	 * Cancel autoshot use for shot itemId
	 * @param itemId int id to disable
	 * @return true if canceled.
	 */
	public bool disableAutoShot(int itemId)
	{
		if (_activeSoulShots.Contains(itemId))
		{
			removeAutoSoulShot(itemId);
			sendPacket(new ExAutoSoulShot(itemId, false, 0));
			
			SystemMessage sm = new SystemMessage(SystemMessageId.THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_DEACTIVATED);
			sm.addItemName(itemId);
			sendPacket(sm);
			return true;
		}
		return false;
	}
	
	/**
	 * Cancel all autoshots for player
	 */
	public void disableAutoShotsAll()
	{
		foreach (int itemId in _activeSoulShots)
		{
			sendPacket(new ExAutoSoulShot(itemId, false, 0));
			SystemMessage sm = new SystemMessage(SystemMessageId.THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_DEACTIVATED);
			sm.addItemName(itemId);
			sendPacket(sm);
		}
		_activeSoulShots.clear();
	}
	
	public BroochJewel getActiveRubyJewel()
	{
		return _activeRubyJewel;
	}
	
	public void setActiveRubyJewel(BroochJewel jewel)
	{
		_activeRubyJewel = jewel;
	}
	
	public BroochJewel getActiveShappireJewel()
	{
		return _activeShappireJewel;
	}
	
	public void setActiveShappireJewel(BroochJewel jewel)
	{
		_activeShappireJewel = jewel;
	}
	
	public void updateActiveBroochJewel()
	{
		BroochJewel[] broochJewels = Enum.GetValues<BroochJewel>();
		// Update active Ruby jewel.
		setActiveRubyJewel(null);
		for (int i = broochJewels.Length - 1; i > 0; i--)
		{
			BroochJewel jewel = broochJewels[i];
			if (jewel.isRuby() && _inventory.isItemEquipped(jewel.getItemId()))
			{
				setActiveRubyJewel(jewel);
				break;
			}
		}
		// Update active Sapphire jewel.
		setActiveShappireJewel(null);
		for (int i = broochJewels.Length - 1; i > 0; i--)
		{
			BroochJewel jewel = broochJewels[i];
			if (jewel.isSapphire() && _inventory.isItemEquipped(jewel.getItemId()))
			{
				setActiveShappireJewel(jewel);
				break;
			}
		}
	}
	
	public EnumIntBitmask<ClanPrivilege> getClanPrivileges()
	{
		return _clanPrivileges;
	}
	
	public void setClanPrivileges(EnumIntBitmask<ClanPrivilege> clanPrivileges)
	{
		_clanPrivileges = clanPrivileges.clone();
	}
	
	public bool hasClanPrivilege(ClanPrivilege privilege)
	{
		return _clanPrivileges.has(privilege);
	}
	
	// baron etc
	public void setPledgeClass(int classId)
	{
		_pledgeClass = classId;
		checkItemRestriction();
	}
	
	public int getPledgeClass()
	{
		return _pledgeClass;
	}
	
	public void setPledgeType(int typeId)
	{
		_pledgeType = typeId;
	}
	
	public override int getPledgeType()
	{
		return _pledgeType;
	}
	
	public int getApprentice()
	{
		return _apprentice;
	}
	
	public void setApprentice(int apprenticeId)
	{
		_apprentice = apprenticeId;
	}
	
	public int getSponsor()
	{
		return _sponsor;
	}
	
	public void setSponsor(int sponsorId)
	{
		_sponsor = sponsorId;
	}
	
	public int getBookMarkSlot()
	{
		return _bookmarkslot;
	}
	
	public void setBookMarkSlot(int slot)
	{
		_bookmarkslot = slot;
		sendPacket(new ExGetBookMarkInfoPacket(this));
	}
	
	public override void sendMessage(String message)
	{
		sendPacket(new SystemMessage(SendMessageLocalisationData.getLocalisation(this, message)));
	}
	
	public void setObserving(bool value)
	{
		_observerMode = value;
		setTarget(null);
		setBlockActions(value);
		setInvul(value);
		setInvisible(value);
		if (hasAI() && !value)
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		}
	}
	
	public void enterObserverMode(Location loc)
	{
		setLastLocation();
		
		// Remove Hide.
		getEffectList().stopEffects(AbnormalType.HIDE);
		
		setObserving(true);
		sendPacket(new ObservationMode(loc));
		teleToLocation(loc, false);
		broadcastUserInfo();
	}
	
	public void setLastLocation()
	{
		_lastLoc = new Location(getX(), getY(), getZ());
	}
	
	public void unsetLastLocation()
	{
		_lastLoc = null;
	}
	
	public void enterOlympiadObserverMode(Location loc, int id)
	{
		if (_pet != null)
		{
			_pet.unSummon(this);
		}
		
		if (hasServitors())
		{
			getServitors().values().forEach(s => s.unSummon(this));
		}
		
		// Remove Hide.
		getEffectList().stopEffects(AbnormalType.HIDE);
		
		if (!_cubics.isEmpty())
		{
			_cubics.values().forEach(x => x.deactivate());
			_cubics.clear();
			sendPacket(new ExUserInfoCubic(this));
		}
		
		if (_party != null)
		{
			_party.removePartyMember(this, PartyMessageType.EXPELLED);
		}
		
		_olympiadGameId = id;
		if (_waitTypeSitting)
		{
			standUp();
		}
		if (!_observerMode)
		{
			setLastLocation();
		}
		
		_observerMode = true;
		setTarget(null);
		setInvul(true);
		setInvisible(true);
		setInstance(OlympiadGameManager.getInstance().getOlympiadTask(id).getStadium().getInstance());
		teleToLocation(loc, false);
		sendPacket(new ExOlympiadMode(3));
		broadcastUserInfo();
	}
	
	public void leaveObserverMode()
	{
		setTarget(null);
		setInstance(null);
		teleToLocation(_lastLoc, false);
		unsetLastLocation();
		sendPacket(new ObservationReturn(getLocation()));
		setBlockActions(false);
		if (!isGM())
		{
			setInvisible(false);
			setInvul(false);
		}
		if (hasAI())
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		}
		
		setFalling(); // prevent receive falling damage
		_observerMode = false;
		broadcastUserInfo();
	}
	
	public void leaveOlympiadObserverMode()
	{
		if (_olympiadGameId == -1)
		{
			return;
		}
		_olympiadGameId = -1;
		_observerMode = false;
		setTarget(null);
		sendPacket(new ExOlympiadMode(0));
		setInstance(null);
		teleToLocation(_lastLoc, true);
		if (!isGM())
		{
			setInvisible(false);
			setInvul(false);
		}
		if (hasAI())
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		}
		unsetLastLocation();
		broadcastUserInfo();
	}
	
	public void setOlympiadSide(int i)
	{
		_olympiadSide = i;
	}
	
	public int getOlympiadSide()
	{
		return _olympiadSide;
	}
	
	public void setOlympiadGameId(int id)
	{
		_olympiadGameId = id;
	}
	
	public int getOlympiadGameId()
	{
		return _olympiadGameId;
	}
	
	public Location getLastLocation()
	{
		return _lastLoc;
	}
	
	public bool inObserverMode()
	{
		return _observerMode;
	}
	
	public AdminTeleportType getTeleMode()
	{
		return _teleportType;
	}
	
	public void setTeleMode(AdminTeleportType type)
	{
		_teleportType = type;
	}
	
	public void setRaceTicket(int i, int value)
	{
		_raceTickets[i] = value;
	}
	
	public int getRaceTicket(int i)
	{
		return _raceTickets[i];
	}
	
	public bool getMessageRefusal()
	{
		return _messageRefusal;
	}
	
	public void setMessageRefusal(bool mode)
	{
		_messageRefusal = mode;
		sendPacket(new EtcStatusUpdate(this));
	}
	
	public void setDietMode(bool mode)
	{
		_dietMode = mode;
	}
	
	public bool getDietMode()
	{
		return _dietMode;
	}
	
	public void setTradeRefusal(bool mode)
	{
		_tradeRefusal = mode;
	}
	
	public bool getTradeRefusal()
	{
		return _tradeRefusal;
	}
	
	public void setExchangeRefusal(bool mode)
	{
		_exchangeRefusal = mode;
	}
	
	public bool getExchangeRefusal()
	{
		return _exchangeRefusal;
	}
	
	public BlockList getBlockList()
	{
		return _blockList;
	}
	
	/**
	 * @param player
	 * @return returns {@code true} if player is current player cannot accepting messages from the target player, {@code false} otherwise
	 */
	public bool isBlocking(Player player)
	{
		return _blockList.isBlockAll() || _blockList.isInBlockList(player);
	}
	
	/**
	 * @param player
	 * @return returns {@code true} if player is current player can accepting messages from the target player, {@code false} otherwise
	 */
	public bool isNotBlocking(Player player)
	{
		return !_blockList.isBlockAll() && !_blockList.isInBlockList(player);
	}
	
	/**
	 * @param player
	 * @return returns {@code true} if player is target player cannot accepting messages from the current player, {@code false} otherwise
	 */
	public bool isBlocked(Player player)
	{
		return player.getBlockList().isBlockAll() || player.getBlockList().isInBlockList(this);
	}
	
	/**
	 * @param player
	 * @return returns {@code true} if player is target player can accepting messages from the current player, {@code false} otherwise
	 */
	public bool isNotBlocked(Player player)
	{
		return !player.getBlockList().isBlockAll() && !player.getBlockList().isInBlockList(this);
	}
	
	public void setHero(bool hero)
	{
		if (hero && (_baseClass == _activeClass))
		{
			foreach (Skill skill in SkillTreeData.getInstance().getHeroSkillTree())
			{
				addSkill(skill, false); // Don't persist hero skills into database
			}
		}
		else
		{
			foreach (Skill skill in SkillTreeData.getInstance().getHeroSkillTree())
			{
				removeSkill(skill, false, true); // Just remove skills from non-hero players
			}
		}
		_hero = hero;
		sendSkillList();
	}
	
	public void setInOlympiadMode(bool value)
	{
		_inOlympiadMode = value;
	}
	
	public void setOlympiadStart(bool value)
	{
		_olympiadStart = value;
	}
	
	public bool isOlympiadStart()
	{
		return _olympiadStart;
	}
	
	public bool isHero()
	{
		return _hero;
	}
	
	public bool isInOlympiadMode()
	{
		return _inOlympiadMode;
	}
	
	public bool isInDuel()
	{
		return _isInDuel;
	}
	
	public void setStartingDuel()
	{
		_startingDuel = true;
	}
	
	public int getDuelId()
	{
		return _duelId;
	}
	
	public void setDuelState(int mode)
	{
		_duelState = mode;
	}
	
	public int getDuelState()
	{
		return _duelState;
	}
	
	/**
	 * Sets up the duel state using a non 0 duelId.
	 * @param duelId 0=not in a duel
	 */
	public void setInDuel(int duelId)
	{
		if (duelId > 0)
		{
			_isInDuel = true;
			_duelState = Duel.DUELSTATE_DUELLING;
			_duelId = duelId;
		}
		else
		{
			if (_duelState == Duel.DUELSTATE_DEAD)
			{
				enableAllSkills();
				getStatus().startHpMpRegeneration();
			}
			_isInDuel = false;
			_duelState = Duel.DUELSTATE_NODUEL;
			_duelId = 0;
		}
		_startingDuel = false;
	}
	
	/**
	 * This returns a SystemMessage stating why the player is not available for duelling.
	 * @return S1_CANNOT_DUEL... message
	 */
	public SystemMessage getNoDuelReason()
	{
		SystemMessage sm = new SystemMessage(_noDuelReason);
		sm.addPcName(this);
		_noDuelReason = SystemMessageId.THERE_IS_NO_OPPONENT_TO_RECEIVE_YOUR_CHALLENGE_FOR_A_DUEL;
		return sm;
	}
	
	/**
	 * Checks if this player might join / start a duel.<br>
	 * To get the reason use getNoDuelReason() after calling this function.
	 * @return true if the player might join/start a duel.
	 */
	public bool canDuel()
	{
		if (isInCombat() || isJailed())
		{
			_noDuelReason = SystemMessageId.C1_CANNOT_DUEL_AS_THEY_ARE_CURRENTLY_IN_BATTLE;
			return false;
		}
		if (isDead() || isAlikeDead() || ((getCurrentHp() < (getMaxHp() / 2)) || (getCurrentMp() < (getMaxMp() / 2))))
		{
			_noDuelReason = SystemMessageId.C1_CANNOT_DUEL_AS_THEIR_HP_MP_50;
			return false;
		}
		if (_isInDuel || _startingDuel)
		{
			_noDuelReason = SystemMessageId.C1_IS_ALREADY_IN_A_DUEL;
			return false;
		}
		if (_inOlympiadMode)
		{
			_noDuelReason = SystemMessageId.C1_IS_PARTICIPATING_IN_THE_OLYMPIAD_OR_THE_CEREMONY_OF_CHAOS_AND_THEREFORE_CANNOT_DUEL;
			return false;
		}
		if (isOnEvent())
		{
			_noDuelReason = SystemMessageId.C1_CANNOT_DUEL_AS_THEY_ARE_CURRENTLY_IN_BATTLE;
			return false;
		}
		if (isCursedWeaponEquipped())
		{
			_noDuelReason = SystemMessageId.C1_IS_IN_A_CHAOTIC_OR_PURPLE_STATE_AND_CANNOT_PARTICIPATE_IN_A_DUEL;
			return false;
		}
		if (_privateStoreType != PrivateStoreType.NONE)
		{
			_noDuelReason = SystemMessageId.C1_CANNOT_DUEL_AS_THEY_ARE_CURRENTLY_IN_A_PRIVATE_STORE_OR_MANUFACTURE;
			return false;
		}
		if (isMounted() || isInBoat())
		{
			_noDuelReason = SystemMessageId.C1_IS_RIDING_A_BOAT_FENRIR_OR_STRIDER_AND_THEREFORE_CANNOT_DUEL;
			return false;
		}
		if (isFishing())
		{
			_noDuelReason = SystemMessageId.C1_CANNOT_DUEL_AS_THEY_ARE_CURRENTLY_FISHING;
			return false;
		}
		if (isInsideZone(ZoneId.PVP) || isInsideZone(ZoneId.PEACE) || isInsideZone(ZoneId.SIEGE) || isInsideZone(ZoneId.NO_PVP))
		{
			_noDuelReason = SystemMessageId.C1_IS_IN_AN_AREA_WHERE_DUEL_IS_NOT_ALLOWED_AND_YOU_CANNOT_APPLY_FOR_A_DUEL;
			return false;
		}
		return true;
	}
	
	public bool isNoble()
	{
		return _noble;
	}
	
	public void setNoble(bool value)
	{
		if (value)
		{
			SkillTreeData.getInstance().getNobleSkillAutoGetTree().forEach(skill => addSkill(skill, false));
		}
		else
		{
			SkillTreeData.getInstance().getNobleSkillTree().forEach(skill => removeSkill(skill, false, true));
		}
		_noble = value;
		sendSkillList();
	}
	
	public void setLvlJoinedAcademy(int lvl)
	{
		_lvlJoinedAcademy = lvl;
	}
	
	public int getLvlJoinedAcademy()
	{
		return _lvlJoinedAcademy;
	}
	
	public override bool isAcademyMember()
	{
		return _lvlJoinedAcademy > 0;
	}
	
	public override void setTeam(Team team)
	{
		base.setTeam(team);
		broadcastUserInfo();
		if ((Config.BLUE_TEAM_ABNORMAL_EFFECT != null) || (Config.RED_TEAM_ABNORMAL_EFFECT != null))
		{
			sendPacket(new ExUserInfoAbnormalVisualEffect(this));
		}
		if (_pet != null)
		{
			_pet.broadcastStatusUpdate();
		}
		if (hasServitors())
		{
			getServitors().values().forEach(x => x.broadcastStatusUpdate());
		}
	}
	
	public void setWantsPeace(int wantsPeace)
	{
		_wantsPeace = wantsPeace;
	}
	
	public int getWantsPeace()
	{
		return _wantsPeace;
	}
	
	public void sendSkillList()
	{
		sendSkillList(0);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void sendSkillList(int lastLearnedSkillId)
	{
		if (_skillListTask == null)
		{
			_skillListTask = ThreadPool.schedule(() =>
			{
				bool isDisabled = false;
				SkillList skillList = new SkillList();
				foreach (Skill skill in getSkillList())
				{
					if (_clan != null)
					{
						isDisabled = skill.isClanSkill() && (_clan.getReputationScore() < 0);
					}
					
					// Consider skill replacements.
					int originalSkillId = getOriginalSkill(skill.getId());
					if (originalSkillId != skill.getDisplayId())
					{
						Skill originalSkill = SkillData.getInstance().getSkill(originalSkillId, skill.getLevel(), skill.getSubLevel());
						skillList.addSkill(originalSkill.getDisplayId(), originalSkill.getReuseDelayGroup(), originalSkill.getDisplayLevel(), originalSkill.getSubLevel(), originalSkill.isPassive(), isDisabled, originalSkill.isEnchantable());
					}
					else
					{
						skillList.addSkill(skill.getDisplayId(), skill.getReuseDelayGroup(), skill.getDisplayLevel(), skill.getSubLevel(), skill.isPassive(), isDisabled, skill.isEnchantable());
					}
				}
				if (lastLearnedSkillId > 0)
				{
					skillList.setLastLearnedSkillId(lastLearnedSkillId);
				}
				
				sendPacket(skillList);
				sendPacket(new AcquireSkillList(this));
				restoreAutoShortcutVisual();
				_skillListTask = null;
			}, 300);
		}
	}
	
	public void sendStorageMaxCount()
	{
		if (_storageCountTask == null)
		{
			_storageCountTask = ThreadPool.schedule(() =>
			{
				sendPacket(new ExStorageMaxCount(this));
				_storageCountTask = null;
			}, 300);
		}
	}
	
	public void sendUserBoostStat()
	{
		if (_userBoostStatTask == null)
		{
			_userBoostStatTask = ThreadPool.schedule(() =>
			{
				sendPacket(new ExUserBoostStat(this, BonusExpType.VITALITY));
				sendPacket(new ExUserBoostStat(this, BonusExpType.BUFFS));
				sendPacket(new ExUserBoostStat(this, BonusExpType.PASSIVE));
				if (Config.ENABLE_VITALITY)
				{
					sendPacket(new ExVitalityEffectInfo(this));
				}
				_userBoostStatTask = null;
			}, 300);
		}
	}
	
	/**
	 * 1. Add the specified class ID as a subclass (up to the maximum number of <b>three</b>) for this character.<br>
	 * 2. This method no longer changes the active _classIndex of the player. This is only done by the calling of setActiveClass() method as that should be the only way to do so.
	 * @param classId
	 * @param classIndex
	 * @param isDualClass
	 * @return bool subclassAdded
	 */
	public bool addSubClass(int classId, int classIndex, bool isDualClass)
	{
		if (_subclassLock)
		{
			return false;
		}
		_subclassLock = true;
		
		try
		{
			if ((getTotalSubClasses() == Config.MAX_SUBCLASS) || (classIndex == 0))
			{
				return false;
			}
			
			if (getSubClasses().containsKey(classIndex))
			{
				return false;
			}
			
			// Note: Never change _classIndex in any method other than setActiveClass().
			
			SubClassHolder newClass = new SubClassHolder();
			newClass.setClassId(classId);
			newClass.setClassIndex(classIndex);
			newClass.setVitalityPoints(PlayerStat.MAX_VITALITY_POINTS);
			if (isDualClass)
			{
				newClass.setDualClassActive(true);
				newClass.setExp(ExperienceData.getInstance().getExpForLevel(Config.BASE_DUALCLASS_LEVEL));
				newClass.setLevel(Config.BASE_DUALCLASS_LEVEL);
			}
			
			try 
			{
                using GameServerDbContext ctx = new();
                				PreparedStatement statement = con.prepareStatement(ADD_CHAR_SUBCLASS);
				// Store the basic info about this new sub-class.
				statement.setInt(1, getObjectId());
				statement.setInt(2, newClass.getClassId());
				statement.setLong(3, newClass.getExp());
				statement.setLong(4, newClass.getSp());
				statement.setInt(5, newClass.getLevel());
				statement.setInt(6, newClass.getVitalityPoints());
				statement.setInt(7, newClass.getClassIndex());
				statement.setBoolean(8, newClass.isDualClass());
				statement.execute();
			}
			catch (Exception e)
			{
				LOGGER.Warn("WARNING: Could not add character sub class for " + getName() + ": " + e);
				return false;
			}
			
			// Commit after database INSERT incase exception is thrown.
			getSubClasses().put(newClass.getClassIndex(), newClass);
			
			ClassId subTemplate = ClassId.getClassId(classId);
			Map<long, SkillLearn> skillTree = SkillTreeData.getInstance().getCompleteClassSkillTree(subTemplate);
			Map<int, Skill> prevSkillList = new();
			foreach (SkillLearn skillInfo in skillTree.values())
			{
				if ((skillInfo.getSkillId() == (int)CommonSkill.DIVINE_INSPIRATION) && !Config.AUTO_LEARN_DIVINE_INSPIRATION)
				{
					continue;
				}
				
				if (skillInfo.getGetLevel() <= newClass.getLevel())
				{
					Skill prevSkill = prevSkillList.get(skillInfo.getSkillId());
					Skill newSkill = SkillData.getInstance().getSkill(skillInfo.getSkillId(), skillInfo.getSkillLevel());
					if (((prevSkill != null) && (prevSkill.getLevel() > newSkill.getLevel())) || SkillTreeData.getInstance().isRemoveSkill(subTemplate, skillInfo.getSkillId()))
					{
						continue;
					}
					
					prevSkillList.put(newSkill.getId(), newSkill);
					storeSkill(newSkill, prevSkill, classIndex);
				}
			}
			return true;
		}
		finally
		{
			_subclassLock = false;
			getStat().recalculateStats(false);
			updateAbnormalVisualEffects();
			sendSkillList();
		}
	}
	
	/**
	 * 1. Completely erase all existance of the subClass linked to the classIndex.<br>
	 * 2. Send over the newClassId to addSubClass() to create a new instance on this classIndex.<br>
	 * 3. Upon Exception, revert the player to their BaseClass to avoid further problems.
	 * @param classIndex the class index to delete
	 * @param newClassId the new class Id
	 * @param isDualClass is subclass dualclass
	 * @return {@code true} if the sub-class was modified, {@code false} otherwise
	 */
	public bool modifySubClass(int classIndex, int newClassId, bool isDualClass)
	{
		// Notify to scripts before class is removed.
		if (!getSubClasses().isEmpty() && EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_PROFESSION_CANCEL, this))
		{
			int classId = getSubClasses().get(classIndex).getClassId();
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerProfessionCancel(this, classId), this);
		}
		
		SubClassHolder subClass = getSubClasses().get(classIndex);
		if (subClass == null)
		{
			return false;
		}
		
		if (subClass.isDualClass())
		{
			getVariables().remove(PlayerVariables.ABILITY_POINTS_DUAL_CLASS);
			getVariables().remove(PlayerVariables.ABILITY_POINTS_USED_DUAL_CLASS);
			int revelationSkill = getVariables().getInt(PlayerVariables.REVELATION_SKILL_1_DUAL_CLASS, 0);
			if (revelationSkill != 0)
			{
				removeSkill(revelationSkill);
			}
			revelationSkill = getVariables().getInt(PlayerVariables.REVELATION_SKILL_2_DUAL_CLASS, 0);
			if (revelationSkill != 0)
			{
				removeSkill(revelationSkill);
			}
		}
		
		// Remove after stats are recalculated.
		getSubClasses().remove(classIndex);
		
		try
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement deleteHennas = con.prepareStatement(DELETE_CHAR_HENNA);
            			PreparedStatement deleteShortcuts = con.prepareStatement(DELETE_CHAR_SHORTCUTS);
            			PreparedStatement deleteSkillReuse = con.prepareStatement(DELETE_SKILL_SAVE);
            			PreparedStatement deleteSkills = con.prepareStatement(DELETE_CHAR_SKILLS);
            			PreparedStatement deleteSubclass = con.prepareStatement(DELETE_CHAR_SUBCLASS);
            
			// Remove all henna info stored for this sub-class.
			deleteHennas.setInt(1, getObjectId());
			deleteHennas.setInt(2, classIndex);
			deleteHennas.execute();
			
			// Remove all shortcuts info stored for this sub-class.
			deleteShortcuts.setInt(1, getObjectId());
			deleteShortcuts.setInt(2, classIndex);
			deleteShortcuts.execute();
			
			// Remove all effects info stored for this sub-class.
			deleteSkillReuse.setInt(1, getObjectId());
			deleteSkillReuse.setInt(2, classIndex);
			deleteSkillReuse.execute();
			
			// Remove all skill info stored for this sub-class.
			deleteSkills.setInt(1, getObjectId());
			deleteSkills.setInt(2, classIndex);
			deleteSkills.execute();
			
			// Remove all basic info stored about this sub-class.
			deleteSubclass.setInt(1, getObjectId());
			deleteSubclass.setInt(2, classIndex);
			deleteSubclass.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not modify sub class for " + getName() + " to class index " + classIndex + ": " + e);
			return false;
		}
		
		return addSubClass(newClassId, classIndex, isDualClass);
	}
	
	public bool isSubClassActive()
	{
		return _classIndex > 0;
	}
	
	public void setDualClassActive(int classIndex)
	{
		if (isSubClassActive())
		{
			getSubClasses().get(_classIndex).setDualClassActive(true);
		}
	}
	
	public bool isDualClassActive()
	{
		if (!isSubClassActive())
		{
			return false;
		}
		if (_subClasses.isEmpty())
		{
			return false;
		}
		SubClassHolder subClass = _subClasses.get(_classIndex);
		if (subClass == null)
		{
			return false;
		}
		return subClass.isDualClass();
	}
	
	public bool hasDualClass()
	{
		foreach (SubClassHolder subClass in _subClasses.values())
		{
			if (subClass.isDualClass())
			{
				return true;
			}
		}
		return false;
	}
	
	public SubClassHolder getDualClass()
	{
		foreach (SubClassHolder subClass in _subClasses.values())
		{
			if (subClass.isDualClass())
			{
				return subClass;
			}
		}
		return null;
	}
	
	public Map<int, SubClassHolder> getSubClasses()
	{
		return _subClasses;
	}
	
	public int getTotalSubClasses()
	{
		return getSubClasses().size();
	}
	
	public int getBaseClass()
	{
		return _baseClass;
	}
	
	public int getActiveClass()
	{
		return _activeClass;
	}
	
	public int getClassIndex()
	{
		return _classIndex;
	}
	
	protected void setClassIndex(int classIndex)
	{
		_classIndex = classIndex;
	}
	
	private void setClassTemplate(int classId)
	{
		_activeClass = classId;
		
		PlayerTemplate pcTemplate = PlayerTemplateData.getInstance().getTemplate(classId);
		if (pcTemplate == null)
		{
			LOGGER.Error("Missing template for classId: " + classId);
			throw new Exception();
		}
		// Set the template of the Player
		setTemplate(pcTemplate);
		
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_PROFESSION_CHANGE, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerProfessionChange(this, pcTemplate, isSubClassActive()), this);
		}
	}
	
	/**
	 * Changes the character's class based on the given class index.<br>
	 * An index of zero specifies the character's original (base) class, while indexes 1-3 specifies the character's sub-classes respectively.<br>
	 * <font color="00FF00"/>WARNING: Use only on subclase change</font>
	 * @param classIndex
	 */
	public void setActiveClass(int classIndex)
	{
		if (_subclassLock)
		{
			return;
		}
		_subclassLock = true;
		
		try
		{
			// Cannot switch or change subclasses while transformed
			if (isTransformed())
			{
				return;
			}
			
			// Remove active item skills before saving char to database
			// because next time when choosing this class, weared items can
			// be different
			foreach (Item item in _inventory.getPaperdollItems(x => x.isAugmented()))
			{
				if ((item != null) && item.isEquipped())
				{
					item.getAugmentation().removeBonus(this);
				}
			}
			
			// abort any kind of cast.
			abortCast();
			
			if (isChannelized())
			{
				getSkillChannelized().abortChannelization();
			}
			
			// 1. Call store() before modifying _classIndex to avoid skill effects rollover.
			// 2. Register the correct _classId against applied 'classIndex'.
			store(Config.SUBCLASS_STORE_SKILL_COOLTIME);
			
			if (_sellingBuffs != null)
			{
				_sellingBuffs.Clear();
			}
			
			resetTimeStamps();
			
			// clear charges
			_charges.set(0);
			stopChargeTask();
			
			if (hasServitors())
			{
				getServitors().values().forEach(s => s.unSummon(this));
			}
			
			if (classIndex == 0)
			{
				setClassTemplate(_baseClass);
			}
			else
			{
				try
				{
					setClassTemplate(getSubClasses().get(classIndex).getClassId());
				}
				catch (Exception e)
				{
					LOGGER.Warn("Could not switch " + getName() + "'s sub class to class index " + classIndex + ": " + e);
					return;
				}
			}
			_classIndex = classIndex;
			if (isInParty())
			{
				_party.recalculatePartyLevel();
			}
			
			// Update the character's change in class status.
			// 1. Remove any active cubics from the player.
			// 2. Renovate the characters table in the database with the new class info, storing also buff/effect data.
			// 3. Remove all existing skills.
			// 4. Restore all the learned skills for the current class from the database.
			// 5. Restore effect/buff data for the new class.
			// 6. Restore henna data for the class, applying the new stat modifiers while removing existing ones.
			// 7. Reset HP/MP/CP stats and send Server=>Client character status packet to reflect changes.
			// 8. Restore shortcut data related to this class.
			// 9. Resend a class change animation effect to broadcast to all nearby players.
			_autoUseSettings.getAutoSkills().Clear();
			_autoUseSettings.getAutoBuffs().Clear();
			foreach (Skill oldSkill in getAllSkills())
			{
				removeSkill(oldSkill, false, true);
			}
			
			// stopAllEffectsExceptThoseThatLastThroughDeath();
			getEffectList().stopEffects(info => !info.getSkill().isStayAfterDeath(), true, false);
			
			// stopAllEffects();
			getEffectList().stopEffects(info => !info.getSkill().isNecessaryToggle() && !info.getSkill().isIrreplacableBuff(), true, false);
			
			// In controversy with isNecessaryToggle above, new class rewarded skills should be rewarded bellow.
			getEffectList().stopAllToggles();
			
			// Update abnormal visual effects.
			sendPacket(new ExUserInfoAbnormalVisualEffect(this));
			stopCubics();
			
			restoreRecipeBook(false);
			
			restoreSkills();
			rewardSkills();
			regiveTemporarySkills();
			getInventory().applyItemSkills();
			restoreCollectionBonuses();
			
			// Prevents some issues when changing between subclases that shares skills
			resetDisabledSkills();
			
			restoreEffects();
			
			sendPacket(new EtcStatusUpdate(this));
			
			restoreHenna();
			sendPacket(new HennaInfo(this));
			if (getCurrentHp() > getMaxHp())
			{
				setCurrentHp(getMaxHp());
			}
			if (getCurrentMp() > getMaxMp())
			{
				setCurrentMp(getMaxMp());
			}
			if (getCurrentCp() > getMaxCp())
			{
				setCurrentCp(getMaxCp());
			}
			
			refreshOverloaded(true);
			broadcastUserInfo();
			
			// Clear resurrect xp calculation
			setExpBeforeDeath(0);
			
			_shortCuts.restoreMe();
			sendPacket(new ShortCutInit(this));
			broadcastPacket(new SocialAction(getObjectId(), SocialAction.LEVEL_UP));
			sendPacket(new SkillCoolTime(this));
			sendStorageMaxCount();
			
			if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_SUB_CHANGE, this))
			{
				EventDispatcher.getInstance().notifyEventAsync(new OnPlayerSubChange(this), this);
			}
		}
		finally
		{
			_subclassLock = false;
			getStat().recalculateStats(false);
			updateAbnormalVisualEffects();
			sendSkillList();
		}
	}
	
	public bool isSubclassLocked()
	{
		return _subclassLock;
	}
	
	public void stopWarnUserTakeBreak()
	{
		if (_taskWarnUserTakeBreak != null)
		{
			_taskWarnUserTakeBreak.cancel(true);
			_taskWarnUserTakeBreak = null;
		}
	}
	
	public void startWarnUserTakeBreak()
	{
		if (_taskWarnUserTakeBreak == null)
		{
			_taskWarnUserTakeBreak = ThreadPool.scheduleAtFixedRate(new WarnUserTakeBreakTask(this), 3600000, 3600000);
		}
	}
	
	public void stopRentPet()
	{
		if (_taskRentPet != null)
		{
			// if the rent of a wyvern expires while over a flying zone, tp to down before unmounting
			if (checkLandingState() && (_mountType == MountType.WYVERN))
			{
				teleToLocation(TeleportWhereType.TOWN);
			}
			
			if (dismount()) // this should always be true now, since we teleported already
			{
				_taskRentPet.cancel(true);
				_taskRentPet = null;
			}
		}
	}
	
	public void startRentPet(int seconds)
	{
		if (_taskRentPet == null)
		{
			_taskRentPet = ThreadPool.scheduleAtFixedRate(new RentPetTask(this), seconds * 1000, seconds * 1000);
		}
	}
	
	public bool isRentedPet()
	{
		return _taskRentPet != null;
	}
	
	public void stopWaterTask()
	{
		if (_taskWater != null)
		{
			_taskWater.cancel(false);
			_taskWater = null;
			sendPacket(new SetupGauge(getObjectId(), 2, 0));
		}
	}
	
	public void startWaterTask()
	{
		if (!isDead() && (_taskWater == null))
		{
			int timeinwater = (int) getStat().getValue(Stat.BREATH, 60000);
			sendPacket(new SetupGauge(getObjectId(), 2, timeinwater));
			_taskWater = ThreadPool.scheduleAtFixedRate(new WaterTask(this), timeinwater, 1000);
		}
	}
	
	public bool isInWater()
	{
		return _taskWater != null;
	}
	
	public void checkWaterState()
	{
		if (isInsideZone(ZoneId.WATER))
		{
			startWaterTask();
		}
		else
		{
			stopWaterTask();
		}
	}
	
	public void onPlayerEnter()
	{
		startWarnUserTakeBreak();
		
		if (isGM() && !Config.GM_STARTUP_BUILDER_HIDE)
		{
			// Bleah, see L2J custom below.
			if (isInvul())
			{
				sendMessage("Entering world in Invulnerable mode.");
			}
			if (isInvisible())
			{
				sendMessage("Entering world in Invisible mode.");
			}
			if (_silenceMode)
			{
				sendMessage("Entering world in Silence mode.");
			}
		}
		
		// Apply item skills.
		_inventory.applyItemSkills();
		
		// Buff and status icons.
		if (Config.STORE_SKILL_COOLTIME)
		{
			restoreEffects();
		}
		
		// TODO : Need to fix that hack!
		if (!isDead())
		{
			setCurrentCp(_originalCp);
			setCurrentHp(_originalHp);
			setCurrentMp(_originalMp);
		}
		
		revalidateZone(true);
		
		notifyFriends(FriendStatus.MODE_ONLINE);
		if (!canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS) && Config.DECREASE_SKILL_LEVEL)
		{
			checkPlayerSkills();
		}
		
		try
		{
			SayuneRequest sayune = getRequest<SayuneRequest>();
			if (sayune != null)
			{
				sayune.onLogout();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		try
		{
			foreach (ZoneType zone in ZoneManager.getInstance().getZones(this))
			{
				zone.onPlayerLoginInside(this);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
		
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_LOGIN, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerLogin(this), this);
		}
		if (isMentee())
		{
			if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_MENTEE_STATUS, this))
			{
				EventDispatcher.getInstance().notifyEventAsync(new OnPlayerMenteeStatus(this, true), this);
			}
		}
		else if (isMentor() && EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_MENTOR_STATUS, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerMentorStatus(this, true), this);
		}
	}
	
	public long getLastAccess()
	{
		return _lastAccess;
	}
	
	protected void setLastAccess(long lastAccess)
	{
		_lastAccess = lastAccess;
	}
	
	public override void doRevive()
	{
		base.doRevive();
		
		if (Config.DISCONNECT_AFTER_DEATH)
		{
			DecayTaskManager.getInstance().cancel(this);
		}
		
		applyKarmaPenalty();
		
		sendPacket(new EtcStatusUpdate(this));
		_revivePet = false;
		_reviveRequested = 0;
		_revivePower = 0;
		
		// Teleport summons to player.
		if (isInsideZone(ZoneId.PEACE) && hasSummon())
		{
			Pet pet = getPet();
			if (pet != null)
			{
				pet.teleToLocation(this, true);
			}
			foreach (Summon summon in getServitors().values())
			{
				if (!summon.isInsideZone(ZoneId.SIEGE))
				{
					summon.teleToLocation(this, true);
				}
			}
		}
		
		if (isMounted())
		{
			startFeed(_mountNpcId);
		}
		
		// Notify instance
		Instance instance = getInstanceWorld();
		if (instance != null)
		{
			instance.doRevive(this);
		}
		
		clearDamageTaken();
	}
	
	public override void doRevive(double revivePower)
	{
		doRevive();
		restoreExp(revivePower);
	}
	
	public void reviveRequest(Player reviver, bool isPet, int power, int reviveHp, int reviveMp, int reviveCp)
	{
		if (isResurrectionBlocked())
		{
			return;
		}
		
		if (_reviveRequested == 1)
		{
			if (_revivePet == isPet)
			{
				reviver.sendPacket(SystemMessageId.RESURRECTION_HAS_ALREADY_BEEN_PROPOSED); // Resurrection is already been proposed.
			}
			else if (isPet)
			{
				reviver.sendPacket(SystemMessageId.A_PET_CANNOT_BE_RESURRECTED_WHILE_IT_S_OWNER_IS_IN_THE_PROCESS_OF_RESURRECTING); // A pet cannot be resurrected while it's owner is in the process of resurrecting.
			}
			else
			{
				reviver.sendPacket(SystemMessageId.WHILE_A_PET_IS_BEING_RESURRECTED_IT_CANNOT_HELP_IN_RESURRECTING_ITS_MASTER); // While a pet is attempting to resurrect, it cannot help in resurrecting its master.
			}
			return;
		}
		if ((isPet && (_pet != null) && _pet.isDead()) || (!isPet && isDead()))
		{
			_reviveRequested = 1;
			_revivePower = Formulas.calculateSkillResurrectRestorePercent(power, reviver);
			_reviveHpPercent = reviveHp;
			_reviveMpPercent = reviveMp;
			_reviveCpPercent = reviveCp;
			_revivePet = isPet;
			if (hasCharmOfCourage())
			{
				ConfirmDlg dlg = new ConfirmDlg(SystemMessageId.YOUR_CHARM_OF_COURAGE_IS_TRYING_TO_RESURRECT_YOU_WOULD_YOU_LIKE_TO_RESURRECT_NOW.getId());
				dlg.addTime(60000);
				sendPacket(dlg);
				return;
			}
			
			long restoreExp = (long)Math.Round(((_expBeforeDeath - getExp()) * _revivePower) / 100);
			ConfirmDlg dlg = new ConfirmDlg(SystemMessageId.C1_IS_ATTEMPTING_TO_DO_A_RESURRECTION_THAT_RESTORES_S2_S3_XP_ACCEPT.getId());
			dlg.getSystemMessage().addPcName(reviver);
			dlg.getSystemMessage().addLong(restoreExp);
			dlg.getSystemMessage().addInt(power);
			sendPacket(dlg);
		}
	}
	
	public void reviveAnswer(int answer)
	{
		if ((_reviveRequested != 1) || (!isDead() && !_revivePet) || (_revivePet && (_pet != null) && !_pet.isDead()))
		{
			return;
		}
		
		if (answer == 1)
		{
			if (!_revivePet)
			{
				if (_revivePower != 0)
				{
					doRevive(_revivePower);
				}
				else
				{
					doRevive();
				}
			}
			else if (_pet != null)
			{
				if (_revivePower != 0)
				{
					_pet.doRevive(_revivePower);
				}
				else
				{
					_pet.doRevive();
				}
			}
		}
		_reviveRequested = 0;
		_revivePower = 0;
		
		// Support for specific HP/MP/CP percentage restored.
		Creature effected = _revivePet ? _pet : this;
		if (effected == null)
		{
			_reviveHpPercent = 0;
			_reviveMpPercent = 0;
			_reviveCpPercent = 0;
			return;
		}
		if (_reviveHpPercent > 0)
		{
			double amount = (effected.getMaxHp() * _reviveHpPercent) / 100;
			if (amount > 0)
			{
				effected.setCurrentHp(amount, true);
			}
			_reviveHpPercent = 0;
		}
		if (_reviveMpPercent > 0)
		{
			double amount = (effected.getMaxMp() * _reviveMpPercent) / 100;
			if (amount > 0)
			{
				effected.setCurrentMp(amount, true);
			}
			_reviveMpPercent = 0;
		}
		if (_reviveCpPercent > 0)
		{
			double amount = (effected.getMaxCp() * _reviveCpPercent) / 100;
			if (amount > 0)
			{
				effected.setCurrentCp(amount, true);
			}
			_reviveCpPercent = 0;
		}
	}
	
	public bool isReviveRequested()
	{
		return (_reviveRequested == 1);
	}
	
	public bool isRevivingPet()
	{
		return _revivePet;
	}
	
	public void removeReviving()
	{
		_reviveRequested = 0;
		_revivePower = 0;
	}
	
	public void onActionRequest()
	{
		if (isSpawnProtected())
		{
			setSpawnProtection(false);
			if (!isInsideZone(ZoneId.PEACE))
			{
				sendPacket(SystemMessageId.YOU_ARE_NO_LONGER_PROTECTED_FROM_AGGRESSIVE_MONSTERS);
			}
			if (Config.RESTORE_SERVITOR_ON_RECONNECT && !hasSummon() && CharSummonTable.getInstance().getServitors().containsKey(getObjectId()))
			{
				CharSummonTable.getInstance().restoreServitor(this);
			}
			if (Config.RESTORE_PET_ON_RECONNECT && !hasSummon() && CharSummonTable.getInstance().getPets().containsKey(getObjectId()))
			{
				CharSummonTable.getInstance().restorePet(this);
			}
		}
		if (isTeleportProtected())
		{
			setTeleportProtection(false);
			if (!isInsideZone(ZoneId.PEACE))
			{
				sendMessage("Teleport spawn protection ended.");
			}
		}
	}
	
	public override void teleToLocation(ILocational loc, bool allowRandomOffset)
	{
		if ((_vehicle != null) && !_vehicle.isTeleporting())
		{
			setVehicle(null);
		}
		
		if (isFlyingMounted() && (loc.getZ() < -1005))
		{
			base.teleToLocation(loc.getX(), loc.getY(), -1005, loc.getHeading());
		}
		base.teleToLocation(loc, allowRandomOffset);
	}
	
	public override void onTeleported()
	{
		// Stop auto peel.
		if (hasRequest<AutoPeelRequest>())
		{
			sendPacket(new ExStopItemAutoPeel(true));
			sendPacket(new ExReadyItemAutoPeel(false, 0));
			removeRequest<AutoPeelRequest>();
		}
		
		base.onTeleported();
		
		if (isInAirShip())
		{
			getAirShip().sendInfo(this);
		}
		else // Update last player position upon teleport.
		{
			setLastServerPosition(getX(), getY(), getZ());
		}
		
		// Force a revalidation.
		revalidateZone(true);
		
		checkItemRestriction();
		
		if ((Config.PLAYER_TELEPORT_PROTECTION > 0) && !_inOlympiadMode)
		{
			setTeleportProtection(true);
		}
		
		// Trained beast is lost after teleport.
		foreach (TamedBeast tamedBeast in _tamedBeast)
		{
			tamedBeast.deleteMe();
		}
		_tamedBeast.clear();
		
		// Modify the position of the pet if necessary.
		if (_pet != null)
		{
			_pet.setFollowStatus(false);
			_pet.teleToLocation(getLocation(), false);
			((SummonAI) _pet.getAI()).setStartFollowController(true);
			_pet.setFollowStatus(true);
			_pet.setInstance(getInstanceWorld());
			_pet.updateAndBroadcastStatus(0);
			sendPacket(new PetSummonInfo(_pet, 0));
		}
		
		getServitors().values().forEach(s =>
		{
			s.setFollowStatus(false);
			s.teleToLocation(getLocation(), false);
			((SummonAI) s.getAI()).setStartFollowController(true);
			s.setFollowStatus(true);
			s.setInstance(getInstanceWorld());
			s.updateAndBroadcastStatus(0);
			sendPacket(new PetSummonInfo(s, 0));
		});
		
		// Show movie if available.
		if (_movieHolder != null)
		{
			sendPacket(new ExStartScenePlayer(_movieHolder.getMovie()));
		}
		
		// Close time limited zone window.
		if (!isInTimedHuntingZone())
		{
			stopTimedHuntingZoneTask();
		}
		
		// Stop auto play.
		AutoPlayTaskManager.getInstance().stopAutoPlay(this);
		AutoUseTaskManager.getInstance().stopAutoUseTask(this);
		sendPacket(new ExAutoPlaySettingSend(_autoPlaySettings.getOptions(), false, _autoPlaySettings.doPickup(), _autoPlaySettings.getNextTargetMode(), _autoPlaySettings.isShortRange(), _autoPlaySettings.getAutoPotionPercent(), _autoPlaySettings.isRespectfulHunting(), _autoPlaySettings.getAutoPetPotionPercent()));
		restoreAutoShortcutVisual();
		
		// Send info to nearby players.
		broadcastInfo();
	}
	
	public override void setTeleporting(bool teleport)
	{
		setTeleporting(teleport, true);
	}
	
	public void setTeleporting(bool teleport, bool useWatchDog)
	{
		base.setTeleporting(teleport);
		if (!useWatchDog)
		{
			return;
		}
		if (teleport)
		{
			if ((_teleportWatchdog == null) && (Config.TELEPORT_WATCHDOG_TIMEOUT > 0))
			{
				lock (this)
				{
					if (_teleportWatchdog == null)
					{
						_teleportWatchdog = ThreadPool.schedule(new TeleportWatchdogTask(this), Config.TELEPORT_WATCHDOG_TIMEOUT * 1000);
					}
				}
			}
		}
		else if (_teleportWatchdog != null)
		{
			_teleportWatchdog.cancel(false);
			_teleportWatchdog = null;
		}
	}
	
	public void setTeleportLocation(Location location)
	{
		_teleportLocation = location;
	}
	
	public Location getTeleportLocation()
	{
		return _teleportLocation;
	}
	
	public void setLastServerPosition(int x, int y, int z)
	{
		_lastServerPosition.setXYZ(x, y, z);
	}
	
	public Location getLastServerPosition()
	{
		return _lastServerPosition;
	}
	
	public void setBlinkActive(bool value)
	{
		_blinkActive.set(value);
	}
	
	public bool isBlinkActive()
	{
		return _blinkActive.get();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public override void addExpAndSp(double addToExp, double addToSp)
	{
		getStat().addExpAndSp(addToExp, addToSp, false);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void addExpAndSp(double addToExp, double addToSp, bool useVitality)
	{
		getStat().addExpAndSp(addToExp, addToSp, useVitality);
	}
	
	public void removeExpAndSp(long removeExp, long removeSp)
	{
		getStat().removeExpAndSp(removeExp, removeSp, true);
	}
	
	public void removeExpAndSp(long removeExp, long removeSp, bool sendMessage)
	{
		getStat().removeExpAndSp(removeExp, removeSp, sendMessage);
	}
	
	public override void reduceCurrentHp(double value, Creature attacker, Skill skill, bool isDOT, bool directlyToHp, bool critical, bool reflect)
	{
		base.reduceCurrentHp(value, attacker, skill, isDOT, directlyToHp, critical, reflect);
		
		// notify the tamed beast of attacks
		foreach (TamedBeast tamedBeast in _tamedBeast)
		{
			tamedBeast.onOwnerGotAttacked(attacker);
		}
	}
	
	public void broadcastSnoop(ChatType type, String name, String text)
	{
		if (!_snoopListener.isEmpty())
		{
			Snoop sn = new Snoop(getObjectId(), getName(), type, name, text);
			foreach (Player pci in _snoopListener)
			{
				if (pci != null)
				{
					pci.sendPacket(sn);
				}
			}
		}
	}
	
	public void addSnooper(Player pci)
	{
		if (!_snoopListener.Contains(pci))
		{
			_snoopListener.add(pci);
		}
	}
	
	public void removeSnooper(Player pci)
	{
		_snoopListener.remove(pci);
	}
	
	public void addSnooped(Player pci)
	{
		if (!_snoopedPlayer.Contains(pci))
		{
			_snoopedPlayer.add(pci);
		}
	}
	
	public void removeSnooped(Player pci)
	{
		_snoopedPlayer.remove(pci);
	}
	
	public void addHtmlAction(HtmlActionScope scope, String action)
	{
		_htmlActionCaches[(int)scope].add(action);
	}
	
	public void clearHtmlActions(HtmlActionScope scope)
	{
		_htmlActionCaches[(int)scope].Clear();
	}
	
	public void setHtmlActionOriginObjectId(HtmlActionScope scope, int npcObjId)
	{
		if (npcObjId < 0)
		{
			throw new ArgumentException();
		}
		
		_htmlActionOriginObjectIds[(int)scope] = npcObjId;
	}
	
	public int getLastHtmlActionOriginId()
	{
		return _lastHtmlActionOriginObjId;
	}
	
	private bool validateHtmlAction(IEnumerable<String> actionIter, String action)
	{
		foreach (String cachedAction in actionIter)
		{
			if (cachedAction[cachedAction.Length - 1] == AbstractHtmlPacket.VAR_PARAM_START_CHAR)
			{
				if (action.startsWith(cachedAction.Substring(0, cachedAction.Length - 1).Trim()))
				{
					return true;
				}
			}
			else if (cachedAction.equals(action))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Check if the HTML action was sent in a HTML packet.<br>
	 * If the HTML action was not sent for whatever reason, -1 is returned.<br>
	 * Otherwise, the NPC object ID or 0 is returned.<br>
	 * 0 means the HTML action was not bound to an NPC<br>
	 * and no range checks need to be made.
	 * @param action the HTML action to check
	 * @return NPC object ID, 0 or -1
	 */
	public int validateHtmlAction(String action)
	{
		for (int i = 0; i < _htmlActionCaches.Length; ++i)
		{
			if (validateHtmlAction(_htmlActionCaches[i], action))
			{
				_lastHtmlActionOriginObjId = _htmlActionOriginObjectIds[i];
				return _lastHtmlActionOriginObjId;
			}
		}
		return -1;
	}
	
	/**
	 * Performs following tests:
	 * <ul>
	 * <li>Inventory contains item</li>
	 * <li>Item owner id == owner id</li>
	 * <li>It isnt pet control item while mounting pet or pet summoned</li>
	 * <li>It isnt active enchant item</li>
	 * <li>It isnt cursed weapon/item</li>
	 * <li>It isnt wear item</li>
	 * </ul>
	 * @param objectId item object id
	 * @param action just for login porpouse
	 * @return
	 */
	public bool validateItemManipulation(int objectId, String action)
	{
		Item item = _inventory.getItemByObjectId(objectId);
		if ((item == null) || (item.getOwnerId() != getObjectId()))
		{
			LOGGER.Info(getObjectId() + ": player tried to " + action + " item he is not owner of");
			return false;
		}
		
		// Pet is summoned and not the item that summoned the pet AND not the buggle from strider you're mounting
		if (((_pet != null) && (_pet.getControlObjectId() == objectId)) || (_mountObjectID == objectId))
		{
			return false;
		}
		
		if (isProcessingItem(objectId))
		{
			return false;
		}
		
		if (CursedWeaponsManager.getInstance().isCursed(item.getId()))
		{
			// can not trade a cursed weapon
			return false;
		}
		
		return true;
	}
	
	/**
	 * @return Returns the inBoat.
	 */
	public bool isInBoat()
	{
		return (_vehicle != null) && _vehicle.isBoat();
	}
	
	/**
	 * @return
	 */
	public Boat getBoat()
	{
		return (Boat) _vehicle;
	}
	
	/**
	 * @return Returns the inAirShip.
	 */
	public bool isInAirShip()
	{
		return (_vehicle != null) && _vehicle.isAirShip();
	}
	
	/**
	 * @return
	 */
	public AirShip getAirShip()
	{
		return (AirShip) _vehicle;
	}
	
	public bool isInShuttle()
	{
		return _vehicle is Shuttle;
	}
	
	public Shuttle getShuttle()
	{
		return (Shuttle) _vehicle;
	}
	
	public Vehicle getVehicle()
	{
		return _vehicle;
	}
	
	public void setVehicle(Vehicle v)
	{
		if ((v == null) && (_vehicle != null))
		{
			_vehicle.removePassenger(this);
		}
		
		_vehicle = v;
	}
	
	public bool isInVehicle()
	{
		return _vehicle != null;
	}
	
	public void setInCrystallize(bool inCrystallize)
	{
		_inCrystallize = inCrystallize;
	}
	
	public bool isInCrystallize()
	{
		return _inCrystallize;
	}
	
	/**
	 * @return
	 */
	public Location getInVehiclePosition()
	{
		return _inVehiclePosition;
	}
	
	public void setInVehiclePosition(Location pt)
	{
		_inVehiclePosition = pt;
	}
	
	/**
	 * Manage the delete task of a Player (Leave Party, Unsummon pet, Save its inventory in the database, Remove it from the world...).<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>If the Player is in observer mode, set its position to its position before entering in observer mode</li>
	 * <li>Set the online Flag to True or False and update the characters table of the database with online status and lastAccess</li>
	 * <li>Stop the HP/MP/CP Regeneration task</li>
	 * <li>Cancel Crafting, Attack or Cast</li>
	 * <li>Remove the Player from the world</li>
	 * <li>Stop Party and Unsummon Pet</li>
	 * <li>Update database with items in its inventory and remove them from the world</li>
	 * <li>Remove all WorldObject from _knownObjects and _knownPlayer of the Creature then cancel Attak or Cast and notify AI</li>
	 * <li>Close the connection with the client</li>
	 * </ul>
	 * <br>
	 * Remember this method is not to be used to half-ass disconnect players! This method is dedicated only to erase the player from the world.<br>
	 * If you intend to disconnect a player please use {@link Disconnection}
	 */
	public override bool deleteMe()
	{
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_LOGOUT, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerLogout(this), this);
		}
		
		try
		{
			foreach (ZoneType zone in ZoneManager.getInstance().getZones(this))
			{
				zone.onPlayerLogoutInside(this);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// Set the online Flag to True or False and update the characters table of the database with online status and lastAccess (called when login and logout)
		try
		{
			if (!_isOnline)
			{
				LOGGER.Error("deleteMe() called on offline character " + this);
			}
			setOnlineStatus(false, true);
			CharInfoTable.getInstance().setLastAccess(getObjectId(), DateTime.Now);
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		try
		{
			_isOnline = false;
			_offlinePlay = false;
			abortAttack();
			abortCast();
			stopMove(null);
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// remove combat flag
		try
		{
			if (_inventory.getItemByItemId(FortManager.ORC_FORTRESS_FLAG) != null)
			{
				Fort fort = FortManager.getInstance().getFort(this);
				if (fort != null)
				{
					FortSiegeManager.getInstance().dropCombatFlag(this, fort.getResidenceId());
				}
				else
				{
					long slot = _inventory.getSlotFromItem(_inventory.getItemByItemId(FortManager.ORC_FORTRESS_FLAG));
					_inventory.unEquipItemInBodySlot(slot);
					destroyItem("CombatFlag", _inventory.getItemByItemId(FortManager.ORC_FORTRESS_FLAG), null, true);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		try
		{
			if (_matchingRoom != null)
			{
				_matchingRoom.deleteMember(this, false);
			}
			MatchingRoomManager.getInstance().removeFromWaitingList(this);
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		try
		{
			if (isFlying())
			{
				removeSkill(SkillData.getInstance().getSkill((int)CommonSkill.WYVERN_BREATH, 1));
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// Exit timed hunting zone.
		if (isInTimedHuntingZone())
		{
			teleToLocation(TeleportWhereType.TOWN);
			storeCharBase();
		}
		
		// Store death points.
		if (_isDeathKnight)
		{
			getVariables().set(PlayerVariables.DEATH_POINT_COUNT, _deathPoints);
		}
		
		// Store beast points.
		if (_isVanguard)
		{
			getVariables().set(PlayerVariables.BEAST_POINT_COUNT, _beastPoints);
		}
		
		if (_isAssassin)
		{
			getVariables().set(PlayerVariables.ASSASSINATION_POINT_COUNT, _assassinationPoints);
		}
		
		// Make sure player variables are stored.
		getVariables().storeMe();
		
		// Make sure account variables are stored.
		getAccountVariables().storeMe();
		
		// Recommendations must be saved before task (timer) is canceled
		try
		{
			storeRecommendations();
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// Stop the HP/MP/CP Regeneration task (scheduled tasks)
		try
		{
			stopAllTimers();
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		try
		{
			setTeleporting(false);
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// Stop crafting, if in progress
		try
		{
			RecipeManager.getInstance().requestMakeItemAbort(this);
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// Cancel Attak or Cast
		try
		{
			setTarget(null);
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		if (isChannelized())
		{
			getSkillChannelized().abortChannelization();
		}
		
		// Stop all toggles.
		getEffectList().stopAllToggles();
		
		// Remove from world regions zones.
		ZoneRegion region = ZoneManager.getInstance().getRegion(this);
		if (region != null)
		{
			region.removeFromZones(this);
		}
		
		// If a Party is in progress, leave it (and festival party)
		if (isInParty())
		{
			try
			{
				leaveParty();
			}
			catch (Exception e)
			{
			LOGGER.Error("deleteMe(): " + e);
			}
		}
		
		stopCubics();
		
		// Remove the Player from the world
		try
		{
			decayMe();
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		if (OlympiadManager.getInstance().isRegistered(this) || (getOlympiadGameId() != -1))
		{
			OlympiadManager.getInstance().removeDisconnectedCompetitor(this);
		}
		
		// If the Player has Pet, unsummon it
		if (hasSummon())
		{
			try
			{
				Summon pet = _pet;
				if (pet != null)
				{
					pet.setRestoreSummon(true);
					pet.unSummon(this);
					// Dead pet wasn't unsummoned, broadcast npcinfo changes (pet will be without owner name - means owner offline)
					pet = _pet;
					if (pet != null)
					{
						pet.broadcastNpcInfo(0);
					}
				}
				
				getServitors().values().forEach(s =>
				{
					s.setRestoreSummon(true);
					s.unSummon(this);
				});
			}
			catch (Exception e)
			{
				LOGGER.Error("deleteMe(): " + e);
			} // returns pet to control item
		}
		
		if (_clan != null)
		{
			// set the status for pledge member list to OFFLINE
			try
			{
				ClanMember clanMember = _clan.getClanMember(getObjectId());
				if (clanMember != null)
				{
					clanMember.setPlayer(null);
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("deleteMe(): " + e);
			}
		}
		
		if (getActiveRequester() != null)
		{
			// deals with sudden exit in the middle of transaction
			setActiveRequester(null);
			cancelActiveTrade();
		}
		
		// If the Player is a GM, remove it from the GM List
		if (isGM())
		{
			try
			{
				AdminData.getInstance().deleteGm(this);
			}
			catch (Exception e)
			{
				LOGGER.Error("deleteMe(): " + e);
			}
		}
		
		try
		{
			// Check if the Player is in observer mode to set its position to its position
			// before entering in observer mode
			if (_observerMode)
			{
				setLocationInvisible(_lastLoc);
			}
			
			if (_vehicle != null)
			{
				_vehicle.oustPlayer(this);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// remove player from instance
		Instance inst = getInstanceWorld();
		if (inst != null)
		{
			try
			{
				inst.onPlayerLogout(this);
			}
			catch (Exception e)
			{
				LOGGER.Error("deleteMe(): " + e);
			}
		}
		
		try
		{
			stopCubics();
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// Update database with items in its inventory and remove them from the world
		try
		{
			_inventory.deleteMe();
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		// Update database with items in its warehouse and remove them from the world
		try
		{
			getWarehouse().deleteMe();
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		try
		{
			_freight.deleteMe();
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		try
		{
			clearRefund();
		}
		catch (Exception e)
		{
			LOGGER.Error("deleteMe(): " + e);
		}
		
		if (isCursedWeaponEquipped())
		{
			try
			{
				CursedWeaponsManager.getInstance().getCursedWeapon(_cursedWeaponEquippedId).setPlayer(null);
			}
			catch (Exception e)
			{
				LOGGER.Error("deleteMe(): " + e);
			}
		}
		
		if (_clanId > 0)
		{
			_clan.broadcastToOtherOnlineMembers(new PledgeShowMemberListUpdate(this), this);
			_clan.broadcastToOnlineMembers(new ExPledgeCount(_clan));
			// ClanTable.getInstance().getClan(getClanId()).broadcastToOnlineMembers(new PledgeShowMemberListAdd(this));
		}
		
		foreach (Player player in _snoopedPlayer)
		{
			player.removeSnooper(this);
		}
		
		foreach (Player player in _snoopListener)
		{
			player.removeSnooped(this);
		}
		
		// Notify to scripts
		if (isMentee())
		{
			if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_MENTEE_STATUS, this))
			{
				EventDispatcher.getInstance().notifyEventAsync(new OnPlayerMenteeStatus(this, false), this);
			}
		}
		else if (isMentor() && EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_MENTOR_STATUS, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerMentorStatus(this, false), this);
		}
		
		try
		{
			notifyFriends(FriendStatus.MODE_OFFLINE);
			
			// Friend list
			SystemMessage sm = new SystemMessage(SystemMessageId.YOUR_FRIEND_S1_HAS_LOGGED_OUT);
			sm.addString(getName());
			foreach (int id in getFriendList())
			{
				WorldObject obj = World.getInstance().findObject(id);
				if (obj != null)
				{
					obj.sendPacket(sm);
				}
			}
			
			// Surveillance list
			ExUserWatcherTargetStatus surveillanceUpdate = new ExUserWatcherTargetStatus(getName(), false);
			sm = new SystemMessage(SystemMessageId.C1_FROM_YOUR_SURVEILLANCE_LIST_IS_OFFLINE);
			sm.addString(getName());
			foreach (Player p in World.getInstance().getPlayers())
			{
				if (p.getSurveillanceList().Contains(getObjectId()))
				{
					p.sendPacket(sm);
					p.sendPacket(surveillanceUpdate);
				}
			}
			
			_blockList.playerLogout();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception on deleteMe() notifyFriends: " + e);
		}
		
		// Stop all passives and augment options
		getEffectList().stopAllPassives(false, false);
		getEffectList().stopAllOptions(false, false);
		
		PlayerAutoSaveTaskManager.getInstance().remove(this);
		
		return base.deleteMe();
	}
	
	public int getInventoryLimit()
	{
		int ivlim;
		if (isGM())
		{
			ivlim = Config.INVENTORY_MAXIMUM_GM;
		}
		else if (getRace() == Race.DWARF)
		{
			ivlim = Config.INVENTORY_MAXIMUM_DWARF;
		}
		else
		{
			ivlim = Config.INVENTORY_MAXIMUM_NO_DWARF;
		}
		ivlim += (int) getStat().getValue(Stat.INVENTORY_NORMAL, 0);
		return ivlim;
	}
	
	public int getWareHouseLimit()
	{
		int whlim;
		if (getRace() == Race.DWARF)
		{
			whlim = Config.WAREHOUSE_SLOTS_DWARF;
		}
		else
		{
			whlim = Config.WAREHOUSE_SLOTS_NO_DWARF;
		}
		whlim += (int) getStat().getValue(Stat.STORAGE_PRIVATE, 0);
		return whlim;
	}
	
	public int getPrivateSellStoreLimit()
	{
		int pslim;
		if (getRace() == Race.DWARF)
		{
			pslim = Config.MAX_PVTSTORESELL_SLOTS_DWARF;
		}
		else
		{
			pslim = Config.MAX_PVTSTORESELL_SLOTS_OTHER;
		}
		pslim += (int) getStat().getValue(Stat.TRADE_SELL, 0);
		return pslim;
	}
	
	public int getPrivateBuyStoreLimit()
	{
		int pblim;
		if (getRace() == Race.DWARF)
		{
			pblim = Config.MAX_PVTSTOREBUY_SLOTS_DWARF;
		}
		else
		{
			pblim = Config.MAX_PVTSTOREBUY_SLOTS_OTHER;
		}
		pblim += (int) getStat().getValue(Stat.TRADE_BUY, 0);
		return pblim;
	}
	
	public int getDwarfRecipeLimit()
	{
		int recdlim = Config.DWARF_RECIPE_LIMIT;
		recdlim += (int) getStat().getValue(Stat.RECIPE_DWARVEN, 0);
		return recdlim;
	}
	
	public int getCommonRecipeLimit()
	{
		int recclim = Config.COMMON_RECIPE_LIMIT;
		recclim += (int) getStat().getValue(Stat.RECIPE_COMMON, 0);
		return recclim;
	}
	
	/**
	 * @return Returns the mountNpcId.
	 */
	public int getMountNpcId()
	{
		return _mountNpcId;
	}
	
	/**
	 * @return Returns the mountLevel.
	 */
	public int getMountLevel()
	{
		return _mountLevel;
	}
	
	public void setMountObjectID(int newID)
	{
		_mountObjectID = newID;
	}
	
	public int getMountObjectID()
	{
		return _mountObjectID;
	}
	
	public SkillUseHolder getQueuedSkill()
	{
		return _queuedSkill;
	}
	
	/**
	 * Create a new SkillUseHolder object and queue it in the player _queuedSkill.
	 * @param queuedSkill
	 * @param item
	 * @param ctrlPressed
	 * @param shiftPressed
	 */
	public void setQueuedSkill(Skill queuedSkill, Item item, bool ctrlPressed, bool shiftPressed)
	{
		if (queuedSkill == null)
		{
			_queuedSkill = null;
			return;
		}
		_queuedSkill = new SkillUseHolder(queuedSkill, item, ctrlPressed, shiftPressed);
	}
	
	/**
	 * @return {@code true} if player is jailed, {@code false} otherwise.
	 */
	public bool isJailed()
	{
		return PunishmentManager.getInstance().hasPunishment(getObjectId(), PunishmentAffect.CHARACTER, PunishmentType.JAIL) //
			|| PunishmentManager.getInstance().hasPunishment(getAccountName(), PunishmentAffect.ACCOUNT, PunishmentType.JAIL) //
			|| PunishmentManager.getInstance().hasPunishment(getIPAddress(), PunishmentAffect.IP, PunishmentType.JAIL) //
			|| ((_client != null) && (_client.getHardwareInfo() != null) && PunishmentManager.getInstance().hasPunishment(_client.getHardwareInfo().getMacAddress(), PunishmentAffect.HWID, PunishmentType.JAIL));
	}
	
	/**
	 * @return {@code true} if player is chat banned, {@code false} otherwise.
	 */
	public bool isChatBanned()
	{
		return PunishmentManager.getInstance().hasPunishment(getObjectId(), PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN) //
			|| PunishmentManager.getInstance().hasPunishment(getAccountName(), PunishmentAffect.ACCOUNT, PunishmentType.CHAT_BAN) //
			|| PunishmentManager.getInstance().hasPunishment(getIPAddress(), PunishmentAffect.IP, PunishmentType.CHAT_BAN) //
			|| ((_client != null) && (_client.getHardwareInfo() != null) && PunishmentManager.getInstance().hasPunishment(_client.getHardwareInfo().getMacAddress(), PunishmentAffect.HWID, PunishmentType.CHAT_BAN));
	}
	
	public void startFameTask(long delay, int fameFixRate)
	{
		if ((getLevel() < 40) || (getClassId().level() < 2))
		{
			return;
		}
		if (_fameTask == null)
		{
			_fameTask = ThreadPool.scheduleAtFixedRate(new FameTask(this, fameFixRate), delay, delay);
		}
	}
	
	public void stopFameTask()
	{
		if (_fameTask != null)
		{
			_fameTask.cancel(false);
			_fameTask = null;
		}
	}
	
	public int getPowerGrade()
	{
		return _powerGrade;
	}
	
	public void setPowerGrade(int power)
	{
		_powerGrade = power;
	}
	
	public bool isCursedWeaponEquipped()
	{
		return _cursedWeaponEquippedId != 0;
	}
	
	public void setCursedWeaponEquippedId(int value)
	{
		_cursedWeaponEquippedId = value;
	}
	
	public int getCursedWeaponEquippedId()
	{
		return _cursedWeaponEquippedId;
	}
	
	public bool isCombatFlagEquipped()
	{
		return _combatFlagEquippedId;
	}
	
	public void setCombatFlagEquipped(bool value)
	{
		_combatFlagEquippedId = value;
	}
	
	/**
	 * Returns the Number of souls.
	 * @param type the type of souls.
	 * @return
	 */
	public int getChargedSouls(SoulType type)
	{
		return _souls.getOrDefault(type, 0);
	}
	
	/**
	 * Increase Souls
	 * @param count
	 * @param type
	 */
	public void increaseSouls(int count, SoulType type)
	{
		if (isTransformed() || hasAbnormalType(AbnormalType.KAMAEL_TRANSFORM))
		{
			return;
		}
		
		int newCount = getChargedSouls(type) + count;
		_souls.put(type, newCount);
		SystemMessage sm = new SystemMessage(SystemMessageId.YOUR_SOUL_COUNT_HAS_INCREASED_BY_S1_IT_IS_NOW_AT_S2);
		sm.addInt(count);
		sm.addInt(newCount);
		sendPacket(sm);
		restartSoulTask();
		sendPacket(new EtcStatusUpdate(this));
		
		if ((getRace() == Race.KAMAEL) && (newCount >= 100))
		{
			if (type == SoulType.LIGHT)
			{
				int skillLevel = getLightMasterLevel();
				if (skillLevel > 0)
				{
					abortCast();
					decreaseSouls(100, type);
					SkillData.getInstance().getSkill(KAMAEL_LIGHT_TRANSFORMATION, skillLevel).applyEffects(this, this);
				}
			}
			else // Shadow.
			{
				int skillLevel = getShadowMasterLevel();
				if (skillLevel > 0)
				{
					abortCast();
					decreaseSouls(100, type);
					SkillData.getInstance().getSkill(KAMAEL_SHADOW_TRANSFORMATION, skillLevel).applyEffects(this, this);
				}
			}
		}
	}
	
	public int getLightMasterLevel()
	{
		return getSkillLevel(KAMAEL_LIGHT_MASTER);
	}
	
	public int getShadowMasterLevel()
	{
		return getSkillLevel(KAMAEL_SHADOW_MASTER);
	}
	
	/**
	 * Decreases existing Souls.
	 * @param count
	 * @param type
	 * @return
	 */
	public bool decreaseSouls(int count, SoulType type)
	{
		int newCount = getChargedSouls(type) - count;
		if (newCount < 0)
		{
			newCount = 0;
		}
		_souls.put(type, newCount);
		
		if (newCount == 0)
		{
			stopSoulTask();
		}
		else
		{
			restartSoulTask();
		}
		
		sendPacket(new EtcStatusUpdate(this));
		return true;
	}
	
	/**
	 * Clear out all Souls from this Player
	 */
	public void clearSouls()
	{
		_souls.clear();
		stopSoulTask();
		sendPacket(new EtcStatusUpdate(this));
	}
	
	/**
	 * Starts/Restarts the SoulTask to Clear Souls after 10 Mins.
	 */
	private void restartSoulTask()
	{
		if (_soulTask != null)
		{
			_soulTask.cancel(false);
			_soulTask = null;
		}
		_soulTask = ThreadPool.schedule(new ResetSoulsTask(this), 600000);
	}
	
	/**
	 * Stops the Clearing Task.
	 */
	public void stopSoulTask()
	{
		if (_soulTask != null)
		{
			_soulTask.cancel(false);
			_soulTask = null;
		}
	}
	
	public int getDeathPoints()
	{
		return _deathPoints;
	}
	
	public int getMaxDeathPoints()
	{
		return _maxDeathPoints;
	}
	
	public void setDeathPoints(int value)
	{
		// Check current death points passive level.
		switch (getAffectedSkillLevel(DEATH_POINTS_PASSIVE))
		{
			case 1:
			{
				_maxDeathPoints = 500;
				break;
			}
			case 2:
			{
				_maxDeathPoints = 700;
				break;
			}
			case 3:
			{
				_maxDeathPoints = 1000;
				break;
			}
		}
		// Set current points.
		_deathPoints = Math.Min(_maxDeathPoints, Math.Max(0, value));
		// Apply devastating mind.
		int expectedLevel = _deathPoints / 100;
		if (expectedLevel > 0)
		{
			if (getAffectedSkillLevel(DEVASTATING_MIND) != expectedLevel)
			{
				getEffectList().stopSkillEffects(SkillFinishType.REMOVED, DEVASTATING_MIND);
				SkillData.getInstance().getSkill(DEVASTATING_MIND, expectedLevel).applyEffects(this, this);
			}
		}
		else
		{
			getEffectList().stopSkillEffects(SkillFinishType.REMOVED, DEVASTATING_MIND);
		}
		// Send StatusUpdate.
		StatusUpdate su = new StatusUpdate(this);
		computeStatusUpdate(su, StatusUpdateType.MAX_DP);
		computeStatusUpdate(su, StatusUpdateType.CUR_DP);
		sendPacket(su);
	}
	
	public int getBeastPoints()
	{
		return _beastPoints;
	}
	
	public int getMaxBeastPoints()
	{
		return _maxBeastPoints;
	}
	
	public void setBeastPoints(int value)
	{
		// TODO: Implement?
		_maxBeastPoints = 1000;
		
		// Set current points.
		_beastPoints = Math.Min(_maxBeastPoints, Math.Max(0, value));
		
		// Send StatusUpdate.
		StatusUpdate su = new StatusUpdate(this);
		computeStatusUpdate(su, StatusUpdateType.MAX_BP);
		computeStatusUpdate(su, StatusUpdateType.CUR_BP);
		sendPacket(su);
	}
	
	public int getAssassinationPoints()
	{
		return _assassinationPoints;
	}
	
	public int getMaxAssassinationPoints()
	{
		return _maxAssassinationPoints;
	}
	
	public void setAssassinationPoints(int value)
	{
		// Set current points.
		_assassinationPoints = Math.Min(_maxAssassinationPoints, Math.Max(0, value));
		
		// Send StatusUpdate.
		StatusUpdate su = new StatusUpdate(this);
		computeStatusUpdate(su, StatusUpdateType.MAX_AP);
		computeStatusUpdate(su, StatusUpdateType.CUR_AP);
		sendPacket(su);
	}
	
	public override Player getActingPlayer()
	{
		return this;
	}
	
	public override void sendDamageMessage(Creature target, Skill skill, int damage, double elementalDamage, bool crit, bool miss, bool elementalCrit)
	{
		// Check if hit is missed
		if (miss)
		{
			if (skill == null)
			{
				if (target.isPlayer())
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.C1_HAS_EVADED_C2_S_ATTACK);
					sm.addPcName(target.getActingPlayer());
					sm.addString(getName());
					target.sendPacket(sm);
				}
				SystemMessage sm = new SystemMessage(SystemMessageId.C1_S_ATTACK_WENT_ASTRAY);
				sm.addPcName(this);
				sendPacket(sm);
			}
			else
			{
				sendPacket(new ExMagicAttackInfo(getObjectId(), target.getObjectId(), ExMagicAttackInfo.EVADED));
			}
			return;
		}
		
		// Check if hit is critical
		if (crit)
		{
			if ((skill == null) || !skill.isMagic())
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.C1_LANDED_A_CRITICAL_HIT);
				sm.addPcName(this);
				sendPacket(sm);
			}
			else
			{
				sendPacket(SystemMessageId.M_CRITICAL);
			}
			
			if (skill != null)
			{
				if (skill.isMagic())
				{
					sendPacket(new ExMagicAttackInfo(getObjectId(), target.getObjectId(), ExMagicAttackInfo.M_CRITICAL));
				}
				else if (skill.isPhysical())
				{
					sendPacket(new ExMagicAttackInfo(getObjectId(), target.getObjectId(), ExMagicAttackInfo.P_CRITICAL));
				}
				else
				{
					sendPacket(new ExMagicAttackInfo(getObjectId(), target.getObjectId(), ExMagicAttackInfo.CRITICAL));
				}
			}
		}
		
		if (elementalCrit)
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.S1_ATTACK_CRITICAL_IS_ACTIVATED);
			sm.addElementalSpirit(getActiveElementalSpiritType());
			sendPacket(sm);
		}
		
		if (isInOlympiadMode() && target.isPlayer() && target.getActingPlayer().isInOlympiadMode() && (target.getActingPlayer().getOlympiadGameId() == getOlympiadGameId()))
		{
			OlympiadGameManager.getInstance().notifyCompetitorDamage(this, damage);
		}
		
		if ((target.isHpBlocked() && !target.isNpc()) || (target.isPlayer() && target.isAffected(EffectFlag.DUELIST_FURY) && !isAffected(EffectFlag.FACEOFF)) || target.isInvul())
		{
			sendPacket(SystemMessageId.THE_ATTACK_HAS_BEEN_BLOCKED);
		}
		else if (target.isDoor() || (target is ControlTower))
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.YOU_VE_HIT_FOR_S1_DAMAGE);
			sm.addInt(damage);
			sendPacket(sm);
		}
		else if (this != target)
		{
			SystemMessage sm;
			if (elementalDamage != 0)
			{
				sm = new SystemMessage(SystemMessageId.S1_HAS_DEALT_S3_DAMAGE_TO_S2_S4_ATTRIBUTE_DAMAGE);
			}
			else
			{
				sm = new SystemMessage(SystemMessageId.C1_HAS_DEALT_S3_DAMAGE_TO_C2);
			}
			
			sm.addPcName(this);
			
			// Localisation related.
			String targetName = target.getName();
			if (Config.MULTILANG_ENABLE && target.isNpc())
			{
				String[] localisation = NpcNameLocalisationData.getInstance().getLocalisation(_lang, target.getId());
				if (localisation != null)
				{
					targetName = localisation[0];
				}
			}
			
			sm.addString(targetName);
			sm.addInt(damage);
			if (elementalDamage != 0)
			{
				sm.addInt((int) elementalDamage);
			}
			sm.addPopup(target.getObjectId(), getObjectId(), -damage);
			sendPacket(sm);
		}
	}
	
	/**
	 * @param npcId
	 */
	public void setAgathionId(int npcId)
	{
		_agathionId = npcId;
	}
	
	/**
	 * @return
	 */
	public int getAgathionId()
	{
		return _agathionId;
	}
	
	public int getVitalityPoints()
	{
		return getStat().getVitalityPoints();
	}
	
	public void setVitalityPoints(int points, bool quiet)
	{
		getStat().setVitalityPoints(points, quiet);
	}
	
	public void updateVitalityPoints(int points, bool useRates, bool quiet)
	{
		if ((_huntPass != null) && _huntPass.toggleSayha())
		{
			return;
		}
		getStat().updateVitalityPoints(points, useRates, quiet);
	}
	
	public void setSayhaGraceSupportEndTime(long endTime)
	{
		if (getVariables().getLong(PlayerVariables.SAYHA_GRACE_SUPPORT_ENDTIME, 0) < System.currentTimeMillis())
		{
			getVariables().set(PlayerVariables.SAYHA_GRACE_SUPPORT_ENDTIME, endTime);
			sendPacket(new ExUserBoostStat(this, BonusExpType.VITALITY));
			sendPacket(new ExVitalityEffectInfo(this));
		}
	}
	
	public DateTime getSayhaGraceSupportEndTime()
	{
		return getVariables().getLong(PlayerVariables.SAYHA_GRACE_SUPPORT_ENDTIME, 0);
	}
	
	public bool setLimitedSayhaGraceEndTime(long endTime)
	{
		if (endTime > getVariables().getLong(PlayerVariables.LIMITED_SAYHA_GRACE_ENDTIME, 0))
		{
			getVariables().set(PlayerVariables.LIMITED_SAYHA_GRACE_ENDTIME, endTime);
			sendPacket(new ExUserBoostStat(this, BonusExpType.VITALITY));
			sendPacket(new ExVitalityEffectInfo(this));
			return true;
		}
		return false;
	}
	
	public long getLimitedSayhaGraceEndTime()
	{
		return getVariables().getLong(PlayerVariables.LIMITED_SAYHA_GRACE_ENDTIME, 0);
	}
	
	public void checkItemRestriction()
	{
		for (int i = 0; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
		{
			Item equippedItem = _inventory.getPaperdollItem(i);
			if ((equippedItem != null) && !equippedItem.getTemplate().checkCondition(this, this, false))
			{
				_inventory.unEquipItemInSlot(i);
				
				InventoryUpdate iu = new InventoryUpdate();
				iu.addModifiedItem(equippedItem);
				sendInventoryUpdate(iu);
				
				SystemMessage sm = null;
				if (equippedItem.getTemplate().getBodyPart() == ItemTemplate.SLOT_BACK)
				{
					sendPacket(SystemMessageId.YOUR_CLOAK_HAS_BEEN_UNEQUIPPED_BECAUSE_YOUR_ARMOR_SET_IS_NO_LONGER_COMPLETE);
					return;
				}
				
				if (equippedItem.getEnchantLevel() > 0)
				{
					sm = new SystemMessage(SystemMessageId.S1_S2_UNEQUIPPED);
					sm.addInt(equippedItem.getEnchantLevel());
					sm.addItemName(equippedItem);
				}
				else
				{
					sm = new SystemMessage(SystemMessageId.S1_UNEQUIPPED);
					sm.addItemName(equippedItem);
				}
				sendPacket(sm);
			}
		}
	}
	
	public void addTransformSkill(Skill skill)
	{
		_transformSkills.put(skill.getId(), skill);
	}
	
	public bool hasTransformSkill(Skill skill)
	{
		return _transformSkills.get(skill.getId()) == skill;
	}
	
	public bool hasTransformSkills()
	{
		return !_transformSkills.isEmpty();
	}
	
	public ICollection<Skill> getAllTransformSkills()
	{
		return _transformSkills.values();
	}
	
	public void removeAllTransformSkills()
	{
		_transformSkills.clear();
	}
	
	/**
	 * @param skillId the id of the skill that this player might have.
	 * @return {@code skill} object refered to this skill id that this player has, {@code null} otherwise.
	 */
	public override Skill getKnownSkill(int skillId)
	{
		return !_transformSkills.isEmpty() ? _transformSkills.getOrDefault(skillId, base.getKnownSkill(skillId)) : base.getKnownSkill(skillId);
	}
	
	/**
	 * @return all visible skills that appear on Alt+K for this player.
	 */
	public ICollection<Skill> getSkillList()
	{
		ICollection<Skill> currentSkills = getAllSkills();
		if (isTransformed() && !_transformSkills.isEmpty())
		{
			// Include transformation skills and those skills that are allowed during transformation.
			List<Skill> filteredSkills = new();
			foreach (Skill skill in currentSkills)
			{
				if (!skill.allowOnTransform())
				{
					continue;
				}
				
				filteredSkills.add(skill);
			}
			currentSkills = filteredSkills;
			
			// Revelation skills.
			if (isDualClassActive())
			{
				int revelationSkill = getVariables().getInt(PlayerVariables.REVELATION_SKILL_1_DUAL_CLASS, 0);
				if (revelationSkill != 0)
				{
					addSkill(SkillData.getInstance().getSkill(revelationSkill, 1), false);
				}
				revelationSkill = getVariables().getInt(PlayerVariables.REVELATION_SKILL_2_DUAL_CLASS, 0);
				if (revelationSkill != 0)
				{
					addSkill(SkillData.getInstance().getSkill(revelationSkill, 1), false);
				}
			}
			else if (!isSubClassActive())
			{
				int revelationSkill = getVariables().getInt(PlayerVariables.REVELATION_SKILL_1_MAIN_CLASS, 0);
				if (revelationSkill != 0)
				{
					addSkill(SkillData.getInstance().getSkill(revelationSkill, 1), false);
				}
				revelationSkill = getVariables().getInt(PlayerVariables.REVELATION_SKILL_2_MAIN_CLASS, 0);
				if (revelationSkill != 0)
				{
					addSkill(SkillData.getInstance().getSkill(revelationSkill, 1), false);
				}
			}
			
			// Include transformation skills.
			currentSkills.addAll(_transformSkills.values());
		}
		
		List<Skill> finalSkills = new();
		foreach (Skill skill in currentSkills)
		{
			if ((skill == null) || skill.isBlockActionUseSkill() || SkillTreeData.getInstance().isAlchemySkill(skill.getId(), skill.getLevel()) || !skill.isDisplayInList())
			{
				continue;
			}
			
			finalSkills.add(skill);
		}
		return finalSkills;
	}
	
	protected void startFeed(int npcId)
	{
		_canFeed = npcId > 0;
		if (!isMounted())
		{
			return;
		}
		if (hasPet())
		{
			setCurrentFeed(_pet.getCurrentFed());
			_controlItemId = _pet.getControlObjectId();
			sendPacket(new SetupGauge(3, (_curFeed * 10000) / getFeedConsume(), (getMaxFeed() * 10000) / getFeedConsume()));
			if (!isDead())
			{
				_mountFeedTask = ThreadPool.scheduleAtFixedRate(new PetFeedTask(this), 10000, 10000);
			}
		}
		else if (_canFeed)
		{
			setCurrentFeed(getMaxFeed());
			sendPacket(new SetupGauge(3, (_curFeed * 10000) / getFeedConsume(), (getMaxFeed() * 10000) / getFeedConsume()));
			if (!isDead())
			{
				_mountFeedTask = ThreadPool.scheduleAtFixedRate(new PetFeedTask(this), 10000, 10000);
			}
		}
	}
	
	public void stopFeed()
	{
		if (_mountFeedTask != null)
		{
			_mountFeedTask.cancel(false);
			_mountFeedTask = null;
		}
	}
	
	private void clearPetData()
	{
		_data = null;
	}
	
	public PetData getPetData(int npcId)
	{
		if (_data == null)
		{
			_data = PetDataTable.getInstance().getPetData(npcId);
		}
		return _data;
	}
	
	private PetLevelData getPetLevelData(int npcId)
	{
		if (_leveldata == null)
		{
			_leveldata = PetDataTable.getInstance().getPetData(npcId).getPetLevelData(getMountLevel());
		}
		return _leveldata;
	}
	
	public int getCurrentFeed()
	{
		return _curFeed;
	}
	
	public int getFeedConsume()
	{
		// if pet is attacking
		if (isAttackingNow())
		{
			return getPetLevelData(_mountNpcId).getPetFeedBattle();
		}
		return getPetLevelData(_mountNpcId).getPetFeedNormal();
	}
	
	public void setCurrentFeed(int num)
	{
		bool lastHungryState = isHungry();
		_curFeed = num > getMaxFeed() ? getMaxFeed() : num;
		sendPacket(new SetupGauge(3, (_curFeed * 10000) / getFeedConsume(), (getMaxFeed() * 10000) / getFeedConsume()));
		// broadcast move speed change when strider becomes hungry / full
		if (lastHungryState != isHungry())
		{
			broadcastUserInfo();
		}
	}
	
	private int getMaxFeed()
	{
		return getPetLevelData(_mountNpcId).getPetMaxFeed();
	}
	
	public bool isHungry()
	{
		return hasPet() && _canFeed && (_curFeed < ((getPetData(_mountNpcId).getHungryLimit() / 100f) * getPetLevelData(_mountNpcId).getPetMaxFeed()));
	}
	
	public void enteredNoLanding(int delay)
	{
		_dismountTask = ThreadPool.schedule(new DismountTask(this), delay * 1000);
	}
	
	public void exitedNoLanding()
	{
		if (_dismountTask != null)
		{
			_dismountTask.cancel(true);
			_dismountTask = null;
		}
	}
	
	public void storePetFood(int petId)
	{
		if ((_controlItemId != 0) && (petId != 0))
		{
			String req;
			req = "UPDATE pets SET fed=? WHERE item_obj_id = ?";
			try
			{
                using GameServerDbContext ctx = new();
                				PreparedStatement statement = con.prepareStatement(req);
				statement.setInt(1, _curFeed);
				statement.setInt(2, _controlItemId);
				statement.executeUpdate();
				_controlItemId = 0;
			}
			catch (Exception e)
			{
				LOGGER.Error("Failed to store Pet [NpcId: " + petId + "] data: " + e);
			}
		}
	}
	
	public void setInSiege(bool value)
	{
		_isInSiege = value;
	}
	
	public bool isInSiege()
	{
		return _isInSiege;
	}
	
	/**
	 * @param isInHideoutSiege sets the value of {@link #_isInHideoutSiege}.
	 */
	public void setInHideoutSiege(bool isInHideoutSiege)
	{
		_isInHideoutSiege = isInHideoutSiege;
	}
	
	/**
	 * @return the value of {@link #_isInHideoutSiege}, {@code true} if the player is participing on a Hideout Siege, otherwise {@code false}.
	 */
	public bool isInHideoutSiege()
	{
		return _isInHideoutSiege;
	}
	
	public bool isFlyingMounted()
	{
		return checkTransformed(x => x.isFlying());
	}
	
	/**
	 * Returns the Number of Charges this Player got.
	 * @return
	 */
	public int getCharges()
	{
		return _charges.get();
	}
	
	public void setCharges(int count)
	{
		restartChargeTask();
		_charges.set(count);
	}
	
	public bool decreaseCharges(int count)
	{
		if (_charges.get() < count)
		{
			return false;
		}
		
		// Charge clear task should be reset every time a charge is decreased and stopped when charges become 0.
		if (_charges.addAndGet(-count) == 0)
		{
			stopChargeTask();
		}
		else
		{
			restartChargeTask();
		}
		
		sendPacket(new EtcStatusUpdate(this));
		return true;
	}
	
	public void clearCharges()
	{
		_charges.set(0);
		sendPacket(new EtcStatusUpdate(this));
	}
	
	/**
	 * Starts/Restarts the ChargeTask to Clear Charges after 10 Mins.
	 */
	private void restartChargeTask()
	{
		if (_chargeTask != null)
		{
			_chargeTask.cancel(false);
			_chargeTask = null;
		}
		_chargeTask = ThreadPool.schedule(new ResetChargesTask(this), 600000);
	}
	
	/**
	 * Stops the Charges Clearing Task.
	 */
	public void stopChargeTask()
	{
		if (_chargeTask != null)
		{
			_chargeTask.cancel(false);
			_chargeTask = null;
		}
	}
	
	public void teleportBookmarkModify(int id, int icon, String tag, String name)
	{
		if (isInsideZone(ZoneId.TIMED_HUNTING))
		{
			return;
		}
		
		TeleportBookmark bookmark = _tpbookmarks.get(id);
		if (bookmark != null)
		{
			bookmark.setIcon(icon);
			bookmark.setTag(tag);
			bookmark.setName(name);
			
			try 
			{
                using GameServerDbContext ctx = new();
                				PreparedStatement statement = con.prepareStatement(UPDATE_TP_BOOKMARK);
				statement.setInt(1, icon);
				statement.setString(2, tag);
				statement.setString(3, name);
				statement.setInt(4, getObjectId());
				statement.setInt(5, id);
				statement.execute();
			}
			catch (Exception e)
			{
				LOGGER.Warn("Could not update character teleport bookmark data: " + e);
			}
		}
		
		sendPacket(new ExGetBookMarkInfoPacket(this));
	}
	
	public void teleportBookmarkDelete(int id)
	{
		if (_tpbookmarks.remove(id) != null)
		{
			try
			{
				using GameServerDbContext ctx = new();
				PreparedStatement statement = con.prepareStatement(DELETE_TP_BOOKMARK);
				statement.setInt(1, getObjectId());
				statement.setInt(2, id);
				statement.execute();
			}
			catch (Exception e)
			{
				LOGGER.Warn("Could not delete character teleport bookmark data: " + e);
			}
			
			sendPacket(new ExGetBookMarkInfoPacket(this));
		}
	}
	
	public void teleportBookmarkGo(int id)
	{
		if (!teleportBookmarkCondition(0))
		{
			return;
		}
		if (_inventory.getInventoryItemCount(13016, 0) == 0)
		{
			sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_BECAUSE_YOU_DO_NOT_HAVE_A_TELEPORT_ITEM);
			return;
		}
		
		SystemMessage sm = new SystemMessage(SystemMessageId.S1_DISAPPEARED);
		sm.addItemName(13016);
		sendPacket(sm);
		
		TeleportBookmark bookmark = _tpbookmarks.get(id);
		if (bookmark != null)
		{
			if (isInTimedHuntingZone(bookmark.getX(), bookmark.getY()))
			{
				sendMessage("You cannot teleport at this location.");
				return;
			}
			
			destroyItem("Consume", _inventory.getItemByItemId(13016).getObjectId(), 1, null, false);
			setTeleportLocation(bookmark);
			doCast(CommonSkill.MY_TELEPORT.getSkill());
		}
		sendPacket(new ExGetBookMarkInfoPacket(this));
	}
	
	public bool teleportBookmarkCondition(int type)
	{
		if (isInCombat())
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_MY_TELEPORTS_DURING_A_BATTLE);
			return false;
		}
		else if (_isInSiege || (_siegeState != 0))
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_MY_TELEPORTS_WHILE_PARTICIPATING_A_LARGE_SCALE_BATTLE_SUCH_AS_A_CASTLE_SIEGE_FORTRESS_SIEGE_OR_CLAN_HALL_SIEGE);
			return false;
		}
		else if (_isInDuel || _startingDuel)
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_MY_TELEPORTS_DURING_A_DUEL);
			return false;
		}
		else if (isFlying())
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_MY_TELEPORTS_WHILE_FLYING);
			return false;
		}
		else if (_inOlympiadMode)
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_MY_TELEPORTS_WHILE_PARTICIPATING_IN_AN_OLYMPIAD_MATCH);
			return false;
		}
		else if (hasBlockActions() && hasAbnormalType(AbnormalType.PARALYZE))
		{
			sendPacket(SystemMessageId.CANNOT_TELEPORT_WHILE_PETRIFIED_OR_PARALYZED);
			return false;
		}
		else if (isDead())
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_TELEPORT_WHILE_YOU_ARE_DEAD);
			return false;
		}
		else if (isInWater())
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_TELEPORT_UNDERWATER);
			return false;
		}
		else if ((type == 1) && (isInsideZone(ZoneId.SIEGE) || isInsideZone(ZoneId.CLAN_HALL) || isInsideZone(ZoneId.JAIL) || isInsideZone(ZoneId.CASTLE) || isInsideZone(ZoneId.NO_SUMMON_FRIEND) || isInsideZone(ZoneId.FORT)))
		{
			sendPacket(SystemMessageId.YOU_CANNOT_USE_MY_TELEPORTS_TO_REACH_THIS_AREA);
			return false;
		}
		else if (isInsideZone(ZoneId.NO_BOOKMARK) || isInBoat() || isInAirShip() || isInsideZone(ZoneId.TIMED_HUNTING))
		{
			if (type == 0)
			{
				sendPacket(SystemMessageId.YOU_CANNOT_USE_TELEPORT_IN_THIS_AREA);
			}
			else if (type == 1)
			{
				sendPacket(SystemMessageId.YOU_CANNOT_USE_MY_TELEPORTS_TO_REACH_THIS_AREA);
			}
			return false;
		}
		/*
		 * TODO: Instant Zone still not implemented else if (isInsideZone(ZoneId.INSTANT)) { sendPacket(new SystemMessage(2357)); return; }
		 */
		else
		{
			return true;
		}
	}
	
	public void teleportBookmarkAdd(int x, int y, int z, int icon, String tag, String name)
	{
		if (!teleportBookmarkCondition(1))
		{
			return;
		}
		
		if (isInsideZone(ZoneId.TIMED_HUNTING))
		{
			return;
		}
		
		if (_tpbookmarks.size() >= _bookmarkslot)
		{
			sendPacket(SystemMessageId.YOU_HAVE_NO_SPACE_TO_SAVE_THE_TELEPORT_LOCATION);
			return;
		}
		
		if (Config.BOOKMARK_CONSUME_ITEM_ID > 0)
		{
			if (_inventory.getInventoryItemCount(Config.BOOKMARK_CONSUME_ITEM_ID, -1) == 0)
			{
				if (Config.BOOKMARK_CONSUME_ITEM_ID == 20033)
				{
					sendPacket(SystemMessageId.YOU_CANNOT_BOOKMARK_THIS_LOCATION_BECAUSE_YOU_DO_NOT_HAVE_A_MY_TELEPORT_FLAG);
				}
				else
				{
					sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
				}
				return;
			}
			
			destroyItem("Consume", _inventory.getItemByItemId(Config.BOOKMARK_CONSUME_ITEM_ID).getObjectId(), 1, null, true);
		}
		
		int id;
		for (id = 1; id <= _bookmarkslot; ++id)
		{
			if (!_tpbookmarks.containsKey(id))
			{
				break;
			}
		}
		_tpbookmarks.put(id, new TeleportBookmark(id, x, y, z, icon, tag, name));
		
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(INSERT_TP_BOOKMARK);
			statement.setInt(1, getObjectId());
			statement.setInt(2, id);
			statement.setInt(3, x);
			statement.setInt(4, y);
			statement.setInt(5, z);
			statement.setInt(6, icon);
			statement.setString(7, tag);
			statement.setString(8, name);
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not insert character teleport bookmark data: " + e);
		}
		sendPacket(new ExGetBookMarkInfoPacket(this));
	}
	
	public void restoreTeleportBookmark()
	{
		try
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(RESTORE_TP_BOOKMARK);
			statement.setInt(1, getObjectId());
			
			{
                ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					_tpbookmarks.put(rset.getInt("Id"), new TeleportBookmark(rset.getInt("Id"), rset.getInt("x"), rset.getInt("y"), rset.getInt("z"), rset.getInt("icon"), rset.getString("tag"), rset.getString("name")));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed restoing character teleport bookmark: " + e);
		}
	}
	
	public ICollection<TeleportBookmark> getTeleportBookmarks()
	{
		return _tpbookmarks.values();
	}
	
	public override void sendInfo(Player player)
	{
		if (isInBoat())
		{
			setXYZ(getBoat().getLocation());
			player.sendPacket(new CharInfo(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
			player.sendPacket(new GetOnVehicle(getObjectId(), getBoat().getObjectId(), _inVehiclePosition));
		}
		else if (isInAirShip())
		{
			setXYZ(getAirShip().getLocation());
			player.sendPacket(new CharInfo(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
			player.sendPacket(new ExGetOnAirShip(this, getAirShip()));
		}
		else
		{
			player.sendPacket(new CharInfo(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
		}
		
		long relation1 = getRelation(player);
		RelationChanged rc1 = new RelationChanged();
		rc1.addRelation(this, relation1, !isInsideZone(ZoneId.PEACE) || !isInsideZone(ZoneId.NO_PVP));
		if (hasSummon())
		{
			if (_pet != null)
			{
				rc1.addRelation(_pet, relation1, !isInsideZone(ZoneId.PEACE) || !isInsideZone(ZoneId.NO_PVP));
			}
			if (hasServitors())
			{
				getServitors().values().forEach(s => rc1.addRelation(s, relation1, !isInsideZone(ZoneId.PEACE) || !isInsideZone(ZoneId.NO_PVP)));
			}
		}
		player.sendPacket(rc1);
		
		long relation2 = player.getRelation(this);
		RelationChanged rc2 = new RelationChanged();
		rc2.addRelation(player, relation2, !player.isInsideZone(ZoneId.PEACE));
		if (player.hasSummon())
		{
			if (_pet != null)
			{
				rc2.addRelation(_pet, relation2, !player.isInsideZone(ZoneId.PEACE));
			}
			if (hasServitors())
			{
				getServitors().values().forEach(s => rc2.addRelation(s, relation2, !player.isInsideZone(ZoneId.PEACE)));
			}
		}
		sendPacket(rc2);
		
		switch (_privateStoreType)
		{
			case PrivateStoreType.SELL:
			{
				player.sendPacket(new PrivateStoreMsgSell(this));
				break;
			}
			case PrivateStoreType.PACKAGE_SELL:
			{
				player.sendPacket(new ExPrivateStoreSetWholeMsg(this));
				break;
			}
			case PrivateStoreType.BUY:
			{
				player.sendPacket(new PrivateStoreMsgBuy(this));
				break;
			}
			case PrivateStoreType.MANUFACTURE:
			{
				player.sendPacket(new RecipeShopMsg(this));
				break;
			}
		}
		
		// Required for showing mount transformations to players that just entered the game.
		if (isTransformed())
		{
			player.sendPacket(new CharInfo(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
		}
	}
	
	public void playMovie(MovieHolder holder)
	{
		if (_movieHolder != null)
		{
			return;
		}
		abortAttack();
		// abortCast(); Confirmed in retail, playing a movie does not abort cast.
		stopMove(null);
		setMovieHolder(holder);
		if (!isTeleporting())
		{
			sendPacket(new ExStartScenePlayer(holder.getMovie()));
		}
	}
	
	public void stopMovie()
	{
		sendPacket(new ExStopScenePlayer(_movieHolder.getMovie()));
		setMovieHolder(null);
	}
	
	public bool isAllowedToEnchantSkills()
	{
		if (isSubclassLocked())
		{
			return false;
		}
		if (isTransformed())
		{
			return false;
		}
		if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(this))
		{
			return false;
		}
		if (isCastingNow())
		{
			return false;
		}
		if (isInBoat() || isInAirShip())
		{
			return false;
		}
		return true;
	}
	
	/**
	 * Set the _createDate of the Player.
	 * @param createDate
	 */
	public void setCreateDate(DateTime createDate)
	{
		_createDate = createDate;
	}
	
	/**
	 * @return the _createDate of the Player.
	 */
	public DateTime getCreateDate()
	{
		return _createDate;
	}
	
	/**
	 * @return number of days to char birthday.
	 */
	public int checkBirthDay()
	{
		// "Characters with a February 29 creation date will receive a gift on February 28."
		if (_createDate.Month == 2 && _createDate.Day == 29)
		{
			_createDate = _createDate.AddDays(-1);
		}

		DateTime date = _createDate;
		DateTime today = DateTime.Today;
		if (date.Day == today.Day && date.Month == today.Month && date.Year != today.Year)
		{
			return 0;
		}
		
		for (int i = 1; i < 6; i++)
		{
			today = today.AddDays(1);
			if (date.Day == today.Day && date.Month == today.Month && date.Year != today.Year)
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public int getBirthdays()
	{
		long time = (System.currentTimeMillis() - _createDate.getTimeInMillis()) / 1000;
		time /= TimeUnit.DAYS.toMillis(365);
		return (int) time;
	}
	
	/**
	 * list of character friends
	 */
	private Set<int> _friendList = new();
	private Set<int> _surveillanceList = new();
	
	public Set<int> getFriendList()
	{
		return _friendList;
	}
	
	public void restoreFriendList()
	{
		_friendList.clear();
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement("SELECT friendId FROM character_friends WHERE charId=? AND relation=0");
			statement.setInt(1, getObjectId());

			{
				ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					int friendId = rset.getInt("friendId");
					if (friendId == getObjectId())
					{
						continue;
					}
					_friendList.add(friendId);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error found in " + getName() + "'s FriendList: " + e);
		}
	}
	
	public void notifyFriends(int type)
	{
		FriendStatus pkt = new FriendStatus(this, type);
		foreach (int id in _friendList)
		{
			Player friend = World.getInstance().getPlayer(id);
			if (friend != null)
			{
				friend.sendPacket(pkt);
			}
		}
	}
	
	public Set<int> getSurveillanceList()
	{
		return _surveillanceList;
	}
	
	public void restoreSurveillanceList()
	{
		_surveillanceList.clear();
		try
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement("SELECT targetId FROM character_surveillances WHERE charId=?");
			statement.setInt(1, getObjectId());
			
			{
                ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					int friendId = rset.getInt("targetId");
					if (friendId == getObjectId())
					{
						continue;
					}
					_surveillanceList.add(friendId);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error found in " + getName() + "'s SurveillanceList: " + e);
		}
	}
	
	public void updateFriendMemo(String name, String memo)
	{
		if (memo.Length > 50)
		{
			return;
		}
		
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement("UPDATE character_friends SET memo=? WHERE charId=? AND friendId=?");
			int friendId = CharInfoTable.getInstance().getIdByName(name);
			statement.setString(1, memo);
			statement.setInt(2, getObjectId());
			statement.setInt(3, friendId);
			statement.execute();
			
			CharInfoTable.getInstance().setFriendMemo(getObjectId(), friendId, memo);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error occurred while updating friend memo: " + e);
		}
	}
	
	/**
	 * Verify if this player is in silence mode.
	 * @return the {@code true} if this player is in silence mode, {@code false} otherwise
	 */
	public bool isSilenceMode()
	{
		return _silenceMode;
	}
	
	/**
	 * While at silenceMode, checks if this player blocks PMs for this user
	 * @param playerObjId the player object Id
	 * @return {@code true} if the given Id is not excluded and this player is in silence mode, {@code false} otherwise
	 */
	public bool isSilenceMode(int playerObjId)
	{
		if (Config.SILENCE_MODE_EXCLUDE && _silenceMode && (_silenceModeExcluded != null))
		{
			return !_silenceModeExcluded.Contains(playerObjId);
		}
		return _silenceMode;
	}
	
	/**
	 * Set the silence mode.
	 * @param mode the value
	 */
	public void setSilenceMode(bool mode)
	{
		_silenceMode = mode;
		if (_silenceModeExcluded != null)
		{
			_silenceModeExcluded.Clear(); // Clear the excluded list on each setSilenceMode
		}
		sendPacket(new EtcStatusUpdate(this));
	}
	
	/**
	 * Add a player to the "excluded silence mode" list.
	 * @param playerObjId the player's object Id
	 */
	public void addSilenceModeExcluded(int playerObjId)
	{
		if (_silenceModeExcluded == null)
		{
			_silenceModeExcluded = new();
		}
		_silenceModeExcluded.add(playerObjId);
	}
	
	private void storeRecipeShopList()
	{
		if (hasManufactureShop())
		{
			try
			{
                using GameServerDbContext ctx = new();
				
				{
                    PreparedStatement st = con.prepareStatement(DELETE_CHAR_RECIPE_SHOP);
					st.setInt(1, getObjectId());
					st.execute();
				}
				
				try
				{
                    PreparedStatement st = con.prepareStatement(INSERT_CHAR_RECIPE_SHOP);
					AtomicInteger slot = new AtomicInteger(1);
					con.setAutoCommit(false);
					foreach (ManufactureItem item in _manufactureItems.values())
					{
						st.setInt(1, getObjectId());
						st.setInt(2, item.getRecipeId());
						st.setLong(3, item.getCost());
						st.setInt(4, slot.getAndIncrement());
						st.addBatch();
					}
					st.executeBatch();
					con.commit();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not store recipe shop for playerId " + getObjectId() + ": " + e);
			}
		}
	}
	
	private void restoreRecipeShopList()
	{
		if (_manufactureItems != null)
		{
			_manufactureItems.clear();
		}
		
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(RESTORE_CHAR_RECIPE_SHOP);
			statement.setInt(1, getObjectId());
			
			{
                ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					getManufactureItems().put(rset.getInt("recipeId"), new ManufactureItem(rset.getInt("recipeId"), rset.getLong("price")));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore recipe shop list data for playerId: " + getObjectId() + ": " + e);
		}
	}
	
	public override float getCollisionRadius()
	{
		if (isMounted() && (_mountNpcId > 0))
		{
			return NpcData.getInstance().getTemplate(getMountNpcId()).getFCollisionRadius();
		}
		
		float defaultCollisionRadius = _appearance.isFemale() ? getBaseTemplate().getFCollisionRadiusFemale() : getBaseTemplate().getFCollisionRadius();
		Transform? transform = getTransformation();
		if (transform != null)
			return transform.getCollisionRadius(this, defaultCollisionRadius);

		return defaultCollisionRadius;
	}
	
	public override float getCollisionHeight()
	{
		if (isMounted() && (_mountNpcId > 0))
		{
			return NpcData.getInstance().getTemplate(getMountNpcId()).getFCollisionHeight();
		}
		
		float defaultCollisionHeight = _appearance.isFemale() ? getBaseTemplate().getFCollisionHeightFemale() : getBaseTemplate().getFCollisionHeight();
		Transform? transform = getTransformation();
		if (transform != null)
			return transform.getCollisionHeight(this, defaultCollisionHeight);

		return defaultCollisionHeight;
	}
	
	public int getClientX()
	{
		return _clientX;
	}
	
	public int getClientY()
	{
		return _clientY;
	}
	
	public int getClientZ()
	{
		return _clientZ;
	}
	
	public int getClientHeading()
	{
		return _clientHeading;
	}
	
	public void setClientX(int value)
	{
		_clientX = value;
	}
	
	public void setClientY(int value)
	{
		_clientY = value;
	}
	
	public void setClientZ(int value)
	{
		_clientZ = value;
	}
	
	public void setClientHeading(int value)
	{
		_clientHeading = value;
	}
	
	/**
	 * @param z
	 * @return true if character falling now on the start of fall return false for correct coord sync!
	 */
	public bool isFalling(int z)
	{
		if (isDead() || isFlying() || isFlyingMounted() || isInsideZone(ZoneId.WATER))
		{
			return false;
		}
		
		if ((_fallingTimestamp != DateTime.MinValue) && (DateTime.Now < _fallingTimestamp))
		{
			return true;
		}
		
		int deltaZ = getZ() - z;
		if (deltaZ <= getBaseTemplate().getSafeFallHeight())
		{
			_fallingTimestamp = 0;
			return false;
		}
		
		// If there is no geodata loaded for the place we are, client Z correction might cause falling damage.
		if (!GeoEngine.getInstance().hasGeo(getX(), getY()))
		{
			_fallingTimestamp = 0;
			return false;
		}
		
		if (_fallingDamage == 0)
		{
			_fallingDamage = (int) Formulas.calcFallDam(this, deltaZ);
		}
		if (_fallingDamageTask != null)
		{
			_fallingDamageTask.cancel(true);
		}
		_fallingDamageTask = ThreadPool.schedule(() =>
		{
			if ((_fallingDamage > 0) && !isInvul())
			{
				reduceCurrentHp(Math.Min(_fallingDamage, getCurrentHp() - 1), this, null, false, true, false, false);
				SystemMessage sm = new SystemMessage(SystemMessageId.YOU_VE_RECEIVED_S1_DAMAGE_FROM_FALLING);
				sm.addInt(_fallingDamage);
				sendPacket(sm);
			}
			_fallingDamage = 0;
			_fallingDamageTask = null;
		}, 1500);
		
		// Prevent falling under ground.
		sendPacket(new ValidateLocation(this));
		setFalling();
		
		return false;
	}
	
	/**
	 * Set falling timestamp
	 */
	public void setFalling()
	{
		_fallingTimestamp = DateTime.Now + FALLING_VALIDATION_DELAY;
	}
	
	/**
	 * @return the _movie
	 */
	public MovieHolder getMovieHolder()
	{
		return _movieHolder;
	}
	
	public void setMovieHolder(MovieHolder movie)
	{
		_movieHolder = movie;
	}
	
	/**
	 * Update last item auction request timestamp to current
	 */
	public void updateLastItemAuctionRequest()
	{
		_lastItemAuctionInfoRequest = DateTime.Now;
	}
	
	/**
	 * @return true if receiving item auction requests<br>
	 *         (last request was in 2 seconds before)
	 */
	public bool isItemAuctionPolling()
	{
		return (DateTime.Now - _lastItemAuctionInfoRequest) < 2000;
	}
	
	public override bool isMovementDisabled()
	{
		return base.isMovementDisabled() || (_movieHolder != null) || _fishing.isFishing();
	}
	
	public String getHtmlPrefix()
	{
		if (!Config.MULTILANG_ENABLE)
		{
			return "";
		}
		return _htmlPrefix;
	}
	
	public String getLang()
	{
		return _lang;
	}
	
	public bool setLang(String lang)
	{
		bool result = false;
		if (Config.MULTILANG_ENABLE)
		{
			if (Config.MULTILANG_ALLOWED.Contains(lang))
			{
				_lang = lang;
				result = true;
			}
			else
			{
				_lang = Config.MULTILANG_DEFAULT;
			}
			
			_htmlPrefix = _lang.equals("en") ? "" : "data/lang/" + _lang + "/";
		}
		else
		{
			_lang = null;
			_htmlPrefix = "";
		}
		
		return result;
	}
	
	public long getOfflineStartTime()
	{
		return _offlineShopStart;
	}
	
	public void setOfflineStartTime(long time)
	{
		_offlineShopStart = time;
	}
	
	public int getPcCafePoints()
	{
		return _pcCafePoints;
	}
	
	public void setPcCafePoints(int count)
	{
		_pcCafePoints = count < Config.PC_CAFE_MAX_POINTS ? count : Config.PC_CAFE_MAX_POINTS;
	}
	
	public long getHonorCoins()
	{
		return getVariables().getLong("HONOR_COINS", 0);
	}
	
	public void setHonorCoins(long value)
	{
		getVariables().set("HONOR_COINS", value);
		sendPacket(new ExPledgeCoinInfo(this));
	}
	
	/**
	 * Check all player skills for skill level. If player level is lower than skill learn level - 9, skill level is decreased to next possible level.
	 */
	public void checkPlayerSkills()
	{
		SkillLearn learn;
		foreach (var e in getSkills())
		{
			learn = SkillTreeData.getInstance().getClassSkill(e.Key, e.Value.getLevel() % 100, getClassId());
			if (learn != null)
			{
				int levelDiff = e.Key == (int)CommonSkill.EXPERTISE ? 0 : 9;
				if (getLevel() < (learn.getGetLevel() - levelDiff))
				{
					deacreaseSkillLevel(e.Value, levelDiff);
				}
			}
		}
	}
	
	private void deacreaseSkillLevel(Skill skill, int levelDiff)
	{
		int nextLevel = -1;
		Map<long, SkillLearn> skillTree = SkillTreeData.getInstance().getCompleteClassSkillTree(getClassId());
		foreach (SkillLearn sl in skillTree.values())
		{
			if ((sl.getSkillId() == skill.getId()) && (nextLevel < sl.getSkillLevel()) && (getLevel() >= (sl.getGetLevel() - levelDiff)))
			{
				nextLevel = sl.getSkillLevel(); // next possible skill level
			}
		}
		
		if (nextLevel == -1)
		{
			LOGGER.Info("Removing skill " + skill + " from " + this);
			removeSkill(skill, true); // there is no lower skill
		}
		else
		{
			LOGGER.Info("Decreasing skill " + skill + " to " + nextLevel + " for " + this);
			addSkill(SkillData.getInstance().getSkill(skill.getId(), nextLevel), true); // replace with lower one
		}
	}
	
	public bool canMakeSocialAction()
	{
		return ((_privateStoreType == PrivateStoreType.NONE) && (getActiveRequester() == null) && !isAlikeDead() && !isAllSkillsDisabled() && !isCastingNow() && (getAI().getIntention() == CtrlIntention.AI_INTENTION_IDLE));
	}
	
	public void setMultiSocialAction(int id, int targetId)
	{
		_multiSociaAction = id;
		_multiSocialTarget = targetId;
	}
	
	public int getMultiSociaAction()
	{
		return _multiSociaAction;
	}
	
	public int getMultiSocialTarget()
	{
		return _multiSocialTarget;
	}
	
	public int getQuestInventoryLimit()
	{
		return Config.INVENTORY_MAXIMUM_QUEST_ITEMS;
	}
	
	public bool canAttackCreature(Creature creature)
	{
		if (creature.isAttackable())
		{
			return true;
		}
		else if (creature.isPlayable())
		{
			if (creature.isInsideZone(ZoneId.PVP) && !creature.isInsideZone(ZoneId.SIEGE))
			{
				return true;
			}
			
			Player target = creature.isSummon() ? ((Summon) creature).getOwner() : (Player) creature;
			if (isInDuel() && target.isInDuel() && (target.getDuelId() == getDuelId()))
			{
				return true;
			}
			else if (isInParty() && target.isInParty())
			{
				if (getParty() == target.getParty())
				{
					return false;
				}
				if (((getParty().getCommandChannel() != null) || (target.getParty().getCommandChannel() != null)) && (getParty().getCommandChannel() == target.getParty().getCommandChannel()))
				{
					return false;
				}
			}
			else if ((getClan() != null) && (target.getClan() != null))
			{
				if (getClanId() == target.getClanId())
				{
					return false;
				}
				if (((getAllyId() > 0) || (target.getAllyId() > 0)) && (getAllyId() == target.getAllyId()))
				{
					return false;
				}
				if (getClan().isAtWarWith(target.getClan().getId()) && target.getClan().isAtWarWith(getClan().getId()))
				{
					return true;
				}
			}
			else if ((getClan() == null) || (target.getClan() == null))
			{
				if ((!target.getPvpFlag()) && (target.getReputation() >= 0))
				{
					return false;
				}
			}
		}
		return true;
	}
	
	/**
	 * Test if player inventory is under 90% capacity
	 * @param includeQuestInv check also quest inventory
	 * @return
	 */
	public bool isInventoryUnder90(bool includeQuestInv)
	{
		return (includeQuestInv ? _inventory.getSize() : _inventory.getNonQuestSize()) <= (getInventoryLimit() * 0.9);
	}
	
	/**
	 * Test if player inventory is under 80% capacity
	 * @param includeQuestInv check also quest inventory
	 * @return
	 */
	public bool isInventoryUnder80(bool includeQuestInv)
	{
		return (includeQuestInv ? _inventory.getSize() : _inventory.getNonQuestSize()) <= (getInventoryLimit() * 0.8);
	}
	
	public bool havePetInvItems()
	{
		return _petItems;
	}
	
	public void setPetInvItems(bool haveit)
	{
		_petItems = haveit;
	}
	
	/**
	 * Restore Pet's inventory items from database.
	 */
	private void restorePetInventoryItems()
	{
		try 
        {
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement("SELECT object_id FROM `items` WHERE `owner_id`=? AND (`loc`='PET' OR `loc`='PET_EQUIP') LIMIT 1;");
			statement.setInt(1, getObjectId());
			
			{
                ResultSet rset = statement.executeQuery();
				setPetInvItems(rset.next() && (rset.getInt("object_id") > 0));
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not check Items in Pet Inventory for playerId: " + getObjectId() + ": " + e);
		}
	}
	
	public String getAdminConfirmCmd()
	{
		return _adminConfirmCmd;
	}
	
	public void setAdminConfirmCmd(String adminConfirmCmd)
	{
		_adminConfirmCmd = adminConfirmCmd;
	}
	
	/**
	 * Load Player Recommendations data.
	 */
	private void loadRecommendations()
	{
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement("SELECT rec_have, rec_left FROM character_reco_bonus WHERE charId = ?");
			statement.setInt(1, getObjectId());
			
			{
                ResultSet rset = statement.executeQuery();
				if (rset.next())
				{
					setRecomHave(rset.getInt("rec_have"));
					setRecomLeft(rset.getInt("rec_left"));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore Recommendations for player: " + getObjectId() + ": " + e);
		}
	}
	
	/**
	 * Update Player Recommendations data.
	 */
	public void storeRecommendations()
	{
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement ps = con.prepareStatement("REPLACE INTO character_reco_bonus (charId,rec_have,rec_left,time_left) VALUES (?,?,?,?)");
			ps.setInt(1, getObjectId());
			ps.setInt(2, _recomHave);
			ps.setInt(3, _recomLeft);
			ps.setLong(4, 0);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not update Recommendations for player: " + getObjectId() + ": " + e);
		}
	}
	
	public void startRecoGiveTask()
	{
		// Create task to give new recommendations
		_recoGiveTask = ThreadPool.scheduleAtFixedRate(new RecoGiveTask(this), 7200000, 3600000);
		
		// Store new data
		storeRecommendations();
	}
	
	public void stopRecoGiveTask()
	{
		if (_recoGiveTask != null)
		{
			_recoGiveTask.cancel(false);
			_recoGiveTask = null;
		}
	}
	
	public bool isRecoTwoHoursGiven()
	{
		return _recoTwoHoursGiven;
	}
	
	public void setRecoTwoHoursGiven(bool value)
	{
		_recoTwoHoursGiven = value;
	}
	
	public void setPremiumStatus(bool premiumStatus)
	{
		_premiumStatus = premiumStatus;
		sendPacket(new ExBrPremiumState(this));
	}
	
	public bool hasPremiumStatus()
	{
		return Config.PREMIUM_SYSTEM_ENABLED && _premiumStatus;
	}
	
	public void setLastPetitionGmName(String gmName)
	{
		_lastPetitionGmName = gmName;
	}
	
	public String getLastPetitionGmName()
	{
		return _lastPetitionGmName;
	}
	
	public ContactList getContactList()
	{
		return _contactList;
	}
	
	public long getNotMoveUntil()
	{
		return _notMoveUntil;
	}
	
	public void updateNotMoveUntil()
	{
		_notMoveUntil = DateTime.Now + Config.PLAYER_MOVEMENT_BLOCK_TIME;
	}
	
	public override bool isPlayer()
	{
		return true;
	}
	
	/**
	 * @param skillId the display skill Id
	 * @return the custom skill
	 */
	public Skill getCustomSkill(int skillId)
	{
		return (_customSkills != null) ? _customSkills.get(skillId) : null;
	}
	
	/**
	 * Add a skill level to the custom skills map.
	 * @param skill the skill to add
	 */
	private void addCustomSkill(Skill skill)
	{
		if ((skill != null) && (skill.getDisplayId() != skill.getId()))
		{
			if (_customSkills == null)
			{
				_customSkills = new();
			}
			_customSkills.put(skill.getDisplayId(), skill);
		}
	}
	
	/**
	 * Remove a skill level from the custom skill map.
	 * @param skill the skill to remove
	 */
	private void removeCustomSkill(Skill skill)
	{
		if ((skill != null) && (_customSkills != null) && (skill.getDisplayId() != skill.getId()))
		{
			_customSkills.remove(skill.getDisplayId());
		}
	}
	
	/**
	 * @return {@code true} if current player can revive and shows 'To Village' button upon death, {@code false} otherwise.
	 */
	public override bool canRevive()
	{
		return _canRevive;
	}
	
	/**
	 * This method can prevent from displaying 'To Village' button upon death.
	 * @param value
	 */
	public override void setCanRevive(bool value)
	{
		_canRevive = value;
	}
	
	public bool isRegisteredOnEvent()
	{
		return _isRegisteredOnEvent || _isOnEvent;
	}
	
	public void setRegisteredOnEvent(bool value)
	{
		_isRegisteredOnEvent = value;
	}
	
	public override bool isOnEvent()
	{
		return _isOnEvent;
	}
	
	public void setOnEvent(bool value)
	{
		_isOnEvent = value;
	}
	
	public bool isOnSoloEvent()
	{
		return _isOnSoloEvent;
	}
	
	public void setOnSoloEvent(bool value)
	{
		_isOnSoloEvent = value;
	}
	
	public bool isBlockedFromDeathPenalty()
	{
		return _isOnEvent || isAffected(EffectFlag.PROTECT_DEATH_PENALTY);
	}
	
	public void setOriginalCpHpMp(double cp, double hp, double mp)
	{
		_originalCp = cp;
		_originalHp = hp;
		_originalMp = mp;
	}
	
	public override void addOverrideCond(params PlayerCondOverride[] excs)
	{
		base.addOverrideCond(excs);
		getVariables().set(COND_OVERRIDE_KEY, _exceptions.ToString());
	}
	
	public override void removeOverridedCond(params PlayerCondOverride[] excs)
	{
		base.removeOverridedCond(excs);
		getVariables().set(COND_OVERRIDE_KEY, _exceptions.ToString());
	}
	
	/**
	 * @return {@code true} if {@link PlayerVariables} instance is attached to current player's scripts, {@code false} otherwise.
	 */
	public bool hasVariables()
	{
		return getScript<PlayerVariables>() != null;
	}
	
	/**
	 * @return {@link PlayerVariables} instance containing parameters regarding player.
	 */
	public PlayerVariables getVariables()
	{
		PlayerVariables vars = getScript<PlayerVariables>();
		return vars != null ? vars : addScript(new PlayerVariables(getObjectId()));
	}
	
	/**
	 * @return {@code true} if {@link AccountVariables} instance is attached to current player's scripts, {@code false} otherwise.
	 */
	public bool hasAccountVariables()
	{
		return getScript<AccountVariables>() != null;
	}
	
	/**
	 * @return {@link AccountVariables} instance containing parameters regarding player.
	 */
	public AccountVariables getAccountVariables()
	{
		AccountVariables vars = getScript<AccountVariables>();
		return vars != null ? vars : addScript(new AccountVariables(getAccountName()));
	}
	
	public override int getId()
	{
		return getClassId().getId();
	}
	
	public bool isPartyBanned()
	{
		return PunishmentManager.getInstance().hasPunishment(getObjectId(), PunishmentAffect.CHARACTER, PunishmentType.PARTY_BAN);
	}
	
	/**
	 * @param act
	 * @return {@code true} if action was added successfully, {@code false} otherwise.
	 */
	public bool addAction(PlayerAction act)
	{
		if (!hasAction(act))
		{
			_actionMask |= act.getMask();
			return true;
		}
		return false;
	}
	
	/**
	 * @param act
	 * @return {@code true} if action was removed successfully, {@code false} otherwise.
	 */
	public bool removeAction(PlayerAction act)
	{
		if (hasAction(act))
		{
			_actionMask &= ~act.getMask();
			return true;
		}
		return false;
	}
	
	/**
	 * @param act
	 * @return {@code true} if action is present, {@code false} otherwise.
	 */
	public bool hasAction(PlayerAction act)
	{
		return (_actionMask & act.getMask()) == act.getMask();
	}
	
	/**
	 * Set true/false if character got Charm of Courage
	 * @param value true/false
	 */
	public void setCharmOfCourage(bool value)
	{
		_hasCharmOfCourage = value;
	}
	
	/**
	 * @return {@code true} if effect is present, {@code false} otherwise.
	 */
	public bool hasCharmOfCourage()
	{
		return _hasCharmOfCourage;
	}
	
	public bool isGood()
	{
		return _isGood;
	}
	
	public bool isEvil()
	{
		return _isEvil;
	}
	
	public void setGood()
	{
		_isGood = true;
		_isEvil = false;
	}
	
	public void setEvil()
	{
		_isGood = false;
		_isEvil = true;
	}
	
	/**
	 * @param target the target
	 * @return {@code true} if this player got war with the target, {@code false} otherwise.
	 */
	public bool atWarWith(Playable target)
	{
		if (target == null)
		{
			return false;
		}
		if ((_clan != null) && !isAcademyMember()) // Current player
		{
			if ((target.getClan() != null) && !target.isAcademyMember()) // Target player
			{
				return _clan.isAtWarWith(target.getClan());
			}
		}
		return false;
	}
	
	/**
	 * Sets the beauty shop hair
	 * @param hairId
	 */
	public void setVisualHair(int hairId)
	{
		getVariables().set("visualHairId", hairId);
	}
	
	/**
	 * Sets the beauty shop hair color
	 * @param colorId
	 */
	public void setVisualHairColor(int colorId)
	{
		getVariables().set("visualHairColorId", colorId);
	}
	
	/**
	 * Sets the beauty shop modified face
	 * @param faceId
	 */
	public void setVisualFace(int faceId)
	{
		getVariables().set("visualFaceId", faceId);
	}
	
	/**
	 * @return the beauty shop hair, or his normal if not changed.
	 */
	public int getVisualHair()
	{
		return getVariables().getInt("visualHairId", _appearance.getHairStyle());
	}
	
	/**
	 * @return the beauty shop hair color, or his normal if not changed.
	 */
	public int getVisualHairColor()
	{
		return getVariables().getInt("visualHairColorId", _appearance.getHairColor());
	}
	
	/**
	 * @return the beauty shop modified face, or his normal if not changed.
	 */
	public int getVisualFace()
	{
		return getVariables().getInt("visualFaceId", _appearance.getFace());
	}
	
	/**
	 * @return {@code true} if player has mentees, {@code false} otherwise
	 */
	public bool isMentor()
	{
		return MentorManager.getInstance().isMentor(getObjectId());
	}
	
	/**
	 * @return {@code true} if player has mentor, {@code false} otherwise
	 */
	public bool isMentee()
	{
		return MentorManager.getInstance().isMentee(getObjectId());
	}
	
	/**
	 * @return the amount of ability points player can spend on learning skills.
	 */
	public int getAbilityPoints()
	{
		return getVariables().getInt(isDualClassActive() ? PlayerVariables.ABILITY_POINTS_DUAL_CLASS : PlayerVariables.ABILITY_POINTS_MAIN_CLASS, 0);
	}
	
	/**
	 * Sets the amount of ability points player can spend on learning skills.
	 * @param points
	 */
	public void setAbilityPoints(int points)
	{
		getVariables().set(isDualClassActive() ? PlayerVariables.ABILITY_POINTS_DUAL_CLASS : PlayerVariables.ABILITY_POINTS_MAIN_CLASS, points);
	}
	
	/**
	 * @return how much ability points player has spend on learning skills.
	 */
	public int getAbilityPointsUsed()
	{
		return getVariables().getInt(isDualClassActive() ? PlayerVariables.ABILITY_POINTS_USED_DUAL_CLASS : PlayerVariables.ABILITY_POINTS_USED_MAIN_CLASS, 0);
	}
	
	/**
	 * Sets how much ability points player has spend on learning skills.
	 * @param points
	 */
	public void setAbilityPointsUsed(int points)
	{
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_ABILITY_POINTS_CHANGED, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerAbilityPointsChanged(this, getAbilityPointsUsed(), points), this);
		}
		
		getVariables().set(isDualClassActive() ? PlayerVariables.ABILITY_POINTS_USED_DUAL_CLASS : PlayerVariables.ABILITY_POINTS_USED_MAIN_CLASS, points);
	}
	
	/**
	 * @return The amount of times player can use world chat
	 */
	public int getWorldChatPoints()
	{
		return (int) ((Config.WORLD_CHAT_POINTS_PER_DAY + getStat().getAdd(Stat.WORLD_CHAT_POINTS, 0)) * getStat().getMul(Stat.WORLD_CHAT_POINTS, 1));
	}
	
	/**
	 * @return The amount of times player has used world chat
	 */
	public int getWorldChatUsed()
	{
		return getVariables().getInt(PlayerVariables.WORLD_CHAT_VARIABLE_NAME, 0);
	}
	
	/**
	 * Sets the amount of times player can use world chat
	 * @param timesUsed how many times world chat has been used up until now.
	 */
	public void setWorldChatUsed(int timesUsed)
	{
		getVariables().set(PlayerVariables.WORLD_CHAT_VARIABLE_NAME, timesUsed);
	}
	
	/**
	 * @return Side of the player.
	 */
	public CastleSide getPlayerSide()
	{
		if (_clan == null)
		{
			return CastleSide.NEUTRAL;
		}
		
		int castleId = _clan.getCastleId();
		if (castleId == 0)
		{
			return CastleSide.NEUTRAL;
		}
		
		Castle castle = CastleManager.getInstance().getCastleById(castleId);
		if (castle == null)
		{
			return CastleSide.NEUTRAL;
		}
		
		return castle.getSide();
	}
	
	/**
	 * @return {@code true} if player is on Dark side, {@code false} otherwise.
	 */
	public bool isOnDarkSide()
	{
		return getPlayerSide() == CastleSide.DARK;
	}
	
	/**
	 * @return {@code true} if player is on Light side, {@code false} otherwise.
	 */
	public bool isOnLightSide()
	{
		return getPlayerSide() == CastleSide.LIGHT;
	}
	
	/**
	 * @return the maximum amount of points that player can use
	 */
	public int getMaxSummonPoints()
	{
		return (int) getStat().getValue(Stat.MAX_SUMMON_POINTS, 0);
	}
	
	/**
	 * @return the amount of points that player used
	 */
	public int getSummonPoints()
	{
		int totalPoints = 0;
		foreach (Summon summon in getServitors().values())
		{
			totalPoints += summon.getSummonPoints();
		}
		return totalPoints;
	}
	
	/**
	 * @param request
	 * @return {@code true} if the request was registered successfully, {@code false} otherwise.
	 */
	public bool addRequest(AbstractRequest request)
	{
		return canRequest(request) && (_requests.putIfAbsent(request.GetType(), request) == null);
	}
	
	public bool canRequest(AbstractRequest request)
	{
		foreach (AbstractRequest r in _requests.values())
		{
			if (!request.canWorkWith(r))
			{
				return false;
			}
		}
		return true;
	}
	
	/**
	 * @param clazz
	 * @return {@code true} if request was successfully removed, {@code false} in case processing set is not created or not containing the request.
	 */
	public bool removeRequest<T>()
        where T: AbstractRequest
	{
		return _requests.remove(typeof(T)) != null;
	}
	
	/**
	 * @param <T>
	 * @param requestClass
	 * @return object that is instance of {@code requestClass} param, {@code null} if not instance or not set.
	 */
	public T getRequest<T>()
        where T: AbstractRequest
	{
		return (T)(_requests.get(typeof(T)));
	}
	
	/**
	 * @return {@code true} if player has any processing request set, {@code false} otherwise.
	 */
	public bool hasRequests()
	{
		return !_requests.isEmpty();
	}
	
	public bool hasItemRequest()
	{
		foreach (AbstractRequest request in _requests.values())
		{
			if (request.isItemRequest())
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * @param requestClass
	 * @param classes
	 * @return {@code true} if player has the provided request and processing it, {@code false} otherwise.
	 */
	public bool hasRequest<T>()
		where T: AbstractRequest
	{
		return _requests.containsKey(typeof(T));
	}
	
	/**
	 * @param objectId
	 * @return {@code true} if item object id is currently in use by some request, {@code false} otherwise.
	 */
	public bool isProcessingItem(int objectId)
	{
		foreach (AbstractRequest request in _requests.values())
		{
			if (request.isUsing(objectId))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Removing all requests associated with the item object id provided.
	 * @param objectId
	 */
	public void removeRequestsThatProcessesItem(int objectId)
	{
		_requests.values().removeIf(req => req.isUsing(objectId));
	}
	
	/**
	 * @return the prime shop points of the player.
	 */
	public int getPrimePoints()
	{
		return getAccountVariables().getInt("PRIME_POINTS", 0);
	}
	
	/**
	 * Sets prime shop for current player.
	 * @param points
	 */
	public void setPrimePoints(int points)
	{
		// Immediate store upon change
		AccountVariables vars = getAccountVariables();
		vars.set("PRIME_POINTS", Math.Max(points, 0));
		vars.storeMe();
	}
	
	private TerminateReturn onExperienceReceived()
	{
		if (isDead())
		{
			return new TerminateReturn(false, false, false);
		}
		return new TerminateReturn(true, true, true);
	}
	
	public void disableExpGain()
	{
		addListener(new FunctionEventListener(this, EventType.ON_PLAYABLE_EXP_CHANGED, (OnPlayableExpChanged event) => onExperienceReceived(), this));
	}
	
	public void enableExpGain()
	{
		removeListenerIf(EventType.ON_PLAYABLE_EXP_CHANGED, listener => listener.getOwner() == this);
	}
	
	/**
	 * Gets the last commission infos.
	 * @return the last commission infos
	 */
	public Map<int, ExResponseCommissionInfo> getLastCommissionInfos()
	{
		return _lastCommissionInfos;
	}
	
	/**
	 * Gets the whisperers.
	 * @return the whisperers
	 */
	public Set<int> getWhisperers()
	{
		return _whisperers;
	}
	
	public MatchingRoom getMatchingRoom()
	{
		return _matchingRoom;
	}
	
	public void setMatchingRoom(MatchingRoom matchingRoom)
	{
		_matchingRoom = matchingRoom;
	}
	
	public bool isInMatchingRoom()
	{
		return _matchingRoom != null;
	}
	
	public int getVitalityItemsUsed()
	{
		return getVariables().getInt(PlayerVariables.VITALITY_ITEMS_USED_VARIABLE_NAME, 0);
	}
	
	public void setVitalityItemsUsed(int used)
	{
		PlayerVariables vars = getVariables();
		vars.set(PlayerVariables.VITALITY_ITEMS_USED_VARIABLE_NAME, used);
		vars.storeMe();
	}
	
	public override bool isVisibleFor(Player player)
	{
		return (base.isVisibleFor(player) || ((player.getParty() != null) && (player.getParty() == getParty())));
	}
	
	/**
	 * Set the Quest zone ID.
	 * @param id the quest zone ID
	 */
	public void setQuestZoneId(int id)
	{
		_questZoneId = id;
	}
	
	/**
	 * Gets the Quest zone ID.
	 * @return int the quest zone ID
	 */
	public int getQuestZoneId()
	{
		return _questZoneId;
	}
	
	public void sendInventoryUpdate(InventoryUpdate iu)
	{
		sendPacket(iu);
		sendPacket(new ExAdenaInvenCount(this));
		sendPacket(new ExUserInfoInvenWeight(this));
	}
	
	public void sendItemList()
	{
		if (_itemListTask == null)
		{
			_itemListTask = ThreadPool.schedule(() =>
			{
				sendPacket(new ItemList(1, this));
				sendPacket(new ItemList(2, this));
				sendPacket(new ExQuestItemList(1, this));
				sendPacket(new ExQuestItemList(2, this));
				sendPacket(new ExAdenaInvenCount(this));
				sendPacket(new ExUserInfoInvenWeight(this));
				_itemListTask = null;
			}, 300);
		}
	}
	
	public Fishing getFishing()
	{
		return _fishing;
	}
	
	public bool isFishing()
	{
		return _fishing.isFishing();
	}
	
	public override MoveType getMoveType()
	{
		if (_waitTypeSitting)
		{
			return MoveType.SITTING;
		}
		return base.getMoveType();
	}
	
	/**
	 * Precautionary method to end all tasks upon disconnection.
	 * @TODO: Rework stopAllTimers() method.
	 */
	public void stopAllTasks()
	{
		if ((_mountFeedTask != null) && !_mountFeedTask.isDone() && !_mountFeedTask.isCancelled())
		{
			_mountFeedTask.cancel(false);
			_mountFeedTask = null;
		}
		if ((_dismountTask != null) && !_dismountTask.isDone() && !_dismountTask.isCancelled())
		{
			_dismountTask.cancel(false);
			_dismountTask = null;
		}
		if ((_fameTask != null) && !_fameTask.isDone() && !_fameTask.isCancelled())
		{
			_fameTask.cancel(false);
			_fameTask = null;
		}
		if ((_teleportWatchdog != null) && !_teleportWatchdog.isDone() && !_teleportWatchdog.isCancelled())
		{
			_teleportWatchdog.cancel(false);
			_teleportWatchdog = null;
		}
		if ((_recoGiveTask != null) && !_recoGiveTask.isDone() && !_recoGiveTask.isCancelled())
		{
			_recoGiveTask.cancel(false);
			_recoGiveTask = null;
		}
		if ((_chargeTask != null) && !_chargeTask.isDone() && !_chargeTask.isCancelled())
		{
			_chargeTask.cancel(false);
			_chargeTask = null;
		}
		if ((_soulTask != null) && !_soulTask.isDone() && !_soulTask.isCancelled())
		{
			_soulTask.cancel(false);
			_soulTask = null;
		}
		if ((_taskRentPet != null) && !_taskRentPet.isDone() && !_taskRentPet.isCancelled())
		{
			_taskRentPet.cancel(false);
			_taskRentPet = null;
		}
		if ((_taskWater != null) && !_taskWater.isDone() && !_taskWater.isCancelled())
		{
			_taskWater.cancel(false);
			_taskWater = null;
		}
		if ((_fallingDamageTask != null) && !_fallingDamageTask.isDone() && !_fallingDamageTask.isCancelled())
		{
			_fallingDamageTask.cancel(false);
			_fallingDamageTask = null;
		}
		if ((_timedHuntingZoneTask != null) && !_timedHuntingZoneTask.isDone() && !_timedHuntingZoneTask.isCancelled())
		{
			_timedHuntingZoneTask.cancel(false);
			_timedHuntingZoneTask = null;
		}
		if ((_taskWarnUserTakeBreak != null) && !_taskWarnUserTakeBreak.isDone() && !_taskWarnUserTakeBreak.isCancelled())
		{
			_taskWarnUserTakeBreak.cancel(false);
			_taskWarnUserTakeBreak = null;
		}
		if ((_onlineTimeUpdateTask != null) && !_onlineTimeUpdateTask.isDone() && !_onlineTimeUpdateTask.isCancelled())
		{
			_onlineTimeUpdateTask.cancel(false);
			_onlineTimeUpdateTask = null;
		}
		foreach (var  entry in _hennaRemoveSchedules)
		{
			ScheduledFuture task = entry.Value;
			if ((task != null) && !task.isCancelled() && !task.isDone())
			{
				task.cancel(false);
			}
			_hennaRemoveSchedules.remove(entry.Key);
		}
		
		lock (_questTimers)
		{
			foreach (QuestTimer timer in _questTimers)
			{
				timer.cancelTask();
			}
			_questTimers.Clear();
		}
		
		lock (_timerHolders)
		{
			for (TimerHolder<> timer in _timerHolders)
			{
				timer.cancelTask();
			}
			_timerHolders.Clear();
		}
	}
	
	public void addQuestTimer(QuestTimer questTimer)
	{
		lock (_questTimers)
		{
			_questTimers.add(questTimer);
		}
	}
	
	public void removeQuestTimer(QuestTimer questTimer)
	{
		lock (_questTimers)
		{
			_questTimers.Remove(questTimer);
		}
	}
	
	public void addTimerHolder(TimerHolder<?> timer)
	{
		lock (_timerHolders)
		{
			_timerHolders.add(timer);
		}
	}
	
	public void removeTimerHolder(TimerHolder<?> timer)
	{
		lock (_timerHolders)
		{
			_timerHolders.Remove(timer);
		}
	}
	
	private void startOnlineTimeUpdateTask()
	{
		if (_onlineTimeUpdateTask != null)
		{
			stopOnlineTimeUpdateTask();
		}
		
		_onlineTimeUpdateTask = ThreadPool.scheduleAtFixedRate(updateOnlineTime, 60 * 1000, 60 * 1000);
	}
	
	private void updateOnlineTime()
	{
		if (_clan != null)
		{
			_clan.addMemberOnlineTime(this);
		}
	}
	
	private void stopOnlineTimeUpdateTask()
	{
		if (_onlineTimeUpdateTask != null)
		{
			_onlineTimeUpdateTask.cancel(true);
			_onlineTimeUpdateTask = null;
		}
	}
	
	public GroupType getGroupType()
	{
		return isInParty() ? (_party.isInCommandChannel() ? GroupType.COMMAND_CHANNEL : GroupType.PARTY) : GroupType.NONE;
	}
	
	public bool isTrueHero()
	{
		return _trueHero;
	}
	
	public void setTrueHero(bool value)
	{
		_trueHero = value;
	}
	
	protected override void initStatusUpdateCache()
	{
		base.initStatusUpdateCache();
		addStatusUpdateValue(StatusUpdateType.LEVEL);
		addStatusUpdateValue(StatusUpdateType.MAX_CP);
		addStatusUpdateValue(StatusUpdateType.CUR_CP);
		if (_isDeathKnight)
		{
			addStatusUpdateValue(StatusUpdateType.CUR_DP);
		}
		else if (_isVanguard)
		{
			addStatusUpdateValue(StatusUpdateType.CUR_BP);
		}
		else if (_isAssassin)
		{
			addStatusUpdateValue(StatusUpdateType.CUR_AP);
		}
	}
	
	public TrainingHolder getTraingCampInfo()
	{
		String info = getAccountVariables().getString(TRAINING_CAMP_VAR, null);
		if (info == null)
		{
			return null;
		}
		return new TrainingHolder(int.Parse(info.Split(";")[0]), int.Parse(info.Split(";")[1]), int.Parse(info.Split(";")[2]), long.Parse(info.Split(";")[3]), long.Parse(info.Split(";")[4]));
	}
	
	public void setTraingCampInfo(TrainingHolder holder)
	{
		getAccountVariables().set(TRAINING_CAMP_VAR, holder.getObjectId() + ";" + holder.getClassIndex() + ";" + holder.getLevel() + ";" + holder.getStartTime() + ";" + holder.getEndTime());
	}
	
	public void removeTraingCampInfo()
	{
		getAccountVariables().remove(TRAINING_CAMP_VAR);
	}
	
	public long getTraingCampDuration()
	{
		return getAccountVariables().getLong(TRAINING_CAMP_DURATION, 0);
	}
	
	public void setTraingCampDuration(long duration)
	{
		getAccountVariables().set(TRAINING_CAMP_DURATION, duration);
	}
	
	public void resetTraingCampDuration()
	{
		getAccountVariables().remove(TRAINING_CAMP_DURATION);
	}
	
	public bool isInTraingCamp()
	{
		TrainingHolder trainingHolder = getTraingCampInfo();
		return (trainingHolder != null) && (trainingHolder.getEndTime() > DateTime.Now);
	}
	
	public AttendanceInfoHolder getAttendanceInfo()
	{
		// Get reset time.
        DateTime time = DateTime.Now;
        if (time.Hour < 6 && time.Minute < 30)
        {
	        time = time.AddDays(-1);
        }

        time = new DateTime(time.Year, time.Month, time.Day, 6, 30, 0);
		
		// Get last player reward time.
		long receiveDate;
		int rewardIndex;
		if (Config.ATTENDANCE_REWARDS_SHARE_ACCOUNT)
		{
			receiveDate = getAccountVariables().getLong(PlayerVariables.ATTENDANCE_DATE, 0);
			rewardIndex = getAccountVariables().getInt(PlayerVariables.ATTENDANCE_INDEX, 0);
		}
		else
		{
			receiveDate = getVariables().getLong(PlayerVariables.ATTENDANCE_DATE, 0);
			rewardIndex = getVariables().getInt(PlayerVariables.ATTENDANCE_INDEX, 0);
		}
		
		// Check if player can receive reward today.
		bool canBeRewarded = false;
		if (time > receiveDate)
		{
			canBeRewarded = true;
			// Reset index if max is reached.
			if (rewardIndex >= AttendanceRewardData.getInstance().getRewardsCount())
			{
				rewardIndex = 0;
			}
		}
		
		return new AttendanceInfoHolder(rewardIndex, canBeRewarded);
	}
	
	public void setAttendanceInfo(int rewardIndex)
	{
		// At 6:30 next day, another reward may be taken.
		DateTime nextReward = DateTime.Now;
		if (nextReward.Hour >= 6)
		{
			nextReward = nextReward.AddDays(1);
		}
        
		nextReward = new DateTime(nextReward.Year, nextReward.Month, nextReward.Day, 6, 30, 0);

		if (Config.ATTENDANCE_REWARDS_SHARE_ACCOUNT)
		{
			getAccountVariables().set(PlayerVariables.ATTENDANCE_DATE, nextReward);
			getAccountVariables().set(PlayerVariables.ATTENDANCE_INDEX, rewardIndex);
		}
		else
		{
			getVariables().set(PlayerVariables.ATTENDANCE_DATE, nextReward);
			getVariables().set(PlayerVariables.ATTENDANCE_INDEX, rewardIndex);
		}
	}
	
	public int getAttendanceDelay()
	{
		DateTime currentTime = DateTime.Now;
		TimeSpan remainingTime = _attendanceDelay - currentTime;
		int remainingSeconds = (int) remainingTime.TotalSeconds;
		return Math.Max(remainingSeconds, 0);
	}
	
	public void setAttendanceDelay(int timeInMinutes)
	{
		DateTime currentTime = DateTime.Now;
		_attendanceDelay = currentTime.AddMilliseconds(timeInMinutes * 60 * 1000);
	}
	
	public byte getVipTier()
	{
		return _vipTier;
	}
	
	public void setVipTier(byte vipTier)
	{
		_vipTier = vipTier;
	}
	
	public long getVipPoints()
	{
		return getAccountVariables().getLong(AccountVariables.VIP_POINTS, 0L);
	}
	
	public long getVipTierExpiration()
	{
		return getAccountVariables().getLong(AccountVariables.VIP_EXPIRATION, 0L);
	}
	
	public void setVipTierExpiration(long expiration)
	{
		getAccountVariables().set(AccountVariables.VIP_EXPIRATION, expiration);
	}
	
	public void updateVipPoints(long points)
	{
		if (points == 0)
		{
			return;
		}
		int currentVipTier = VipManager.getInstance().getVipTier(getVipPoints());
		getAccountVariables().set(AccountVariables.VIP_POINTS, getVipPoints() + points);
		byte newTier = VipManager.getInstance().getVipTier(getVipPoints());
		if (newTier != currentVipTier)
		{
			_vipTier = newTier;
			if (newTier > 0)
			{
				getAccountVariables().set(AccountVariables.VIP_EXPIRATION, DateTime.Now.AddDays(30));
				VipManager.getInstance().manageTier(this);
			}
			else
			{
				getAccountVariables().set(AccountVariables.VIP_EXPIRATION, 0L);
			}
		}
		getAccountVariables().storeMe(); // force to store to prevent falty purchases after a crash.
		sendPacket(new ReceiveVipInfo(this));
	}
	
	public void initElementalSpirits()
	{
		tryLoadSpirits();
		
		if (_spirits == null)
		{
			ElementalType[] types = ElementalType.values();
			_spirits = new ElementalSpirit[types.Length - 1]; // exclude None
			foreach (ElementalType type in types)
			{
				if (ElementalType.NONE == type)
				{
					continue;
				}
				
				ElementalSpirit spirit = new ElementalSpirit(type, this);
				_spirits[type.getId() - 1] = spirit;
				spirit.save();
			}
		}
		
		if (_activeElementalSpiritType == null)
		{
			changeElementalSpirit(ElementalType.FIRE.getId());
		}
	}
	
	private void tryLoadSpirits()
	{
		List<ElementalSpiritDataHolder> restoredSpirits = new();
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement stmt = con.prepareStatement(RESTORE_ELEMENTAL_SPIRITS);
			stmt.setInt(1, getObjectId());
			 
			{
                ResultSet rset = stmt.executeQuery();
				while (rset.next())
				{
					ElementalSpiritDataHolder newHolder = new ElementalSpiritDataHolder();
					newHolder.setCharId(rset.getInt("charId"));
					byte type = rset.getByte("type");
					newHolder.setType(type);
					byte level = rset.getByte("level");
					newHolder.setLevel(level);
					byte stage = rset.getByte("stage");
					newHolder.setStage(stage);
					long experience = Math.min(rset.getLong("experience"), ElementalSpiritData.getInstance().getSpirit(type, stage).getMaxExperienceAtLevel(level));
					newHolder.setExperience(experience);
					newHolder.setAttackPoints(rset.getByte("attack_points"));
					newHolder.setDefensePoints(rset.getByte("defense_points"));
					newHolder.setCritRatePoints(rset.getByte("crit_rate_points"));
					newHolder.setCritDamagePoints(rset.getByte("crit_damage_points"));
					newHolder.setInUse(rset.getByte("in_use") == 1);
					restoredSpirits.add(newHolder);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
		
		if (!restoredSpirits.isEmpty())
		{
			_spirits = new ElementalSpirit[ElementalType.values().length - 1];
			foreach (ElementalSpiritDataHolder spiritData in restoredSpirits)
			{
				_spirits[spiritData.getType() - 1] = new ElementalSpirit(spiritData, this);
				if (spiritData.isInUse())
				{
					_activeElementalSpiritType = ElementalType.of(spiritData.getType());
				}
			}
			ThreadPool.schedule(() =>
			{
				sendPacket(new ElementalSpiritInfo(this, (byte) 0));
				sendPacket(new ExElementalSpiritAttackType(this));
			}, 4000);
		}
	}
	
	public double getActiveElementalSpiritAttack()
	{
		return getStat().getElementalSpiritPower(_activeElementalSpiritType, CommonUtil.zeroIfNullOrElse(getElementalSpirit(_activeElementalSpiritType), ElementalSpirit::getAttack));
	}
	
	public double getFireSpiritAttack()
	{
		return getElementalSpiritAttackOf(ElementalType.FIRE);
	}
	
	public double getWaterSpiritAttack()
	{
		return getElementalSpiritAttackOf(ElementalType.WATER);
	}
	
	public double getWindSpiritAttack()
	{
		return getElementalSpiritAttackOf(ElementalType.WIND);
	}
	
	public double getEarthSpiritAttack()
	{
		return getElementalSpiritAttackOf(ElementalType.EARTH);
	}
	
	public double getFireSpiritDefense()
	{
		return getElementalSpiritDefenseOf(ElementalType.FIRE);
	}
	
	public double getWaterSpiritDefense()
	{
		return getElementalSpiritDefenseOf(ElementalType.WATER);
	}
	
	public double getWindSpiritDefense()
	{
		return getElementalSpiritDefenseOf(ElementalType.WIND);
	}
	
	public double getEarthSpiritDefense()
	{
		return getElementalSpiritDefenseOf(ElementalType.EARTH);
	}
	
	public override double getElementalSpiritDefenseOf(ElementalType type)
	{
		return getStat().getElementalSpiritDefense(type, CommonUtil.zeroIfNullOrElse(getElementalSpirit(type), x => x.getDefense()));
	}
	
	public override double getElementalSpiritAttackOf(ElementalType type)
	{
		return getStat().getElementSpiritAttack(type, CommonUtil.zeroIfNullOrElse(getElementalSpirit(type), x => x.getAttack()));
	}
	
	public double getElementalSpiritCritRate()
	{
		return getStat().getElementalSpiritCriticalRate(CommonUtil.zeroIfNullOrElse(getElementalSpirit(_activeElementalSpiritType), x => x.getCriticalRate()));
	}
	
	public double getElementalSpiritCritDamage()
	{
		return getStat().getElementalSpiritCriticalDamage(CommonUtil.zeroIfNullOrElse(getElementalSpirit(_activeElementalSpiritType), x => x.getCriticalDamage()));
	}
	
	public double getElementalSpiritXpBonus()
	{
		return getStat().getElementalSpiritXpBonus();
	}
	
	public ElementalSpirit getElementalSpirit(ElementalType type)
	{
		if ((_spirits == null) || (type == null) || (type == ElementalType.NONE))
		{
			return null;
		}
		return _spirits[type.getId() - 1];
	}
	
	public byte getActiveElementalSpiritType()
	{
		return (byte) CommonUtil.zeroIfNullOrElse(_activeElementalSpiritType, x => x.getId());
	}
	
	public void changeElementalSpirit(byte element)
	{
		_activeElementalSpiritType = ElementalType.of(element);
		if (_spirits != null)
		{
			foreach (ElementalSpirit spirit in _spirits)
			{
				if (spirit != null)
				{
					spirit.setInUse(spirit.getType() == element);
					sendPacket(new ExElementalSpiritAttackType(this));
				}
			}
		}
		
		UserInfo userInfo = new UserInfo(this, false);
		userInfo.addComponentType(UserInfoType.ATT_SPIRITS);
		sendPacket(userInfo);
	}
	
	public ElementalSpirit[] getSpirits()
	{
		return _spirits;
	}
	
	public bool isInBattle()
	{
		return AttackStanceTaskManager.getInstance().hasAttackStanceTask(this);
	}
	
	public AutoPlaySettingsHolder getAutoPlaySettings()
	{
		return _autoPlaySettings;
	}
	
	public AutoUseSettingsHolder getAutoUseSettings()
	{
		return _autoUseSettings;
	}
	
	public void setAutoPlaying(bool value)
	{
		_autoPlaying.set(value);
	}
	
	public bool isAutoPlaying()
	{
		return _autoPlaying.get();
	}
	
	public void setResumedAutoPlay(bool value)
	{
		_resumedAutoPlay = value;
	}
	
	public bool hasResumedAutoPlay()
	{
		return _resumedAutoPlay;
	}
	
	public void restoreAutoSettings()
	{
		if (!Config.ENABLE_AUTO_PLAY || !getVariables().Contains(PlayerVariables.AUTO_USE_SETTINGS))
		{
			return;
		}
		
		List<int> settings = getVariables().getIntegerList(PlayerVariables.AUTO_USE_SETTINGS);
		if (settings.isEmpty())
		{
			return;
		}
		
		int options = settings.get(0);
		bool active = Config.RESUME_AUTO_PLAY && (settings.get(1) == 1);
		bool pickUp = settings.get(2) == 1;
		int nextTargetMode = settings.get(3);
		bool shortRange = settings.get(4) == 1;
		int potionPercent = settings.get(5);
		bool respectfulHunting = settings.get(6) == 1;
		int petPotionPercent = settings.size() < 8 ? 0 : settings.get(7);
		int macroIndex = settings.size() < 9 ? 0 : settings.get(8);
		
		getAutoPlaySettings().setAutoPotionPercent(potionPercent);
		getAutoPlaySettings().setOptions(options);
		getAutoPlaySettings().setPickup(pickUp);
		getAutoPlaySettings().setNextTargetMode(nextTargetMode);
		getAutoPlaySettings().setShortRange(shortRange);
		getAutoPlaySettings().setRespectfulHunting(respectfulHunting);
		getAutoPlaySettings().setAutoPetPotionPercent(petPotionPercent);
		getAutoPlaySettings().setMacroIndex(macroIndex);
		
		sendPacket(new ExAutoPlaySettingSend(options, active, pickUp, nextTargetMode, shortRange, potionPercent, respectfulHunting, petPotionPercent));
		
		if (active)
		{
			AutoPlayTaskManager.getInstance().startAutoPlay(this);
		}
		
		_resumedAutoPlay = true;
	}
	
	public void restoreAutoShortcutVisual()
	{
		if (!getVariables().Contains(PlayerVariables.AUTO_USE_SHORTCUTS))
		{
			return;
		}
		
		List<int> positions = getVariables().getIntegerList(PlayerVariables.AUTO_USE_SHORTCUTS);
		foreach (Shortcut shortcut in getAllShortCuts())
		{
			int position = shortcut.getSlot() + (shortcut.getPage() * ShortCuts.MAX_SHORTCUTS_PER_BAR);
			if (!positions.Contains(position))
			{
				continue;
			}
			
			if (shortcut.getType() == ShortcutType.SKILL)
			{
				Skill knownSkill = getKnownSkill(shortcut.getId());
				if (knownSkill != null)
				{
					sendPacket(new ExActivateAutoShortcut(shortcut, true));
				}
			}
			else if (shortcut.getType() == ShortcutType.ACTION)
			{
				sendPacket(new ExActivateAutoShortcut(shortcut, true));
			}
			else
			{
				Item item = getInventory().getItemByObjectId(shortcut.getId());
				if (item != null)
				{
					sendPacket(new ExActivateAutoShortcut(shortcut, true));
				}
			}
		}
	}
	
	public void restoreAutoShortcuts()
	{
		if (!getVariables().Contains(PlayerVariables.AUTO_USE_SHORTCUTS))
		{
			return;
		}
		
		List<int> positions = getVariables().getIntegerList(PlayerVariables.AUTO_USE_SHORTCUTS);
		foreach (Shortcut shortcut in getAllShortCuts())
		{
			int position = shortcut.getSlot() + (shortcut.getPage() * ShortCuts.MAX_SHORTCUTS_PER_BAR);
			if (!positions.Contains(position))
			{
				continue;
			}
			
			if (shortcut.getType() == ShortcutType.ACTION)
			{
				sendPacket(new ExActivateAutoShortcut(shortcut, true));
				AutoUseTaskManager.getInstance().addAutoAction(this, shortcut.getId());
				continue;
			}
			
			Skill knownSkill = getKnownSkill(shortcut.getId());
			if (knownSkill != null)
			{
				shortcut.setAutoUse(true);
				sendPacket(new ExActivateAutoShortcut(shortcut, true));
				if (knownSkill.isBad())
				{
					AutoUseTaskManager.getInstance().addAutoSkill(this, shortcut.getId());
				}
				else
				{
					AutoUseTaskManager.getInstance().addAutoBuff(this, shortcut.getId());
				}
			}
			else
			{
				Item item = getInventory().getItemByObjectId(shortcut.getId());
				if (item != null)
				{
					shortcut.setAutoUse(true);
					sendPacket(new ExActivateAutoShortcut(shortcut, true));
					if (item.isPotion())
					{
						AutoUseTaskManager.getInstance().setAutoPotionItem(this, item.getId());
					}
					else
					{
						AutoUseTaskManager.getInstance().addAutoSupplyItem(this, item.getId());
					}
				}
			}
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void addAutoShortcut(int slot, int page)
	{
		List<int> positions = getVariables().getIntegerList(PlayerVariables.AUTO_USE_SHORTCUTS);
		Shortcut usedShortcut = getShortCut(slot, page);
		if (usedShortcut == null)
		{
			int position = slot + (page * ShortCuts.MAX_SHORTCUTS_PER_BAR);
			positions.Remove(position);
		}
		else
		{
			foreach (Shortcut shortcut in getAllShortCuts())
			{
				if ((usedShortcut.getId() == shortcut.getId()) && (usedShortcut.getType() == shortcut.getType()))
				{
					shortcut.setAutoUse(true);
					sendPacket(new ExActivateAutoShortcut(shortcut, true));
					int position = shortcut.getSlot() + (shortcut.getPage() * ShortCuts.MAX_SHORTCUTS_PER_BAR);
					if (!positions.Contains(position))
					{
						positions.add(position);
					}
				}
			}
		}
		
		getVariables().setIntegerList(PlayerVariables.AUTO_USE_SHORTCUTS, positions);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeAutoShortcut(int slot, int page)
	{
		if (!getVariables().Contains(PlayerVariables.AUTO_USE_SHORTCUTS))
		{
			return;
		}
		
		List<int> positions = getVariables().getIntegerList(PlayerVariables.AUTO_USE_SHORTCUTS);
		Shortcut usedShortcut = getShortCut(slot, page);
		if (usedShortcut == null)
		{
			int position = slot + (page * ShortCuts.MAX_SHORTCUTS_PER_BAR);
			positions.Remove(position);
		}
		else
		{
			foreach (Shortcut shortcut in getAllShortCuts())
			{
				if ((usedShortcut.getId() == shortcut.getId()) && (usedShortcut.getType() == shortcut.getType()))
				{
					shortcut.setAutoUse(false);
					sendPacket(new ExActivateAutoShortcut(shortcut, false));
					int position = shortcut.getSlot() + (shortcut.getPage() * ShortCuts.MAX_SHORTCUTS_PER_BAR);
					positions.Remove(position);
				}
			}
		}
		
		getVariables().setIntegerList(PlayerVariables.AUTO_USE_SHORTCUTS, positions);
	}
	
	public bool isInTimedHuntingZone(int zoneId)
	{
		return isInTimedHuntingZone(zoneId, getX(), getY());
	}
	
	public bool isInTimedHuntingZone(int zoneId, int locX, int locY)
	{
		TimedHuntingZoneHolder holder = TimedHuntingZoneData.getInstance().getHuntingZone(zoneId);
		if (holder == null)
		{
			return false;
		}
		
		int instanceId = holder.getInstanceId();
		if (instanceId > 0)
		{
			return isInInstance() && (instanceId == getInstanceWorld().getTemplateId());
		}
		
		foreach (MapHolder map in holder.getMaps())
		{
			if ((map.getX() == (((locX - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN)) && (map.getY() == (((locY - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN)))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool isInTimedHuntingZone()
	{
		return isInTimedHuntingZone(getX(), getY());
	}
	
	public bool isInTimedHuntingZone(int x, int y)
	{
		foreach (TimedHuntingZoneHolder holder in TimedHuntingZoneData.getInstance().getAllHuntingZones())
		{
			if (isInTimedHuntingZone(holder.getZoneId(), x, y))
			{
				return true;
			}
		}
		return false;
	}
	
	public TimedHuntingZoneHolder getTimedHuntingZone()
	{
		foreach (TimedHuntingZoneHolder holder in TimedHuntingZoneData.getInstance().getAllHuntingZones())
		{
			if (isInTimedHuntingZone(holder.getZoneId()))
			{
				return holder;
			}
		}
		return null;
	}
	
	public void startTimedHuntingZone(int zoneId, long delay)
	{
		// Stop previous task.
		stopTimedHuntingZoneTask();
		
		_timedHuntingZoneTask = ThreadPool.scheduleAtFixedRate(() =>
		{
			if (isInTimedHuntingZone(zoneId))
			{
				long time = getTimedHuntingZoneRemainingTime(zoneId);
				if (time > 0)
				{
					if (time < 300000)
					{
						SystemMessage sm = new SystemMessage(SystemMessageId.THE_TIME_FOR_HUNTING_IN_THIS_ZONE_EXPIRES_IN_S1_MIN_PLEASE_ADD_MORE_TIME);
						sm.addLong(time / 60000);
						sendPacket(sm);
					}
					getVariables().set(PlayerVariables.HUNTING_ZONE_TIME + zoneId, time - 60000);
				}
				else
				{
					_timedHuntingZoneTask.cancel(false);
					_timedHuntingZoneTask = null;
					abortCast();
					stopMove(null);
					teleToLocation(MapRegionManager.getInstance().getTeleToLocation(this, TeleportWhereType.TOWN));
					sendPacket(SystemMessageId.THE_HUNTING_ZONE_S_USE_TIME_HAS_EXPIRED_SO_YOU_WERE_MOVED_OUTSIDE);
					setInstance(null);
				}
			}
		}, 60000, 60000);
	}
	
	public void stopTimedHuntingZoneTask()
	{
		if ((_timedHuntingZoneTask != null) && !_timedHuntingZoneTask.isCancelled() && !_timedHuntingZoneTask.isDone())
		{
			_timedHuntingZoneTask.cancel(true);
			_timedHuntingZoneTask = null;
		}
	}
	
	public int getTimedHuntingZoneRemainingTime(int zoneId)
	{
		return Math.Max(getVariables().getInt(PlayerVariables.HUNTING_ZONE_TIME + zoneId, 0), 0);
	}
	
	public long getTimedHuntingZoneInitialEntry(int zoneId)
	{
		return Math.Max(getVariables().getLong(PlayerVariables.HUNTING_ZONE_ENTRY + zoneId, 0), 0);
	}
	
	private void restoreRandomCraft()
	{
		_randomCraft = new PlayerRandomCraft(this);
		_randomCraft.restore();
	}
	
	public PlayerRandomCraft getRandomCraft()
	{
		return _randomCraft;
	}
	
	public PetEvolveHolder getPetEvolve(int controlItemId)
	{
		PetEvolveHolder evolve = _petEvolves.get(controlItemId);
		if (evolve != null)
		{
			return evolve;
		}
		
		Item item = getInventory().getItemByObjectId(controlItemId);
		PetData petData = item == null ? null : PetDataTable.getInstance().getPetDataByItemId(item.getId());
		return new PetEvolveHolder(petData == null ? 0 : petData.getIndex(), EvolveLevel.None, "", 1, 0);
	}
	
	public void setPetEvolve(int itemObjectId, PetEvolveHolder entry)
	{
		_petEvolves.put(itemObjectId, entry);
	}
	
	public void restorePetEvolvesByItem()
	{
		getInventory().getItems().forEach(it =>
		{
			try 
			{
				using GameServerDbContext ctx = new();
				PreparedStatement ps2 = con.prepareStatement("SELECT pet_evolves.index, pet_evolves.level as evolve, pets.name, pets.level, pets.exp FROM pet_evolves, pets WHERE pet_evolves.itemObjId=? AND pet_evolves.itemObjId = pets.item_obj_id");
				ps2.setInt(1, it.getObjectId());
				
				{
                    ResultSet rset = ps2.executeQuery();
					while (rset.next())
					{
						EvolveLevel evolve = EvolveLevel.values()[rset.getInt("evolve")];
						if (evolve != null)
						{
							_petEvolves.put(it.getObjectId(), new PetEvolveHolder(rset.getInt("index"), rset.getInt("evolve"), rset.getString("name"), rset.getInt("level"), rset.getLong("exp")));
						}
					}
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not restore pet evolve for playerId: " + getObjectId() + ": " + e);
			}
		});
	}
	
	public void calculateStatIncreaseSkills()
	{
		// Use a task to prevent multiple execution from effects.
		if (_statIncreaseSkillTask != null)
		{
			return;
		}
		
		_statIncreaseSkillTask = ThreadPool.schedule(() =>
		{
			Skill knownSkill;
			double statValue;
			bool update = false;
			
			// Remove stat increase skills.
			for (int i = (int)CommonSkill.STR_INCREASE_BONUS_1; i <= (int)CommonSkill.MEN_INCREASE_BONUS_1; i++)
			{
				knownSkill = getKnownSkill(i);
				if (knownSkill != null)
				{
					removeSkill(knownSkill);
					update = true;
				}
			}
			
			// STR bonus.
			statValue = getStat().getValue(Stat.STAT_STR);
			if ((statValue >= 60) && (statValue < 70))
			{
				addSkill(CommonSkill.STR_INCREASE_BONUS_1.getSkill(), false);
				update = true;
			}
			else if ((statValue >= 70) && (statValue < 90))
			{
				addSkill(CommonSkill.STR_INCREASE_BONUS_2.getSkill(), false);
				update = true;
			}
			else if (statValue >= 90)
			{
				addSkill(CommonSkill.STR_INCREASE_BONUS_3.getSkill(), false);
				update = true;
			}
			
			// INT bonus.
			statValue = getStat().getValue(Stat.STAT_INT);
			if ((statValue >= 60) && (statValue < 70))
			{
				addSkill(CommonSkill.INT_INCREASE_BONUS_1.getSkill(), false);
				update = true;
			}
			else if ((statValue >= 70) && (statValue < 90))
			{
				addSkill(CommonSkill.INT_INCREASE_BONUS_2.getSkill(), false);
				update = true;
			}
			else if (statValue >= 90)
			{
				addSkill(CommonSkill.INT_INCREASE_BONUS_3.getSkill(), false);
				update = true;
			}
			
			// DEX bonus.
			statValue = getStat().getValue(Stat.STAT_DEX);
			if ((statValue >= 50) && (statValue < 60))
			{
				addSkill(CommonSkill.DEX_INCREASE_BONUS_1.getSkill(), false);
				update = true;
			}
			else if ((statValue >= 60) && (statValue < 80))
			{
				addSkill(CommonSkill.DEX_INCREASE_BONUS_2.getSkill(), false);
				update = true;
			}
			else if (statValue >= 80)
			{
				addSkill(CommonSkill.DEX_INCREASE_BONUS_3.getSkill(), false);
				update = true;
			}
			
			// WIT bonus.
			statValue = getStat().getValue(Stat.STAT_WIT);
			if ((statValue >= 40) && (statValue < 50))
			{
				addSkill(CommonSkill.WIT_INCREASE_BONUS_1.getSkill(), false);
				update = true;
			}
			else if ((statValue >= 50) && (statValue < 70))
			{
				addSkill(CommonSkill.WIT_INCREASE_BONUS_2.getSkill(), false);
				update = true;
			}
			else if (statValue >= 70)
			{
				addSkill(CommonSkill.WIT_INCREASE_BONUS_3.getSkill(), false);
				update = true;
			}
			
			// CON bonus.
			statValue = getStat().getValue(Stat.STAT_CON);
			if ((statValue >= 50) && (statValue < 65))
			{
				addSkill(CommonSkill.CON_INCREASE_BONUS_1.getSkill(), false);
				update = true;
			}
			else if ((statValue >= 65) && (statValue < 90))
			{
				addSkill(CommonSkill.CON_INCREASE_BONUS_2.getSkill(), false);
				update = true;
			}
			else if (statValue >= 90)
			{
				addSkill(CommonSkill.CON_INCREASE_BONUS_3.getSkill(), false);
				update = true;
			}
			
			// MEN bonus.
			statValue = getStat().getValue(Stat.STAT_MEN);
			if ((statValue >= 45) && (statValue < 60))
			{
				addSkill(CommonSkill.MEN_INCREASE_BONUS_1.getSkill(), false);
				update = true;
			}
			else if ((statValue >= 60) && (statValue < 85))
			{
				addSkill(CommonSkill.MEN_INCREASE_BONUS_2.getSkill(), false);
				update = true;
			}
			else if (statValue >= 85)
			{
				addSkill(CommonSkill.MEN_INCREASE_BONUS_3.getSkill(), false);
				update = true;
			}
			
			// Update skill list.
			if (update)
			{
				sendSkillList();
			}
			
			_statIncreaseSkillTask = null;
		}, 500);
	}
	
	public List<PlayerCollectionData> getCollections()
	{
		return _collections;
	}
	
	public List<int> getCollectionFavorites()
	{
		return _collectionFavorites;
	}
	
	public void addCollectionFavorite(int id)
	{
		_collectionFavorites.add(id);
	}
	
	public void removeCollectionFavorite(int id)
	{
		_collectionFavorites.Remove(id);
	}
	
	public void storeCollections()
	{
		try
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement st = con.prepareStatement(INSERT_COLLECTION);
			_collections.forEach(data =>
			{
				try
				{
					st.setString(1, getAccountName());
					st.setInt(2, data.getItemId());
					st.setInt(3, data.getCollectionId());
					st.setInt(4, data.getIndex());
					st.addBatch();
				}
				catch (Exception e)
				{
					LOGGER.Error("Could not store collection for playerId " + getObjectId() + ": " + e);
				}
			});
			st.executeBatch();
			con.commit();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store collection for playerId " + getObjectId() + ": " + e);
		}
	}
	
	public void storeCollectionFavorites()
	{
		try 
        {
		    using GameServerDbContext ctx = new();
            {
                PreparedStatement st = con.prepareStatement(DELETE_COLLECTION_FAVORITE);
				st.setString(1, getAccountName());
				st.execute();
			}


			{
				PreparedStatement st = con.prepareStatement(INSERT_COLLECTION_FAVORITE);
				_collectionFavorites.forEach(data =>
				{
					try
					{
						st.setString(1, getAccountName());
						st.setInt(2, data);
						st.addBatch();
					}
					catch (Exception e)
					{
						LOGGER.Error("Could not store collection favorite for playerId " + getObjectId() + ": " + e);
					}
				});
				st.executeBatch();
				con.commit();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store collection favorite for playerId " + getObjectId() + ": " + e);
		}
	}
	
	private void restoreCollections()
	{
		_collections.Clear();
		
		try 
		{
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(RESTORE_COLLECTION);
			statement.setString(1, getAccountName());
			
			{
                ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					int collectionId = rset.getInt("collectionId");
					if (CollectionData.getInstance().getCollection(collectionId) != null)
					{
						_collections.add(new PlayerCollectionData(collectionId, rset.getInt("itemId"), rset.getInt("index")));
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore collection list data for playerId: " + getObjectId() + ": " + e);
		}
	}
	
	private void restoreCollectionBonuses()
	{
		Set<int> collectionIds = new();
		foreach (PlayerCollectionData collection in _collections)
		{
			collectionIds.add(collection.getCollectionId());
		}
		
		foreach (int collectionId in collectionIds)
		{
			CollectionDataHolder collection = CollectionData.getInstance().getCollection(collectionId);
			int count = 0;
			foreach (PlayerCollectionData data in _collections)
			{
				if (data.getCollectionId() == collectionId)
				{
					count++;
				}
			}
			if (count >= collection.getCompleteCount())
			{
				Options.Options options = OptionData.getInstance().getOptions(collection.getOptionId());
				if (options != null)
				{
					options.apply(this);
				}
			}
		}
	}
	
	private void restoreCollectionFavorites()
	{
		_collectionFavorites.Clear();
		
		try 
        {
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(RESTORE_COLLECTION_FAVORITE);
			statement.setString(1, getAccountName());
			try			
            {
                ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					_collectionFavorites.add(rset.getInt("collectionId"));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore collection favorite list data for playerId: " + getObjectId() + ": " + e);
		}
	}
	
	public Map<int, PurgePlayerHolder> getPurgePoints()
	{
		return _purgePoints;
	}
	
	public void storeSubjugation()
	{
		try 
		{
            using GameServerDbContext ctx = new();
			
			{
                PreparedStatement st = con.prepareStatement(DELETE_SUBJUGATION);
				st.setInt(1, getObjectId());
				st.execute();
			}


			{
				PreparedStatement st = con.prepareStatement(INSERT_SUBJUGATION);
				getPurgePoints().forEach((category, data) =>
				{
					try
					{
						st.setInt(1, getObjectId());
						st.setInt(2, category);
						st.setInt(3, data.getPoints());
						st.setInt(4, data.getKeys());
						st.setInt(5, data.getRemainingKeys());
						st.addBatch();
					}
					catch (Exception e)
					{
						LOGGER.Error("Could not store subjugation data for playerId " + getObjectId() + ": " + e);
					}
				});
				st.executeBatch();
				con.commit();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store subjugation data for playerId " + getObjectId() + ": " + e);
		}
	}
	
	private void restoreSubjugation()
	{
		if (_purgePoints != null)
		{
			_purgePoints.clear();
		}
		
		try 
        {
            using GameServerDbContext ctx = new();
            			PreparedStatement statement = con.prepareStatement(RESTORE_SUBJUGATION);
			statement.setInt(1, getObjectId());

			{
				ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					_purgePoints.put(rset.getInt("category"), new PurgePlayerHolder(rset.getInt("points"), rset.getInt("keys"), rset.getInt("remainingKeys")));
					
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore subjugation data for playerId: " + getObjectId() + ": " + e);
		}
	}
	
	public int getPurgeLastCategory()
	{
		return getVariables().getInt(PlayerVariables.PURGE_LAST_CATEGORY, 1);
	}
	
	public void setPurgeLastCategory(int category)
	{
		getVariables().set(PlayerVariables.PURGE_LAST_CATEGORY, category);
	}
	
	public int getClanDonationPoints()
	{
		return getVariables().getInt(PlayerVariables.CLAN_DONATION_POINTS, 3);
	}
	
	public MissionLevelPlayerDataHolder getMissionLevelProgress()
	{
		if (_missionLevelProgress == null)
		{
			String variable = PlayerVariables.MISSION_LEVEL_PROGRESS + MissionLevel.getInstance().getCurrentSeason();
			if (getVariables().hasVariable(variable))
			{
				_missionLevelProgress = new MissionLevelPlayerDataHolder(getVariables().getString(variable));
			}
			else
			{
				_missionLevelProgress = new MissionLevelPlayerDataHolder();
				_missionLevelProgress.storeInfoInVariable(this);
			}
		}
		return _missionLevelProgress;
	}
	
	private void storeDualInventory()
	{
		getVariables().set(PlayerVariables.DUAL_INVENTORY_SLOT, _dualInventorySlot);
		getVariables().setIntegerList(PlayerVariables.DUAL_INVENTORY_SET_A, _dualInventorySetA);
		getVariables().setIntegerList(PlayerVariables.DUAL_INVENTORY_SET_B, _dualInventorySetB);
	}
	
	public void restoreDualInventory()
	{
		_dualInventorySlot = getVariables().getInt(PlayerVariables.DUAL_INVENTORY_SLOT, 0);
		
		if (getVariables().Contains(PlayerVariables.DUAL_INVENTORY_SET_A))
		{
			_dualInventorySetA = getVariables().getIntegerList(PlayerVariables.DUAL_INVENTORY_SET_A);
		}
		else
		{
			List<int> list = new(Inventory.PAPERDOLL_TOTALSLOTS);
			for (int i = 0; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
			{
				list.add(getInventory().getPaperdollObjectId(i));
			}
			getVariables().setIntegerList(PlayerVariables.DUAL_INVENTORY_SET_A, list);
			_dualInventorySetA = list;
		}
		
		if (getVariables().Contains(PlayerVariables.DUAL_INVENTORY_SET_B))
		{
			_dualInventorySetB = getVariables().getIntegerList(PlayerVariables.DUAL_INVENTORY_SET_B);
		}
		else
		{
			List<int> list = new(Inventory.PAPERDOLL_TOTALSLOTS);
			for (int i = 0; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
			{
				list.add(0);
			}
			getVariables().setIntegerList(PlayerVariables.DUAL_INVENTORY_SET_B, list);
			_dualInventorySetB = list;
		}
		
		sendPacket(new ExDualInventorySwap(_dualInventorySlot));
	}
	
	public void setDualInventorySlot(int slot)
	{
		_dualInventorySlot = slot;
		
		bool changed = false;
		List<int> itemObjectIds = getDualInventorySet();
		for (int i = 0; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
		{
			int existingObjectId = getInventory().getPaperdollObjectId(i);
			int itemObjectId = itemObjectIds.get(i);
			if (existingObjectId != itemObjectId)
			{
				changed = true;
				
				if (existingObjectId > 0)
				{
					getInventory().unEquipItemInSlot(i);
				}
				
				if (itemObjectId > 0)
				{
					Item item = getInventory().getItemByObjectId(itemObjectId);
					if (item != null)
					{
						useEquippableItem(item, false);
					}
				}
			}
		}
		
		sendPacket(new ExDualInventorySwap(slot));
		
		if (changed)
		{
			sendItemList();
			broadcastUserInfo();
		}
	}
	
	private List<int> getDualInventorySet()
	{
		return _dualInventorySlot == 0 ? _dualInventorySetA : _dualInventorySetB;
	}
	
	public int getSkillEnchantExp(int level)
	{
		return getVariables().getInt(PlayerVariables.SKILL_ENCHANT_STAR + level, 0);
	}
	
	public void setSkillEnchantExp(int level, int exp)
	{
		getVariables().set(PlayerVariables.SKILL_ENCHANT_STAR + level, exp);
	}
	
	public void increaseTrySkillEnchant(int level)
	{
		int currentTry = getSkillTryEnchant(level) + 1;
		getVariables().set(PlayerVariables.SKILL_TRY_ENCHANT + level, currentTry);
	}
	
	public int getSkillTryEnchant(int level)
	{
		return getVariables().getInt(PlayerVariables.SKILL_TRY_ENCHANT + level, 1);
	}
	
	public void setSkillTryEnchant(int level)
	{
		getVariables().set(PlayerVariables.SKILL_TRY_ENCHANT + level, 1);
	}
}