using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Events;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
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
using L2Dn.GameServer.Model.DailyMissions;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Events.Impl.Playables;
using L2Dn.GameServer.Model.Events.Impl.Players;
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
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPeel;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPlay;
using L2Dn.GameServer.Network.OutgoingPackets.Commission;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.GameServer.Network.OutgoingPackets.Friends;
using L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;
using L2Dn.GameServer.Network.OutgoingPackets.LimitShop;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.Network.OutgoingPackets.Surveillance;
using L2Dn.GameServer.Network.OutgoingPackets.Vip;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using ClanWar = L2Dn.GameServer.Model.Clans.ClanWar;
using FortManager = L2Dn.GameServer.InstanceManagers.FortManager;
using Forum = L2Dn.GameServer.CommunityBbs.BB.Forum;
using Pet = L2Dn.GameServer.Model.Actor.Instances.Pet;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor;

/**
 * This class represents all player characters in the world.<br>
 * There is always a client-thread connected to this (except if a player-store is activated upon logout).
 */
public class Player: Playable
{
	private const string COND_OVERRIDE_KEY = "cond_override";
	
	public const string NEWBIE_KEY = "NEWBIE";
	
	public const int ID_NONE = -1;
	
	public const int REQUEST_TIMEOUT = 15;
	
	private int _pcCafePoints;
	
	private GameSession? _client;
	
	private readonly int _accountId;
	private readonly string _accountName;
	private DateTime? _deleteTime;
	private DateTime _createDate = DateTime.UtcNow;
	
	private string? _lang;
	
	private volatile bool _isOnline;
	private bool _offlinePlay;
	private bool _enteredWorld;
	private TimeSpan _onlineTime;
	private DateTime? _onlineBeginTime;
	private DateTime? _lastAccess;
	private DateTime _uptime;
	
	private ScheduledFuture _itemListTask;
	private ScheduledFuture _skillListTask;
	private ScheduledFuture _storageCountTask;
	private ScheduledFuture _userBoostStatTask;
	private ScheduledFuture _abnormalVisualEffectTask;
	private ScheduledFuture _updateAndBroadcastStatusTask;
	private ScheduledFuture _broadcastCharInfoTask;
	
	private bool _subclassLock;
	protected CharacterClass _baseClass;
	protected CharacterClass _activeClass;
	protected int _classIndex;
	private bool _isDeathKnight;
	private bool _isVanguard;
	private bool _isAssassin;
	
	/** data for mounted pets */
	private int _controlItemId;
	private PetData _data;
	private PetLevelData _leveldata;
	private int _curFeed;
	protected ScheduledFuture _mountFeedTask;
	private ScheduledFuture _dismountTask;
	private bool _petItems;
	
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
	private int _totalKills;
	
	/** The total death counter of the Player */
	private int _totalDeaths;
	
	/** The PvP Flag state of the Player (0=White, 1=Purple) */
	private PvpFlagStatus _pvpFlag;
	
	private int _einhasadOverseeingLevel;
	
	private readonly List<DamageTakenHolder> _lastDamageTaken = new();
	
	/** The Fame of this Player */
	private int _fame;
	private ScheduledFuture _fameTask;
	
	/** The Raidboss points of this Player */
	private int _raidbossPoints;
	
	private ScheduledFuture _teleportWatchdog;
	
	/** The Siege state of the Player */
	private byte _siegeState;
	
	/** The id of castle/fort which the Player is registered for siege */
	private int _siegeSide;
	
	private int _curWeightPenalty;
	
	private int _lastCompassZone; // the last compass zone update send to the client
	
	private readonly ContactList _contactList;
	
	private int _bookmarkslot; // The Teleport Bookmark Slot
	
	private readonly Map<int, TeleportBookmark> _tpbookmarks = new();
	
	private bool _canFeed;
	private bool _isInSiege;
	private bool _isInHideoutSiege;
	
	/** Olympiad */
	private bool _inOlympiadMode;
	private bool _olympiadStart;
	private int _olympiadGameId = -1;
	private int _olympiadSide = -1;
	
	/** Duel */
	private bool _isInDuel;
	private bool _startingDuel;
	private int _duelState = Duel.DUELSTATE_NODUEL;
	private int _duelId;
	private SystemMessageId _noDuelReason = SystemMessageId.THERE_IS_NO_OPPONENT_TO_RECEIVE_YOUR_CHALLENGE_FOR_A_DUEL;
	
	/** Boat and AirShip */
	private Vehicle _vehicle;
	private Location _inVehiclePosition;
	
	private MountType _mountType = MountType.NONE;
	private int _mountNpcId;
	private int _mountLevel;
	/** Store object used to summon the strider you are mounting **/
	private int _mountObjectID;
	
	private AdminTeleportType _teleportType = AdminTeleportType.NORMAL;
	
	private bool _inCrystallize;
	private bool _isCrafting;
	
	private DateTime? _offlineShopStart;
	
	/** The table containing all RecipeList of the Player */
	private readonly Map<int, RecipeList> _dwarvenRecipeBook = new();
	private readonly Map<int, RecipeList> _commonRecipeBook = new();
	
	/** Premium Items */
	private readonly Map<int, PremiumItem> _premiumItems = new();
	
	/** True if the Player is sitting */
	private bool _waitTypeSitting;
	
	/** Location before entering Observer Mode */
	private Location _lastLoc;
	private bool _observerMode;
	
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
	protected bool _recoTwoHoursGiven;
	
	private ScheduledFuture _onlineTimeUpdateTask;
	
	private readonly PlayerInventory _inventory;
	private readonly PlayerFreight _freight;
	private readonly PlayerWarehouse _warehouse;
	private PlayerRefund _refund;
	private PrivateStoreType _privateStoreType = PrivateStoreType.NONE;
	private TradeList _activeTradeList;
	private ItemContainer _activeWarehouse;
	private Map<int, ManufactureItem> _manufactureItems;
	private string _storeName = "";
	private TradeList _sellList;
	private TradeList _buyList;
	
	// Multisell
	private PreparedMultisellListHolder _currentMultiSell;
	
	private bool _noble;
	private bool _hero;
	private bool _trueHero;
	
	/** Premium System */
	private bool _premiumStatus;
	
	/** Faction System */
	private bool _isGood;
	private bool _isEvil;
	
	/** The Npc corresponding to the last Folk which one the player talked. */
	private Npc _lastFolkNpc;
	
	/** Last NPC Id talked on a quest */
	private int _questNpcObject;
	
	/** Used for simulating Quest onTalk */
	private bool _simulatedTalking;
	
	/** The table containing all Quests began by the Player */
	private readonly Map<string, QuestState> _quests = new(StringComparer.InvariantCultureIgnoreCase);
	
	/** The list containing all shortCuts of this player. */
	private readonly ShortCuts _shortCuts;
	
	/** The list containing all macros of this player. */
	private readonly MacroList _macros;
	
	private readonly Set<Player> _snoopListener = new();
	private readonly Set<Player> _snoopedPlayer = new();
	
	/** Hennas */
	private readonly HennaPoten[] _hennaPoten = new HennaPoten[4];
	private readonly Map<BaseStat, int> _hennaBaseStats = new();
	private readonly Map<int, ScheduledFuture> _hennaRemoveSchedules = new();
	
	/** Hennas Potential */
	
	/** The Pet of the Player */
	private Pet _pet;
	/** Servitors of the Player */
	private readonly Map<int, Summon> _servitors = new();
	/** The Agathion of the Player */
	private int _agathionId;
	// apparently, a Player CAN have both a summon AND a tamed beast at the same time!!
	// after Freya players can control more than one tamed beast
	private readonly Set<TamedBeast> _tamedBeast = new();
	
	private bool _minimapAllowed;
	
	// client radar
	// TODO: This needs to be better integrated and saved/loaded
	private readonly Radar _radar;
	
	private MatchingRoom _matchingRoom;
	
	private ScheduledFuture _taskWarnUserTakeBreak;
	
	// Clan related attributes
	/** The Clan Identifier of the Player */
	private int? _clanId;
	
	/** The Clan object of the Player */
	private Clan _clan;
	
	/** Apprentice and Sponsor IDs */
	private int _apprentice;
	private int? _sponsor;
	
	private DateTime? _clanJoinExpiryTime;
	private DateTime? _clanCreateExpiryTime;
	
	private int _powerGrade;
	private ClanPrivilege _clanPrivileges;
	
	/** Player's pledge class (knight, Baron, etc.) */
	private SocialClass _pledgeClass;
	private int _pledgeType;
	
	/** Level at which the player joined the clan as an academy member */
	private int _lvlJoinedAcademy;
	
	private bool _wantsPeace;
	
	// charges
	private readonly AtomicInteger _charges = new AtomicInteger();
	private ScheduledFuture _chargeTask;
	
	// Absorbed Souls
	private const int KAMAEL_LIGHT_MASTER = 45178;
	private const int KAMAEL_SHADOW_MASTER = 45179;
	private const int KAMAEL_LIGHT_TRANSFORMATION = 397;
	private const int KAMAEL_SHADOW_TRANSFORMATION = 398;
	private readonly Map<SoulType, int> _souls = new();
	private ScheduledFuture _soulTask;
	
	// Death Points
	private const int DEATH_POINTS_PASSIVE = 45352;
	private const int DEVASTATING_MIND = 45300;
	private int _deathPoints;
	private int _maxDeathPoints;
	
	// Beast points
	private int _beastPoints;
	private int _maxBeastPoints;
	
	// Assasination points
	private int _assassinationPoints;
	private readonly int _maxAssassinationPoints = 100000;
	
	// WorldPosition used by TARGET_SIGNET_GROUND
	private Location _currentSkillWorldPosition;
	
	private AccessLevel _accessLevel;
	
	private bool _messageRefusal; // message refusal mode
	
	private bool _silenceMode; // silence mode
	private List<int> _silenceModeExcluded; // silence mode
	private bool _dietMode; // ignore weight penalty
	private bool _tradeRefusal; // Trade refusal
	private bool _exchangeRefusal; // Exchange refusal
	
	private Party _party;
	
	// this is needed to find the inviting player for Party response
	// there can only be one active party request at once
	private Player _activeRequester;
	private long _requestExpireTime;
	private readonly Model.Request _request;
	
	// Used for protection after teleport
	private DateTime? _spawnProtectEndTime;
	private DateTime? _teleportProtectEndTime;
	
	private readonly Map<int, ExResponseCommissionInfoPacket> _lastCommissionInfos = new();
	
	// protects a char from aggro mobs when getting up from fake death
	private long _recentFakeDeathEndTime;
	
	/** The fists Weapon of the Player (used when no weapon is equipped) */
	private Weapon _fistsWeaponItem;
	
	private readonly Map<int, string> _chars = new();
	
	private readonly Map<Type, AbstractRequest> _requests = new();
	
	protected bool _inventoryDisable;
	/** Player's cubics. */
	private readonly Map<int, Cubic> _cubics = new();
	/** Active shots. */
	protected Set<int> _activeSoulShots = new();
	/** Active Brooch Jewels **/
	private BroochJewel? _activeRubyJewel;
	private BroochJewel? _activeShappireJewel;
	
	private int _lastAmmunitionId;
	
	/** Event parameters */
	private bool _isRegisteredOnEvent;
	private bool _isOnSoloEvent;
	private bool _isOnEvent;
	
	/** new race ticket **/
	private readonly int[] _raceTickets = new int[2];
	
	private readonly BlockList _blockList;
	
	private readonly Map<int, Skill> _transformSkills = new();
	private ScheduledFuture _taskRentPet;
	private ScheduledFuture _taskWater;
	
	private Forum _forumMail;
	private Forum _forumMemo;
	
	/** Skills queued because a skill is already in progress */
	private SkillUseHolder _queuedSkill;
	
	private int _cursedWeaponEquippedId;
	private bool _combatFlagEquippedId;
	
	private bool _canRevive = true;
	private int _reviveRequested;
	private double _revivePower;
	private int _reviveHpPercent;
	private int _reviveMpPercent;
	private int _reviveCpPercent;
	private bool _revivePet;
	
	private double _cpUpdateIncCheck;
	private double _cpUpdateDecCheck;
	private double _cpUpdateInterval;
	private double _mpUpdateIncCheck;
	private double _mpUpdateDecCheck;
	private double _mpUpdateInterval;
	
	private double _originalCp;
	private double _originalHp;
	private double _originalMp;
	
	/** Char Coords from Client */
	private int _clientX;
	private int _clientY;
	private int _clientZ;
	private int _clientHeading;
	
	// during fall validations will be disabled for 1000 ms.
	private static readonly TimeSpan FALLING_VALIDATION_DELAY = TimeSpan.FromSeconds(1);
	private DateTime? _fallingTimestamp;
	private volatile int _fallingDamage;
	private ScheduledFuture _fallingDamageTask;
	
	private int _multiSocialTarget;
	private int _multiSociaAction;
	
	private MovieHolder _movieHolder;
	
	private string _adminConfirmCmd;
	
	private DateTime? _lastItemAuctionInfoRequest;
	
	private DateTime _pvpFlagLasts;
	
	private DateTime? _notMoveUntil;
	
	/** Map containing all custom skills of this player. */
	private Map<int, Skill> _customSkills;
	
	private volatile int _actionMask;
	
	private int _questZoneId = -1;
	
	private readonly Fishing _fishing;
	private readonly PlayerDailyMissionList _dailyMissions;
	
	public void setPvpFlagLasts(DateTime time)
	{
		_pvpFlagLasts = time;
	}
	
	public DateTime getPvpFlagLasts()
	{
		return _pvpFlagLasts;
	}
	
	public void startPvPFlag()
	{
		updatePvPFlag(PvpFlagStatus.Enabled);
		PvpFlagTaskManager.getInstance().add(this);
	}
	
	public void stopPvpRegTask()
	{
		PvpFlagTaskManager.getInstance().remove(this);
	}
	
	public void stopPvPFlag()
	{
		stopPvpRegTask();
		updatePvPFlag(PvpFlagStatus.None);
	}
	
	// Training Camp
	private const string TRAINING_CAMP_VAR = "TRAINING_CAMP";
	private const string TRAINING_CAMP_DURATION = "TRAINING_CAMP_DURATION";
	
	// Save responder name for log it
	private string _lastPetitionGmName;
	
	private bool _hasCharmOfCourage;
	
	private readonly Set<int> _whisperers = new();
	
	private ElementalSpirit[] _spirits;
	private ElementalType _activeElementalSpiritType;
	
	private int _vipTier;
	
	private DateTime _attendanceDelay;
	
	private readonly AutoPlaySettingsHolder _autoPlaySettings = new AutoPlaySettingsHolder();
	private readonly AutoUseSettingsHolder _autoUseSettings = new AutoUseSettingsHolder();
	private readonly AtomicBoolean _autoPlaying = new AtomicBoolean();
	private bool _resumedAutoPlay;
	
	private ScheduledFuture _timedHuntingZoneTask;
	
	private PlayerRandomCraft _randomCraft;
	
	private ScheduledFuture _statIncreaseSkillTask;
	
	private readonly List<PlayerCollectionData> _collections = new();
	private readonly List<int> _collectionFavorites = new();
	
	private readonly Map<int, PurgePlayerHolder> _purgePoints = new();
	
	private readonly HuntPass _huntPass;
	private readonly AchievementBox _achivemenetBox;
	
	private readonly ChallengePoint _challengePoints;
	
	private readonly RankingHistory _rankingHistory;
	
	private readonly Map<int, PetEvolveHolder> _petEvolves = new();
	
	private MissionLevelPlayerDataHolder _missionLevelProgress;
	
	private int _dualInventorySlot;
	private List<int> _dualInventorySetA;
	private List<int> _dualInventorySetB;
	
	private readonly List<QuestTimer> _questTimers = new();
	private readonly List<TimerHolder> _timerHolders = new();
	
	// Selling buffs system
	private bool _isSellingBuffs;
	private List<SellBuffHolder> _sellingBuffs;
	
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
	public static Player create(PlayerTemplate template, int accountId, string accountName, string name, PlayerAppearance app)
	{
		// Create a new Player with an account name
		Player player = new Player(template, accountId, accountName, app);
		// Set the name of the Player
		player.setName(name);
		// Set access level
		player.setAccessLevel(0, false, false);
		// Set Character's create time
		player.setCreateDate(DateTime.UtcNow);
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
	
	public int getAccountId()
	{
		return _client == null ? _accountId : _client.AccountId;
	}
	
	public string getAccountName()
	{
		return _client == null ? _accountName : _client.AccountName;
	}
	
	public string getAccountNamePlayer()
	{
		return _accountName;
	}
	
	public Map<int, string> getAccountChars()
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
			result |= RelationChangedPacket.RELATION_CLAN_MEMBER;
			if (clan == target.getClan())
			{
				result |= RelationChangedPacket.RELATION_CLAN_MATE;
			}
			if (getAllyId() != 0)
			{
				result |= RelationChangedPacket.RELATION_ALLY_MEMBER;
			}
		}
		if (isClanLeader())
		{
			result |= RelationChangedPacket.RELATION_LEADER;
		}
		if ((party != null) && (party == target.getParty()))
		{
			result |= RelationChangedPacket.RELATION_HAS_PARTY;
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
						result |= RelationChangedPacket.RELATION_PARTYLEADER; // 0x10
						break;
					}
					case 1:
					{
						result |= RelationChangedPacket.RELATION_PARTY4; // 0x8
						break;
					}
					case 2:
					{
						result |= RelationChangedPacket.RELATION_PARTY3 + RelationChangedPacket.RELATION_PARTY2 + RelationChangedPacket.RELATION_PARTY1; // 0x7
						break;
					}
					case 3:
					{
						result |= RelationChangedPacket.RELATION_PARTY3 + RelationChangedPacket.RELATION_PARTY2; // 0x6
						break;
					}
					case 4:
					{
						result |= RelationChangedPacket.RELATION_PARTY3 + RelationChangedPacket.RELATION_PARTY1; // 0x5
						break;
					}
					case 5:
					{
						result |= RelationChangedPacket.RELATION_PARTY3; // 0x4
						break;
					}
					case 6:
					{
						result |= RelationChangedPacket.RELATION_PARTY2 + RelationChangedPacket.RELATION_PARTY1; // 0x3
						break;
					}
					case 7:
					{
						result |= RelationChangedPacket.RELATION_PARTY2; // 0x2
						break;
					}
					case 8:
					{
						result |= RelationChangedPacket.RELATION_PARTY1; // 0x1
						break;
					}
				}
			}
		}
		if (_siegeState != 0)
		{
			result |= RelationChangedPacket.RELATION_INSIEGE;
			if (getSiegeState() != target.getSiegeState())
			{
				result |= RelationChangedPacket.RELATION_ENEMY;
			}
			else
			{
				result |= RelationChangedPacket.RELATION_ALLY;
			}
			if (_siegeState == 1)
			{
				result |= RelationChangedPacket.RELATION_ATTACKER;
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
							result |= RelationChangedPacket.RELATION_DECLARED_WAR;
						}
						break;
					}
					case ClanWarState.MUTUAL:
					{
						result |= RelationChangedPacket.RELATION_MUTUAL_WAR;
						break;
					}
				}
			}
		}
		if (target.getSurveillanceList().Contains(getObjectId()))
		{
			result |= RelationChangedPacket.RELATION_SURVEILLANCE;
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
	private Player(int objectId, PlayerTemplate template, int accountId, string accountName, PlayerAppearance app): base(objectId, template)
	{
		setInstanceType(InstanceType.Player);
		_accountName = accountName;
		_accountId = accountId;
		app.setOwner(this);
		_appearance = app;
		
		_contactList = new ContactList(this);
		_inventory = new PlayerInventory(this);
		_freight = new PlayerFreight(this);
		_warehouse = new PlayerWarehouse(this);
		_shortCuts = new ShortCuts(this);
		_macros = new MacroList(this);
		_request = new Model.Request(this);
		_blockList = new BlockList(this);
		_fishing = new Fishing(this);
		_dailyMissions = new PlayerDailyMissionList(this);

		_huntPass = Config.ENABLE_HUNT_PASS ? new HuntPass(this) : null;
		_achivemenetBox = Config.ENABLE_ACHIEVEMENT_BOX ? new AchievementBox(this) : null;

		// Create a Radar object
		_radar = new Radar(this);
		_challengePoints = new ChallengePoint(this);
		_rankingHistory = new RankingHistory(this);
		
		initCharStatusUpdateValues();
		initPcStatusUpdateValues();
		
		// Create an AI
		getAI();
	}
	
	/**
	 * Creates a player.
	 * @param template the player template
	 * @param accountName the account name
	 * @param app the player appearance
	 */
	private Player(PlayerTemplate template, int accountId, string accountName, PlayerAppearance app)
		: this(IdManager.getInstance().getNextId(), template, accountId, accountName, app)
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
	public void setTemplate(CharacterClass newclass)
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
	
	public void setBaseClass(CharacterClass classId)
	{
		_baseClass = classId;
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterRecipeBooks.Add(new CharacterRecipeBook()
			{
				CharacterId = getObjectId(),
				Id = recipeId,
				ClassIndex = (short)(isDwarf ? _classIndex : 0),
				Type = isDwarf ? 1 : 0
			});

			ctx.SaveChanges();
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
			int classIndex = isDwarf ? _classIndex : 0;
			ctx.CharacterRecipeBooks
				.Where(r => r.CharacterId == characterId && r.Id == recipeId && r.ClassIndex == classIndex)
				.ExecuteDelete();
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
	public QuestState getQuestState(string quest)
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
	public bool hasQuestState(string quest)
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
	public void delQuestState(string quest)
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
	
	public void processQuestEvent(string questName, string ev)
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
	public void setPvpFlag(PvpFlagStatus pvpFlag)
	{
		_pvpFlag = pvpFlag;
	}
	
	public override PvpFlagStatus getPvpFlag()
	{
		return _pvpFlag;
	}
	
	public override void updatePvPFlag(PvpFlagStatus value)
	{
		if (_pvpFlag == value)
		{
			return;
		}
		
		setPvpFlag(value);
		
		StatusUpdatePacket su = new StatusUpdatePacket(this);
		computeStatusUpdate(su, StatusUpdateType.PVP_FLAG);
		if (su.hasUpdates())
		{
			broadcastPacket(su);
			sendPacket(su);
		}
		
		// If this player has a pet update the pets pvp flag as well
		if (hasSummon())
		{
			RelationChangedPacket rc = new RelationChangedPacket();
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
			bool isAutoAttackable = this.isAutoAttackable(player);
			RelationCache oldrelation = getKnownRelations().get(player.getObjectId());
			if ((oldrelation == null) || (oldrelation.getRelation() != relation) || (oldrelation.isAutoAttackable() != isAutoAttackable))
			{
				RelationChangedPacket rc = new RelationChangedPacket();
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
	
	public override void revalidateZone(bool force)
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
		
		ZoneManager.getInstance().getRegion(getLocation().ToLocation2D())?.revalidateZones(this);
		
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
			if (_lastCompassZone == ExSetCompassZoneCodePacket.ALTEREDZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCodePacket.ALTEREDZONE;
			sendPacket(new ExSetCompassZoneCodePacket(ExSetCompassZoneCodePacket.ALTEREDZONE));
		}
		else if (isInsideZone(ZoneId.SIEGE))
		{
			if (_lastCompassZone == ExSetCompassZoneCodePacket.SIEGEWARZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCodePacket.SIEGEWARZONE;
			sendPacket(new ExSetCompassZoneCodePacket(ExSetCompassZoneCodePacket.SIEGEWARZONE));
		}
		else if (isInsideZone(ZoneId.PVP))
		{
			if (_lastCompassZone == ExSetCompassZoneCodePacket.PVPZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCodePacket.PVPZONE;
			sendPacket(new ExSetCompassZoneCodePacket(ExSetCompassZoneCodePacket.PVPZONE));
		}
		else if (isInsideZone(ZoneId.PEACE))
		{
			if (_lastCompassZone == ExSetCompassZoneCodePacket.PEACEZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCodePacket.PEACEZONE;
			sendPacket(new ExSetCompassZoneCodePacket(ExSetCompassZoneCodePacket.PEACEZONE));
		}
		else if (isInsideZone(ZoneId.NO_PVP))
		{
			if (_lastCompassZone == ExSetCompassZoneCodePacket.NOPVPZONE)
			{
				return;
			}
			_lastCompassZone = ExSetCompassZoneCodePacket.NOPVPZONE;
			sendPacket(new ExSetCompassZoneCodePacket(ExSetCompassZoneCodePacket.NOPVPZONE));
		}
		else
		{
			if (_lastCompassZone == ExSetCompassZoneCodePacket.GENERALZONE)
			{
				return;
			}
			if (_lastCompassZone == ExSetCompassZoneCodePacket.SIEGEWARZONE)
			{
				updatePvPStatus();
			}
			_lastCompassZone = ExSetCompassZoneCodePacket.GENERALZONE;
			sendPacket(new ExSetCompassZoneCodePacket(ExSetCompassZoneCodePacket.GENERALZONE));
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
		if (Events.HasSubscribers<OnPlayerPKChanged>())
		{
			Events.NotifyAsync(new OnPlayerPKChanged(this, _pkKills, pkKills));
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
	public DateTime? getDeleteTime()
	{
		return _deleteTime;
	}
	
	/**
	 * Set the _deleteTimer of the Player.
	 * @param deleteTimer
	 */
	public void setDeleteTimer(DateTime? deleteTime)
	{
		_deleteTime = deleteTime;
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
	public override void setReputation(int value)
	{
		// Notify to scripts.
		if (Events.HasSubscribers<OnPlayerReputationChanged>())
		{
			Events.NotifyAsync(new OnPlayerReputationChanged(this, getReputation(), value));
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

		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_REPUTATION_HAS_BEEN_CHANGED_TO_S1);
		sm.Params.addInt(getReputation());
		sendPacket(sm);
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
					sendPacket(new EtcStatusUpdatePacket(this));
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
		if (isEquiped)
		{
			getDualInventorySet().set(item.getLocationSlot(), 0);
			
			SystemMessagePacket sm;
			if (item.getEnchantLevel() > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.Params.addInt(item.getEnchantLevel());
				sm.Params.addItemName(item);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
				sm.Params.addItemName(item);
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
				SystemMessagePacket sm;
				if (item.getEnchantLevel() > 0)
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S2_EQUIPPED);
					sm.Params.addInt(item.getEnchantLevel());
					sm.Params.addItemName(item);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_EQUIPPED);
					sm.Params.addItemName(item);
				}
				sendPacket(sm);
				
				// Consume mana - will start a task if required; returns if item is not a shadow item
				item.decreaseMana(false);
				
				if ((item.getTemplate().getBodyPart() & ItemTemplate.SLOT_MULTI_ALLWEAPON) != 0)
				{
					rechargeShots(true, true, false);
				}
				
				// Notify to scripts
				EventContainer events = item.getTemplate().Events; 
				if (events.HasSubscribers<OnPlayerItemEquip>())
				{
					events.NotifyAsync(new OnPlayerItemEquip(this, item));
				}
				
				getDualInventorySet().set(item.getLocationSlot(), item.getObjectId());
			}
			else
			{
				sendPacket(SystemMessageId.NO_EQUIPMENT_SLOT_AVAILABLE);
			}
		}
		
		broadcastUserInfo();
		ThreadPool.schedule(() => sendPacket(new ExUserInfoEquipSlotPacket(this)), 100);
		
		InventoryUpdatePacket iu = new InventoryUpdatePacket(items);
		sendInventoryUpdate(iu);
		
		if (abortAttack)
		{
			this.abortAttack();
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
		if (Events.HasSubscribers<OnPlayerPvPChanged>())
		{
			Events.NotifyAsync(new OnPlayerPvPChanged(this, _pvpKills, pvpKills));
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
		
		if (Events.HasSubscribers<OnPlayerFameChanged>())
		{
			Events.NotifyAsync(new OnPlayerFameChanged(this, _fame, newFame));
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
	public CharacterClass getClassId()
	{
		return getTemplate().getClassId();
	}
	
	/**
	 * Set the template of the Player.
	 * @param id The Identifier of the PlayerTemplate to set to the Player
	 */
	public void setClassId(CharacterClass id)
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
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.S1_IS_DISMISSED_FROM_THE_CLAN);
				msg.Params.addPcName(this);
				_clan.broadcastToOnlineMembers(msg);
				_clan.broadcastToOnlineMembers(new PledgeShowMemberListDeletePacket(getName()));
				_clan.removeClanMember(getObjectId(), null);
				sendPacket(SystemMessageId.CONGRATULATIONS_YOU_WILL_NOW_GRADUATE_FROM_THE_CLAN_ACADEMY_AND_LEAVE_YOUR_CURRENT_CLAN_YOU_CAN_NOW_JOIN_A_CLAN_WITHOUT_BEING_SUBJECT_TO_ANY_PENALTIES);
				
				// receive graduation gift
				_inventory.addItem("Gift", 8181, 1, this, null); // give academy circlet
			}
			if (isSubClassActive())
			{
				getSubClasses().get(_classIndex).setClassId(id);
			}
			setTarget(this);
			broadcastPacket(new MagicSkillUsePacket(this, 5103, 1, TimeSpan.Zero, TimeSpan.Zero));
			setClassTemplate(id);
			if (getClassId().GetLevel() == 3)
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
				_party.broadcastPacket(new PartySmallWindowUpdatePacket(this, PartySmallWindowUpdateType.All));
			}
			
			if (_clan != null)
			{
				_clan.broadcastToOnlineMembers(new PledgeShowMemberListUpdatePacket(this));
			}
			
			sendPacket(new ExSubJobInfoPacket(this, SubclassInfoType.CLASS_CHANGED));
			
			// Add AutoGet skills and normal skills and/or learnByFS depending on configurations.
			rewardSkills();
			
			if (!canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS) && Config.DECREASE_SKILL_LEVEL)
			{
				checkPlayerSkills();
			}
			
			notifyFriends(FriendStatusPacket.MODE_CLASS);
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
	public Weapon findFistsWeaponItem(CharacterClass characterClass)
	{
		// TODO the method looks like dirty hack
		int classId = (int)characterClass;
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
			Skill updatedSkill = skill;
			if ((oldSkill != null) && (oldSkill.getSubLevel() > 0) && (skill.getSubLevel() == 0) && (oldSkill.getLevel() < skillLevel))
			{
				updatedSkill = SkillData.getInstance().getSkill(skillId, skillLevel, oldSkill.getSubLevel());
			}
			
			addSkill(updatedSkill, false);
			skillsForStore.add(updatedSkill);
			
			if (Config.AUTO_LEARN_SKILLS)
			{
				updateShortCuts(skillId, skillLevel, updatedSkill.getSubLevel());
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
	public override int? getClanId()
	{
		return _clanId;
	}
	
	/**
	 * @return the Clan Crest Identifier of the Player or 0.
	 */
	public int? getClanCrestId()
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
	public int? getClanCrestLargeId()
	{
		if ((_clan != null) && ((_clan.getCastleId() != 0) || (_clan.getHideoutId() != 0)))
		{
			return _clan.getCrestLargeId();
		}
		return 0;
	}
	
	public DateTime? getClanJoinExpiryTime()
	{
		return _clanJoinExpiryTime;
	}
	
	public void setClanJoinExpiryTime(DateTime? time)
	{
		_clanJoinExpiryTime = time;
	}
	
	public DateTime? getClanJoinTime()
	{
		DateTime time = getVariables().getDateTime(PlayerVariables.CLAN_JOIN_TIME, DateTime.MinValue);
		if (time == DateTime.MinValue)
			return null;
		return time;
	}
	
	public void setClanJoinTime(DateTime time)
	{
		getVariables().set(PlayerVariables.CLAN_JOIN_TIME, time);
	}
	
	public DateTime? getClanCreateExpiryTime()
	{
		return _clanCreateExpiryTime;
	}
	
	public void setClanCreateExpiryTime(DateTime? time)
	{
		_clanCreateExpiryTime = time;
	}
	
	public void setOnlineTime(TimeSpan time)
	{
		_onlineTime = time;
		_onlineBeginTime = DateTime.UtcNow;
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
			broadcastPacket(new ChangeWaitTypePacket(this, ChangeWaitTypePacket.WT_SITTING));
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
			
			broadcastPacket(new ChangeWaitTypePacket(this, ChangeWaitTypePacket.WT_STANDING));
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
	public void addAdena(string process, long count, WorldObject reference, bool sendMessage)
	{
		if (sendMessage)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_ADENA_2);
			sm.Params.addLong(count);
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
				InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(_inventory.getAdenaInstance(), ItemChangeType.MODIFIED));
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
	public bool reduceAdena(string process, long count, WorldObject reference, bool sendMessage)
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
			InventoryUpdatePacket iu = new InventoryUpdatePacket(adenaItem);
			sendInventoryUpdate(iu);
			
			if (sendMessage)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_ADENA_DISAPPEARED);
				sm.Params.addLong(count);
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
	public bool reduceBeautyTickets(string process, long count, WorldObject reference, bool sendMessage)
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
			InventoryUpdatePacket iu = new InventoryUpdatePacket(beautyTickets);
			sendInventoryUpdate(iu);
			
			if (sendMessage)
			{
				if (count > 1)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_X_S2_DISAPPEARED);
					sm.Params.addItemName(Inventory.BEAUTY_TICKET_ID);
					sm.Params.addLong(count);
					sendPacket(sm);
				}
				else
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
					sm.Params.addItemName(Inventory.BEAUTY_TICKET_ID);
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
	public void addAncientAdena(string process, long count, WorldObject reference, bool sendMessage)
	{
		if (sendMessage)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
			sm.Params.addItemName(Inventory.ANCIENT_ADENA_ID);
			sm.Params.addLong(count);
			sendPacket(sm);
		}
		
		if (count > 0)
		{
			_inventory.addAncientAdena(process, count, this, reference);
			
			InventoryUpdatePacket iu = new InventoryUpdatePacket(_inventory.getAncientAdenaInstance());
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
	public bool reduceAncientAdena(string process, long count, WorldObject reference, bool sendMessage)
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
			
			InventoryUpdatePacket iu = new InventoryUpdatePacket(ancientAdenaItem);
			sendInventoryUpdate(iu);
			
			if (sendMessage)
			{
				if (count > 1)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_X_S2_DISAPPEARED);
					sm.Params.addItemName(Inventory.ANCIENT_ADENA_ID);
					sm.Params.addLong(count);
					sendPacket(sm);
				}
				else
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
					sm.Params.addItemName(Inventory.ANCIENT_ADENA_ID);
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
	public void addItem(string process, Item item, WorldObject reference, bool sendMessage)
	{
		if (item.getCount() > 0)
		{
			// Sends message to client if requested
			if (sendMessage)
			{
				if (item.getCount() > 1)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
					sm.Params.addItemName(item);
					sm.Params.addLong(item.getCount());
					sendPacket(sm);
				}
				else if (item.getEnchantLevel() > 0)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_S2);
					sm.Params.addInt(item.getEnchantLevel());
					sm.Params.addItemName(item);
					sendPacket(sm);
				}
				else
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1);
					sm.Params.addItemName(item);
					sendPacket(sm);
				}
			}
			
			// Add the item to inventory
			Item newitem = _inventory.addItem(process, item, this, reference);
			
			// If over capacity, drop the item
			if (!canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !_inventory.validateCapacity(0, item.isQuestItem()) && newitem.isDropable() && (!newitem.isStackable() || (newitem.getLastChange() != ItemChangeType.MODIFIED)))
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
				fort.getSiege().announceToPlayer(new SystemMessagePacket(SystemMessageId.C1_HAS_ACQUIRED_THE_FLAG), getName());
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
	public Item addItem(string process, int itemId, long count, WorldObject reference, bool sendMessage)
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
	public Item addItem(string process, int itemId, long count, int enchantLevel, WorldObject reference, bool sendMessage)
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
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
						sm.Params.addItemName(itemId);
						sm.Params.addLong(count);
						sendPacket(sm);
					}
					else
					{
						SystemMessagePacket sm;
						if (enchantLevel > 0)
						{
							sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_S2_X_S3);
							sm.Params.addInt(enchantLevel);
						}
						else
						{
							sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
						}
						
						sm.Params.addItemName(itemId);
						sm.Params.addLong(count);
						sendPacket(sm);
					}
				}
				else if (process.equalsIgnoreCase("Sweeper") || process.equalsIgnoreCase("Quest"))
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1);
					sm.Params.addItemName(itemId);
					sendPacket(sm);
				}
				else
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1);
					sm.Params.addItemName(itemId);
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
				if (!canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !_inventory.validateCapacity(0, item.isQuestItem()) && createdItem.isDropable() && (!createdItem.isStackable() || (createdItem.getLastChange() != ItemChangeType.MODIFIED)))
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
	public void addItem(string process, ItemHolder item, WorldObject reference, bool sendMessage)
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
	public bool destroyItem(string process, Item item, WorldObject reference, bool sendMessage)
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
	public bool destroyItem(string process, Item item, long count, WorldObject reference, bool sendMessage)
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
		ItemChangeType itemChangeType;
		if (destoyedItem.isStackable() && (destoyedItem.getCount() > 0))
		{
			itemChangeType = ItemChangeType.MODIFIED;
		}
		else
		{
			itemChangeType = ItemChangeType.REMOVED;
		}

		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(new ItemInfo(destoyedItem, itemChangeType));
		sendInventoryUpdate(playerIU);
		
		// Sends message to client if requested
		if (sendMessage)
		{
			if (count > 1)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_X_S2_DISAPPEARED);
				sm.Params.addItemName(destoyedItem);
				sm.Params.addLong(count);
				sendPacket(sm);
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
				sm.Params.addItemName(destoyedItem);
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
	public override bool destroyItem(string process, int objectId, long count, WorldObject reference, bool sendMessage)
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
	public bool destroyItemWithoutTrace(string process, int objectId, long count, WorldObject reference, bool sendMessage)
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
	public override bool destroyItemByItemId(string process, int itemId, long count, WorldObject reference, bool sendMessage)
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
		ItemChangeType itemChangeType;
		if (item.isStackable() && (item.getCount() > 0))
		{
			itemChangeType = ItemChangeType.MODIFIED;
		}
		else
		{
			itemChangeType = ItemChangeType.REMOVED;
		}

		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(new ItemInfo(item, itemChangeType));
		sendInventoryUpdate(playerIU);
		
		// Sends message to client if requested
		if (sendMessage)
		{
			if (quantity > 1)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_X_S2_DISAPPEARED);
				sm.Params.addItemName(itemId);
				sm.Params.addLong(quantity);
				sendPacket(sm);
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
				sm.Params.addItemName(itemId);
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
	public Item transferItem(string process, int objectId, long count, Inventory target, WorldObject reference)
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
		ItemChangeType itemChangeType;
		if ((oldItem.getCount() > 0) && (oldItem != newItem))
		{
			itemChangeType = ItemChangeType.MODIFIED;
		}
		else
		{
			itemChangeType = ItemChangeType.REMOVED;
		}

		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(new ItemInfo(oldItem, itemChangeType));
		sendInventoryUpdate(playerIU);
		
		// Send target update packet
		if (target is PlayerInventory)
		{
			Player targetPlayer = ((PlayerInventory) target).getOwner();
			if (newItem.getCount() > count)
			{
				itemChangeType = ItemChangeType.MODIFIED;
			}
			else
			{
				itemChangeType = ItemChangeType.ADDED;
			}
			InventoryUpdatePacket targetIU = new InventoryUpdatePacket(new ItemInfo(newItem, itemChangeType));
			targetPlayer.sendPacket(targetIU);
		}
		
		// LCoin UI update.
		if (newItem.getId() == Inventory.LCOIN_ID)
		{
			sendPacket(new ExBloodyCoinCountPacket(this));
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
	public bool exchangeItemsById(string process, WorldObject reference, int coinId, long cost, int rewardId, long count, bool sendMessage)
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
	public bool dropItem(string process, Item item, WorldObject reference, bool sendMessage, bool protectItem)
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
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(droppedItem);
		sendInventoryUpdate(playerIU);
		
		// Sends message to client if requested
		if (sendMessage)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_DROPPED_S1);
			sm.Params.addItemName(droppedItem);
			sendPacket(sm);
		}
		
		// LCoin UI update.
		if (item.getId() == Inventory.LCOIN_ID)
		{
			sendPacket(new ExBloodyCoinCountPacket(this));
		}
		
		return true;
	}
	
	public bool dropItem(string process, Item item, WorldObject reference, bool sendMessage)
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
	public Item dropItem(string process, int objectId, long count, int x, int y, int z, WorldObject reference, bool sendMessage, bool protectItem)
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
		if ((Config.AUTODESTROY_ITEM_AFTER > 0) && Config.DESTROY_DROPPED_PLAYER_ITEM && !Config.LIST_PROTECTED_ITEMS.Contains(item.getId()) && ((item.isEquipable() && Config.DESTROY_EQUIPABLE_PLAYER_ITEM) || !item.isEquipable()))
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
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(invitem);
		sendInventoryUpdate(playerIU);
		
		// Sends message to client if requested
		if (sendMessage)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_DROPPED_S1);
			sm.Params.addItemName(item);
			sendPacket(sm);
		}
		
		// LCoin UI update.
		if (item.getId() == Inventory.LCOIN_ID)
		{
			sendPacket(new ExBloodyCoinCountPacket(this));
		}
		
		return item;
	}
	
	public Item checkItemManipulation(int objectId, long count, string action)
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
		return (_spawnProtectEndTime != null) && (_spawnProtectEndTime > DateTime.UtcNow);
	}
	
	public bool isTeleportProtected()
	{
		return (_teleportProtectEndTime != null) && (_teleportProtectEndTime > DateTime.UtcNow);
	}
	
	public void setSpawnProtection(bool protect)
	{
		_spawnProtectEndTime = protect ? DateTime.UtcNow.AddMilliseconds(Config.PLAYER_SPAWN_PROTECTION * 1000) : null;
	}
	
	public void setTeleportProtection(bool protect)
	{
		_teleportProtectEndTime = protect ? DateTime.UtcNow.AddMilliseconds(Config.PLAYER_TELEPORT_PROTECTION * 1000) : null;
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
	public GameSession? getClient()
	{
		return _client;
	}
	
	public void setClient(GameSession? client)
	{
		_client = client;
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
		StatusUpdatePacket su = new StatusUpdatePacket(this);
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
		
		bool needCpUpdate = this.needCpUpdate();
		bool needHpUpdate = this.needHpUpdate();
		bool needMpUpdate = this.needMpUpdate();
		Party party = getParty();
		
		// Check if a party is in progress and party window update is usefull
		if ((_party != null) && (needCpUpdate || needHpUpdate || needMpUpdate))
		{
			PartySmallWindowUpdateType partySmallWindowUpdateType = PartySmallWindowUpdateType.None;
			if (needCpUpdate)
				partySmallWindowUpdateType |= PartySmallWindowUpdateType.CURRENT_CP | PartySmallWindowUpdateType.MAX_CP;
			
			if (needHpUpdate)
				partySmallWindowUpdateType |= PartySmallWindowUpdateType.CURRENT_HP | PartySmallWindowUpdateType.MAX_HP;
            
			if (needMpUpdate)
				partySmallWindowUpdateType |= PartySmallWindowUpdateType.CURRENT_MP | PartySmallWindowUpdateType.MAX_MP;
			
			PartySmallWindowUpdatePacket partyWindow =
				new PartySmallWindowUpdatePacket(this, partySmallWindowUpdateType);

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
			DuelManager.getInstance().broadcastToOppositTeam(this, new ExDuelUpdateUserInfoPacket(this));
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
		sendPacket(new UserInfoPacket(this));
		sendPacket(new ExUserViewInfoParameterPacket(this));
	}
	
	public void broadcastUserInfo(params UserInfoType[] types)
	{
		// Send user info to the current player
		UserInfoPacket ui = new UserInfoPacket(this, false);
		types.forEach(x => ui.addComponentType(x));
		sendPacket(ui);
		
		// Broadcast char info to all known players
		broadcastCharInfo();
	}
	
	public void broadcastCharInfo()
	{
		// Client is disconnected.
		if (getOnlineStatus() == CharacterOnlineStatus.Offline)
		{
			return;
		}
		
		if (_broadcastCharInfoTask == null)
		{
			_broadcastCharInfoTask = ThreadPool.schedule(() =>
			{
				CharacterInfoPacket charInfo = new CharacterInfoPacket(this, false);
				World.getInstance().forEachVisibleObject<Player>(this, player =>
				{
					if (isVisibleFor(player))
					{
						if (isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS))
						{
							player.sendPacket(new CharacterInfoPacket(this, true));
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
							RelationChangedPacket rc = new RelationChangedPacket();
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
		broadcastPacket(new NicknameChangedPacket(this));
	}
	
	public override void broadcastPacket<TPacket>(TPacket packet, bool includeSelf)
	{
		if (packet is CharacterInfoPacket)
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
		if (packet is CharacterInfoPacket)
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
	public override int? getAllyId()
	{
		return _clan == null ? 0 : _clan.getAllyId();
	}
	
	public int? getAllyCrestId()
	{
		return getAllyId() == 0 ? 0 : _clan.getAllyCrestId();
	}
	
	public override void sendPacket<TPacket>(TPacket packet)
	{
		_client?.Connection?.Send(ref packet);
	}
	
	/**
	 * Send SystemMessage packet.
	 * @param id SystemMessageId
	 */
	public override void sendPacket(SystemMessageId id)
	{
		sendPacket(new SystemMessagePacket(id));
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
					sendPacket(new PrivateStoreListSellPacket(this, targetPlayer));
				}
			}
			else if (targetPlayer.getPrivateStoreType() == PrivateStoreType.BUY)
			{
				sendPacket(new PrivateStoreListBuyPacket(this, targetPlayer));
			}
			else if (targetPlayer.getPrivateStoreType() == PrivateStoreType.MANUFACTURE)
			{
				sendPacket(new RecipeShopSellListPacket(this, targetPlayer));
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
		SystemMessagePacket smsg;
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
				smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
				smsg.Params.addItemName(target);
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
				smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
				smsg.Params.addItemName(target);
				sendPacket(smsg);
				return;
			}
			
			if ((target.getOwnerId() != 0) && (target.getOwnerId() != getObjectId()) && !isInLooterParty(target.getOwnerId()))
			{
				if (target.getId() == Inventory.ADENA_ID)
				{
					smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1_ADENA);
					smsg.Params.addLong(target.getCount());
				}
				else if (target.getCount() > 1)
				{
					smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S2_S1_S);
					smsg.Params.addItemName(target);
					smsg.Params.addLong(target.getCount());
				}
				else
				{
					smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
					smsg.Params.addItemName(target);
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
					smsg = new SystemMessagePacket(SystemMessageId.ATTENTION_C1_HAS_PICKED_UP_S2_S3);
					smsg.Params.addPcName(this);
					smsg.Params.addInt(target.getEnchantLevel());
					smsg.Params.addItemName(target.getId());
					broadcastPacket(smsg, 1400);
				}
				else
				{
					smsg = new SystemMessagePacket(SystemMessageId.ATTENTION_C1_HAS_PICKED_UP_S2);
					smsg.Params.addPcName(this);
					smsg.Params.addItemName(target.getId());
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
					sendPacket(new SystemMessagePacket(SystemMessageId.YOU_CANNOT_OPEN_A_PRIVATE_STORE_HERE));
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
				sendPacket(new PrivateStoreManageListBuyPacket(1, this));
				sendPacket(new PrivateStoreManageListBuyPacket(2, this));
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
					sendPacket(new ValidateLocationPacket(newTarget));
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
				sendPacket(new ValidateLocationPacket(target));
			}
			
			// Show the client his new target.
			sendPacket(new MyTargetSelectedPacket(this, target));
			
			// Register target to listen for hp changes.
			target.addStatusListener(this);
			
			// Send max/current hp.
			StatusUpdatePacket su = new StatusUpdatePacket(target);
			su.addUpdate(StatusUpdateType.MAX_HP, target.getMaxHp());
			su.addUpdate(StatusUpdateType.CUR_HP, (int) target.getCurrentHp());
			sendPacket(su);
			
			// To others the new target, and not yourself!
			Broadcast.toKnownPlayers(this, new TargetSelectedPacket(getObjectId(), newTarget.getObjectId(), getX(), getY(), getZ()));
			
			// Send buffs
			sendPacket(new ExAbnormalStatusUpdateFromTargetPacket(target));
		}
		
		// Target was removed?
		if ((newTarget == null) && (getTarget() != null))
		{
			broadcastPacket(new TargetUnselectedPacket(this));
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
			sendPacket(new ExStopItemAutoPeelPacket(true));
			sendPacket(new ExReadyItemAutoPeelPacket(false, 0));
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
					if (Events.HasSubscribers<OnPlayerPvPKill>())
					{
						Events.NotifyAsync(new OnPlayerPvPKill(pk, this));
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
						if (Config.REWARD_PVP_ITEM && (_pvpFlag != PvpFlagStatus.None))
						{
							pk.addItem("PvP Item Reward", Config.REWARD_PVP_ITEM_ID, Config.REWARD_PVP_ITEM_AMOUNT, this, Config.REWARD_PVP_ITEM_MESSAGE);
						}
						// pk
						if (Config.REWARD_PK_ITEM && (_pvpFlag == PvpFlagStatus.None))
						{
							pk.addItem("PK Item Reward", Config.REWARD_PK_ITEM_ID, Config.REWARD_PK_ITEM_AMOUNT, this, Config.REWARD_PK_ITEM_MESSAGE);
						}
					}
				}
				
				// announce pvp/pk
				if (Config.ANNOUNCE_PK_PVP && (((pk != null) && !pk.isGM()) || fpcKill))
				{
					string msg = "";
					if (_pvpFlag == PvpFlagStatus.None)
					{
						msg = Config.ANNOUNCE_PK_MSG.Replace("$killer", killer.getName()).Replace("$target", getName());
						if (Config.ANNOUNCE_PK_PVP_NORMAL_MESSAGE)
						{
							SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_3);
							sm.Params.addString(msg);
							Broadcast.toAllOnlinePlayers(sm);
						}
						else
						{
							Broadcast.toAllOnlinePlayers(msg, false);
						}
					}
					else if (_pvpFlag != PvpFlagStatus.None)
					{
						msg = Config.ANNOUNCE_PVP_MSG.Replace("$killer", killer.getName()).Replace("$target", getName());
						if (Config.ANNOUNCE_PK_PVP_NORMAL_MESSAGE)
						{
							SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_3);
							sm.Params.addString(msg);
							Broadcast.toAllOnlinePlayers(sm);
						}
						else
						{
							Broadcast.toAllOnlinePlayers(msg, false);
						}
					}
				}
				
				if (fpcKill && Config.FAKE_PLAYER_KILL_KARMA && (_pvpFlag != PvpFlagStatus.None) && (getReputation() >= 0))
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
		
		sendPacket(new ExDieInfoPacket(droppedItems == null ? new List<Item>() : droppedItems, _lastDamageTaken));
		
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
			sendPacket(new TimeRestrictFieldDieLimitTimePacket());
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
		if ((getReputation() >= 0) && (pk != null) && (pk.getClan() != null) && (getClan() != null) && (pk.getClan().isAtWarWith(_clanId.Value)
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
						(Config.KARMA_NONDROPPABLE_ITEMS.Contains(itemDrop.getId()) || // Item listed in the non droppable item list
						(Config.KARMA_NONDROPPABLE_PET_ITEMS.Contains(itemDrop.getId())))) // Item listed in the non droppable pet item list
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
						this.dropItem("DieDrop", itemDrop, killer, true);
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
		
		setPvpFlagLasts(DateTime.UtcNow + Config.PVP_NORMAL_TIME);
		if (_pvpFlag != PvpFlagStatus.None)
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
				setPvpFlagLasts(DateTime.UtcNow + Config.PVP_PVP_TIME);
			}
			else
			{
				setPvpFlagLasts(DateTime.UtcNow + Config.PVP_NORMAL_TIME);
			}
			if (_pvpFlag != PvpFlagStatus.None)
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
		summons.AddRange(getServitors().values());
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
	public Model.Request getRequest()
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
		
		SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_BEGIN_TRADING_WITH_C1);
		msg.Params.addPcName(partner);
		sendPacket(msg);
		sendPacket(new TradeStartPacket(1, this));
		sendPacket(new TradeStartPacket(2, this));
	}
	
	public void onTradeConfirm(Player partner)
	{
		SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_HAS_CONFIRMED_THE_TRADE);
		msg.Params.addPcName(partner);
		sendPacket(msg);
		sendPacket(TradeOtherDonePacket.STATIC_PACKET);
	}
	
	public void onTradeCancel(Player partner)
	{
		if (_activeTradeList == null)
		{
			return;
		}
		
		_activeTradeList.@lock();
		_activeTradeList = null;
		sendPacket(new TradeDonePacket(0));
		SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_HAS_CANCELLED_THE_TRADE);
		msg.Params.addPcName(partner);
		sendPacket(msg);
	}
	
	public void onTradeFinish(bool successfull)
	{
		_activeTradeList = null;
		sendPacket(new TradeDonePacket(1));
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
	public string getStoreName()
	{
		return _storeName;
	}
	
	/**
	 * Set the store name.
	 * @param name the store name to set
	 */
	public void setStoreName(string name)
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
		if (Config.OFFLINE_DISCONNECT_FINISHED && (privateStoreType == PrivateStoreType.NONE) && ((_client == null) || _client.IsDetached))
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
			_clanId = null;
			_clanPrivileges = ClanPrivilege.None;
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
		CharInfoTable.getInstance().setClanId(getObjectId(), _clanId.Value);
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
			InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(ammunition, ItemChangeType.MODIFIED));
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
		InventoryUpdatePacket iu = new InventoryUpdatePacket(unequipped.Select(itm => new ItemInfo(itm, ItemChangeType.MODIFIED)).ToList());
		
		sendInventoryUpdate(iu);
		abortAttack();
		broadcastUserInfo();
		
		// This can be 0 if the user pressed the right mousebutton twice very fast.
		if (!unequipped.isEmpty())
		{
			SystemMessagePacket sm;
			Item unequippedItem = unequipped.get(0);
			if (unequippedItem.getEnchantLevel() > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.Params.addInt(unequippedItem.getEnchantLevel());
				sm.Params.addItemName(unequippedItem);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
				sm.Params.addItemName(unequippedItem);
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
			InventoryUpdatePacket iu = new InventoryUpdatePacket(unequipped.Select(itm => new ItemInfo(itm, ItemChangeType.MODIFIED)).ToList());
			sendInventoryUpdate(iu);
			
			abortAttack();
			broadcastUserInfo();
			
			// this can be 0 if the user pressed the right mousebutton twice very fast
			if (!unequipped.isEmpty())
			{
				SystemMessagePacket sm;
				Item unequippedItem = unequipped.get(0);
				if (unequippedItem.getEnchantLevel() > 0)
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
					sm.Params.addInt(unequippedItem.getEnchantLevel());
					sm.Params.addItemName(unequippedItem);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
					sm.Params.addItemName(unequippedItem);
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
		broadcastPacket(new RidePacket(this));
		
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
		broadcastPacket(new RidePacket(this));
		
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
		if (ZoneManager.getInstance().getZone<WaterZone>(new Location3D(getX(), getY(), getZ() - 300)) == null)
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
		sendPacket(new SetupGaugePacket(3, 0, TimeSpan.Zero));
		int petId = _mountNpcId;
		setMount(0, 0);
		stopFeed();
		clearPetData();
		if (wasFlying)
		{
			removeSkill((int)CommonSkill.WYVERN_BREATH);
		}
		broadcastPacket(new RidePacket(this));
		setMountObjectID(0);
		storePetFood(petId);
		
		// Notify self and others about speed change
		broadcastUserInfo();
		
		return true;
	}
	
	public void setUptime(DateTime time)
	{
		_uptime = time;
	}
	
	public TimeSpan getUptime()
	{
		return DateTime.UtcNow - _uptime;
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
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int characterId = getObjectId();
				ctx.Characters.Where(c => c.Id == characterId)
					.ExecuteUpdate(s => s.SetProperty(c => c.AccessLevel, accessLevel.getLevel()));
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
        // TODO do we need access level in the login server?
		//LoginServerThread.getInstance().sendAccessLevel(getAccountName(), level);
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
				RelationChangedPacket rc = new RelationChangedPacket();
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
			ctx.Characters.Where(c => c.Id == characterId).ExecuteUpdate(s =>
				s.SetProperty(c => c.OnlineStatus, getOnlineStatus()).SetProperty(c => c.LastAccess, DateTime.UtcNow));
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			AccountRef? account = ctx.AccountRefs.SingleOrDefault(r => r.Id == _accountId);
			if (account is null)
			{
				account = new AccountRef();
				account.Id = _accountId;
				account.Login = _accountName;
				ctx.AccountRefs.Add(account);
				ctx.SaveChanges();
			}
			
			ctx.Characters.Add(new Character()
			{
				Id = getObjectId(),
				AccountId = _accountId,
                Name = getName(),
                Level = (short)getLevel(),
                MaxHp = getMaxHp(),
                CurrentHp = (int)getCurrentHp(),
                MaxCp = getMaxCp(),
                CurrentCp = (int)getCurrentCp(),
                MaxMp = getMaxMp(),
                CurrentMp = (int)getCurrentMp(),
                Face = _appearance.getFace(),
                HairStyle = _appearance.getHairStyle(),
                HairColor = _appearance.getHairColor(),
                Sex = _appearance.getSex(),
                Exp = getExp(),
                Sp = getSp(),
                Reputation = getReputation(),
                Fame = _fame,
                RaidBossPoints = _raidbossPoints,
                PvpKills = _pvpKills,
                PkKills = _pkKills,
                ClanId = _clanId,
                Class = getClassId(),
                DeleteTime = _deleteTime,
                HasDwarvenCraft = hasDwarvenCraft(),
				Title = getTitle(),
				TitleColor = _appearance.getTitleColor().Value,
				OnlineStatus = getOnlineStatus(),
				ClanPrivileges = (int)_clanPrivileges,
				WantsPeace = _wantsPeace,
				BaseClass = _baseClass,
				IsNobless = isNoble(),
				PowerGrade = 0,
				VitalityPoints = PlayerStat.MIN_VITALITY_POINTS,
				Created = _createDate,
				Kills = getTotalKills(),
				Deaths = getTotalDeaths()
			});

			ctx.SaveChanges();
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			// Retrieve the Player from the characters table of the database
			Character? character = ctx.Characters.SingleOrDefault(c => c.Id == objectId);
			if (character is not null)
			{
				// Retrieve account name
				int accountId = character.AccountId;
				string accountName = ctx.AccountRefs.Where(a => a.Id == accountId).Select(a => a.Login).Single();

				CharacterClass activeClassId = character.Class;
				PlayerTemplate template = PlayerTemplateData.getInstance().getTemplate(activeClassId);
				PlayerAppearance app = new PlayerAppearance(character.Face, character.HairColor, character.HairStyle, character.Sex);
				player = new Player(objectId, template, accountId, accountName, app);
				player.setName(character.Name);
				player.setLastAccess(character.LastAccess);

				player.getStat().setExp(character.Exp);
				player.setExpBeforeDeath(character.ExpBeforeDeath);
				player.getStat().setLevel(character.Level);
				player.getStat().setSp(character.Sp);

				player.setWantsPeace(character.WantsPeace);

				player.setHeading(character.Heading);

				player.setInitialReputation(character.Reputation);
				player.setFame(character.Fame);
				player.setRaidbossPoints(character.RaidBossPoints);
				player.setPvpKills(character.PvpKills);
				player.setPkKills(character.PkKills);
				player.setOnlineTime(character.OnlineTime);
				player.setNoble(character.IsNobless);

				byte? factionId = character.Faction;
				if (factionId == 1)
				{
					player.setGood();
				}

				if (factionId == 2)
				{
					player.setEvil();
				}

				player.setClanJoinExpiryTime(character.ClanJoinExpiryTime);
				if (player.getClanJoinExpiryTime() < DateTime.UtcNow)
				{
					player.setClanJoinExpiryTime(null);
				}

				player.setClanCreateExpiryTime(character.ClanCreateExpiryTime);
				if (player.getClanCreateExpiryTime() < DateTime.UtcNow)
				{
					player.setClanCreateExpiryTime(null);
				}

				player.setPcCafePoints(character.PcCafePoints);

				int? clanId = character.ClanId;
				player.setPowerGrade(character.PowerGrade);
				player.getStat().setVitalityPoints(character.VitalityPoints);
				player.setPledgeType(character.SubPledge);
				// player.setApprentice(rset.getInt("apprentice"));

				if (clanId != null)
				{
					player.setClan(ClanTable.getInstance().getClan(clanId.Value));
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
						player.setClanPrivileges(ClanPrivilege.All);
						player.setPowerGrade(1);
					}

					player.setPledgeClass(ClanMember.calculatePledgeClass(player));
				}
				else
				{
					if (player.isNoble())
					{
						player.setPledgeClass(SocialClass.ELDER);
					}

					if (player.isHero())
					{
						player.setPledgeClass(SocialClass.COUNT);
					}

					player.setClanPrivileges(ClanPrivilege.None);
				}

				player.setTotalDeaths(character.Deaths);
				player.setTotalKills(character.Kills);
				player.setDeleteTimer(character.DeleteTime);
				player.setTitle(character.Title);
				player.setAccessLevel(character.AccessLevel, false, false);
				int titleColor = character.TitleColor;
				if (titleColor != PlayerAppearance.DEFAULT_TITLE_COLOR)
				{
					player.getAppearance().setTitleColor(new Color(titleColor));
				}

				player.setFistsWeaponItem(player.findFistsWeaponItem(activeClassId));
				player.setUptime(DateTime.UtcNow);

				currentHp = character.CurrentHp;
				currentCp = character.CurrentCp;
				currentMp = character.CurrentMp;
				player.setClassIndex(0);
				try
				{
					player.setBaseClass(character.BaseClass);
				}
				catch (Exception e)
				{
					player.setBaseClass(activeClassId);
					LOGGER.Warn(
						"Exception during player.setBaseClass for player: " + player + " base class: " +
						character.BaseClass, e);
				}

				// Restore Subclass Data (cannot be done earlier in function)
				if (restoreSubClassData(player) && (activeClassId != player.getBaseClass()))
				{
					foreach (SubClassHolder subClass in player.getSubClasses().values())
					{
						if (subClass.getClassDefinition() == activeClassId)
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
					LOGGER.Warn(player +
					            " reverted to base class. Possibly has tried a relogin exploit while subclassing.");
				}
				else
				{
					player._activeClass = activeClassId;
				}

				if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS,
					    player.getBaseTemplate().getClassId()))
				{
					player._isDeathKnight = true;
				}
				else if (CategoryData.getInstance()
				         .isInCategory(CategoryType.VANGUARD_ALL_CLASS, player.getBaseTemplate().getClassId()))
				{
					player._isVanguard = true;
				}
				else if (CategoryData.getInstance()
				         .isInCategory(CategoryType.ASSASSIN_ALL_CLASS, player.getBaseTemplate().getClassId()))
				{
					player._isAssassin = true;
				}

				player.setApprentice(character.Apprentice);
				player.setSponsor(character.SponsorId);
				player.setLvlJoinedAcademy(character.LevelJoinedAcademy);

				// Set Hero status if it applies.
				player.setHero(Hero.getInstance().isHero(objectId));

				CursedWeaponsManager.getInstance().checkPlayer(player);

				// Set the x,y,z position of the Player and make it invisible
				int x = character.X;
				int y = character.Y;
				int z = character.Z;
				player.setXYZInvisible(x, y, z);
				player.setLastServerPosition(x, y, z);

				// Set Teleport Bookmark Slot
				player.setBookMarkSlot(character.BookmarkSlot);

				// character creation Time
				player.setCreateDate(character.Created);

				// Language
				player.setLang(character.Language);

				// Retrieve the name and ID of the other characters assigned to this account.
				var query = ctx.Characters.Where(c => c.AccountId == accountId)
					.Select(c => new { c.Id, c.Name, c.SlotIndex });

				foreach (var record in query)
				{
					player._chars.put(record.Id, record.Name);
				}
			}

			if (player == null)
			{
				return null;
			}
			
			if (player.Events.HasSubscribers<OnPlayerLoad>())
			{
				player.Events.NotifyAsync(new OnPlayerLoad(player));
			}
			
			if (player.isGM())
			{
				long masks = player.getVariables().getLong(COND_OVERRIDE_KEY, int.MaxValue);
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = player.getObjectId();
			var query = ctx.CharacterSubClasses.Where(r => r.CharacterId == characterId).OrderBy(r => r.ClassIndex);
			foreach (var record in query)
			{
				SubClassHolder subClass = new SubClassHolder();
				subClass.setClassId(record.SubClass);
				subClass.setDualClassActive(record.DualClass);
				subClass.setVitalityPoints(record.VitalityPoints);
				subClass.setLevel(record.Level);
				subClass.setExp(record.Exp);
				subClass.setSp(record.Sp);
				subClass.setClassIndex(record.ClassIndex);
				
				// Enforce the correct indexing of _subClasses against their class indexes.
				player.getSubClasses().put(subClass.getClassIndex(), subClass);
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
		try 
		{
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            int characterId = getObjectId();
            var query = loadCommon
	            ? ctx.CharacterRecipeBooks.Where(r => r.CharacterId == characterId)
	            : ctx.CharacterRecipeBooks.Where(r =>
		            r.CharacterId == characterId && r.ClassIndex == _classIndex && r.Type == 1);

			_dwarvenRecipeBook.clear();

			RecipeData rd = RecipeData.getInstance();            
            foreach (var record in query)
            {
	            RecipeList recipe = rd.getRecipeList(record.Id);
	            if (loadCommon)
				{
					if (record.Type == 1)
					{
						if (record.ClassIndex == _classIndex)
							registerDwarvenRecipeList(recipe, false);
					}
					else
						registerCommonRecipeList(recipe, false);
				}
				else
					registerDwarvenRecipeList(recipe, false);
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
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
			var query = ctx.CharacterPremiumItems.Where(r => r.CharacterId == characterId);
			foreach (var record in query)
			{
				int itemNum = record.ItemNumber;
				int itemId = record.ItemId;
				long itemCount = record.ItemCount;
				string itemSender = record.ItemSender;
				_premiumItems.put(itemNum, new PremiumItem(itemId, itemCount, itemSender));
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
			ctx.CharacterPremiumItems.Where(r => r.CharacterId == characterId && r.ItemNumber == itemNum)
				.ExecuteUpdate(s => s.SetProperty(c => c.ItemCount, newcount));
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
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            int characterId = getObjectId();
            ctx.CharacterPremiumItems.Where(r => r.CharacterId == characterId && r.ItemNumber == itemNum)
	            .ExecuteDelete();
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
		
		// Store daily missions.
		_dailyMissions.store();
		
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
            Character? character = ctx.Characters.SingleOrDefault(r => r.Id == characterId);
            if (character is null)
            {
	            character = new Character();
	            character.Id = characterId;
	            ctx.Characters.Add(character);
            }
            
            character.Level = (short)level;
            character.MaxHp = getMaxHp();
            character.CurrentHp = (int)getCurrentHp();
            character.MaxCp = getMaxCp();
            character.CurrentCp = (int)getCurrentCp();
			character.MaxMp = getMaxMp();
			character.CurrentMp = (int)getCurrentMp();
			character.Face = _appearance.getFace();
			character.HairStyle = _appearance.getHairStyle();
            character.HairColor = _appearance.getHairColor();
			character.Sex = _appearance.getSex();
            character.Heading = getHeading();
            character.X = _lastLoc != null ? _lastLoc.getX() : getX();
            character.Y = _lastLoc != null ? _lastLoc.getY() : getY();
            character.Z = _lastLoc != null ? _lastLoc.getZ() : getZ();
            character.Exp = exp;
            character.ExpBeforeDeath = _expBeforeDeath;
            character.Sp = sp;
            character.Reputation = getReputation();
            character.Fame = _fame;
            character.RaidBossPoints = _raidbossPoints;
            character.PvpKills = _pvpKills;
            character.PkKills = _pkKills;
            character.ClanId = _clanId;
            character.Class = getClassId();
            character.DeleteTime = _deleteTime;
            character.Title = getTitle();
            character.TitleColor = _appearance.getTitleColor().Value;
            character.OnlineStatus = getOnlineStatus();
            character.ClanPrivileges = (int)_clanPrivileges;
			character.WantsPeace = _wantsPeace;
			character.BaseClass = _baseClass;
			TimeSpan totalOnlineTime = _onlineTime;
			if (_onlineBeginTime != null)
				totalOnlineTime += DateTime.UtcNow - _onlineBeginTime.Value;
            
			character.OnlineTime = _offlineShopStart != null ? _onlineTime : totalOnlineTime;
			character.IsNobless = isNoble();
			character.PowerGrade = _powerGrade;
            character.SubPledge = _pledgeType;
            character.LevelJoinedAcademy = (byte)_lvlJoinedAcademy;
            character.Apprentice = _apprentice;
            character.SponsorId = _sponsor;
            character.ClanJoinExpiryTime = _clanJoinExpiryTime;
            character.ClanCreateExpiryTime = _clanCreateExpiryTime;
            character.Name = getName();
            character.BookmarkSlot = _bookmarkslot;
            character.VitalityPoints = getStat().getBaseVitalityPoints();
            character.Language = _lang;

            byte? factionId = null;
			if (_isGood)
				factionId = 1;

			if (_isEvil)
				factionId = 2;
            
			character.Faction = factionId;
            character.PcCafePoints = _pcCafePoints;
            character.Kills = getTotalKills();
            character.Deaths = getTotalDeaths();
			
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store char base data: " + this + " - " + e);
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = getObjectId();
			foreach (SubClassHolder subClass in getSubClasses().values())
			{
                int classIndex = subClass.getClassIndex();
                CharacterSubClass? record =
	                ctx.CharacterSubClasses.SingleOrDefault(r => r.CharacterId == characterId && r.ClassIndex == classIndex);

                if (record is null)
                {
	                record = new CharacterSubClass();
	                record.CharacterId = characterId;
                    record.ClassIndex = (byte)classIndex;
                    ctx.CharacterSubClasses.Add(record);
                }

                record.Exp = subClass.getExp();
                record.Sp = subClass.getSp();
                record.Level = (short)subClass.getLevel();
                record.VitalityPoints = subClass.getVitalityPoints();
                record.SubClass = subClass.getClassDefinition();
                record.DualClass = subClass.isDualClass();
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store sub class data for " + getName() + ": " + e);
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
			int characterId = getObjectId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			
			// Delete all current stored effects for char to avoid dupe
			ctx.CharacterSkillReuses.Where(r => r.CharacterId == characterId && r.ClassIndex == _classIndex)
				.ExecuteDelete();
			
			int buffIndex = 0;
			List<long> storedSkills = new();
			DateTime currentTime = DateTime.UtcNow;
			
			// Store all effect data along with calculated remaining
			// reuse delays for matching skills. 'restore_type'= 0.
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
					
					TimeStamp t = getSkillReuseTimeStamp(skill.getReuseHashCode());

                    ++buffIndex;
					ctx.CharacterSkillReuses.Add(new CharacterSkillReuse()
                    {
	                    CharacterId = characterId,
                        ClassIndex = (byte)_classIndex,
                        SkillId = skill.getId(),
                        SkillLevel = (short)skill.getLevel(),
                        SkillSubLevel = (short)skill.getSubLevel(),
                        RemainingTime = info.getTime(),
                        ReuseDelay = (t != null) && (currentTime < t.getStamp()) ? t.getReuse() : TimeSpan.Zero,
                        SysTime = (t != null) && (currentTime < t.getStamp()) ? t.getStamp() : null,
                        RestoreType = 0, // Store type 0, active buffs/debuffs.
                        BuffIndex = (byte)buffIndex
                    });
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

					++buffIndex;
					ctx.CharacterSkillReuses.Add(new CharacterSkillReuse()
					{
						CharacterId = characterId,
						ClassIndex = (byte)_classIndex,
						SkillId = t.getSkillId(),
						SkillLevel = (short)t.getSkillLevel(),
						SkillSubLevel = (short)t.getSkillSubLevel(),
						RemainingTime = null,
						ReuseDelay = t.getReuse(),
						SysTime = t.getStamp(),
						RestoreType = 0, // Store type 0, active buffs/debuffs.
						BuffIndex = (byte)buffIndex
					});
				}
			}
			
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store char effect data: " + e);
		}
	}
	
	private void storeItemReuseDelay()
	{
		try 
		{
			int characterId = getObjectId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.CharacterItemReuses.Where(r => r.CharacterId == characterId).ExecuteDelete();
			
			DateTime currentTime = DateTime.UtcNow;
			foreach (TimeStamp ts in getItemReuseTimeStamps().values())
			{
				if ((ts != null) && (currentTime < ts.getStamp()))
				{
					ctx.CharacterItemReuses.Add(new CharacterItemReuse()
					{
						CharacterId = characterId,
						ItemId = ts.getItemId(),
						ItemObjectId = ts.getItemObjectId(),
						ReuseDelay = ts.getReuse(),
						SysTime = ts.getStamp() ?? DateTime.UtcNow
					});
				}
			}
            
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store char item reuse data: " + e);
		}
	}
	
	/**
	 * @return True if the Player is online.
	 */
	public bool isOnline()
	{
		return _isOnline;
	}
	
	public CharacterOnlineStatus getOnlineStatus()
	{
		if (_isOnline && (_client != null))
		{
			return _client.IsDetached ? CharacterOnlineStatus.OnlineDetached : CharacterOnlineStatus.Online;
		}
		return CharacterOnlineStatus.Offline;
	}
	
	public void startOfflinePlay()
	{
		sendPacket(LeaveWorldPacket.STATIC_PACKET);
		
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
		_client.IsDetached = true;
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
		return (_client == null) || _client.IsDetached;
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
                int characterId = getObjectId();
                int skillId = oldSkill.getId();
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

				// Remove a Player skill from the character_skills table of the database
                ctx.CharacterSkills.Where(r => r.CharacterId == characterId && r.SkillId == skillId).ExecuteDelete();
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
  			int characterId = getObjectId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			if ((oldSkill != null) && (newSkill != null))
			{
                int skillId = oldSkill.getId();

                ctx.CharacterSkills
	                .Where(r => r.CharacterId == characterId && r.ClassIndex == classIndex && r.SkillId == skillId)
	                .ExecuteUpdate(s =>
		                s.SetProperty(r => r.SkillLevel, (short)newSkill.getLevel())
			                .SetProperty(r => r.SkillSubLevel, (short)newSkill.getSubLevel()));
			}
			else if (newSkill != null)
			{
                int skillId = newSkill.getId();
                CharacterSkill? record = ctx.CharacterSkills.SingleOrDefault(r =>
	                r.CharacterId == characterId && r.ClassIndex == classIndex && r.SkillId == skillId);

				if (record is null)
				{
					record = new CharacterSkill();
					record.CharacterId = characterId;
					record.ClassIndex = (byte)classIndex;
					record.SkillId = skillId;
					ctx.CharacterSkills.Add(record);
				}
				
				record.SkillLevel = (short)newSkill.getLevel(); 
				record.SkillSubLevel = (short)newSkill.getSubLevel();
				ctx.SaveChanges();
			}
			// else
			// {
			// LOGGER.warning("Could not store new skill, it's null!");
			// }
		}
		catch (Exception e)
		{
			LOGGER.Error("Error could not store char skills: " + e);
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
            int characterId = getObjectId();
			const string ADD_NEW_SKILLS = "REPLACE INTO character_skills (charId,skill_id,skill_level,skill_sub_level,class_index) VALUES (?,?,?,?,?)";
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (Skill addSkill in newSkills)
			{
                int skillId = addSkill.getId();
				CharacterSkill? record = ctx.CharacterSkills.SingleOrDefault(r =>
					r.CharacterId == characterId && r.ClassIndex == classIndex && r.SkillId == skillId);

				if (record is null)
				{
					record = new CharacterSkill();
					record.CharacterId = characterId;
					record.ClassIndex = (byte)classIndex;
					record.SkillId = skillId;
					ctx.CharacterSkills.Add(record);
				}
				
				record.SkillLevel = (short)addSkill.getLevel(); 
				record.SkillSubLevel = (short)addSkill.getSubLevel();
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Error could not store char skills: " + e);
		}
	}
	
	/**
	 * Retrieve from the database all skills of this Player and add them to _skills.
	 */
	private void restoreSkills()
	{
		try 
		{
            const string RESTORE_SKILLS_FOR_CHAR = "SELECT skill_id,skill_level,skill_sub_level FROM character_skills WHERE charId=? AND class_index=?";
            
            int characterId = getObjectId();
            
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			// Retrieve all skills of this Player from the database
			var query = ctx.CharacterSkills.Where(r => r.CharacterId == characterId && r.ClassIndex == _classIndex);
			foreach (var record in query)
			{
					int id = record.SkillId;
					int level = record.SkillLevel;
					int subLevel = record.SkillSubLevel;
					
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
		catch (Exception e)
		{
			LOGGER.Error("Could not restore character " + this + " skills: " + e);
		}
	}
	
	/**
	 * Retrieve from the database all skill effects of this Player and add them to the player.
	 */
	public override void restoreEffects()
	{
		try
		{
            int characterId = getObjectId();
            
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.CharacterSkillReuses.Where(r => r.CharacterId == characterId && r.ClassIndex == _classIndex);
            foreach (var record in query)
            {
					TimeSpan? remainingTime = record.RemainingTime;
					TimeSpan reuseDelay = record.ReuseDelay;
					DateTime? systime = record.SysTime;
					int restoreType = record.RestoreType;
					Skill skill = SkillData.getInstance().getSkill(record.SkillId, record.SkillLevel, record.SkillSubLevel);
					if (skill == null)
					{
						continue;
					}
					
					TimeSpan? time = systime - DateTime.UtcNow;
					if (time > TimeSpan.FromMilliseconds(10))
					{
						disableSkill(skill, time.Value);
						addTimeStamp(skill, reuseDelay, systime);
					}
					
					// Restore Type 1 The remaining skills lost effect upon logout but were still under a high reuse delay.
					if (restoreType > 0)
					{
						continue;
					}
					
					// Restore Type 0 These skill were still in effect on the character upon logout.
					// Some of which were self casted and might still have had a long reuse delay which also is restored.
					skill.applyEffects(this, this, false, remainingTime ?? TimeSpan.Zero);
            }

			// Remove previously restored skills
			ctx.CharacterSkillReuses.Where(r => r.CharacterId == characterId && r.ClassIndex == _classIndex)
				.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore " + this + " active effect data: " + e);
		}
	}
	
	/**
	 * Retrieve from the database all Item Reuse Time of this Player and add them to the player.
	 */
	private void restoreItemReuse()
	{
		try 
        {
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = ctx.CharacterItemReuses.Where(r => r.CharacterId == characterId);
			foreach (var record in query)
			{
				DateTime currentTime = DateTime.UtcNow;
				int itemId = record.ItemId;
				TimeSpan reuseDelay = record.ReuseDelay;
				DateTime systime = record.SysTime;
				bool isInInventory = true;
				TimeSpan remainingTime;
					
				// Using item Id
				Item item = _inventory.getItemByItemId(itemId);
				if (item == null)
				{
					item = getWarehouse().getItemByItemId(itemId);
					isInInventory = false;
				}
					
				if ((item != null) && (item.getId() == itemId) && (item.getReuseDelay() > TimeSpan.Zero))
				{
					remainingTime = systime - currentTime;
					if (remainingTime > TimeSpan.FromMilliseconds(10))
					{
						addTimeStampItem(item, reuseDelay, systime);
						if (isInInventory && item.isEtcItem())
						{
							int group = item.getSharedReuseGroup();
							if (group > 0)
							{
								sendPacket(new ExUseSharedGroupItemPacket(itemId, group, remainingTime, reuseDelay));
							}
						}
					}
				}
			}
			
			// Delete item reuse.
			ctx.CharacterItemReuses.Where(r => r.CharacterId == characterId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore " + this + " Item Reuse data: " + e);
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
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.CharacterHennas.Where(r => r.CharacterId == characterId && r.ClassIndex == _classIndex);
			DateTime currentTime = DateTime.UtcNow;
            foreach (var record in query)
            {
				int slot = record.Slot;
				if ((slot < 1) || (slot > getAvailableHennaSlots()))
				{
					continue;
				}
				
				int symbolId = record.SymbolId;
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
					TimeSpan remainingTime = getVariables().getDateTime("HennaDuration" + slot, currentTime) - currentTime;
					if (remainingTime < TimeSpan.Zero)
					{
						removeHenna(slot);
						continue;
					}
					
					// Add the new task.
					_hennaRemoveSchedules.put(slot, ThreadPool.schedule(new HennaDurationTask(this, slot), remainingTime));
				}
					
				_hennaPoten[slot - 1].setHenna(henna);
					
				// Reward henna skills
				foreach (Skill skill in henna.getSkills())
				{
					addSkill(skill, false);
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
		if (getClassId().GetLevel() == 1)
		{
			totalSlots = 2;
		}
		else if (getClassId().GetLevel() > 1)
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
            int characterId = getObjectId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.CharacterHennas.Where(r => r.CharacterId == characterId && r.ClassIndex == _classIndex && r.Slot == slot).ExecuteDelete();
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
            DateTime now = DateTime.UtcNow;
			TimeSpan remainingTime = getVariables().getDateTime("HennaDuration" + slot, now) - now;
			if ((remainingTime > TimeSpan.Zero) || (henna.getDuration() < 0))
			{
				_inventory.addItem("Henna", henna.getDyeItemId(), henna.getCancelCount(), this, null);
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
				sm.Params.addItemName(henna.getDyeItemId());
				sm.Params.addLong(henna.getCancelCount());
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
		if (Events.HasSubscribers<OnPlayerHennaRemove>())
		{
			Events.NotifyAsync(new OnPlayerHennaRemove(this, henna));
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
					using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                    ctx.CharacterHennas.Add(new CharacterHenna()
                    {
                        CharacterId = getObjectId(),
                        SymbolId = henna.getDyeId(),
                        Slot = slotId,
                        ClassIndex = (byte)_classIndex
                    });
                    
                    ctx.SaveChanges();
				}
				catch (Exception e)
				{
					LOGGER.Error("Failed saving character henna: " + e);
				}
				
				// Task for henna duration
				if (henna.getDuration() > 0)
				{
					getVariables().set("HennaDuration" + slotId, DateTime.UtcNow + TimeSpan.FromMilliseconds(henna.getDuration() * 60000));
					_hennaRemoveSchedules.put(slotId, ThreadPool.schedule(new HennaDurationTask(this, slotId), TimeSpan.FromMilliseconds(henna.getDuration() * 60000)));
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
				if (Events.HasSubscribers<OnPlayerHennaAdd>())
				{
					Events.NotifyAsync(new OnPlayerHennaAdd(this, henna));
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
				_hennaBaseStats.merge(entry.Key, entry.Value, (x, y) => x + y);
			}
		}
	}
	
	private void restoreDyePoten()
	{
		int pos = 0;
		try 
		{
            int characterId = getObjectId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = ctx.CharacterHennaPotens.Where(r => r.CharacterId == characterId);
            foreach (var record in query)
            {
				_hennaPoten[pos] = new HennaPoten();
				_hennaPoten[pos].setSlotPosition(record.SlotPosition);
				_hennaPoten[pos].setEnchantLevel(record.EnchantLevel);
				_hennaPoten[pos].setEnchantExp(record.EnchantExp);
				_hennaPoten[pos].setPotenId(record.PotenId);
				pos++;
            }
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed restoring character " + this + " henna potential: " + e);
		}
		
		for (int i = pos; i < 4; i++)
		{
			_hennaPoten[i] = new HennaPoten(); // TODO: cleared for some reason??? 
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
					int characterId = getObjectId();
                    int slotPosition = _hennaPoten[i].getSlotPosition();
					
                    using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

                    CharacterHennaPoten? record = ctx.CharacterHennaPotens.SingleOrDefault(r =>
	                    r.CharacterId == characterId && r.SlotPosition == slotPosition);
                    
                    if (record is null)
                    {
	                    record = new CharacterHennaPoten();
                        record.CharacterId = characterId;
                        record.SlotPosition = slotPosition;
                    }

                    record.PotenId = _hennaPoten[i].getPotenId();
                    record.EnchantLevel = _hennaPoten[i].getEnchantLevel(); 
                    record.EnchantExp = _hennaPoten[i].getEnchantExp();
                    
                    ctx.SaveChanges();
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
				if ((!getWantsPeace()) && (!attackerPlayer.getWantsPeace()) && !isAcademyMember())
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
		if ((getReputation() < 0) || (_pvpFlag != PvpFlagStatus.None))
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
			SystemMessagePacket sm;
			if (hasSkillReuse(usedSkill.getReuseHashCode()))
			{
				TimeSpan remainingTime = getSkillRemainingReuseTime(usedSkill.getReuseHashCode());
				if (remainingTime.TotalHours > 0)
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_H_S3_MIN_S4_SEC);
					sm.Params.addSkillName(usedSkill);
					sm.Params.addInt(remainingTime.Hours);
					sm.Params.addInt(remainingTime.Minutes);
				}
				else if (remainingTime.TotalMinutes > 0)
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_MIN_S3_SEC);
					sm.Params.addSkillName(usedSkill);
					sm.Params.addInt(remainingTime.Minutes);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_SEC);
					sm.Params.addSkillName(usedSkill);
				}
				
				sm.Params.addInt(remainingTime.Seconds);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_IS_NOT_AVAILABLE_AT_THIS_TIME_BEING_PREPARED_FOR_REUSE);
				sm.Params.addSkillName(usedSkill);
			}
			
			sendPacket(sm);
			sendPacket(ActionFailedPacket.STATIC_PACKET);
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
					sendPacket(new MagicSkillUsePacket(this, this, usedSkill.getDisplayId(), usedSkill.getDisplayLevel(), TimeSpan.Zero, TimeSpan.Zero, usedSkill.getReuseDelayGroup(), -1, SkillCastingType.NORMAL, true));
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
		return getClassId().GetClassInfo().isMage();
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
		MountType type = MountTypeUtil.findByNpcId(npcId);
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
					addSkill(CommonSkill.STRIDER_SIEGE_ASSAULT.getSkill(), false);
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
				sendPacket(new ExUserInfoCubicPacket(this));
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
				sendPacket(new ExUserInfoAbnormalVisualEffectPacket(this));
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
	public void disableAutoShotByCrystalType(CrystalType crystalType)
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
			sendPacket(new ExAutoSoulShotPacket(itemId, false, 0));
			
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_DEACTIVATED);
			sm.Params.addItemName(itemId);
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
			sendPacket(new ExAutoSoulShotPacket(itemId, false, 0));
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_DEACTIVATED);
			sm.Params.addItemName(itemId);
			sendPacket(sm);
		}
		_activeSoulShots.clear();
	}
	
	public BroochJewel? getActiveRubyJewel()
	{
		return _activeRubyJewel;
	}
	
	public void setActiveRubyJewel(BroochJewel? jewel)
	{
		_activeRubyJewel = jewel;
	}
	
	public BroochJewel? getActiveShappireJewel()
	{
		return _activeShappireJewel;
	}
	
	public void setActiveShappireJewel(BroochJewel? jewel)
	{
		_activeShappireJewel = jewel;
	}
	
	public void updateActiveBroochJewel()
	{
		ImmutableArray<BroochJewel> broochJewels = EnumUtil.GetValues<BroochJewel>();
		// Update active Ruby jewel.
		setActiveRubyJewel(null);
		for (int i = broochJewels.Length - 1; i > 0; i--)
		{
			BroochJewel jewel = broochJewels[i];
			if (jewel.isRuby() && _inventory.isItemEquipped(jewel.GetItemId()))
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
			if (jewel.isSapphire() && _inventory.isItemEquipped(jewel.GetItemId()))
			{
				setActiveShappireJewel(jewel);
				break;
			}
		}
	}
	
	public ClanPrivilege getClanPrivileges()
	{
		return _clanPrivileges;
	}
	
	public void setClanPrivileges(ClanPrivilege clanPrivileges)
	{
		_clanPrivileges = clanPrivileges;
	}
	
	public bool hasClanPrivilege(ClanPrivilege privilege)
	{
		return (_clanPrivileges & privilege) != 0;
	}
	
	// baron etc
	public void setPledgeClass(SocialClass classId)
	{
		_pledgeClass = classId;
		checkItemRestriction();
	}
	
	public SocialClass getPledgeClass()
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
	
	public int? getSponsor()
	{
		return _sponsor;
	}
	
	public void setSponsor(int? sponsorId)
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
	
	public override void sendMessage(string message)
	{
		sendPacket(new SystemMessagePacket(SendMessageLocalisationData.getLocalisation(this, message)));
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
		sendPacket(new ObservationModePacket(loc));
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
			sendPacket(new ExUserInfoCubicPacket(this));
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
		sendPacket(new ExOlympiadModePacket(3));
		broadcastUserInfo();
	}
	
	public void leaveObserverMode()
	{
		setTarget(null);
		setInstance(null);
		teleToLocation(_lastLoc, false);
		unsetLastLocation();
		sendPacket(new ObservationReturnPacket(getLocation()));
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
		sendPacket(new ExOlympiadModePacket(0));
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
		sendPacket(new EtcStatusUpdatePacket(this));
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
	public SystemMessagePacket getNoDuelReason()
	{
		SystemMessagePacket sm = new SystemMessagePacket(_noDuelReason);
		sm.Params.addPcName(this);
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
			sendPacket(new ExUserInfoAbnormalVisualEffectPacket(this));
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
	
	public void setWantsPeace(bool wantsPeace)
	{
		_wantsPeace = wantsPeace;
	}
	
	public bool getWantsPeace()
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
				SkillListPacket skillList = new SkillListPacket(lastLearnedSkillId);
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
				
				sendPacket(skillList);
				sendPacket(new AcquireSkillListPacket(this));
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
				sendPacket(new ExStorageMaxCountPacket(this));
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
				sendPacket(new ExUserBoostStatPacket(this, BonusExpType.VITALITY));
				sendPacket(new ExUserBoostStatPacket(this, BonusExpType.BUFFS));
				sendPacket(new ExUserBoostStatPacket(this, BonusExpType.PASSIVE));
				if (Config.ENABLE_VITALITY)
				{
					sendPacket(new ExVitalityEffectInfoPacket(this));
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
	public bool addSubClass(CharacterClass classId, int classIndex, bool isDualClass)
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
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

				// Store the basic info about this new sub-class.
                ctx.CharacterSubClasses.Add(new CharacterSubClass()
                {
                    CharacterId = getObjectId(),
                    SubClass = newClass.getClassDefinition(),
                    Exp = newClass.getExp(),
                    Sp = newClass.getSp(),
                    Level = (short)newClass.getLevel(),
                    VitalityPoints = newClass.getVitalityPoints(),
                    DualClass = newClass.isDualClass()
                });
                
                ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Warn("WARNING: Could not add character sub class for " + getName() + ": " + e);
				return false;
			}
			
			// Commit after database INSERT incase exception is thrown.
			getSubClasses().put(newClass.getClassIndex(), newClass);
			
			CharacterClass subTemplate = classId;
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
	public bool modifySubClass(int classIndex, CharacterClass newClassId, bool isDualClass)
	{
		// Notify to scripts before class is removed.
		if (!getSubClasses().isEmpty() && Events.HasSubscribers<OnPlayerProfessionCancel>())
		{
			CharacterClass classId = getSubClasses().get(classIndex).getClassDefinition();
			Events.NotifyAsync(new OnPlayerProfessionCancel(this, classId));
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
            int characterId = getObjectId();
            
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			// Remove all henna info stored for this sub-class.
            ctx.CharacterHennas.Where(r => r.CharacterId == characterId && r.ClassIndex == classIndex).ExecuteDelete();

			// Remove all shortcuts info stored for this sub-class.
            ctx.CharacterShortCuts.Where(r => r.CharacterId == characterId && r.ClassIndex == classIndex).ExecuteDelete();

			// Remove all effects info stored for this sub-class.
            ctx.CharacterSkillReuses.Where(r => r.CharacterId == characterId && r.ClassIndex == classIndex).ExecuteDelete();

			// Remove all skill info stored for this sub-class.
            ctx.CharacterSkills.Where(r => r.CharacterId == characterId && r.ClassIndex == classIndex).ExecuteDelete();

			// Remove all basic info stored about this sub-class.
            ctx.CharacterSubClasses.Where(r => r.CharacterId == characterId && r.ClassIndex == classIndex).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not modify sub class for " + getName() + " to class index " + classIndex + ": " + e);
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
	
	public CharacterClass getBaseClass()
	{
		return _baseClass;
	}
	
	public CharacterClass getActiveClass()
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
	
	private void setClassTemplate(CharacterClass classId)
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
		if (Events.HasSubscribers<OnPlayerProfessionChange>())
		{
			Events.NotifyAsync(new OnPlayerProfessionChange(this, pcTemplate, isSubClassActive()));
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
					setClassTemplate(getSubClasses().get(classIndex).getClassDefinition());
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
			sendPacket(new ExUserInfoAbnormalVisualEffectPacket(this));
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
			
			sendPacket(new EtcStatusUpdatePacket(this));
			
			restoreHenna();
			sendPacket(new HennaInfoPacket(this));
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
			sendPacket(new ShortCutInitPacket(this));
			broadcastPacket(new SocialActionPacket(getObjectId(), SocialActionPacket.LEVEL_UP));
			sendPacket(new SkillCoolTimePacket(this));
			sendStorageMaxCount();
			
			if (Events.HasSubscribers<OnPlayerSubChange>())
			{
				Events.NotifyAsync(new OnPlayerSubChange(this));
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
			sendPacket(new SetupGaugePacket(getObjectId(), 2, TimeSpan.Zero));
		}
	}
	
	public void startWaterTask()
	{
		if (!isDead() && (_taskWater == null))
		{
			TimeSpan timeinwater = TimeSpan.FromMilliseconds(getStat().getValue(Stat.BREATH, 60000));
			sendPacket(new SetupGaugePacket(getObjectId(), 2, timeinwater));
			_taskWater = ThreadPool.scheduleAtFixedRate(new WaterTask(this), timeinwater, TimeSpan.FromSeconds(1));
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
		
		revalidateZone(true);
		
		notifyFriends(FriendStatusPacket.MODE_ONLINE);
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
			foreach (ZoneType zone in ZoneManager.getInstance().getZones(getLocation().ToLocation3D()))
			{
				zone.onPlayerLoginInside(this);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
		
		// Notify to scripts
		if (Events.HasSubscribers<OnPlayerLogin>())
		{
			Events.NotifyAsync(new OnPlayerLogin(this));
		}
		
		if (isMentee())
		{
			if (Events.HasSubscribers<OnPlayerMenteeStatus>())
			{
				Events.NotifyAsync(new OnPlayerMenteeStatus(this, true));
			}
		}
		else if (isMentor() && Events.HasSubscribers<OnPlayerMentorStatus>())
		{
			Events.NotifyAsync(new OnPlayerMentorStatus(this, true));
		}

		// TODO : Need to fix that hack!
		if (!isDead())
		{
			// Run on a separate thread to give time to above events to be notified.
			ThreadPool.schedule(() =>
			{
				setCurrentCp(_originalCp);
				setCurrentHp(_originalHp);
				setCurrentMp(_originalMp);
			}, 300);
		}
	}
	
	public DateTime? getLastAccess()
	{
		return _lastAccess;
	}
	
	protected void setLastAccess(DateTime? lastAccess)
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
		
		sendPacket(new EtcStatusUpdatePacket(this));
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

			ConfirmDialogPacket dlg;
			if (hasCharmOfCourage())
			{
				dlg = new ConfirmDialogPacket(SystemMessageId.YOUR_CHARM_OF_COURAGE_IS_TRYING_TO_RESURRECT_YOU_WOULD_YOU_LIKE_TO_RESURRECT_NOW, 60000);
				sendPacket(dlg);
				return;
			}
			
			long restoreExp = (long)Math.Round(((_expBeforeDeath - getExp()) * _revivePower) / 100);
			dlg = new ConfirmDialogPacket(SystemMessageId.C1_IS_ATTEMPTING_TO_DO_A_RESURRECTION_THAT_RESTORES_S2_S3_XP_ACCEPT);
			dlg.Params.addPcName(reviver);
			dlg.Params.addLong(restoreExp);
			dlg.Params.addInt(power);
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
			sendPacket(new ExStopItemAutoPeelPacket(true));
			sendPacket(new ExReadyItemAutoPeelPacket(false, 0));
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
			sendPacket(new PetSummonInfoPacket(_pet, 0));
		}
		
		getServitors().values().forEach(s =>
		{
			s.setFollowStatus(false);
			s.teleToLocation(getLocation(), false);
			((SummonAI) s.getAI()).setStartFollowController(true);
			s.setFollowStatus(true);
			s.setInstance(getInstanceWorld());
			s.updateAndBroadcastStatus(0);
			sendPacket(new PetSummonInfoPacket(s, 0));
		});
		
		// Show movie if available.
		if (_movieHolder != null)
		{
			sendPacket(new ExStartScenePlayerPacket(_movieHolder.getMovie()));
		}
		
		// Close time limited zone window.
		if (!isInTimedHuntingZone())
		{
			stopTimedHuntingZoneTask();
		}
		
		// Stop auto play.
		AutoPlayTaskManager.getInstance().stopAutoPlay(this);
		AutoUseTaskManager.getInstance().stopAutoUseTask(this);
		sendPacket(new ExAutoPlaySettingSendPacket(_autoPlaySettings.getOptions(), false, _autoPlaySettings.doPickup(), _autoPlaySettings.getNextTargetMode(), _autoPlaySettings.isShortRange(), _autoPlaySettings.getAutoPotionPercent(), _autoPlaySettings.isRespectfulHunting(), _autoPlaySettings.getAutoPetPotionPercent()));
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
	
	public void broadcastSnoop(ChatType type, string name, string text)
	{
		if (!_snoopListener.isEmpty())
		{
			SnoopPacket sn = new SnoopPacket(getObjectId(), getName(), type, name, text);
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
	public bool validateItemManipulation(int objectId, string action)
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
		if (Events.HasSubscribers<OnPlayerLogout>())
		{
			Events.NotifyAsync(new OnPlayerLogout(this));
		}
		
		try
		{
			foreach (ZoneType zone in ZoneManager.getInstance().getZones(getLocation().ToLocation3D()))
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
			CharInfoTable.getInstance().setLastAccess(getObjectId(), DateTime.UtcNow);
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
		ZoneRegion? region = ZoneManager.getInstance().getRegion(getLocation().ToLocation2D());
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
		
		if (_clanId != null)
		{
			_clan.broadcastToOtherOnlineMembers(new PledgeShowMemberListUpdatePacket(this), this);
			_clan.broadcastToOnlineMembers(new ExPledgeCountPacket(_clan));
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
			if (Events.HasSubscribers<OnPlayerMenteeStatus>())
			{
				Events.NotifyAsync(new OnPlayerMenteeStatus(this, false));
			}
		}
		else if (isMentor() && Events.HasSubscribers<OnPlayerMentorStatus>())
		{
			Events.NotifyAsync(new OnPlayerMentorStatus(this, false));
		}
		
		try
		{
			notifyFriends(FriendStatusPacket.MODE_OFFLINE);
			
			// Friend list
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_FRIEND_S1_HAS_LOGGED_OUT);
			sm.Params.addString(getName());
			foreach (int id in getFriendList())
			{
				WorldObject obj = World.getInstance().findObject(id);
				if (obj != null)
				{
					obj.sendPacket(sm);
				}
			}
			
			// Surveillance list
			ExUserWatcherTargetStatusPacket surveillanceUpdate = new ExUserWatcherTargetStatusPacket(getName(), false);
			sm = new SystemMessagePacket(SystemMessageId.C1_FROM_YOUR_SURVEILLANCE_LIST_IS_OFFLINE);
			sm.Params.addString(getName());
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
		string? ipAddress = _client?.IpAddress.ToString();
		string? macAddress = _client?.HardwareInfo?.getMacAddress();
		return PunishmentManager.getInstance().hasPunishment(getObjectId().ToString(), PunishmentAffect.CHARACTER, PunishmentType.JAIL) //
		       || PunishmentManager.getInstance().hasPunishment(getAccountName(), PunishmentAffect.ACCOUNT, PunishmentType.JAIL) //
		       || (ipAddress != null && PunishmentManager.getInstance().hasPunishment(ipAddress, PunishmentAffect.IP, PunishmentType.JAIL)) //
		       || (macAddress != null && PunishmentManager.getInstance().hasPunishment(macAddress, PunishmentAffect.HWID, PunishmentType.JAIL));
	}
	
	/**
	 * @return {@code true} if player is chat banned, {@code false} otherwise.
	 */
	public bool isChatBanned()
	{
		string? ipAddress = _client?.IpAddress.ToString();
		string? macAddress = _client?.HardwareInfo?.getMacAddress();
		return PunishmentManager.getInstance().hasPunishment(getObjectId().ToString(), PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN) //
			|| PunishmentManager.getInstance().hasPunishment(getAccountName(), PunishmentAffect.ACCOUNT, PunishmentType.CHAT_BAN) //
			|| (ipAddress != null && PunishmentManager.getInstance().hasPunishment(ipAddress, PunishmentAffect.IP, PunishmentType.CHAT_BAN)) //
			|| (macAddress != null && PunishmentManager.getInstance().hasPunishment(macAddress, PunishmentAffect.HWID, PunishmentType.CHAT_BAN));
	}
	
	public void startFameTask(TimeSpan delay, int fameFixRate)
	{
		if ((getLevel() < 40) || (getClassId().GetLevel() < 2))
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
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_SOUL_COUNT_HAS_INCREASED_BY_S1_IT_IS_NOW_AT_S2);
		sm.Params.addInt(count);
		sm.Params.addInt(newCount);
		sendPacket(sm);
		restartSoulTask();
		sendPacket(new EtcStatusUpdatePacket(this));
		
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
		
		sendPacket(new EtcStatusUpdatePacket(this));
		return true;
	}
	
	/**
	 * Clear out all Souls from this Player
	 */
	public void clearSouls()
	{
		_souls.clear();
		stopSoulTask();
		sendPacket(new EtcStatusUpdatePacket(this));
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
		StatusUpdatePacket su = new StatusUpdatePacket(this);
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
		StatusUpdatePacket su = new StatusUpdatePacket(this);
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
		StatusUpdatePacket su = new StatusUpdatePacket(this);
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
				SystemMessagePacket sm;
				if (target.isPlayer())
				{
					sm = new SystemMessagePacket(SystemMessageId.C1_HAS_EVADED_C2_S_ATTACK);
					sm.Params.addPcName(target.getActingPlayer());
					sm.Params.addString(getName());
					target.sendPacket(sm);
				}
				sm = new SystemMessagePacket(SystemMessageId.C1_S_ATTACK_WENT_ASTRAY);
				sm.Params.addPcName(this);
				sendPacket(sm);
			}
			else
			{
				sendPacket(new ExMagicAttackInfoPacket(getObjectId(), target.getObjectId(), ExMagicAttackInfoPacket.EVADED));
			}
			return;
		}
		
		// Check if hit is critical
		if (crit)
		{
			if ((skill == null) || !skill.isMagic())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_LANDED_A_CRITICAL_HIT);
				sm.Params.addPcName(this);
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
					sendPacket(new ExMagicAttackInfoPacket(getObjectId(), target.getObjectId(), ExMagicAttackInfoPacket.M_CRITICAL));
				}
				else if (skill.isPhysical())
				{
					sendPacket(new ExMagicAttackInfoPacket(getObjectId(), target.getObjectId(), ExMagicAttackInfoPacket.P_CRITICAL));
				}
				else
				{
					sendPacket(new ExMagicAttackInfoPacket(getObjectId(), target.getObjectId(), ExMagicAttackInfoPacket.CRITICAL));
				}
			}
		}
		
		if (elementalCrit)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_ATTACK_CRITICAL_IS_ACTIVATED);
			sm.Params.addElementalSpirit(getActiveElementalSpiritType());
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
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_HIT_FOR_S1_DAMAGE);
			sm.Params.addInt(damage);
			sendPacket(sm);
		}
		else if (this != target)
		{
			SystemMessagePacket sm;
			if (elementalDamage != 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_HAS_DEALT_S3_DAMAGE_TO_S2_S4_ATTRIBUTE_DAMAGE);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.C1_HAS_DEALT_S3_DAMAGE_TO_C2);
			}
			
			sm.Params.addPcName(this);
			
			// Localisation related.
			string targetName = target.getName();
			if (Config.MULTILANG_ENABLE && target.isNpc())
			{
				string[] localisation = NpcNameLocalisationData.getInstance().getLocalisation(_lang, target.getId());
				if (localisation != null)
				{
					targetName = localisation[0];
				}
			}
			
			sm.Params.addString(targetName);
			sm.Params.addInt(damage);
			if (elementalDamage != 0)
			{
				sm.Params.addInt((int) elementalDamage);
			}
			sm.Params.addPopup(target.getObjectId(), getObjectId(), -damage);
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
	
	public void setSayhaGraceSupportEndTime(DateTime endTime)
	{
		if (getVariables().getDateTime(PlayerVariables.SAYHA_GRACE_SUPPORT_ENDTIME, DateTime.MinValue) < DateTime.UtcNow)
		{
			getVariables().set(PlayerVariables.SAYHA_GRACE_SUPPORT_ENDTIME, endTime);
			sendPacket(new ExUserBoostStatPacket(this, BonusExpType.VITALITY));
			sendPacket(new ExVitalityEffectInfoPacket(this));
		}
	}
	
	public DateTime? getSayhaGraceSupportEndTime()
	{
        DateTime value = getVariables().getDateTime(PlayerVariables.SAYHA_GRACE_SUPPORT_ENDTIME, DateTime.MinValue);
		return value == DateTime.MinValue ? null : value;
	}
	
	public bool setLimitedSayhaGraceEndTime(DateTime endTime)
	{
		if (endTime > getVariables().getDateTime(PlayerVariables.LIMITED_SAYHA_GRACE_ENDTIME, DateTime.MinValue))
		{
			getVariables().set(PlayerVariables.LIMITED_SAYHA_GRACE_ENDTIME, endTime);
			sendPacket(new ExUserBoostStatPacket(this, BonusExpType.VITALITY));
			sendPacket(new ExVitalityEffectInfoPacket(this));
			return true;
		}
		return false;
	}
	
	public DateTime? getLimitedSayhaGraceEndTime()
	{
		DateTime value = getVariables().getDateTime(PlayerVariables.LIMITED_SAYHA_GRACE_ENDTIME, DateTime.MinValue);
        return value == DateTime.MinValue ? null : value;
	}
	
	public void checkItemRestriction()
	{
		for (int i = 0; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
		{
			Item equippedItem = _inventory.getPaperdollItem(i);
			if ((equippedItem != null) && !equippedItem.getTemplate().checkCondition(this, this, false))
			{
				_inventory.unEquipItemInSlot(i);
				
				InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(equippedItem, ItemChangeType.MODIFIED));
				sendInventoryUpdate(iu);
				
				if (equippedItem.getTemplate().getBodyPart() == ItemTemplate.SLOT_BACK)
				{
					sendPacket(SystemMessageId.YOUR_CLOAK_HAS_BEEN_UNEQUIPPED_BECAUSE_YOUR_ARMOR_SET_IS_NO_LONGER_COMPLETE);
					return;
				}
				
				SystemMessagePacket sm;
				if (equippedItem.getEnchantLevel() > 0)
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
					sm.Params.addInt(equippedItem.getEnchantLevel());
					sm.Params.addItemName(equippedItem);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
					sm.Params.addItemName(equippedItem);
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
            foreach (Skill skill in _transformSkills.values()) 
			    currentSkills.Add(skill);
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
			TimeSpan duration = TimeSpan.FromMilliseconds((1.0 * getMaxFeed() * 10000) / getFeedConsume());
			sendPacket(new SetupGaugePacket(3, (_curFeed * 10000) / getFeedConsume(), duration));
			if (!isDead())
			{
				_mountFeedTask = ThreadPool.scheduleAtFixedRate(new PetFeedTask(this), 10000, 10000);
			}
		}
		else if (_canFeed)
		{
			setCurrentFeed(getMaxFeed());
			TimeSpan duration = TimeSpan.FromMilliseconds((1.0 * getMaxFeed() * 10000) / getFeedConsume());
			sendPacket(new SetupGaugePacket(3, (_curFeed * 10000) / getFeedConsume(), duration));
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
		TimeSpan duration = TimeSpan.FromMilliseconds((1.0 * getMaxFeed() * 10000) / getFeedConsume());
		sendPacket(new SetupGaugePacket(3, (_curFeed * 10000) / getFeedConsume(), duration));
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
			try
			{
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                ctx.Pets.Where(p => p.ItemObjectId == _controlItemId)
	                .ExecuteUpdate(s => s.SetProperty(p => p.Fed, _curFeed));

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
		
		sendPacket(new EtcStatusUpdatePacket(this));
		return true;
	}
	
	public void clearCharges()
	{
		_charges.set(0);
		sendPacket(new EtcStatusUpdatePacket(this));
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
	
	public void teleportBookmarkModify(int id, int icon, string tag, string name)
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
                int characterId = getObjectId();
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                ctx.CharacterTeleportBookmarks.Where(r => r.CharacterId == characterId && r.Id == id).ExecuteUpdate(s =>
	                s.SetProperty(r => r.Icon, icon).SetProperty(r => r.Tag, tag).SetProperty(r => r.Name, name));
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
                int characterId = getObjectId();
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                ctx.CharacterTeleportBookmarks.Where(r => r.CharacterId == characterId && r.Id == id).ExecuteDelete();
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
		
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
		sm.Params.addItemName(13016);
		sendPacket(sm);
		
		TeleportBookmark bookmark = _tpbookmarks.get(id);
		if (bookmark != null)
		{
			if (isInTimedHuntingZone(bookmark.Location.X, bookmark.Location.Y))
			{
				sendMessage("You cannot teleport at this location.");
				return;
			}
			
			destroyItem("Consume", _inventory.getItemByItemId(13016).getObjectId(), 1, null, false);
			setTeleportLocation(new Location(bookmark.Location.X, bookmark.Location.Y, bookmark.Location.Z));
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
	
	public void teleportBookmarkAdd(int x, int y, int z, int icon, string tag, string name)
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

		_tpbookmarks.put(id, new TeleportBookmark(id, new Location3D(x, y, z), icon, tag, name));
		
		try 
		{
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.CharacterTeleportBookmarks.Add(new CharacterTeleportBookmark()
            {
                CharacterId = getObjectId(),
                Id = id,
                X = x,
                Y = y,
                Z = z,
                Icon = icon,
                Tag = tag,
                Name = name
            });
            
            ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not insert character teleport bookmark data: " + e);
		}
        
		sendPacket(new ExGetBookMarkInfoPacket(this));
	}
	
	public void restoreTeleportBookmark()
	{
		try
		{
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.CharacterTeleportBookmarks.Where(r => r.CharacterId == characterId);
            foreach (var record in query)
            {
				_tpbookmarks.put(record.Id, new TeleportBookmark(record.Id, new Location3D(record.X, record.Y, record.Z), record.Icon, record.Tag, record.Name));
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
			player.sendPacket(new CharacterInfoPacket(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
			player.sendPacket(new GetOnVehiclePacket(getObjectId(), getBoat().getObjectId(), _inVehiclePosition));
		}
		else if (isInAirShip())
		{
			setXYZ(getAirShip().getLocation());
			player.sendPacket(new CharacterInfoPacket(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
			player.sendPacket(new ExGetOnAirShipPacket(this, getAirShip()));
		}
		else
		{
			player.sendPacket(new CharacterInfoPacket(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
		}
		
		long relation1 = getRelation(player);
		RelationChangedPacket rc1 = new RelationChangedPacket();
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
		RelationChangedPacket rc2 = new RelationChangedPacket();
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
				player.sendPacket(new PrivateStoreMsgSellPacket(this));
				break;
			}
			case PrivateStoreType.PACKAGE_SELL:
			{
				player.sendPacket(new ExPrivateStoreSetWholeMsgPacket(this));
				break;
			}
			case PrivateStoreType.BUY:
			{
				player.sendPacket(new PrivateStoreMsgBuyPacket(this));
				break;
			}
			case PrivateStoreType.MANUFACTURE:
			{
				player.sendPacket(new RecipeShopMsgPacket(this));
				break;
			}
		}
		
		// Required for showing mount transformations to players that just entered the game.
		if (isTransformed())
		{
			player.sendPacket(new CharacterInfoPacket(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
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
			sendPacket(new ExStartScenePlayerPacket(holder.getMovie()));
		}
	}
	
	public void stopMovie()
	{
		sendPacket(new ExStopScenePlayerPacket(_movieHolder.getMovie()));
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
		TimeSpan time = (DateTime.UtcNow - _createDate) / 1000;
		return (int)(time.TotalDays / 365); // TODO: calculate full years
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
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.CharacterFriends.Where(r => r.CharacterId == characterId && r.Relation == 0)
	            .Select(r => r.FriendId);

            foreach (int friendId in query)
			{
				if (friendId == getObjectId())
				{
					continue;
				}

				_friendList.add(friendId);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error found in " + getName() + "'s FriendList: " + e);
		}
	}
	
	public void notifyFriends(int type)
	{
		FriendStatusPacket pkt = new FriendStatusPacket(this, type);
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
			int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.CharacterSurveillances.Where(r => r.CharacterId == characterId).Select(r => r.TargetId);
            foreach (int targetId in query)
            {
				if (targetId == getObjectId())
				{
					continue;
				}
                
                _surveillanceList.add(targetId);
            }
		}
		catch (Exception e)
		{
			LOGGER.Error("Error found in " + getName() + "'s SurveillanceList: " + e);
		}
	}
	
	public void updateFriendMemo(string name, string memo)
	{
		if (memo.Length > 50)
		{
			return;
		}
		
		try 
		{
            int characterId = getObjectId();
			int friendId = CharInfoTable.getInstance().getIdByName(name);
            
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.CharacterFriends.Where(r => r.CharacterId == characterId && r.FriendId == friendId).
                ExecuteUpdate(s => s.SetProperty(r => r.Memo, memo));
			
			CharInfoTable.getInstance().setFriendMemo(getObjectId(), friendId, memo);
		}
		catch (Exception e)
		{
			LOGGER.Error("Error occurred while updating friend memo: " + e);
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
		sendPacket(new EtcStatusUpdatePacket(this));
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
                int characterId = getObjectId(); 
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

                ctx.CharacterRecipeShopLists.Where(r => r.CharacterId == characterId).ExecuteDelete();

                int slot = 1;
				foreach (ManufactureItem item in _manufactureItems.values())
				{
                    ctx.CharacterRecipeShopLists.Add(new CharacterRecipeShopList()
                    {
                        CharacterId = characterId,
                        RecipeId = item.getRecipeId(),
                        Price = item.getCost(),
                        Index = (short)slot
                    });

                    slot++;                    
				}
                
                ctx.SaveChanges();
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
            int characterId = getObjectId(); 
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.CharacterRecipeShopLists.Where(r => r.CharacterId == characterId).OrderBy(r => r.Index);

            foreach (var record in query)
			{
				getManufactureItems().put(record.RecipeId, new ManufactureItem(record.RecipeId, record.Price));
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
		
		float defaultCollisionRadius = _appearance.getSex() == Sex.Female ? getBaseTemplate().getFCollisionRadiusFemale() : getBaseTemplate().getFCollisionRadius();
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
		
		float defaultCollisionHeight = _appearance.getSex() == Sex.Female ? getBaseTemplate().getFCollisionHeightFemale() : getBaseTemplate().getFCollisionHeight();
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
		
		if ((_fallingTimestamp != DateTime.MinValue) && (DateTime.UtcNow < _fallingTimestamp))
		{
			return true;
		}
		
		int deltaZ = getZ() - z;
		if (deltaZ <= getBaseTemplate().getSafeFallHeight())
		{
			_fallingTimestamp = null;
			return false;
		}
		
		// If there is no geodata loaded for the place we are, client Z correction might cause falling damage.
		if (!GeoEngine.getInstance().hasGeo(getX(), getY()))
		{
			_fallingTimestamp = null;
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
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_RECEIVED_S1_DAMAGE_FROM_FALLING);
				sm.Params.addInt(_fallingDamage);
				sendPacket(sm);
			}
			_fallingDamage = 0;
			_fallingDamageTask = null;
		}, 1500);
		
		// Prevent falling under ground.
		sendPacket(new ValidateLocationPacket(this));
		setFalling();
		
		return false;
	}
	
	/**
	 * Set falling timestamp
	 */
	public void setFalling()
	{
		_fallingTimestamp = DateTime.UtcNow + FALLING_VALIDATION_DELAY;
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
		_lastItemAuctionInfoRequest = DateTime.UtcNow;
	}
	
	/**
	 * @return true if receiving item auction requests<br>
	 *         (last request was in 2 seconds before)
	 */
	public bool isItemAuctionPolling()
	{
		return (DateTime.UtcNow - _lastItemAuctionInfoRequest) < TimeSpan.FromSeconds(2);
	}
	
	public override bool isMovementDisabled()
	{
		return base.isMovementDisabled() || (_movieHolder != null) || _fishing.isFishing();
	}
	
	public string getLang()
	{
		return _lang;
	}
	
	public bool setLang(string? lang)
	{
		if (lang != null && Config.MULTILANG_ENABLE)
		{
			if (Config.MULTILANG_ALLOWED.Contains(lang))
			{
				_lang = lang;
				return true;
			}

			_lang = Config.MULTILANG_DEFAULT;
			return false;
		}

		_lang = null;
		return false;
	}
	
	public DateTime? getOfflineStartTime()
	{
		return _offlineShopStart;
	}
	
	public void setOfflineStartTime(DateTime? time)
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
		sendPacket(new ExPledgeCoinInfoPacket(this));
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
				if ((target.getPvpFlag() != PvpFlagStatus.None) && (target.getReputation() >= 0))
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
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            bool petItems = ctx.Items.Where(r =>
	            r.OwnerId == characterId &&
	            (r.Location == (int)ItemLocation.PET || r.Location == (int)ItemLocation.PET_EQUIP)).Any();
            
			setPetInvItems(petItems);
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not check Items in Pet Inventory for playerId: " + getObjectId() + ": " + e);
		}
	}
	
	public string getAdminConfirmCmd()
	{
		return _adminConfirmCmd;
	}
	
	public void setAdminConfirmCmd(string adminConfirmCmd)
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
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            CharacterRecoBonus? record = ctx.CharacterRecoBonuses.Where(r => r.CharacterId == characterId).
                SingleOrDefault();
            
            if (record is not null)
            {
				setRecomHave(record.RecHave);
				setRecomLeft(record.RecLeft);
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
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            CharacterRecoBonus? record = ctx.CharacterRecoBonuses.SingleOrDefault(r => r.CharacterId == characterId);

            if (record is null)
            {
                record = new CharacterRecoBonus();
                record.CharacterId = characterId;
            }
                    
            record.RecHave = (short)_recomHave; 
            record.RecLeft = (short)_recomLeft; 
            record.TimeLeft = TimeSpan.Zero; 

			ctx.SaveChanges();
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
		sendPacket(new ExBrPremiumStatePacket(this));
	}
	
	public bool hasPremiumStatus()
	{
		return Config.PREMIUM_SYSTEM_ENABLED && _premiumStatus;
	}
	
	public void setLastPetitionGmName(string gmName)
	{
		_lastPetitionGmName = gmName;
	}
	
	public string getLastPetitionGmName()
	{
		return _lastPetitionGmName;
	}
	
	public ContactList getContactList()
	{
		return _contactList;
	}
	
	public DateTime? getNotMoveUntil()
	{
		return _notMoveUntil;
	}
	
	public void updateNotMoveUntil()
	{
		_notMoveUntil = DateTime.UtcNow + TimeSpan.FromMilliseconds(Config.PLAYER_MOVEMENT_BLOCK_TIME);
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
		return vars != null ? vars : addScript(new AccountVariables(getAccountId()));
	}
	
	public override int getId()
	{
		return (int)getClassId();
	}
	
	public bool isPartyBanned()
	{
		return PunishmentManager.getInstance().hasPunishment(getObjectId().ToString(), PunishmentAffect.CHARACTER, PunishmentType.PARTY_BAN);
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
		if (Events.HasSubscribers<OnPlayerAbilityPointsChanged>())
		{
			Events.NotifyAsync(new OnPlayerAbilityPointsChanged(this, getAbilityPointsUsed(), points));
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
		
		int? castleId = _clan.getCastleId();
		if (castleId is null)
		{
			return CastleSide.NEUTRAL;
		}
		
		Castle castle = CastleManager.getInstance().getCastleById(castleId.Value);
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
		return canRequest(request) && _requests.TryAdd(request.GetType(), request);
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
		var requests = _requests.ToList();
        foreach (var kvp in requests)
        {
            if (kvp.Value.isUsing(objectId))
				_requests.remove(kvp.Key);
        }    
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
	
	private void DisableExperienceReceived(OnPlayableExpChanged arg)
	{
		if (!isDead())
		{
			arg.Terminate = true;
			arg.Abort = true;
		}
	}
	
	public void disableExpGain()
	{
		Events.Subscribe(this, (Action<OnPlayableExpChanged>)DisableExperienceReceived); 
	}
	
	public void enableExpGain()
	{
		Events.Unsubscribe((Action<OnPlayableExpChanged>)DisableExperienceReceived); 
	}
    
	/**
	 * Gets the last commission infos.
	 * @return the last commission infos
	 */
	public Map<int, ExResponseCommissionInfoPacket> getLastCommissionInfos()
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
	
	public void sendInventoryUpdate(InventoryUpdatePacket iu)
	{
		sendPacket(iu);
		sendPacket(new ExAdenaInvenCountPacket(this));
		sendPacket(new ExUserInfoInventoryWeightPacket(this));
	}
	
	public void sendItemList()
	{
		if (_itemListTask == null)
		{
			_itemListTask = ThreadPool.schedule(() =>
			{
				sendPacket(new ItemListPacket(1, this));
				sendPacket(new ItemListPacket(2, this));
				sendPacket(new ExQuestItemListPacket(1, this));
				sendPacket(new ExQuestItemListPacket(2, this));
				sendPacket(new ExAdenaInvenCountPacket(this));
				sendPacket(new ExUserInfoInventoryWeightPacket(this));
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
	
    public PlayerDailyMissionList getDailyMissions() => _dailyMissions;
    
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
			foreach (TimerHolder timer in _timerHolders)
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
	
	public void addTimerHolder(TimerHolder timer)
	{
		lock (_timerHolders)
		{
			_timerHolders.add(timer);
		}
	}
	
	public void removeTimerHolder(TimerHolder timer)
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
		return isInParty() ? (_party.isInCommandChannel() ? GroupType.COMMAND_CHANNEL : GroupType.PARTY) : GroupType.None;
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
		string info = getAccountVariables().getString(TRAINING_CAMP_VAR, null);
		if (info == null)
		{
			return null;
		}

        string[] comps = info.Split(";");
		return new TrainingHolder(int.Parse(comps[0]), int.Parse(comps[1]),
			int.Parse(comps[2]), new DateTime(long.Parse(comps[3])), new DateTime(long.Parse(comps[4])));
	}
	
	public void setTraingCampInfo(TrainingHolder holder)
	{
		getAccountVariables().set(TRAINING_CAMP_VAR,
			holder.getObjectId() + ";" + holder.getClassIndex() + ";" + holder.getLevel() + ";" +
			holder.getStartTime().Ticks + ";" + (holder.getEndTime()?.Ticks ?? 0));
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
		return (trainingHolder != null) && (trainingHolder.getEndTime() > DateTime.UtcNow);
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
		DateTime receiveDate;
		int rewardIndex;
		if (Config.ATTENDANCE_REWARDS_SHARE_ACCOUNT)
		{
			receiveDate = getAccountVariables().getDateTime(PlayerVariables.ATTENDANCE_DATE, DateTime.MinValue);
			rewardIndex = getAccountVariables().getInt(PlayerVariables.ATTENDANCE_INDEX, 0);
		}
		else
		{
			receiveDate = getVariables().getDateTime(PlayerVariables.ATTENDANCE_DATE, DateTime.MinValue);
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
	
	public TimeSpan getAttendanceDelay()
	{
		DateTime currentTime = DateTime.UtcNow;
		TimeSpan remainingTime = _attendanceDelay - currentTime;
        if (remainingTime < TimeSpan.Zero)
            return TimeSpan.Zero;
        
		return remainingTime;
	}
	
	public void setAttendanceDelay(TimeSpan timeInMinutes)
	{
		_attendanceDelay = DateTime.UtcNow + timeInMinutes;
	}
	
	public int getVipTier()
	{
		return _vipTier;
	}
	
	public void setVipTier(int vipTier)
	{
		_vipTier = vipTier;
	}
	
	public long getVipPoints()
	{
		return getAccountVariables().getLong(AccountVariables.VIP_POINTS, 0L);
	}
	
	public DateTime? getVipTierExpiration()
	{
		DateTime value = getAccountVariables().getDateTime(AccountVariables.VIP_EXPIRATION, DateTime.MinValue);
        return value == DateTime.MinValue ? null : value;
	}
	
	public void setVipTierExpiration(DateTime expiration)
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
		int newTier = VipManager.getInstance().getVipTier(getVipPoints());
		if (newTier != currentVipTier)
		{
			_vipTier = newTier;
			if (newTier > 0)
			{
				getAccountVariables().set(AccountVariables.VIP_EXPIRATION, DateTime.UtcNow.AddDays(30));
				VipManager.getInstance().manageTier(this);
			}
			else
			{
				getAccountVariables().set(AccountVariables.VIP_EXPIRATION, 0L);
			}
		}
		getAccountVariables().storeMe(); // force to store to prevent falty purchases after a crash.
		sendPacket(new ReceiveVipInfoPacket(this));
	}
	
	public void initElementalSpirits()
	{
		tryLoadSpirits();
		
		if (_spirits == null)
		{
			ImmutableArray<ElementalType> types = EnumUtil.GetValues<ElementalType>();
			_spirits = new ElementalSpirit[types.Length - 1]; // exclude None
			foreach (ElementalType type in types)
			{
				if (ElementalType.NONE == type)
				{
					continue;
				}
				
				ElementalSpirit spirit = new ElementalSpirit(type, this);
				_spirits[(int)type - 1] = spirit;
				spirit.save();
			}
		}
		
		if (_activeElementalSpiritType == null)
		{
			changeElementalSpirit(ElementalType.FIRE);
		}
	}
	
	private void tryLoadSpirits()
	{
		List<ElementalSpiritDataHolder> restoredSpirits = new();
		try 
		{
            int characterId = getObjectId(); 
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = ctx.CharacterSpirits.Where(r => r.CharacterId == characterId);
            foreach (var record in query)
            {
				ElementalSpiritDataHolder newHolder = new ElementalSpiritDataHolder();
                
				newHolder.setCharId(record.CharacterId);
				ElementalType type = (ElementalType)record.Type;
				newHolder.setType(type);
				byte level = record.Level;
				newHolder.setLevel(level);
				byte stage = record.Stage;
				newHolder.setStage(stage);
				long experience = Math.Min(record.Exp, ElementalSpiritData.getInstance().getSpirit(type, stage).getMaxExperienceAtLevel(level));
				newHolder.setExperience(experience);
    			newHolder.setAttackPoints(record.AttackPoints);
				newHolder.setDefensePoints(record.DefensePoints);
				newHolder.setCritRatePoints(record.CriticalRatePoints);
				newHolder.setCritDamagePoints(record.CriticalDamagePoints);
				newHolder.setInUse(record.IsInUse);
                
				restoredSpirits.add(newHolder);
            }
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
		
		if (!restoredSpirits.isEmpty())
		{
			_spirits = new ElementalSpirit[EnumUtil.GetValues<ElementalType>().Length - 1];
			foreach (ElementalSpiritDataHolder spiritData in restoredSpirits)
			{
				_spirits[(int)spiritData.getType() - 1] = new ElementalSpirit(spiritData, this);
				if (spiritData.isInUse())
				{
					_activeElementalSpiritType = spiritData.getType();
				}
			}
            
			ThreadPool.schedule(() =>
			{
				sendPacket(new ElementalSpiritInfoPacket(this, (byte) 0));
				sendPacket(new ExElementalSpiritAttackTypePacket(this));
			}, 4000);
		}
	}
	
	public double getActiveElementalSpiritAttack()
	{
		return getStat().getElementalSpiritPower(_activeElementalSpiritType, 
			getElementalSpirit(_activeElementalSpiritType)?.getAttack() ?? 0);
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
		return getStat().getElementalSpiritDefense(type, getElementalSpirit(type)?.getDefense() ?? 0);
	}
	
	public override double getElementalSpiritAttackOf(ElementalType type)
	{
		return getStat().getElementSpiritAttack(type, getElementalSpirit(type)?.getAttack() ?? 0);
	}
	
	public double getElementalSpiritCritRate()
	{
		return getStat().getElementalSpiritCriticalRate(getElementalSpirit(_activeElementalSpiritType)?.getCriticalRate() ?? 0);
	}
	
	public double getElementalSpiritCritDamage()
	{
		return getStat().getElementalSpiritCriticalDamage(getElementalSpirit(_activeElementalSpiritType)?.getCriticalDamage() ?? 0);
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
		return _spirits[(int)type - 1];
	}
	
	public ElementalType getActiveElementalSpiritType()
	{
		return _activeElementalSpiritType;
	}
	
	public void changeElementalSpirit(ElementalType element)
	{
		_activeElementalSpiritType = element;
		if (_spirits != null)
		{
			foreach (ElementalSpirit spirit in _spirits)
			{
				if (spirit != null)
				{
					spirit.setInUse(spirit.getType() == element);
					sendPacket(new ExElementalSpiritAttackTypePacket(this));
				}
			}
		}
		
		UserInfoPacket userInfo = new UserInfoPacket(this, false);
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
		
		sendPacket(new ExAutoPlaySettingSendPacket(options, active, pickUp, nextTargetMode, shortRange, potionPercent, respectfulHunting, petPotionPercent));
		
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
					shortcut.setAutoUse(true);
				}
			}
			else if (shortcut.getType() == ShortcutType.ACTION)
			{
				shortcut.setAutoUse(true);
			}
			else
			{
				Item item = getInventory().getItemByObjectId(shortcut.getId());
				if (item != null)
				{
					shortcut.setAutoUse(true);
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
				shortcut.setAutoUse(true);
				AutoUseTaskManager.getInstance().addAutoAction(this, shortcut.getId());
				continue;
			}
			
			Skill knownSkill = getKnownSkill(shortcut.getId());
			if (knownSkill != null)
			{
				shortcut.setAutoUse(true);
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
					sendPacket(new ExActivateAutoShortcutPacket(shortcut, true));
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
					sendPacket(new ExActivateAutoShortcutPacket(shortcut, false));
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
	
	public void startTimedHuntingZone(int zoneId, DateTime delay)
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
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_TIME_FOR_HUNTING_IN_THIS_ZONE_EXPIRES_IN_S1_MIN_PLEASE_ADD_MORE_TIME);
						sm.Params.addLong(time / 60000);
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
	
	public DateTime getTimedHuntingZoneInitialEntry(int zoneId)
	{
		return getVariables().getDateTime(PlayerVariables.HUNTING_ZONE_ENTRY + zoneId, DateTime.MinValue);
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
			// TODO: this needs to be optimized
			
			try 
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                
                int itemObjectId = it.getObjectId(); 
                
                var query = 
                    from ev in ctx.PetEvolves
                    from p in ctx.Pets
                    where ev.ItemObjectId == itemObjectId && p.ItemObjectId == ev.ItemObjectId
                    select new 
                    {
                        ev.Index,
                        EvolveLevel = ev.Level,
                        p.Name,
                        p.Level,
                        p.Exp
                    };
                
                foreach (var record in query)
                {
                    EvolveLevel evolveLevel = (EvolveLevel)record.EvolveLevel;
  				    _petEvolves.put(it.getObjectId(), new PetEvolveHolder(record.Index, evolveLevel, record.Name, record.Level, record.Exp));
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
			for (CommonSkill i = CommonSkill.STR_INCREASE_BONUS; i <= CommonSkill.MEN_INCREASE_BONUS; i++)
			{
				knownSkill = getKnownSkill((int)i);
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
				addSkill(CommonSkill.STR_INCREASE_BONUS.getSkill(1), false);
				update = true;
			}
			else if ((statValue >= 70) && (statValue < 90))
			{
				addSkill(CommonSkill.STR_INCREASE_BONUS.getSkill(2), false);
				update = true;
			}
			else if (statValue >= 90)
			{
				addSkill(CommonSkill.STR_INCREASE_BONUS.getSkill(3), false);
				update = true;
			}
			
			// INT bonus.
			statValue = getStat().getValue(Stat.STAT_INT);
			if ((statValue >= 60) && (statValue < 70))
			{
				addSkill(CommonSkill.INT_INCREASE_BONUS.getSkill(1), false);
				update = true;
			}
			else if ((statValue >= 70) && (statValue < 90))
			{
				addSkill(CommonSkill.INT_INCREASE_BONUS.getSkill(2), false);
				update = true;
			}
			else if (statValue >= 90)
			{
				addSkill(CommonSkill.INT_INCREASE_BONUS.getSkill(3), false);
				update = true;
			}
			
			// DEX bonus.
			statValue = getStat().getValue(Stat.STAT_DEX);
			if ((statValue >= 50) && (statValue < 60))
			{
				addSkill(CommonSkill.DEX_INCREASE_BONUS.getSkill(1), false);
				update = true;
			}
			else if ((statValue >= 60) && (statValue < 80))
			{
				addSkill(CommonSkill.DEX_INCREASE_BONUS.getSkill(2), false);
				update = true;
			}
			else if (statValue >= 80)
			{
				addSkill(CommonSkill.DEX_INCREASE_BONUS.getSkill(3), false);
				update = true;
			}
			
			// WIT bonus.
			statValue = getStat().getValue(Stat.STAT_WIT);
			if ((statValue >= 40) && (statValue < 50))
			{
				addSkill(CommonSkill.WIT_INCREASE_BONUS.getSkill(1), false);
				update = true;
			}
			else if ((statValue >= 50) && (statValue < 70))
			{
				addSkill(CommonSkill.WIT_INCREASE_BONUS.getSkill(2), false);
				update = true;
			}
			else if (statValue >= 70)
			{
				addSkill(CommonSkill.WIT_INCREASE_BONUS.getSkill(3), false);
				update = true;
			}
			
			// CON bonus.
			statValue = getStat().getValue(Stat.STAT_CON);
			if ((statValue >= 50) && (statValue < 65))
			{
				addSkill(CommonSkill.CON_INCREASE_BONUS.getSkill(1), false);
				update = true;
			}
			else if ((statValue >= 65) && (statValue < 90))
			{
				addSkill(CommonSkill.CON_INCREASE_BONUS.getSkill(2), false);
				update = true;
			}
			else if (statValue >= 90)
			{
				addSkill(CommonSkill.CON_INCREASE_BONUS.getSkill(3), false);
				update = true;
			}
			
			// MEN bonus.
			statValue = getStat().getValue(Stat.STAT_MEN);
			if ((statValue >= 45) && (statValue < 60))
			{
				addSkill(CommonSkill.MEN_INCREASE_BONUS.getSkill(1), false);
				update = true;
			}
			else if ((statValue >= 60) && (statValue < 85))
			{
				addSkill(CommonSkill.MEN_INCREASE_BONUS.getSkill(2), false);
				update = true;
			}
			else if (statValue >= 85)
			{
				addSkill(CommonSkill.MEN_INCREASE_BONUS.getSkill(3), false);
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
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            
            foreach (var data in _collections)
            {
	            int collectionId = data.getCollectionId();
                int index = data.getIndex();  
                var record = ctx.AccountCollections.SingleOrDefault(r => r.AccountId == _accountId && r.CollectionId == collectionId && r.Index == index);
                if (record is null)
                {
					record = new AccountCollection();
                    record.AccountId = _accountId;
                    record.CollectionId = (short)collectionId;
                    record.Index = (short)index;
                }
                
                record.ItemId = data.getItemId();
            }       
            
            ctx.SaveChanges();
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
		    using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.AccountCollectionFavorites.Where(r => r.AccountId == _accountId).ExecuteDelete();

            foreach (var data in _collectionFavorites)
            {
                ctx.AccountCollectionFavorites.Add(new AccountCollectionFavorite()
                {
                    AccountId = _accountId,
                    CollectionId = (short)data
                });
            }
            
            ctx.SaveChanges();
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
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.AccountCollections.Where(r => r.AccountId == _accountId).
                OrderBy(r => r.Index);


            foreach (var record in query)
            {
				int collectionId = record.CollectionId;
				if (CollectionData.getInstance().getCollection(collectionId) != null)
				{
					_collections.add(new PlayerCollectionData(collectionId, record.ItemId, record.Index));
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
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            _collectionFavorites.AddRange(ctx.AccountCollectionFavorites.Where(r => r.AccountId == _accountId)
	            .Select(r => (int)r.CollectionId));
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
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
	
			ctx.CharacterPurges.Where(r => r.CharacterId == characterId).ExecuteDelete();

            foreach (var kvp in getPurgePoints())
            {
                int category = kvp.Key;
                PurgePlayerHolder data = kvp.Value;
                
                ctx.CharacterPurges.Add(new CharacterPurge() 
                {
                    CharacterId = characterId,
                    Category = (short)category,
                    Points = data.getPoints(),
                    Keys = data.getKeys(),
                    RemainingKeys = data.getRemainingKeys()
                });
            }    
            
            ctx.SaveChanges();
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
            int characterId = getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.CharacterPurges.Where(r => r.CharacterId == characterId);
            foreach (var record in query)
            {
				_purgePoints.put(record.Category, new PurgePlayerHolder(record.Points, record.Keys, record.RemainingKeys));
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
			string variable = PlayerVariables.MISSION_LEVEL_PROGRESS + MissionLevel.getInstance().getCurrentSeason();
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
		
		sendPacket(new ExDualInventorySwapPacket(_dualInventorySlot));
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
		
		sendPacket(new ExDualInventorySwapPacket(slot));
		
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
	
	public void setSkillEnchantExp(int level, long exp)
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