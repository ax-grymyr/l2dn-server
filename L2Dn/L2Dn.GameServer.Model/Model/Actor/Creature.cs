using System.Runtime.CompilerServices;
using L2Dn.Events;
using L2Dn.Extensions;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Geo.PathFindings;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Status;
using L2Dn.GameServer.Model.Actor.Tasks.CreatureTasks;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Packets;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor;

/**
 * Mother class of all character objects of the world (PC, NPC...)<br>
 * Creature:<br>
 * <ul>
 * <li>Door</li>
 * <li>Playable</li>
 * <li>Npc</li>
 * <li>StaticObject</li>
 * <li>Trap</li>
 * <li>Vehicle</li>
 * </ul>
 * <b>Concept of CreatureTemplate:</b><br>
 * Each Creature owns generic and static properties (ex : all Keltir have the same number of HP...).<br>
 * All of those properties are stored in a different template for each type of Creature.<br>
 * Each template is loaded once in the server cache memory (reduce memory use).<br>
 * When a new instance of Creature is spawned, server just create a link between the instance and the template.<br>
 * This link is stored in {@link #_template}
 * @version $Revision: 1.53.2.45.2.34 $ $Date: 2005/04/11 10:06:08 $
 */
public abstract class Creature: WorldObject, ISkillsHolder, IEventContainerProvider
{
	public static readonly Logger LOGGER = LogManager.GetLogger(nameof(Creature));

	private Set<WeakReference<Creature>> _attackByList;

	private bool _isDead;
	private bool _isImmobilized;
	private bool _isOverloaded; // the char is carrying too much
	private bool _isPendingRevive;
	private bool _isRunning;
	protected bool _showSummonAnimation;
	protected bool _isTeleporting;
	private bool _isInvul;
	private bool _isUndying;
	private bool _isFlying;

	private bool _blockActions;
	private readonly Map<int, AtomicInteger> _blockActionsAllowedSkills = new();

	private CreatureStat _stat;
	private CreatureStatus _status;
	private CreatureTemplate _template; // The link on the CreatureTemplate object containing generic and static properties of this Creature type (ex : Max HP, Speed...)
	private string _title;

	public const double MAX_HP_BAR_PX = 352.0;

	private double _hpUpdateIncCheck;
	private double _hpUpdateDecCheck;
	private double _hpUpdateInterval;

	private int _reputation;

	/** Map containing all skills of this character. */
	private readonly Map<int, Skill> _skills = new();
	/** Map containing the skill reuse time stamps. */
	private readonly Map<long, TimeStamp> _reuseTimeStampsSkills = new();
	/** Map containing the item reuse time stamps. */
	private readonly Map<int, TimeStamp> _reuseTimeStampsItems = new();
	/** Map containing all the disabled skills. */
	private readonly Map<long, DateTime> _disabledSkills = new();
	private bool _allSkillsDisabled;

	private readonly byte[] _zones = new byte[(int)EnumUtil.GetMaxValue<ZoneId>() + 1];
	protected Location3D _lastZoneValidateLocation;

	private readonly object _attackLock = new();

	private Team _team = Team.NONE;

	protected long _exceptions;

	private bool _lethalable = true;

	private Map<int, OptionSkillHolder> _triggerSkills;

	private Map<int, IgnoreSkillHolder> _ignoreSkillEffects;
	/** Creatures effect list. */
	private readonly EffectList _effectList;
	/** The creature that summons this character. */
	private Creature _summoner;

	/** Map of summoned NPCs by this creature. */
	private Map<int, Npc> _summonedNpcs;

	private SkillChannelizer _channelizer;

	private SkillChannelized _channelized;

	private BuffFinishTask _buffFinishTask;

	private Transform? _transform;

	/** Movement data of this Creature */
	protected MoveData? _move;
	private bool _cursorKeyMovement;
	private bool _suspendedMovement;

	private ScheduledFuture _broadcastModifiedStatTask;
	private readonly Set<Stat> _broadcastModifiedStatChanges = new();

	/** This creature's target. */
	private WorldObject? _target;

	// set by the start of attack, in game ticks
	private DateTime _attackEndTime;
	private DateTime _disableRangedAttackEndTime;

	private CreatureAI? _ai;

	/** Future Skill Cast */
	protected Map<SkillCastingType, SkillCaster> _skillCasters = new();

	private readonly AtomicInteger _abnormalShieldBlocks = new AtomicInteger();

	private readonly Map<int, RelationCache> _knownRelations = new();

	private Set<Creature> _seenCreatures;
	private int _seenCreatureRange = Config.ALT_PARTY_RANGE;

	private readonly Map<StatusUpdateType, int> _statusUpdates = new();

	/** A map holding info about basic property mesmerizing system. */
	private Map<BasicProperty, BasicPropertyResist> _basicPropertyResists;

	/** A set containing the shot types currently charged. */
	private Set<ShotType> _chargedShots = new();

	/** A list containing the dropped items of this fake player. */
	private readonly List<Item> _fakePlayerDrops = new();

	private readonly EventContainer _eventContainer;
	private OnCreatureAttack? _onCreatureAttack;
	private OnCreatureAttacked? _onCreatureAttacked;
	private OnCreatureDamageDealt? _onCreatureDamageDealt;
	private OnCreatureDamageReceived? _onCreatureDamageReceived;
	private OnCreatureAttackAvoid? _onCreatureAttackAvoid;
	public OnCreatureSkillFinishCast? onCreatureSkillFinishCast;
	public OnCreatureSkillUse? onCreatureSkillUse;

	/**
	 * Creates a creature.
	 * @param template the creature template
	 */
	protected Creature(CreatureTemplate template)
		: this(IdManager.getInstance().getNextId(), template)
	{
	}

	/**
	 * Constructor of Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Each Creature owns generic and static properties (ex : all Keltir have the same number of HP...).<br>
	 * All of those properties are stored in a different template for each type of Creature. Each template is loaded once in the server cache memory (reduce memory use).<br>
	 * When a new instance of Creature is spawned, server just create a link between the instance and the template This link is stored in <b>_template</b><br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Set the _template of the Creature</li>
	 * <li>Set _overloaded to false (the character can take more items)</li>
	 * <li>If Creature is a Npc, copy skills from template to object</li>
	 * <li>If Creature is a Npc, link _calculators to NPC_STD_CALCULATOR</li>
	 * <li>If Creature is NOT a Npc, create an empty _skills slot</li>
	 * <li>If Creature is a Player or Summon, copy basic Calculator set to object</li>
	 * </ul>
	 * @param objectId Identifier of the object to initialized
	 * @param template The CreatureTemplate to apply to the object
	 */
	protected Creature(int objectId, CreatureTemplate template): base(objectId)
	{
		ArgumentNullException.ThrowIfNull(template, nameof(template));

		InstanceType = InstanceType.Creature;
		_effectList = new EffectList(this);
		_isRunning = isPlayer();
		_lastZoneValidateLocation = new Location3D(base.getX(), base.getY(), base.getZ());

		// Set its template to the new Creature
		_template = template;
		_eventContainer = new EventContainer($"Creature {objectId}", template.Events);
		initCharStat();
		initCharStatus();

		if (isNpc())
		{
			// Copy the skills of the Npc from its template to the Creature Instance
			// The skills list can be affected by spell effects so it's necessary to make a copy
			// to avoid that a spell affecting a Npc, affects others Npc of the same type too.
			foreach (Skill skill in template.getSkills().Values)
			{
				addSkill(skill);
			}
		}
		else if (isSummon())
		{
			// Copy the skills of the Summon from its template to the Creature Instance
			// The skills list can be affected by spell effects so it's necessary to make a copy
			// to avoid that a spell affecting a Summon, affects others Summon of the same type too.
			foreach (Skill skill in template.getSkills().Values)
			{
				addSkill(skill);
			}
		}

		setInvul(true);
	}

	public EventContainer Events => _eventContainer;

	public EffectList getEffectList()
	{
		return _effectList;
	}

	/**
	 * @return character inventory, default null, overridden in Playable types and in Npc
	 */
	public virtual Inventory getInventory()
	{
		return null;
	}

	public virtual bool destroyItemByItemId(string process, int itemId, long count, WorldObject reference, bool sendMessage)
	{
		// Default: NPCs consume virtual items for their skills
		// TODO: should be logged if even happens.. should be false
		return true;
	}

	public virtual bool destroyItem(string process, int objectId, long count, WorldObject reference, bool sendMessage)
	{
		// Default: NPCs consume virtual items for their skills
		// TODO: should be logged if even happens.. should be false
		return true;
	}

	/**
	 * Check if the character is in the given zone Id.
	 * @param zone the zone Id to check
	 * @return {code true} if the character is in that zone
	 */
	public override bool isInsideZone(ZoneId zone)
	{
		Instance instance = getInstanceWorld();
		switch (zone)
		{
			case ZoneId.PVP:
			{
				if (instance != null && instance.isPvP())
				{
					return true;
				}
				return _zones[(int)ZoneId.PVP] > 0 && _zones[(int)ZoneId.PEACE] == 0 && _zones[(int)ZoneId.NO_PVP] == 0;
			}
			case ZoneId.PEACE:
			{
				if (instance != null && instance.isPvP())
				{
					return false;
				}

				break;
			}
		}

		return _zones[(int)zone] > 0;
	}

	/**
	 * @param zone
	 * @param state
	 */
	public void setInsideZone(ZoneId zone, bool state)
	{
		lock (_zones)
		{
			if (state)
			{
				_zones[(int)zone]++;
			}
			else if (_zones[(int)zone] > 0)
			{
				_zones[(int)zone]--;
			}
		}
	}

	/**
	 * @return {@code true} if this creature is transformed including stance transformation {@code false} otherwise.
	 */
	public bool isTransformed()
	{
		return _transform != null;
	}

	/**
	 * @param filter any conditions to be checked for the transformation, {@code null} otherwise.
	 * @return {@code true} if this creature is transformed under the given filter conditions, {@code false} otherwise.
	 */
	public bool checkTransformed(Predicate<Transform> filter)
	{
		return _transform != null && filter(_transform);
	}

	/**
	 * Tries to transform this creature with the specified template id.
	 * @param id the id of the transformation template
	 * @param addSkills {@code true} if skills of this transformation template should be added, {@code false} otherwise.
	 * @return {@code true} if template is found and transformation is done, {@code false} otherwise.
	 */
	public bool transform(int id, bool addSkills)
	{
		Transform transform = TransformData.getInstance().getTransform(id);
		if (transform != null)
		{
			this.transform(transform, addSkills);
			return true;
		}
		return false;
	}

	public void transform(Transform transformation, bool addSkills)
	{
		if (!Config.ALLOW_MOUNTS_DURING_SIEGE && transformation.isRiding() && isInsideZone(ZoneId.SIEGE))
		{
			return;
		}

		_transform = transformation;
		transformation.onTransform(this, addSkills);
	}

	public void untransform()
	{
		_transform?.onUntransform(this);
		_transform = null;

		// Mobius: Tempfix for untransform not showing stats.
		// Resend UserInfo to player.
		if (isPlayer())
		{
			getStat().recalculateStats(true);
			getActingPlayer().updateUserInfo();
		}
	}

	public Transform? getTransformation()
	{
		return _transform;
	}

	/**
	 * This returns the transformation Id of the current transformation. For example, if a player is transformed as a Buffalo, and then picks up the Zariche, the transform Id returned will be that of the Zariche, and NOT the Buffalo.
	 * @return Transformation Id
	 */
	public int getTransformationId()
	{
		return _transform?.getId() ?? 0;
	}

	public int getTransformationDisplayId()
	{
		if (_transform != null && _transform.isStance())
			return _transform.getDisplayId();

		return 0;
	}

	public virtual float getCollisionRadius()
	{
		float defaultCollisionRadius = _template.getCollisionRadius();
		if (_transform is null)
			return defaultCollisionRadius;

		return _transform.getCollisionRadius(this, defaultCollisionRadius);
	}

	public virtual float getCollisionHeight()
	{
		float defaultCollisionHeight = _template.getCollisionHeight();
		if (_transform is null)
			return defaultCollisionHeight;

		return _transform.getCollisionHeight(this, defaultCollisionHeight);
	}

	/**
	 * This will return true if the player is GM,<br>
	 * but if the player is not GM it will return false.
	 * @return GM status
	 */
	public virtual bool isGM()
	{
		return false;
	}

	/**
	 * Overridden in Player.
	 * @return the access level.
	 */
	public virtual AccessLevel getAccessLevel()
	{
		return null;
	}

	protected void initCharStatusUpdateValues()
	{
		_hpUpdateIncCheck = _stat.getMaxHp();
		_hpUpdateInterval = _hpUpdateIncCheck / MAX_HP_BAR_PX;
		_hpUpdateDecCheck = _hpUpdateIncCheck - _hpUpdateInterval;
	}

	/**
	 * Remove the Creature from the world when the decay task is launched.<br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T REMOVE the object from _allObjects of World </b></font><br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T SEND Server=>Client packets to players</b></font>
	 */
	public virtual void onDecay()
	{
		if (isPlayer())
		{
			if (getActingPlayer().isInTimedHuntingZone())
			{
				getActingPlayer().stopTimedHuntingZoneTask();
				abortCast();
				stopMove(null);
				this.teleToLocation(TeleportWhereType.TOWN);
				setInstance(null);
			}
			else if (Config.DISCONNECT_AFTER_DEATH)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId
					.SIXTY_MIN_HAVE_PASSED_AFTER_THE_DEATH_OF_YOUR_CHARACTER_SO_YOU_WERE_DISCONNECTED_FROM_THE_GAME);

				Disconnection.of(getActingPlayer()).deleteMe().defaultSequence(ref sm);
			}
		}
		else
		{
			decayMe();
			ZoneRegion? region = ZoneManager.getInstance().getRegion(Location.Location2D);
			if (region != null)
			{
				region.removeFromZones(this);
			}

			// Removes itself from the summoned list.
			if (_summoner != null)
			{
				_summoner.removeSummonedNpc(ObjectId);
			}

			// Enable AI.
			_disabledAI = false;

			_onCreatureAttack = null;
			_onCreatureAttacked = null;
			_onCreatureDamageDealt = null;
			_onCreatureDamageReceived = null;
			_onCreatureAttackAvoid = null;
			onCreatureSkillFinishCast = null;
			onCreatureSkillUse = null;
		}
	}

	public override void onSpawn()
	{
		base.onSpawn();
		revalidateZone(true);

		// Custom boss announcements configuration.
		if (this is GrandBoss)
		{
			if (Config.GRANDBOSS_SPAWN_ANNOUNCEMENTS && (!isInInstance() || Config.GRANDBOSS_INSTANCE_ANNOUNCEMENTS) && !isMinion() && !isRaidMinion())
			{
				string name = NpcData.getInstance().getTemplate(getId()).getName();
				if (name != null)
				{
					Broadcast.toAllOnlinePlayers(name + " has spawned!");
					Broadcast.toAllOnlinePlayersOnScreen(name + " has spawned!");
				}
			}
		}
		else if (isRaid() && Config.RAIDBOSS_SPAWN_ANNOUNCEMENTS && (!isInInstance() || Config.RAIDBOSS_INSTANCE_ANNOUNCEMENTS) && !isMinion() && !isRaidMinion())
		{
			string name = NpcData.getInstance().getTemplate(getId()).getName();
			if (name != null)
			{
				Broadcast.toAllOnlinePlayers(name + " has spawned!");
				Broadcast.toAllOnlinePlayersOnScreen(name + " has spawned!");
			}
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public virtual void onTeleported()
	{
		if (!_isTeleporting)
		{
			return;
		}

		spawnMe(Location.Location3D);
		setTeleporting(false);

		if (_eventContainer.HasSubscribers<OnCreatureTeleported>())
		{
			_eventContainer.NotifyAsync(new OnCreatureTeleported(this));
		}
	}

	/**
	 * Add Creature instance that is attacking to the attacker list.
	 * @param creature The Creature that attacks this one
	 */
	public virtual void addAttackerToAttackByList(Creature creature)
	{
		// DS: moved to Attackable
	}

	/**
	 * Send a packet to the Creature AND to all Player in the _KnownPlayers of the Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Player in the detection area of the Creature are identified in <b>_knownPlayers</b>.<br>
	 * In order to inform other players of state modification on the Creature, server just need to go through _knownPlayers to send Server=>Client Packet
	 * @param mov
	 */
	public void broadcastPacket<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		broadcastPacket(packet, true);
	}

	public virtual void broadcastPacket<TPacket>(TPacket packet, bool includeSelf)
		where TPacket: struct, IOutgoingPacket
	{
		// TODO: Maybe add some nearby player count logic here.
		//packet.sendInBroadcast(true); // TODO: cache packet data

		World.getInstance().forEachVisibleObject<Player>(this, player =>
		{
			if (isVisibleFor(player))
			{
				player.sendPacket(packet);
			}
		});
	}

	/**
	 * Send a packet to the Creature AND to all Player in the radius (max knownlist radius) from the Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Player in the detection area of the Creature are identified in <b>_knownPlayers</b>.<br>
	 * In order to inform other players of state modification on the Creature, server just need to go through _knownPlayers to send Server=>Client Packet
	 * @param mov
	 * @param radiusInKnownlist
	 */
	public virtual void broadcastPacket<TPacket>(TPacket packet, int radiusInKnownlist)
		where TPacket: struct, IOutgoingPacket
	{
		World.getInstance().forEachVisibleObjectInRange<Player>(this, radiusInKnownlist, player =>
		{
			if (isVisibleFor(player))
			{
				player.sendPacket(packet);
			}
		});
	}

	public void broadcastMoveToLocation()
	{
		MoveData move = _move;
		if (move == null)
		{
			return;
		}

		// Broadcast MoveToLocation (once per 300ms).
		int gameTicks = GameTimeTaskManager.getInstance().getGameTicks();
		if (gameTicks - move.lastBroadcastTime < 3)
		{
			return;
		}

		move.lastBroadcastTime = gameTicks;

		if (isPlayable())
		{
			broadcastPacket(new MoveToLocationPacket(this));
		}
		else
		{
			WorldRegion? region = getWorldRegion();
			if (region != null && region.areNeighborsActive())
			{
				broadcastPacket(new MoveToLocationPacket(this));
			}
		}
	}

	public void broadcastSocialAction(int id)
	{
		if (isPlayable())
		{
			broadcastPacket(new SocialActionPacket(ObjectId, id));
		}
		else
		{
			WorldRegion? region = getWorldRegion();
			if (region != null && region.areNeighborsActive())
			{
				broadcastPacket(new SocialActionPacket(ObjectId, id));
			}
		}
	}

	/**
	 * @return true if hp update should be done, false if not.
	 */
	protected bool needHpUpdate()
	{
		double currentHp = _status.getCurrentHp();
		double maxHp = _stat.getMaxHp();
		if (currentHp <= 1.0 || maxHp < MAX_HP_BAR_PX)
		{
			return true;
		}

		if (currentHp <= _hpUpdateDecCheck || currentHp >= _hpUpdateIncCheck)
		{
			if (currentHp == maxHp)
			{
				_hpUpdateIncCheck = currentHp + 1;
				_hpUpdateDecCheck = currentHp - _hpUpdateInterval;
			}
			else
			{
				double doubleMulti = currentHp / _hpUpdateInterval;
				int intMulti = (int) doubleMulti;
				_hpUpdateDecCheck = _hpUpdateInterval * (doubleMulti < intMulti ? intMulti - 1 : intMulti);
				_hpUpdateIncCheck = _hpUpdateDecCheck + _hpUpdateInterval;
			}

			return true;
		}

		return false;
	}

	public void broadcastStatusUpdate()
	{
		broadcastStatusUpdate(null);
	}

	/**
	 * Send the Server=>Client packet StatusUpdate with current HP and MP to all other Player to inform.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Create the Server=>Client packet StatusUpdate with current HP and MP</li>
	 * <li>Send the Server=>Client packet StatusUpdate with current HP and MP to all Creature called _statusListener that must be informed of HP/MP updates of this Creature</li>
	 * </ul>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T SEND CP information</b></font>
	 * @param caster
	 */
	public virtual void broadcastStatusUpdate(Creature? caster)
	{
		StatusUpdatePacket su = new StatusUpdatePacket(this);
		if (caster != null)
		{
			su.addCaster(caster);
		}

		// HP
		su.addUpdate(StatusUpdateType.MAX_HP, _stat.getMaxHp());
		su.addUpdate(StatusUpdateType.CUR_HP, (int) _status.getCurrentHp());

		// MP
		computeStatusUpdate(su, StatusUpdateType.MAX_MP);
		computeStatusUpdate(su, StatusUpdateType.CUR_MP);
		broadcastPacket(su);
	}

	/**
	 * @param text
	 */
	public virtual void sendMessage(string text)
	{
		// default implementation
	}

	/**
	 * Teleport a Creature and its pet if necessary.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Stop the movement of the Creature</li>
	 * <li>Set the x,y,z position of the WorldObject and if necessary modify its _worldRegion</li>
	 * <li>Send a Server=>Client packet TeleportToLocationt to the Creature AND to all Player in its _KnownPlayers</li>
	 * <li>Modify the position of the pet if necessary</li>
	 * </ul>
	 * @param xValue
	 * @param yValue
	 * @param zValue
	 * @param headingValue
	 * @param instanceValue
	 */
	public virtual void teleToLocation(Location location, Instance? instance)
	{
		// Prevent teleporting for players that disconnected unexpectedly.
		if (isPlayer() && !getActingPlayer().isOnline())
		{
			return;
		}

		if (!_isFlying)
			location = location with { Z = GeoEngine.getInstance().getHeight(location.Location3D) };

		if (_eventContainer.HasSubscribers<OnCreatureTeleport>())
		{
			OnCreatureTeleport onCreatureTeleport = new(this, location, instance);
			if (_eventContainer.Notify(onCreatureTeleport))
			{
				if (onCreatureTeleport.Terminate)
				{
					return;
				}

				if (onCreatureTeleport.OverridenLocation != null)
					location = onCreatureTeleport.OverridenLocation.Value;

				if (onCreatureTeleport.OverridenInstance != null)
					instance = onCreatureTeleport.OverridenInstance;
			}
		}

		// Prepare creature for teleport.
		if (_isPendingRevive)
		{
			doRevive();
		}

		// Abort any client actions, casting and remove target.
		sendPacket(new ActionFailedPacket(SkillCastingType.NORMAL));
		sendPacket(new ActionFailedPacket(SkillCastingType.NORMAL_SECOND));
		if (isMoving())
		{
			stopMove(null);
		}
		abortCast();
		setTarget(null);

		setTeleporting(true);

		getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);

		// Remove the object from its old location.
		decayMe();

		// Adjust position a bit.
		location = location with { Z = location.Z + 5 };

		// Send teleport packet where needed.
		broadcastPacket(new TeleportToLocationPacket(ObjectId, location));

		// Change instance world.
		if (getInstanceWorld() != instance)
		{
			setInstance(instance);
		}

		// Set the x,y,z position of the WorldObject and if necessary modify its _worldRegion.
		setXYZ(location.Location3D);
		// Also adjust heading.
		if (location.Heading != 0)
		{
			setHeading(location.Heading);
		}

		// Send teleport finished packet to player.
		sendPacket(new ExTeleportToLocationActivatePacket(this));

		// Allow recall of the detached characters.
		if (!isPlayer() || (getActingPlayer().getClient() != null && getActingPlayer().getClient().IsDetached))
		{
			onTeleported();
		}

		revalidateZone(true);
	}

	/**
	 * Launch a physical attack against a target (Simple, Bow, Pole or Dual).<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Get the active weapon (always equipped in the right hand)</li>
	 * <li>If weapon is a bow, check for arrows, MP and bow re-use delay (if necessary, equip the Player with arrows in left hand)</li>
	 * <li>If weapon is a bow, consume MP and set the new period of bow non re-use</li>
	 * <li>Get the Attack Speed of the Creature (delay (in milliseconds) before next attack)</li>
	 * <li>Select the type of attack to start (Simple, Bow, Pole or Dual) and verify if SoulShot are charged then start calculation</li>
	 * <li>If the Server=>Client packet Attack contains at least 1 hit, send the Server=>Client packet Attack to the Creature AND to all Player in the _KnownPlayers of the Creature</li>
	 * <li>Notify AI with EVT_READY_TO_ACT</li>
	 * </ul>
	 * @param target The Creature targeted
	 */
	public virtual void doAutoAttack(Creature target)
	{
		lock (_attackLock)
		{
			if (target == null || (isAttackDisabled() && !isSummon()) || !target.isTargetable())
			{
				return;
			}

			if (!isAlikeDead())
			{
				if ((isNpc() && target.isAlikeDead()) || !isInSurroundingRegion(target) ||
				    (isPlayer() && target.isDead()))
				{
					getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
					sendPacket(ActionFailedPacket.STATIC_PACKET);
					return;
				}

				if (checkTransformed(transform => !transform.canAttack()))
				{
					sendPacket(ActionFailedPacket.STATIC_PACKET);
					return;
				}
			}

			if (getActingPlayer() != null)
			{
				if (getActingPlayer().inObserverMode())
				{
					sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_FUNCTION_IN_THE_SPECTATOR_MODE);
					sendPacket(ActionFailedPacket.STATIC_PACKET);
					return;
				}

				if (getActingPlayer().isSiegeFriend(target))
				{
					sendPacket(SystemMessageId
						.FORCE_ATTACK_IS_IMPOSSIBLE_AGAINST_A_TEMPORARY_ALLIED_MEMBER_DURING_A_SIEGE);
					sendPacket(ActionFailedPacket.STATIC_PACKET);
					return;
				}

				// Checking if target has moved to peace zone
				if (target.isInsidePeaceZone(getActingPlayer()))
				{
					getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
					sendPacket(ActionFailedPacket.STATIC_PACKET);
					return;
				}

				// Events.
				if (getActingPlayer().isOnEvent() && !getActingPlayer().isOnSoloEvent() && target.isPlayable() &&
				    getActingPlayer().getTeam() == target.getActingPlayer().getTeam())
				{
					sendPacket(ActionFailedPacket.STATIC_PACKET);
					return;
				}
			}
			else if (isInsidePeaceZone(this, target))
			{
				getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				return;
			}

			stopEffectsOnAction();

			// GeoData Los Check here (or dz > 1000)
			if (!GeoEngine.getInstance().canSeeTarget(this, target))
			{
				sendPacket(SystemMessageId.CANNOT_SEE_TARGET);
				getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				return;
			}

			// Get the active weapon item corresponding to the active weapon instance (always equipped in the right hand)
			Weapon weaponItem = getActiveWeaponItem();
			WeaponType weaponType = getAttackType();

			// BOW and CROSSBOW checks
			if (weaponItem != null)
			{
				if (!weaponItem.isAttackWeapon() && !isGM())
				{
					if (weaponItem.getItemType() == WeaponType.FISHINGROD)
					{
						sendPacket(SystemMessageId.YOU_CANNOT_ATTACK_WHILE_FISHING);
					}
					else
					{
						sendPacket(SystemMessageId.YOU_CANNOT_ATTACK_WITH_THIS_WEAPON);
					}

					sendPacket(ActionFailedPacket.STATIC_PACKET);
					return;
				}

				// Ranged weapon checks.
				if (weaponItem.getItemType().isRanged())
				{
					// Check if bow delay is still active.
					if (_disableRangedAttackEndTime > DateTime.UtcNow)
					{
						if (isPlayer())
						{
							ThreadPool.schedule(new NotifyAITask(this, CtrlEvent.EVT_READY_TO_ACT), 300);
							sendPacket(ActionFailedPacket.STATIC_PACKET);
						}

						return;
					}

					// Check for arrows and MP
					if (isPlayer())
					{
						// Check if there are arrows to use or else cancel the attack.
						if (!checkAndEquipAmmunition(weaponItem.getItemType().isPistols() ? EtcItemType.ELEMENTAL_ORB :
							    weaponItem.getItemType().isCrossbow() ? EtcItemType.BOLT : EtcItemType.ARROW))
						{
							// Cancel the action because the Player have no arrow
							getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
							sendPacket(ActionFailedPacket.STATIC_PACKET);
							if (weaponItem.getItemType().isPistols())
							{
								sendPacket(SystemMessageId.YOU_CANNOT_ATTACK_BECAUSE_YOU_DON_T_HAVE_AN_ELEMENTAL_ORB);
							}
							else
							{
								sendPacket(SystemMessageId.YOU_HAVE_RUN_OUT_OF_ARROWS);
							}

							return;
						}

						// Checking if target has moved to peace zone - only for player-bow attacks at the moment
						// Other melee is checked in movement code and for offensive spells a check is done every time
						if (target.isInsidePeaceZone(getActingPlayer()))
						{
							getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
							sendPacket(SystemMessageId.YOU_CANNOT_ATTACK_IN_A_PEACEFUL_ZONE);
							sendPacket(ActionFailedPacket.STATIC_PACKET);
							return;
						}

						// Check if player has enough MP to shoot.
						int mpConsume = weaponItem.getMpConsume();
						if (weaponItem.getReducedMpConsume() > 0 &&
						    Rnd.get(100) < weaponItem.getReducedMpConsumeChance())
						{
							mpConsume = weaponItem.getReducedMpConsume();
						}

						mpConsume = isAffected(EffectFlag.CHEAPSHOT) ? 0 : mpConsume;
						if (_status.getCurrentMp() < mpConsume)
						{
							// If Player doesn't have enough MP, stop the attack
							ThreadPool.schedule(new NotifyAITask(this, CtrlEvent.EVT_READY_TO_ACT), 1000);
							sendPacket(SystemMessageId.NOT_ENOUGH_MP);
							sendPacket(ActionFailedPacket.STATIC_PACKET);
							return;
						}

						// If Player have enough MP, the bow consumes it
						if (mpConsume > 0)
						{
							_status.reduceMp(mpConsume);
						}
					}
				}
			}

			// Mobius: Do not move when attack is launched.
			if (isMoving())
			{
				stopMove(Location);
			}

			WeaponType attackType = getAttackType();
			bool isTwoHanded = weaponItem != null && weaponItem.getBodyPart() == ItemTemplate.SLOT_LR_HAND;
			int timeAtk = Formulas.calculateTimeBetweenAttacks(_stat.getPAtkSpd());
			int timeToHit = Formulas.calculateTimeToHit(timeAtk, weaponType, isTwoHanded, false);
			DateTime currentTime = DateTime.UtcNow;
			_attackEndTime = currentTime.AddMilliseconds(timeAtk);
			// Precaution. It has happened in the past. Probably impossible to happen now, but will not risk it.
			if (_attackEndTime < currentTime)
			{
				_attackEndTime = currentTime.AddMilliseconds(int.MaxValue);
			}

			// Make sure that char is facing selected target
			// also works: setHeading(Util.convertDegreeToClientHeading(Util.calculateAngleFrom(this, target)));
			setHeading(new Location2D(getX(), getY()).HeadingTo(new Location2D(target.getX(), target.getY())));

			// Always try to charge soulshots.
			if (!isChargedShot(ShotType.SOULSHOTS) && !isChargedShot(ShotType.BLESSED_SOULSHOTS))
			{
				rechargeShots(true, false, false);
			}

			// Get the Attack Reuse Delay of the Weapon
			AttackPacket attack = generateAttackTargetData(target, weaponItem, attackType);
			bool crossbow = false;
			switch (attackType)
			{
				case WeaponType.CROSSBOW:
				case WeaponType.TWOHANDCROSSBOW:
				{
					crossbow = true;
					// fallthrough
					goto case WeaponType.BOW;
				}
				case WeaponType.BOW:
				{
					// Old method.
					TimeSpan reuse = Formulas.calculateReuseTime(this, weaponItem);
					// Try to do what is expected by having more attack speed.
					// int reuse = (int) (Formulas.calculateReuseTime(this, weaponItem) / (Math.Max(1, _stat.getAttackSpeedMultiplier() - 1)));

					// Consume ammunition.
					Inventory inventory = getInventory();
					if (inventory != null)
					{
						inventory.reduceAmmunitionCount(crossbow ? EtcItemType.BOLT : EtcItemType.ARROW);
					}

					// Check if the Creature is a Player
					if (isPlayer())
					{
						if (crossbow)
						{
							sendPacket(SystemMessageId.YOUR_CROSSBOW_IS_PREPARING_TO_FIRE);
						}

						sendPacket(new SetupGaugePacket(ObjectId, SetupGaugePacket.RED, reuse));
					}

					// Calculate and set the disable delay of the bow in function of the Attack Speed
					_disableRangedAttackEndTime = currentTime + reuse;
					// Precaution. It happened in the past for _attackEndTime. Will not risk it.
					if (_disableRangedAttackEndTime < currentTime)
					{
						_disableRangedAttackEndTime = currentTime.AddMilliseconds(int.MaxValue);
					}

					CreatureAttackTaskManager.getInstance()
						.onHitTimeNotDual(this, weaponItem, attack, timeToHit, timeAtk);
					break;
				}
				case WeaponType.PISTOLS:
				{
					TimeSpan reuse = Formulas.calculateReuseTime(this, weaponItem);
					_disableRangedAttackEndTime = currentTime + reuse;
					// Precaution. It happened in the past for _attackEndTime. Will not risk it.
					if (_disableRangedAttackEndTime < currentTime)
					{
						_disableRangedAttackEndTime = currentTime.AddMilliseconds(int.MaxValue);
					}

					CreatureAttackTaskManager.getInstance()
						.onHitTimeNotDual(this, weaponItem, attack, timeToHit, timeAtk);
					break;
				}
				case WeaponType.FIST:
				{
					if (!isPlayer())
					{
						CreatureAttackTaskManager.getInstance()
							.onHitTimeNotDual(this, weaponItem, attack, timeToHit, timeAtk);
						break;
					}

					goto case WeaponType.DUAL;
				}
				case WeaponType.DUAL:
				case WeaponType.DUALFIST:
				case WeaponType.DUALBLUNT:
				case WeaponType.DUALDAGGER:
				{
					int delayForSecondAttack =
						Formulas.calculateTimeToHit(timeAtk, weaponType, isTwoHanded, true) - timeToHit;
					CreatureAttackTaskManager.getInstance().onFirstHitTimeForDual(this, weaponItem, attack, timeToHit,
						timeAtk, delayForSecondAttack);
					break;
				}
				default:
				{
					CreatureAttackTaskManager.getInstance()
						.onHitTimeNotDual(this, weaponItem, attack, timeToHit, timeAtk);
					break;
				}
			}

			// If the Server=>Client packet Attack contains at least 1 hit, send the Server=>Client packet Attack
			// to the Creature AND to all Player in the _KnownPlayers of the Creature
			if (attack.hasHits())
			{
				broadcastPacket(attack);
			}

			// Flag the attacker if it's a Player outside a PvP area
			Player player = getActingPlayer();
			if (player != null && !player.isInsideZone(ZoneId.PVP) &&
			    player != target) // Prevent players from flagging in PvP Zones.
			{
				AttackStanceTaskManager.getInstance().addAttackStanceTask(player);
				player.updatePvPStatus(target);
			}

			if (isFakePlayer() && !Config.FAKE_PLAYER_AUTO_ATTACKABLE && (target.isPlayable() || target.isFakePlayer()))
			{
				Npc npc = (Npc)this;
				if (!npc.isScriptValue(1))
				{
					npc.setScriptValue(1); // in combat
					broadcastInfo(); // update flag status
					QuestManager.getInstance().getQuest("PvpFlaggingStopTask").notifyEvent("FLAG_CHECK", npc, null);
				}
			}
		}
	}

	private AttackPacket generateAttackTargetData(Creature target, Weapon weapon, WeaponType weaponType)
	{
		bool isDual = WeaponType.DUAL == weaponType || WeaponType.DUALBLUNT == weaponType || WeaponType.DUALDAGGER == weaponType || WeaponType.DUALFIST == weaponType;
		AttackPacket attack = new AttackPacket(this, target);
		bool shotConsumed = false;

		// Calculate the main target hit.
		Hit hit = generateHit(target, weapon, shotConsumed, isDual);
		attack.addHit(hit);
		shotConsumed = hit.isShotUsed();

		// Second hit for the dual attack.
		if (isDual)
		{
			hit = generateHit(target, weapon, shotConsumed, isDual);
			attack.addHit(hit);
			shotConsumed = hit.isShotUsed();
		}

		// H5 Changes: without Polearm Mastery (skill 216) max simultaneous attacks is 3 (1 by default + 2 in skill 3599).
		int attackCountMax = (int) _stat.getValue(Stat.ATTACK_COUNT_MAX, 1);
		if (attackCountMax > 1 && _stat.getValue(Stat.PHYSICAL_POLEARM_TARGET_SINGLE, 0) <= 0)
		{
			double headingAngle = HeadingUtil.ConvertHeadingToDegrees(getHeading());
			int maxRadius = _stat.getPhysicalAttackRadius();
			int physicalAttackAngle = _stat.getPhysicalAttackAngle();
			foreach (Creature obj in World.getInstance().getVisibleObjectsInRange<Creature>(this, maxRadius))
			{
				// Skip main target.
				if (obj == target)
				{
					continue;
				}

				// Skip dead or fake dead target.
				if (obj.isAlikeDead())
				{
					continue;
				}

				// Check if target is auto attackable.
				if (!obj.isAutoAttackable(this))
				{
					continue;
				}

				// Check if target is within attack angle.
				if (Math.Abs(this.AngleDegreesTo(obj) - headingAngle) > physicalAttackAngle)
				{
					continue;
				}

				// Launch a simple attack against the additional target.
				hit = generateHit(obj, weapon, shotConsumed, false);
				attack.addHit(hit);
				shotConsumed = hit.isShotUsed();
				if (--attackCountMax <= 0)
				{
					break;
				}
			}
		}

		return attack;
	}

	private Hit generateHit(Creature target, Weapon weapon, bool shotConsumedValue, bool halfDamage)
	{
		int damage = 0;
		byte shld = 0;
		bool crit = false;
		bool shotConsumed = shotConsumedValue;
		bool shotBlessed = false;
		bool miss = Formulas.calcHitMiss(this, target);
		if (!shotConsumed)
		{
			if (isChargedShot(ShotType.BLESSED_SOULSHOTS))
			{
				shotBlessed = true;
				shotConsumed = !miss && unchargeShot(ShotType.BLESSED_SOULSHOTS);
			}
			else
			{
				shotConsumed = !miss && unchargeShot(ShotType.SOULSHOTS);
			}
		}

		ItemGrade ssGrade = shotConsumed && weapon != null ? weapon.getItemGrade() : ItemGrade.NONE;

		// Check if hit isn't missed
		if (!miss)
		{
			shld = Formulas.calcShldUse(this, target);
			crit = Formulas.calcCrit(_stat.getCriticalHit(), this, target, null);
			damage = (int) Formulas.calcAutoAttackDamage(this, target, shld, crit, shotConsumed, shotBlessed);
			if (halfDamage)
			{
				damage /= 2;
			}
		}

		return new Hit(target, damage, miss, crit, shld, shotConsumed, ssGrade);
	}

	public virtual void doCast(Skill skill)
	{
		doCast(skill, null, false, false);
	}

	/**
	 * Manage the casting task (casting and interrupt time, re-use delay...) and display the casting bar and animation on client.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Verify the possibility of the the cast : skill is a spell, caster isn't muted...</li>
	 * <li>Get the list of all targets (ex : area effects) and define the Creature targeted (its stats will be used in calculation)</li>
	 * <li>Calculate the casting time (base + modifier of MAtkSpd), interrupt time and re-use delay</li>
	 * <li>Send a Server=>Client packet MagicSkillUser (to display casting animation), a packet SetupGauge (to display casting bar) and a system message</li>
	 * <li>Disable all skills during the casting time (create a task EnableAllSkills)</li>
	 * <li>Disable the skill during the re-use delay (create a task EnableSkill)</li>
	 * <li>Create a task MagicUseTask (that will call method onMagicUseTimer) to launch the Magic Skill at the end of the casting time</li>
	 * </ul>
	 * @param skill The Skill to use
	 * @param item the referenced item of this skill cast
	 * @param ctrlPressed if the player has pressed ctrl key during casting, aka force use.
	 * @param shiftPressed if the player has pressed shift key during casting, aka dont move.
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public virtual void doCast(Skill skill, Item? item, bool ctrlPressed, bool shiftPressed)
	{
		// Attackables cannot cast while moving.
		if (isAttackable() && isMoving())
		{
			return;
		}

		// Get proper casting type.
		SkillCastingType castingType = SkillCastingType.NORMAL;
		if (skill.canDoubleCast() && isAffected(EffectFlag.DOUBLE_CAST) && isCastingNow(castingType))
		{
			castingType = SkillCastingType.NORMAL_SECOND;
		}

		// Try casting the skill
		SkillCaster skillCaster = SkillCaster.castSkill(this, _target, skill, item, castingType, ctrlPressed, shiftPressed);
		if (skillCaster == null && isPlayer())
		{
			// Skill casting failed, notify player.
			sendPacket(new ActionFailedPacket(castingType));
			getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}

		// Players which are 9 levels above a Raid Boss and cast a skill nearby, are silenced with the Raid Curse skill.
		if (!Config.RAID_DISABLE_CURSE && isPlayer())
		{
			World.getInstance().forEachVisibleObjectInRange<Attackable>(this, Config.ALT_PARTY_RANGE, attackable =>
			{
				if (attackable.giveRaidCurse() && attackable.isInCombat() && getLevel() - attackable.getLevel() > 8)
				{
					CommonSkill curse = skill.isBad() ? CommonSkill.RAID_CURSE2 : CommonSkill.RAID_CURSE;
					curse.getSkill().applyEffects(attackable, this);
				}
			});
		}
	}

	/**
	 * Gets the item reuse time stamps map.
	 * @return the item reuse time stamps map
	 */
	public Map<int, TimeStamp> getItemReuseTimeStamps()
	{
		return _reuseTimeStampsItems;
	}

	/**
	 * Adds a item reuse time stamp.
	 * @param item the item
	 * @param reuse the reuse
	 */
	public void addTimeStampItem(Item item, TimeSpan reuse)
	{
		addTimeStampItem(item, reuse, null);
	}

	/**
	 * Adds a item reuse time stamp.<br>
	 * Used for restoring purposes.
	 * @param item the item
	 * @param reuse the reuse
	 * @param systime the system time
	 */
	public void addTimeStampItem(Item item, TimeSpan reuse, DateTime? systime)
	{
		_reuseTimeStampsItems.put(item.ObjectId, new TimeStamp(item, reuse, systime));
	}

	/**
	 * Gets the item remaining reuse time for a given item object ID.
	 * @param itemObjId the item object ID
	 * @return if the item has a reuse time stamp, the remaining time, otherwise -1
	 */
	public TimeSpan getItemRemainingReuseTime(int itemObjId)
	{
		TimeStamp reuseStamp = _reuseTimeStampsItems.get(itemObjId);
		return reuseStamp != null ? reuseStamp.getRemaining() : TimeSpan.Zero;
	}

	/**
	 * Gets the item remaining reuse time for a given shared reuse item group.
	 * @param group the shared reuse item group
	 * @return if the shared reuse item group has a reuse time stamp, the remaining time, otherwise -1
	 */
	public TimeSpan? getReuseDelayOnGroup(int group)
	{
		if (group > 0 && _reuseTimeStampsItems.Count != 0)
		{
			DateTime currentTime = DateTime.UtcNow;
			foreach (TimeStamp ts in _reuseTimeStampsItems.Values)
			{
				if (ts.getSharedReuseGroup() == group)
				{
					DateTime? stamp = ts.getStamp();
					if (currentTime < stamp)
					{
						return stamp.Value - currentTime;
					}
				}
			}
		}

		return null;
	}

	/**
	 * Gets the skill reuse time stamps map.
	 * @return the skill reuse time stamps map
	 */
	public Map<long, TimeStamp> getSkillReuseTimeStamps()
	{
		return _reuseTimeStampsSkills;
	}

	/**
	 * Adds the skill reuse time stamp.
	 * @param skill the skill
	 * @param reuse the delay
	 */
	public void addTimeStamp(Skill skill, TimeSpan reuse)
	{
		addTimeStamp(skill, reuse, null);
	}

	/**
	 * Adds the skill reuse time stamp.<br>
	 * Used for restoring purposes.
	 * @param skill the skill
	 * @param reuse the reuse
	 * @param systime the system time
	 */
	public void addTimeStamp(Skill skill, TimeSpan reuse, DateTime? systime)
	{
		_reuseTimeStampsSkills.put(skill.getReuseHashCode(), new TimeStamp(skill, reuse, systime));
	}

	/**
	 * Removes a skill reuse time stamp.
	 * @param skill the skill to remove
	 */
	public void removeTimeStamp(Skill skill)
	{
		_reuseTimeStampsSkills.remove(skill.getReuseHashCode());
	}

	/**
	 * Removes all skill reuse time stamps.
	 */
	public void resetTimeStamps()
	{
		_reuseTimeStampsSkills.Clear();
	}

	/**
	 * Gets the skill remaining reuse time for a given skill hash code.
	 * @param hashCode the skill hash code
	 * @return if the skill has a reuse time stamp, the remaining time, otherwise -1
	 */
	public TimeSpan getSkillRemainingReuseTime(long hashCode)
	{
		TimeStamp reuseStamp = _reuseTimeStampsSkills.get(hashCode);
		return reuseStamp != null ? reuseStamp.getRemaining() : TimeSpan.Zero;
	}

	/**
	 * Verifies if the skill is under reuse time.
	 * @param hashCode the skill hash code
	 * @return {@code true} if the skill is under reuse time, {@code false} otherwise
	 */
	public bool hasSkillReuse(long hashCode)
	{
		TimeStamp reuseStamp = _reuseTimeStampsSkills.get(hashCode);
		return reuseStamp != null && reuseStamp.hasNotPassed();
	}

	/**
	 * Gets the skill reuse time stamp.
	 * @param hashCode the skill hash code
	 * @return if the skill has a reuse time stamp, the skill reuse time stamp, otherwise {@code null}
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public TimeStamp getSkillReuseTimeStamp(long hashCode)
	{
		return _reuseTimeStampsSkills.get(hashCode);
	}

	/**
	 * Gets the disabled skills map.
	 * @return the disabled skills map
	 */
	public Map<long, DateTime> getDisabledSkills()
	{
		return _disabledSkills;
	}

	/**
	 * Enables a skill.
	 * @param skill the skill to enable
	 */
	public virtual void enableSkill(Skill skill)
	{
		if (skill == null)
		{
			return;
		}
		_disabledSkills.remove(skill.getReuseHashCode());
	}

	/**
	 * Disables a skill for a given time.<br>
	 * If delay is lesser or equal than zero, skill will be disabled "forever".
	 * @param skill the skill to disable
	 * @param delay delay in milliseconds
	 */
	public void disableSkill(Skill skill, TimeSpan delay)
	{
		if (skill == null)
		{
			return;
		}

		_disabledSkills.put(skill.getReuseHashCode(), delay > TimeSpan.Zero ? DateTime.UtcNow + delay : DateTime.MaxValue);
	}

	/**
	 * Removes all the disabled skills.
	 */
	public void resetDisabledSkills()
	{
		_disabledSkills.Clear();
	}

	/**
	 * Verifies if the skill is disabled.
	 * @param skill the skill
	 * @return {@code true} if the skill is disabled, {@code false} otherwise
	 */
	public bool isSkillDisabled(Skill skill)
	{
		if (skill == null)
		{
			return false;
		}

		if (_allSkillsDisabled || (!skill.canCastWhileDisabled() && isAllSkillsDisabled()))
		{
			return true;
		}

		if (isAffected(EffectFlag.CONDITIONAL_BLOCK_ACTIONS) && !isBlockedActionsAllowedSkill(skill))
		{
			return true;
		}

		long hashCode = skill.getReuseHashCode();
		if (hasSkillReuse(hashCode))
		{
			return true;
		}

		if (_disabledSkills.Count == 0)
		{
			return false;
		}

		if (!_disabledSkills.TryGetValue(hashCode, out DateTime stamp))
			return false;

		if (stamp < DateTime.UtcNow)
		{
			_disabledSkills.remove(hashCode);
			return false;
		}

		return true;
	}

	/**
	 * Disables all skills.
	 */
	public void disableAllSkills()
	{
		_allSkillsDisabled = true;
	}

	/**
	 * Enables all skills, except those under reuse time or previously disabled.
	 */
	public void enableAllSkills()
	{
		_allSkillsDisabled = false;
	}

	/**
	 * Kill the Creature.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Set target to null and cancel Attack or Cast</li>
	 * <li>Stop movement</li>
	 * <li>Stop HP/MP/CP Regeneration task</li>
	 * <li>Stop all active skills effects in progress on the Creature</li>
	 * <li>Send the Server=>Client packet StatusUpdate with current HP and MP to all other Player to inform</li>
	 * <li>Notify Creature AI</li>
	 * </ul>
	 * @param killer The Creature who killed it
	 * @return false if the creature hasn't been killed.
	 */
	public virtual bool doDie(Creature killer)
	{
		// killing is only possible one time
		lock (this)
		{
			if (_isDead)
			{
				return false;
			}

			// now reset currentHp to zero
			setCurrentHp(0);
			setDead(true);
		}

		if (_eventContainer.HasSubscribers<OnCreatureDeath>())
		{
			_eventContainer.Notify(new OnCreatureDeath(killer, this));
		}

		if (killer.Events.HasSubscribers<OnCreatureKilled>())
		{
			killer._eventContainer.Notify(new OnCreatureKilled(killer, this));
		}

		if (killer != null && killer.isPlayer())
		{
			Player player = killer.getActingPlayer();
			if (player.isAssassin() && player.isAffectedBySkill((int)CommonSkill.BRUTALITY))
			{
				player.setAssassinationPoints(player.getAssassinationPoints() + 10000);
			}
		}

		abortAttack();
		abortCast();

		// Calculate rewards for main damage dealer.
		Creature? mainDamageDealer = isMonster() ? ((Monster) this).getMainDamageDealer() : null;
		calculateRewards(mainDamageDealer ?? killer);

		// Set target to null and cancel Attack or Cast
		setTarget(null);

		// Stop movement
		stopMove(null);

		// Stop HP/MP/CP Regeneration task
		_status.stopHpMpRegeneration();

		if (isAttackable())
		{
			Spawn spawn = ((Npc) this).getSpawn();
			if (spawn != null && spawn.isRespawnEnabled())
			{
				stopAllEffects();
			}
			else
			{
				_effectList.stopAllEffectsWithoutExclusions(true, true);
			}

			// Clan help range aggro on kill.
			if (killer != null && killer.isPlayable() && !killer.getActingPlayer().isGM())
			{
				NpcTemplate template = ((Attackable) this).getTemplate();
				Set<int> clans = template.getClans();
				if (clans != null && !clans.isEmpty())
				{
					World.getInstance().forEachVisibleObjectInRange<Attackable>(this, template.getClanHelpRange(), called =>
					{
						// Don't call dead npcs, npcs without ai or npcs which are too far away.
						if (called.isDead() || !called.hasAI() || Math.Abs(killer.getZ() - called.getZ()) > 600)
						{
							return;
						}
						// Don't call npcs who are already doing some action (e.g. attacking, casting).
						if (called.getAI().getIntention() != CtrlIntention.AI_INTENTION_IDLE && called.getAI().getIntention() != CtrlIntention.AI_INTENTION_ACTIVE)
						{
							return;
						}
						// Don't call npcs who aren't in the same clan.
						if (!template.isClan(called.getTemplate().getClans()))
						{
							return;
						}

						// By default, when a faction member calls for help, attack the caller's attacker.
						called.getAI().notifyEvent(CtrlEvent.EVT_AGGRESSION, killer, 1);

						if (called.Events.HasSubscribers<OnAttackableFactionCall>())
						{
							called.Events.Notify(new OnAttackableFactionCall(called, (Attackable)this,
								killer.getActingPlayer(), killer.isSummon()));
						}
					});
				}
			}
		}
		else
		{
			stopAllEffectsExceptThoseThatLastThroughDeath();
		}

		// Send the Server=>Client packet StatusUpdate with current HP and MP to all other Player to inform
		broadcastStatusUpdate();

		// Notify Creature AI
		if (hasAI())
		{
			getAI().notifyEvent(CtrlEvent.EVT_DEAD);
		}

		ZoneManager.getInstance().getRegion(Location.Location2D)?.onDeath(this);

		getAttackByList().clear();

		if (isChannelized())
		{
			getSkillChannelized().abortChannelization();
		}

		// Custom boss announcements configuration.
		if (this is GrandBoss)
		{
			if (Config.GRANDBOSS_DEFEAT_ANNOUNCEMENTS && (!isInInstance() || Config.GRANDBOSS_INSTANCE_ANNOUNCEMENTS) && !isMinion() && !isRaidMinion())
			{
				string name = NpcData.getInstance().getTemplate(getId()).getName();
				if (name != null)
				{
					Broadcast.toAllOnlinePlayers(name + " has been defeated!");
					Broadcast.toAllOnlinePlayersOnScreen(name + " has been defeated!");
				}
			}
		}
		else if (isRaid() && Config.RAIDBOSS_DEFEAT_ANNOUNCEMENTS && (!isInInstance() || Config.RAIDBOSS_INSTANCE_ANNOUNCEMENTS) && !isMinion() && !isRaidMinion())
		{
			string name = NpcData.getInstance().getTemplate(getId()).getName();
			if (name != null)
			{
				Broadcast.toAllOnlinePlayers(name + " has been defeated!");
				Broadcast.toAllOnlinePlayersOnScreen(name + " has been defeated!");
			}
		}

		return true;
	}

	public override bool decayMe()
	{
		if (hasAI())
		{
			if (isAttackable())
			{
				getAttackByList().clear();
				((Attackable) this).clearAggroList();
				getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
			}
			getAI().stopAITask();
		}
		return base.decayMe();
	}

	public virtual bool deleteMe()
	{
		if (hasAI())
		{
			getAI().stopAITask();
		}

		// Removes itself from the summoned list.
		if (_summoner != null)
		{
			_summoner.removeSummonedNpc(ObjectId);
		}

		// Remove all active, passive and option effects, do not broadcast changes.
		_effectList.stopAllEffectsWithoutExclusions(false, false);

		// Forget all seen creatures.
		if (_seenCreatures != null)
		{
			CreatureSeeTaskManager.getInstance().remove(this);
			_seenCreatures.clear();
		}

		// Cancel the BuffFinishTask related to this creature.
		cancelBuffFinishTask();

		// Set world region to null.
		setWorldRegion(null);

		return true;
	}

	protected virtual void calculateRewards(Creature killer)
	{
	}

	/** Sets HP, MP and CP and revives the Creature. */
	public virtual void doRevive()
	{
		if (!_isDead)
		{
			return;
		}
		if (!_isTeleporting)
		{
			setIsPendingRevive(false);
			setDead(false);

			if (Config.RESPAWN_RESTORE_CP > 0 && _status.getCurrentCp() < _stat.getMaxCp() * Config.RESPAWN_RESTORE_CP)
			{
				_status.setCurrentCp(_stat.getMaxCp() * Config.RESPAWN_RESTORE_CP);
			}
			if (Config.RESPAWN_RESTORE_HP > 0 && _status.getCurrentHp() < _stat.getMaxHp() * Config.RESPAWN_RESTORE_HP)
			{
				_status.setCurrentHp(_stat.getMaxHp() * Config.RESPAWN_RESTORE_HP);
			}
			if (Config.RESPAWN_RESTORE_MP > 0 && _status.getCurrentMp() < _stat.getMaxMp() * Config.RESPAWN_RESTORE_MP)
			{
				_status.setCurrentMp(_stat.getMaxMp() * Config.RESPAWN_RESTORE_MP);
			}

			// Start broadcast status
			broadcastPacket(new RevivePacket(this));
			ZoneManager.getInstance().getRegion(Location.Location2D)?.onRevive(this);
		}
		else
		{
			setIsPendingRevive(true);
		}
	}

	/**
	 * Revives the Creature using skill.
	 * @param revivePower
	 */
	public virtual void doRevive(double revivePower)
	{
		doRevive();
	}

	/**
	 * Gets this creature's AI.
	 * @return the AI
	 */
	public CreatureAI getAI()
	{
		CreatureAI ai = _ai;
		if (ai == null)
		{
			lock (this)
			{
				ai = _ai;
				if (ai == null)
				{
					_ai = ai = initAI();
				}
			}
		}
		return ai;
	}

	/**
	 * Initialize this creature's AI.<br>
	 * OOP approach to be overridden in child classes.
	 * @return the new AI
	 */
	protected virtual CreatureAI initAI()
	{
		return new CreatureAI(this);
	}

	public virtual void detachAI()
	{
		if (isWalker())
		{
			return;
		}
		setAI(null);
	}

	public void setAI(CreatureAI? newAI)
	{
		CreatureAI? oldAI = _ai;
		if (oldAI != null && oldAI != newAI && oldAI is AttackableAI)
		{
			oldAI.stopAITask();
		}

        _ai = newAI;
	}

	/**
	 * Verifies if this creature has an AI,
	 * @return {@code true} if this creature has an AI, {@code false} otherwise
	 */
	public bool hasAI()
	{
		return _ai != null;
	}

	/**
	 * @return True if the Creature is RaidBoss or his minion.
	 */
	public virtual bool isRaid()
	{
		return false;
	}

	/**
	 * @return True if the Creature is minion.
	 */
	public virtual bool isMinion()
	{
		return false;
	}

	/**
	 * @return True if the Creature is minion of RaidBoss.
	 */
	public virtual bool isRaidMinion()
	{
		return false;
	}

	/**
	 * @return a list of Creature that attacked.
	 */
	public Set<WeakReference<Creature>> getAttackByList()
	{
		if (_attackByList == null)
		{
			lock (this)
			{
				if (_attackByList == null)
				{
					_attackByList = new();
				}
			}
		}
		return _attackByList;
	}

	public bool isControlBlocked()
	{
		return isAffected(EffectFlag.BLOCK_CONTROL);
	}

	/**
	 * @return True if the Creature can't use its skills (ex : stun, sleep...).
	 */
	public bool isAllSkillsDisabled()
	{
		return _allSkillsDisabled || hasBlockActions();
	}

	/**
	 * @return True if the Creature can't attack (attackEndTime, attackMute, fake death, stun, sleep, paralyze).
	 */
	public bool isAttackDisabled()
	{
		return isAttackingNow() || isDisabled();
	}

	/**
	 * @return True if the Creature is disabled (attackMute, fake death, stun, sleep, paralyze).
	 */
	public bool isDisabled()
	{
		return _disabledAI || isAlikeDead() || isPhysicalAttackMuted() || hasBlockActions();
	}

	public bool isConfused()
	{
		return isAffected(EffectFlag.CONFUSED);
	}

	/**
	 * @return True if the Creature is dead or use fake death.
	 */
	public virtual bool isAlikeDead()
	{
		return _isDead;
	}

	/**
	 * @return True if the Creature is dead.
	 */
	public bool isDead()
	{
		return _isDead;
	}

	public void setDead(bool value)
	{
		_isDead = value;
	}

	public bool isImmobilized()
	{
		return _isImmobilized;
	}

	public virtual void setImmobilized(bool value)
	{
		_isImmobilized = value;
	}

	public bool isMuted()
	{
		return isAffected(EffectFlag.MUTED);
	}

	public bool isPhysicalMuted()
	{
		return isAffected(EffectFlag.PSYCHICAL_MUTED);
	}

	public bool isPhysicalAttackMuted()
	{
		return isAffected(EffectFlag.PSYCHICAL_ATTACK_MUTED);
	}

	/**
	 * @return True if the Creature can't move (stun, root, sleep, overload, paralyzed).
	 */
	public virtual bool isMovementDisabled()
	{
		// check for isTeleporting to prevent teleport cheating (if appear packet not received)
		return hasBlockActions() || isRooted() || _isOverloaded || _isImmobilized || isAlikeDead() || _isTeleporting;
	}

	public bool isOverloaded()
	{
		return _isOverloaded;
	}

	/**
	 * Set the overloaded status of the Creature is overloaded (if True, the Player can't take more item).
	 * @param value
	 */
	public void setOverloaded(bool value)
	{
		_isOverloaded = value;
	}

	public bool isPendingRevive()
	{
		return _isDead && _isPendingRevive;
	}

	public void setIsPendingRevive(bool value)
	{
		_isPendingRevive = value;
	}

	public bool isDisarmed()
	{
		return isAffected(EffectFlag.DISARMED);
	}

	/**
	 * @return the summon
	 */
	public virtual Summon getPet()
	{
		return null;
	}

	/**
	 * @return the summon
	 */
	public virtual Map<int, Summon> getServitors()
	{
		return new();
	}

	public virtual Summon getServitor(int objectId)
	{
		return null;
	}

	/**
	 * @return {@code true} if the character has a summon, {@code false} otherwise
	 */
	public bool hasSummon()
	{
		return getPet() != null || getServitors().Count != 0;
	}

	/**
	 * @return {@code true} if the character has a pet, {@code false} otherwise
	 */
	public bool hasPet()
	{
		return getPet() != null;
	}

	public bool hasServitor(int objectId)
	{
		return getServitors().ContainsKey(objectId);
	}

	/**
	 * @return {@code true} if the character has a servitor, {@code false} otherwise
	 */
	public bool hasServitors()
	{
		return getServitors().Count != 0;
	}

	public void removeServitor(int objectId)
	{
		getServitors().remove(objectId);
	}

	public bool isRooted()
	{
		return isAffected(EffectFlag.ROOTED);
	}

	/**
	 * @return True if the Creature is running.
	 */
	public bool isRunning()
	{
		return _isRunning;
	}

	private void setRunning(bool value)
	{
		if (_isRunning == value)
		{
			return;
		}

		_isRunning = value;
		if (_stat.getRunSpeed() != 0)
		{
			broadcastPacket(new ChangeMoveTypePacket(this));
		}
		if (isPlayer())
		{
			getActingPlayer().broadcastUserInfo();
		}
		else if (isSummon())
		{
			broadcastStatusUpdate();
		}
		else if (isNpc())
		{
			World.getInstance().forEachVisibleObject<Player>(this, player =>
			{
				if (!isVisibleFor(player))
				{
					return;
				}

				if (isFakePlayer())
				{
					player.sendPacket(new FakePlayerInfoPacket((Npc) this));
				}
				else if (_stat.getRunSpeed() == 0)
				{
					player.sendPacket(new ServerObjectInfoPacket((Npc) this, player));
				}
				else
				{
					player.sendPacket(new NpcInfoPacket((Npc) this));
				}
			});
		}
	}

	/** Set the Creature movement type to run and send Server=>Client packet ChangeMoveType to all others Player. */
	public void setRunning()
	{
		setRunning(true);
	}

	public bool hasBlockActions()
	{
		return _blockActions || isAffected(EffectFlag.BLOCK_ACTIONS) || isAffected(EffectFlag.CONDITIONAL_BLOCK_ACTIONS);
	}

	public void setBlockActions(bool blockActions)
	{
		_blockActions = blockActions;
	}

	public bool isBetrayed()
	{
		return isAffected(EffectFlag.BETRAYED);
	}

	public bool isTeleporting()
	{
		return _isTeleporting;
	}

	public virtual void setTeleporting(bool value)
	{
		_isTeleporting = value;
	}

	public virtual void setInvul(bool value)
	{
		_isInvul = value;
	}

	public override bool isInvul()
	{
		return _isInvul || _isTeleporting;
	}

	public void setUndying(bool undying)
	{
		_isUndying = undying;
	}

	public virtual bool isUndying()
	{
		return _isUndying || isInvul() || isAffected(EffectFlag.IGNORE_DEATH) || isInsideZone(ZoneId.UNDYING);
	}

	public bool isHpBlocked()
	{
		return isInvul() || isAffected(EffectFlag.HP_BLOCK);
	}

	public bool isMpBlocked()
	{
		return isInvul() || isAffected(EffectFlag.MP_BLOCK);
	}

	public bool isBuffBlocked()
	{
		return isAffected(EffectFlag.BUFF_BLOCK);
	}

	public bool isDebuffBlocked()
	{
		return isInvul() || isAffected(EffectFlag.DEBUFF_BLOCK);
	}

	public virtual bool isUndead()
	{
		return false;
	}

	public bool isResurrectionBlocked()
	{
		return isAffected(EffectFlag.BLOCK_RESURRECTION);
	}

	public bool isFlying()
	{
		return _isFlying;
	}

	public void setFlying(bool mode)
	{
		_isFlying = mode;
	}

	public virtual CreatureStat getStat()
	{
		return _stat;
	}

	/**
	 * Initializes the CharStat class of the WorldObject, is overwritten in classes that require a different CharStat Type.<br>
	 * Removes the need for instanceof checks.
	 */
	public virtual void initCharStat()
	{
		_stat = new CreatureStat(this);
	}

	public void setStat(CreatureStat value)
	{
		_stat = value;
	}

	public virtual CreatureStatus getStatus()
	{
		return _status;
	}

	/**
	 * Initializes the CharStatus class of the WorldObject, is overwritten in classes that require a different CharStatus Type.<br>
	 * Removes the need for instanceof checks.
	 */
	public virtual void initCharStatus()
	{
		_status = new CreatureStatus(this);
	}

	public void setStatus(CreatureStatus value)
	{
		_status = value;
	}

	public virtual CreatureTemplate getTemplate()
	{
		return _template;
	}

	/**
	 * Set the template of the Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Each Creature owns generic and static properties (ex : all Keltir have the same number of HP...).<br>
	 * All of those properties are stored in a different template for each type of Creature.<br>
	 * Each template is loaded once in the server cache memory (reduce memory use).<br>
	 * When a new instance of Creature is spawned, server just create a link between the instance and the template This link is stored in <b>_template</b>.
	 * @param template
	 */
	protected void setTemplate(CreatureTemplate template)
	{
		_template = template;
	}

	/**
	 * @return the Title of the Creature.
	 */
	public string getTitle()
	{
		// Custom level titles
		if (isMonster() && (Config.SHOW_NPC_LEVEL || Config.SHOW_NPC_AGGRESSION))
		{
			string t1 = "";
			if (Config.SHOW_NPC_LEVEL)
			{
				t1 += "Lv " + getLevel();
			}
			string t2 = "";
			if (Config.SHOW_NPC_AGGRESSION)
			{
				if (!string.IsNullOrEmpty(t1))
				{
					t2 += " ";
				}
				Monster monster = (Monster) this;
				if (monster.isAggressive())
				{
					t2 += "[A]"; // Aggressive.
				}
				if (monster.getTemplate().getClans() != null && monster.getTemplate().getClanHelpRange() > 0)
				{
					t2 += "[G]"; // Group.
				}
			}
			t1 += t2;
			if (!string.IsNullOrEmpty(_title))
			{
				t1 += " " + _title;
			}
			return isChampion() ? Config.CHAMP_TITLE + " " + t1 : t1;
		}
		// Champion titles
		if (isChampion())
		{
			return Config.CHAMP_TITLE;
		}
		// Set trap title
		if (isTrap() && ((Trap) this).getOwner() != null)
		{
			_title = ((Trap) this).getOwner().getName();
		}
		return _title != null ? _title : "";
	}

	/**
	 * Set the Title of the Creature.
	 * @param value
	 */
	public void setTitle(string value)
	{
		if (value == null)
		{
			_title = "";
		}
		else
		{
			_title = isPlayer() && value.Length > 21 ? value.Substring(0, 20) : value;
		}
	}

	/**
	 * Set the Creature movement type to walk and send Server=>Client packet ChangeMoveType to all others Player.
	 */
	public void setWalking()
	{
		setRunning(false);
	}

	/**
	 * Active the abnormal effect Fake Death flag, notify the Creature AI and send Server=>Client UserInfo/CharInfo packet.
	 */
	public void startFakeDeath()
	{
		if (!isPlayer())
		{
			return;
		}

		// Aborts any attacks/casts if fake dead
		abortAttack();
		abortCast();
		stopMove(null);
		getAI().notifyEvent(CtrlEvent.EVT_FAKE_DEATH);
		broadcastPacket(new ChangeWaitTypePacket(this, ChangeWaitTypePacket.WT_START_FAKEDEATH));

		// Remove target from those that have the untargetable creature on target.
		if (Config.FAKE_DEATH_UNTARGET)
		{
			World.getInstance().forEachVisibleObject<Creature>(this, c =>
			{
				if (c.getTarget() == this)
				{
					c.setTarget(null);
				}
			});
		}
	}

	public void startParalyze()
	{
		// Aborts any attacks/casts if paralyzed
		abortAttack();
		abortCast();
		stopMove(null);
		getAI().notifyEvent(CtrlEvent.EVT_ACTION_BLOCKED);
	}

	/**
	 * Stop all active skills effects in progress on the Creature.
	 */
	public virtual void stopAllEffects()
	{
		_effectList.stopAllEffects(true);
	}

	/**
	 * Stops all effects, except those that last through death.
	 */
	public virtual void stopAllEffectsExceptThoseThatLastThroughDeath()
	{
		_effectList.stopAllEffectsExceptThoseThatLastThroughDeath();
	}

	/**
	 * Stop and remove the effects corresponding to the skill ID.
	 * @param type determines the system message that will be sent.
	 * @param skillId the skill Id
	 */
	public virtual void stopSkillEffects(SkillFinishType type, int skillId)
	{
		_effectList.stopSkillEffects(type, skillId);
	}

	public void stopSkillEffects(Skill skill)
	{
		_effectList.stopSkillEffects(SkillFinishType.REMOVED, skill.getId());
	}

	public void stopEffects(EffectFlag effectFlag)
	{
		_effectList.stopEffects(effectFlag);
	}

	/**
	 * Exits all buffs effects of the skills with "removedOnAnyAction" set.<br>
	 * Called on any action except movement (attack, cast).
	 */
	public void stopEffectsOnAction()
	{
		_effectList.stopEffectsOnAction();
	}

	/**
	 * Exits all buffs effects of the skills with "removedOnDamage" set.<br>
	 * Called on decreasing HP and mana burn.
	 */
	public void stopEffectsOnDamage()
	{
		_effectList.stopEffectsOnDamage();
	}

	/**
	 * Stop a specified/all Fake Death abnormal Effect.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Delete a specified/all (if effect=null) Fake Death abnormal Effect from Creature and update client magic icon</li>
	 * <li>Set the abnormal effect flag _fake_death to False</li>
	 * <li>Notify the Creature AI</li>
	 * </ul>
	 * @param removeEffects
	 */
	public void stopFakeDeath(bool removeEffects)
	{
		if (removeEffects)
		{
			stopEffects(EffectFlag.FAKE_DEATH);
		}

		// if this is a player instance, start the grace period for this character (grace from mobs only)!
		if (isPlayer())
		{
			getActingPlayer().setRecentFakeDeath(true);
		}

		broadcastPacket(new ChangeWaitTypePacket(this, ChangeWaitTypePacket.WT_STOP_FAKEDEATH));
		// TODO: Temp hack: players see FD on ppl that are moving: Teleport to someone who uses FD - if he gets up he will fall down again for that client -
		// even tho he is actually standing... Probably bad info in CharInfo packet?
		broadcastPacket(new RevivePacket(this));
	}

	/**
	 * Stop all block actions (stun) effects.
	 * @param removeEffects {@code true} removes all block actions effects, {@code false} only notifies AI to think.
	 */
	public void stopStunning(bool removeEffects)
	{
		if (removeEffects)
		{
			_effectList.stopEffects(AbnormalType.STUN);
		}

		if (!isPlayer())
		{
			getAI().notifyEvent(CtrlEvent.EVT_THINK);
		}
	}

	/**
	 * Stop Effect: Transformation.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Remove Transformation Effect</li>
	 * <li>Notify the Creature AI</li>
	 * <li>Send Server=>Client UserInfo/CharInfo packet</li>
	 * </ul>
	 * @param removeEffects
	 */
	public void stopTransformation(bool removeEffects)
	{
		if (removeEffects && !_effectList.stopEffects(AbnormalType.TRANSFORM))
		{
			_effectList.stopEffects(AbnormalType.CHANGEBODY);
		}

		if (_transform != null)
		{
			untransform();
		}

		if (!isPlayer())
		{
			getAI().notifyEvent(CtrlEvent.EVT_THINK);
		}
		updateAbnormalVisualEffects();
	}

	/**
	 * Updates the visual abnormal state of this character.
	 */
	public virtual void updateAbnormalVisualEffects()
	{
		// overridden
	}

	/**
	 * Update active skills in progress (In Use and Not In Use because stacked) icons on client.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * All active skills effects in progress (In Use and Not In Use because stacked) are represented by an icon on the client.<br>
	 * <font color=#FF0000><b><u>Caution</u>: This method ONLY UPDATE the client of the player and not clients of all players in the party.</b></font>
	 */
	public void updateEffectIcons()
	{
		updateEffectIcons(false);
	}

	/**
	 * Updates Effect Icons for this character(player/summon) and his party if any.
	 * @param partyOnly
	 */
	public virtual void updateEffectIcons(bool partyOnly)
	{
		// overridden
	}

	public bool isAffectedBySkill(SkillHolder skill)
	{
		return isAffectedBySkill(skill.getSkillId());
	}

	public bool isAffectedBySkill(int skillId)
	{
		return _effectList.isAffectedBySkill(skillId);
	}

	public int getAffectedSkillLevel(int skillId)
	{
		BuffInfo info = _effectList.getBuffInfoBySkillId(skillId);
		return info == null ? 0 : info.getSkill().getLevel();
	}

	/// <summary>
	/// This class group all movement data.
	/// </summary>
	protected sealed class MoveData
	{
		// When we retrieve x/y/z we use GameTimeControl.getGameTicks()
		// If we are moving, but move timestamp==gameticks, we don't need to recalculate position.
		public int moveStartTime;
		public int moveTimestamp; // Last movement update.
		public int xDestination;
		public int yDestination;
		public int zDestination;
		public double xAccurate; // Otherwise there would be rounding errors.
		public double yAccurate;
		public double zAccurate;
		public int heading;

		public bool disregardingGeodata;
		public int onGeodataPathIndex;
		public List<AbstractNodeLoc> geoPath;
		public int geoPathAccurateTx;
		public int geoPathAccurateTy;
		public int geoPathGtx;
		public int geoPathGty;

		public int lastBroadcastTime;
	}

	public void broadcastModifiedStats(Set<Stat> changed)
	{
		if (!isSpawned())
		{
			return;
		}

		if (changed == null || changed.isEmpty())
		{
			return;
		}

		// Don't broadcast modified stats on login.
		if (isPlayer() && !getActingPlayer().isOnline())
		{
			return;
		}

		lock (_broadcastModifiedStatChanges)
		{
			_broadcastModifiedStatChanges.addAll(changed);
		}
		if (_broadcastModifiedStatTask == null)
		{
			_broadcastModifiedStatTask = ThreadPool.schedule(() =>
			{
				Set<Stat> currentChanges;
				lock (_broadcastModifiedStatChanges)
				{
					if (_broadcastModifiedStatChanges.isEmpty())
					{
						return;
					}

					currentChanges = new();
					currentChanges.addAll(_broadcastModifiedStatChanges);

					_broadcastModifiedStatChanges.clear();
				}

				// If this creature was previously moving, but now due to stat change can no longer move, broadcast StopMove packet.
				if (isMoving() && getMoveSpeed() <= 0)
				{
					stopMove(null);
				}

				if (isSummon())
				{
					Summon summon = (Summon) this;
					if (summon.getOwner() != null)
					{
						summon.updateAndBroadcastStatus(1);
					}
				}
				else if (isPlayer())
				{
					UserInfoPacket info = new UserInfoPacket(getActingPlayer(), false);
					info.addComponentType(UserInfoType.SLOTS);
					info.addComponentType(UserInfoType.ENCHANTLEVEL);

					bool updateWeight = false;
					foreach (Stat stat in currentChanges)
					{
						switch (stat)
						{
							case Stat.MOVE_SPEED:
							case Stat.RUN_SPEED:
							case Stat.WALK_SPEED:
							case Stat.SWIM_RUN_SPEED:
							case Stat.SWIM_WALK_SPEED:
							case Stat.FLY_RUN_SPEED:
							case Stat.FLY_WALK_SPEED:
							{
								info.addComponentType(UserInfoType.MULTIPLIER);
								break;
							}
							case Stat.PHYSICAL_ATTACK_SPEED:
							{
								info.addComponentType(UserInfoType.MULTIPLIER);
								info.addComponentType(UserInfoType.STATS);
								break;
							}
							case Stat.PHYSICAL_ATTACK:
							case Stat.PHYSICAL_DEFENCE:
							case Stat.EVASION_RATE:
							case Stat.ACCURACY_COMBAT:
							case Stat.CRITICAL_RATE:
							case Stat.MAGIC_CRITICAL_RATE:
							case Stat.MAGIC_EVASION_RATE:
							case Stat.ACCURACY_MAGIC:
							case Stat.MAGIC_ATTACK:
							case Stat.MAGIC_ATTACK_SPEED:
							case Stat.MAGICAL_DEFENCE:
							{
								info.addComponentType(UserInfoType.STATS);
								break;
							}
							case Stat.MAX_CP:
							{
								info.addComponentType(UserInfoType.MAX_HPCPMP);
								break;
							}
							case Stat.MAX_HP:
							{
								info.addComponentType(UserInfoType.MAX_HPCPMP);
								break;
							}
							case Stat.MAX_MP:
							{
								info.addComponentType(UserInfoType.MAX_HPCPMP);
								break;
							}
							case Stat.STAT_STR:
							case Stat.STAT_CON:
							case Stat.STAT_DEX:
							case Stat.STAT_INT:
							case Stat.STAT_WIT:
							case Stat.STAT_MEN:
							{
								info.addComponentType(UserInfoType.BASE_STATS);
								updateWeight = true;
								break;
							}
							case Stat.FIRE_RES:
							case Stat.WATER_RES:
							case Stat.WIND_RES:
							case Stat.EARTH_RES:
							case Stat.HOLY_RES:
							case Stat.DARK_RES:
							{
								info.addComponentType(UserInfoType.ELEMENTALS);
								break;
							}
							case Stat.FIRE_POWER:
							case Stat.WATER_POWER:
							case Stat.WIND_POWER:
							case Stat.EARTH_POWER:
							case Stat.HOLY_POWER:
							case Stat.DARK_POWER:
							{
								info.addComponentType(UserInfoType.ATK_ELEMENTAL);
								break;
							}
							case Stat.WEIGHT_LIMIT:
							case Stat.WEIGHT_PENALTY:
							{
								updateWeight = true;
								break;
							}
							case Stat.ELEMENTAL_SPIRIT_EARTH_ATTACK:
							case Stat.ELEMENTAL_SPIRIT_EARTH_DEFENSE:
							case Stat.ELEMENTAL_SPIRIT_FIRE_ATTACK:
							case Stat.ELEMENTAL_SPIRIT_FIRE_DEFENSE:
							case Stat.ELEMENTAL_SPIRIT_WATER_ATTACK:
							case Stat.ELEMENTAL_SPIRIT_WATER_DEFENSE:
							case Stat.ELEMENTAL_SPIRIT_WIND_ATTACK:
							case Stat.ELEMENTAL_SPIRIT_WIND_DEFENSE:
							{
								info.addComponentType(UserInfoType.ATT_SPIRITS);
								break;
							}
						}
					}

					Player player = getActingPlayer();
					if (updateWeight)
					{
						player.refreshOverloaded(true);
					}

					sendPacket(info);

					player.broadcastCharInfo();

					if (hasServitors() && hasAbnormalType(AbnormalType.ABILITY_CHANGE))
					{
						getServitors().Values.ForEach(x => x.broadcastStatusUpdate());
					}
				}
				else if (isNpc())
				{
					World.getInstance().forEachVisibleObject<Player>(this, player =>
					{
						if (!isVisibleFor(player))
						{
							return;
						}

						if (isFakePlayer())
						{
							player.sendPacket(new FakePlayerInfoPacket((Npc) this));
						}
						else if (getRunSpeed() == 0)
						{
							player.sendPacket(new ServerObjectInfoPacket((Npc) this, player));
						}
						else
						{
							player.sendPacket(new NpcInfoPacket((Npc) this));
						}
					});
				}

				_broadcastModifiedStatTask = null;
			}, 50);
		}
	}

	public int getXdestination()
	{
		MoveData move = _move;
		if (move != null)
		{
			return move.xDestination;
		}

		return getX();
	}

	/**
	 * @return the Y destination of the Creature or the Y position if not in movement.
	 */
	public int getYdestination()
	{
		MoveData move = _move;
		if (move != null)
		{
			return move.yDestination;
		}

		return getY();
	}

	/**
	 * @return the Z destination of the Creature or the Z position if not in movement.
	 */
	public int getZdestination()
	{
		MoveData move = _move;
		if (move != null)
		{
			return move.zDestination;
		}

		return getZ();
	}

	/**
	 * @return True if the Creature is in combat.
	 */
	public virtual bool isInCombat()
	{
		return hasAI() && getAI().isAutoAttacking();
	}

	/**
	 * @return True if the Creature is moving.
	 */
	public bool isMoving()
	{
		return _move != null;
	}

	/**
	 * @return True if the Creature is traveling a calculated path.
	 */
	public bool isOnGeodataPath()
	{
		MoveData move = _move;
		if (move == null)
		{
			return false;
		}

		return isOnGeodataPath(move);
	}

	/**
	 * @param move the MoveData to check (must not be null).
	 * @return True if the Creature is traveling a calculated path.
	 */
	protected bool isOnGeodataPath(MoveData move)
	{
		if (move.onGeodataPathIndex == -1)
		{
			return false;
		}

		if (move.onGeodataPathIndex == (move.geoPath.Count - 1))
		{
			return false;
		}

		return true;
	}

	/**
	 * This method returns a list of {@link AbstractNodeLoc} objects representing the movement path.<br>
	 * If the move operation is defined (not null), it returns the path from the 'geoPath' field of the move.<br>
	 * Otherwise, it returns null.
	 * @return List of {@link AbstractNodeLoc} representing the movement path, or null if move is undefined.
	 */
	public List<AbstractNodeLoc> getGeoPath()
	{
		MoveData move = _move;
		if (move != null)
		{
			return move.geoPath;
		}

		return null;
	}

	/**
	 * @return True if the Creature is casting any kind of skill, including simultaneous skills like potions.
	 */
	public bool isCastingNow()
	{
		return _skillCasters.Count != 0;
	}

	public bool isCastingNow(SkillCastingType skillCastingType)
	{
		return _skillCasters.ContainsKey(skillCastingType);
	}

	public bool isCastingNow(Predicate<SkillCaster> filter)
	{
		foreach (SkillCaster skillCaster in _skillCasters.Values)
		{
			if (filter(skillCaster))
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * @return True if the Creature is attacking.
	 */
	public bool isAttackingNow()
	{
		return _attackEndTime > DateTime.UtcNow;
	}

	/**
	 * Abort the attack of the Creature and send Server=>Client ActionFailed packet.
	 */
	public void abortAttack()
	{
		if (isAttackingNow())
		{
			CreatureAttackTaskManager.getInstance().abortAttack(this);
			sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
	}

	/**
	 * Abort the cast of all skills.
	 */
	public void abortAllSkillCasters()
	{
		foreach (SkillCaster skillCaster in getSkillCasters())
		{
			skillCaster.stopCasting(true);
			if (isPlayer())
			{
				getActingPlayer().setQueuedSkill(null, null, false, false);
			}
		}
	}

	/**
	 * Abort the cast of normal non-simultaneous skills.
	 * @return {@code true} if a skill casting has been aborted, {@code false} otherwise.
	 */
	public bool abortCast()
	{
		return abortCast(x => x.isAnyNormalType());
	}

	/**
	 * Try to break this character's casting using the given filters.
	 * @param filter
	 * @return {@code true} if a skill casting has been aborted, {@code false} otherwise.
	 */
	public bool abortCast(Predicate<SkillCaster> filter)
	{
		SkillCaster skillCaster = getSkillCaster(x => x.canAbortCast(), filter);
		if (skillCaster != null)
		{
			skillCaster.stopCasting(true);
			if (isPlayer())
			{
				getActingPlayer().setQueuedSkill(null, null, false, false);
			}
			return true;
		}
		return false;
	}

	/**
	 * Update the position of the Creature during a movement and return True if the movement is finished.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * At the beginning of the move action, all properties of the movement are stored in the MoveData object called <b>_move</b> of the Creature.<br>
	 * The position of the start point and of the destination permit to estimated in function of the movement speed the time to achieve the destination.<br>
	 * When the movement is started (ex : by MovetoLocation), this method will be called each 0.1 sec to estimate and update the Creature position on the server.<br>
	 * Note, that the current server position can differe from the current client position even if each movement is straight foward.<br>
	 * That's why, client send regularly a Client=>Server ValidatePosition packet to eventually correct the gap on the server.<br>
	 * But, it's always the server position that is used in range calculation. At the end of the estimated movement time,<br>
	 * the Creature position is automatically set to the destination position even if the movement is not finished.<br>
	 * <font color=#FF0000><b><u>Caution</u>: The current Z position is obtained FROM THE CLIENT by the Client=>Server ValidatePosition Packet.<br>
	 * But x and y positions must be calculated to avoid that players try to modify their movement speed.</b></font>
	 * @return True if the movement is finished
	 */
	public virtual bool updatePosition()
	{
		if (!isSpawned())
		{
			_move = null;
			return true;
		}

		// Get movement data
		MoveData move = _move;
		if (move == null)
		{
			return true;
		}

		// Check if this is the first update
		if (move.moveTimestamp == 0)
		{
			move.moveTimestamp = move.moveStartTime;
			move.xAccurate = getX();
			move.yAccurate = getY();
		}

		// Check if the position has already been calculated
		int gameTicks = GameTimeTaskManager.getInstance().getGameTicks();
		if (move.moveTimestamp == gameTicks)
		{
			return false;
		}

		_suspendedMovement = false;
		int xPrev = getX();
		int yPrev = getY();
		int zPrev = getZ(); // the z coordinate may be modified by coordinate synchronizations
		double dx = move.xDestination - move.xAccurate;
		double dy = move.yDestination - move.yAccurate;
		double dz = move.zDestination - zPrev; // Z coordinate will follow client values
		if (isPlayer() && !_isFlying)
		{
			// In case of cursor movement, avoid moving through obstacles.
			if (_cursorKeyMovement)
			{
				double angle = HeadingUtil.ConvertHeadingToDegrees(getHeading());
				double radian = double.DegreesToRadians(angle);
				double course = double.DegreesToRadians(180);
				double frontDistance = 10 * (1.0 * _stat.getMoveSpeed() / 100);
				int x1 = (int) (Math.Cos(Math.PI + radian + course) * frontDistance);
				int y1 = (int) (Math.Sin(Math.PI + radian + course) * frontDistance);
				int x = xPrev + x1;
				int y = yPrev + y1;
				if (!GeoEngine.getInstance().canMoveToTarget(new Location3D(xPrev, yPrev, zPrev), new Location3D(x, y, zPrev), getInstanceWorld()))
				{
					_move.onGeodataPathIndex = -1;
					stopMove(new Location(getActingPlayer().getLastServerPosition(), 0));
					return false;
				}
			}
			else // Mouse click movement.
			{
				// Stop movement when player has clicked far away and intersected with an obstacle.
				double distance = double.Hypot(dx, dy);
				if (distance > 3000)
				{
					double angle = HeadingUtil.ConvertHeadingToDegrees(getHeading());
					double radian = double.DegreesToRadians(angle);
					double course = double.DegreesToRadians(180);
					double frontDistance = 10 * (1.0 * _stat.getMoveSpeed() / 100);
					int x1 = (int) (Math.Cos(Math.PI + radian + course) * frontDistance);
					int y1 = (int) (Math.Sin(Math.PI + radian + course) * frontDistance);
					int x = xPrev + x1;
					int y = yPrev + y1;
					if (!GeoEngine.getInstance().canMoveToTarget(new Location3D(xPrev, yPrev, zPrev), new Location3D(x, y, zPrev), getInstanceWorld()))
					{
						_move.onGeodataPathIndex = -1;
						if (hasAI())
						{
							if (getAI().isFollowing())
							{
								getAI().stopFollow();
							}
							getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
						}
						return false;
					}
				}
				else // Check for nearby doors or fences.
				{
					// Support for player attack with direct movement. Tested at retail on May 11th 2023.
					if (hasAI() && getAI().getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
					{
						double angle = HeadingUtil.ConvertHeadingToDegrees(getHeading());
						double radian = double.DegreesToRadians(angle);
						double course = double.DegreesToRadians(180);
						double frontDistance = 10 * (1.0 * _stat.getMoveSpeed() / 100);
						int x1 = (int) (Math.Cos(Math.PI + radian + course) * frontDistance);
						int y1 = (int) (Math.Sin(Math.PI + radian + course) * frontDistance);
						int x = xPrev + x1;
						int y = yPrev + y1;
						if (!GeoEngine.getInstance().canMoveToTarget(new Location3D(xPrev, yPrev, zPrev), new Location3D(x, y, zPrev), getInstanceWorld()))
						{
							_suspendedMovement = true;
							_move.onGeodataPathIndex = -1;
							broadcastPacket(new StopMovePacket(this));
							return false;
						}
					}
					else // Check for nearby doors or fences.
					{
						WorldRegion region = getWorldRegion();
						if (region != null)
						{
							bool hasDoors = region.getDoors().Count != 0;
							bool hasFences = region.getFences().Count != 0;
							if (hasDoors || hasFences)
							{
								double angle = HeadingUtil.ConvertHeadingToDegrees(getHeading());
								double radian = double.DegreesToRadians(angle);
								double course = double.DegreesToRadians(180);
								double frontDistance = 10 * (1.0 * _stat.getMoveSpeed() / 100);
								int x1 = (int) (Math.Cos(Math.PI + radian + course) * frontDistance);
								int y1 = (int) (Math.Sin(Math.PI + radian + course) * frontDistance);
								int x = xPrev + x1;
								int y = yPrev + y1;
								if ((hasDoors && DoorData.getInstance().checkIfDoorsBetween(new Location3D(xPrev, yPrev, zPrev), new Location3D(x, y, zPrev), getInstanceWorld(), false)) //
									|| (hasFences && FenceData.getInstance().checkIfFenceBetween(new Location3D(xPrev, yPrev, zPrev), new Location3D(x, y, zPrev), getInstanceWorld())))
								{
									_move.onGeodataPathIndex = -1;
									if (hasAI())
									{
										if (getAI().isFollowing())
										{
											getAI().stopFollow();
										}
										getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
									}
									stopMove(null);
									return false;
								}
							}
						}
					}
				}
			}
		}

		// Distance from destination.
		double delta = dx * dx + dy * dy;
		bool isFloating = _isFlying || (isInsideZone(ZoneId.WATER) && !isInsideZone(ZoneId.CASTLE));
		if (!isFloating && delta < 10000 && dz * dz > 2500) // Close enough, allows error between client and server geodata if it cannot be avoided.
		{
			delta = Math.Sqrt(delta);
		}
		else
		{
			delta = Math.Sqrt(delta + dz * dz);
		}

		// Target collision should be subtracted from current distance.
		double collision;
		WorldObject target = _target;
		if (target != null && target.isCreature() && hasAI() && getAI().getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
		{
			collision = ((Creature) target).getCollisionRadius();
		}
		else
		{
			collision = getCollisionRadius();
		}
		delta = Math.Max(0.00001, delta - collision);

		double distFraction = double.MaxValue;
		if (delta > 1)
		{
			double distPassed = _stat.getMoveSpeed() * (gameTicks - move.moveTimestamp) / GameTimeTaskManager.TICKS_PER_SECOND;
			distFraction = distPassed / delta;
		}

		if (distFraction > 1.79)
		{
			// Set the position of the Creature to the destination.
			base.setXYZ(move.xDestination, move.yDestination, move.zDestination);
		}
		else
		{
			move.xAccurate += dx * distFraction;
			move.yAccurate += dy * distFraction;

			// Set the position of the Creature to estimated after parcial move.
			base.setXYZ((int) move.xAccurate, (int) move.yAccurate, zPrev + (int) (dz * distFraction + 0.895));
		}
		revalidateZone(false);

		// Set the timer of last position update to now.
		move.moveTimestamp = gameTicks;

		// Broadcast MoveToLocation when Playable tries to reach a Playable target (once per second).
		if (isPlayable() && ((gameTicks - move.lastBroadcastTime) >= 3) && isOnGeodataPath(move))
		{
			move.lastBroadcastTime = gameTicks;
			broadcastPacket(new MoveToLocationPacket(this));
		}

		return distFraction > 1.79; // Arrived.
	}

	public virtual void revalidateZone(bool force)
	{
		// This function is called too often from movement code.
		if (!force && this.Distance3D(_lastZoneValidateLocation) < (isNpc() && !isInCombat() ? Config.MAX_DRIFT_RANGE : 100))
		{
			return;
		}

		_lastZoneValidateLocation = Location.Location3D;

		ZoneRegion? region = ZoneManager.getInstance().getRegion(Location.Location2D);
		if (region != null)
		{
			region.revalidateZones(this);
		}
		else // Precaution. Moved at invalid region?
		{
			World.getInstance().disposeOutOfBoundsObject(this);
		}
	}

	/**
	 * Stop movement of the Creature (Called by AI Accessor only).<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Delete movement data of the Creature</li>
	 * <li>Set the current position (x,y,z), its current WorldRegion if necessary and its heading</li>
	 * <li>Remove the WorldObject object from _gmList of GmListTable</li>
	 * <li>Remove object from _knownObjects and _knownPlayer of all surrounding WorldRegion Creatures</li>
	 * </ul>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T send Server=>Client packet StopMove/StopRotation</b></font>
	 * @param loc
	 */
	public virtual void stopMove(Location? location) // TODO: overload without argument
	{
		// Delete movement data of the Creature
		_move = null;
		_cursorKeyMovement = false;

		// All data are contained in a Location object
		if (location != null)
		{
			Location loc = location.Value;
			setXYZ(loc.X, loc.Y, loc.Z);
			setHeading(loc.Heading);
			revalidateZone(true);
		}

		broadcastPacket(new StopMovePacket(this));
	}

	/**
	 * @return Returns the showSummonAnimation.
	 */
	public bool isShowSummonAnimation()
	{
		return _showSummonAnimation;
	}

	/**
	 * @param showSummonAnimation The showSummonAnimation to set.
	 */
	public void setShowSummonAnimation(bool showSummonAnimation)
	{
		_showSummonAnimation = showSummonAnimation;
	}

	/**
	 * Target a WorldObject (add the target to the Creature _target, _knownObject and Creature to _KnownObject of the WorldObject).<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * The WorldObject (including Creature) targeted is identified in <b>_target</b> of the Creature.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Set the _target of Creature to WorldObject</li>
	 * <li>If necessary, add WorldObject to _knownObject of the Creature</li>
	 * <li>If necessary, add Creature to _KnownObject of the WorldObject</li>
	 * <li>If object==null, cancel Attak or Cast</li>
	 * </ul>
	 * @param object L2object to target
	 */
	public virtual void setTarget(WorldObject? obj)
	{
		if (obj != null && !obj.isSpawned())
		{
			_target = null;
			return;
		}
		_target = obj;
	}

	/**
	 * @return the identifier of the WorldObject targeted or -1.
	 */
	public int getTargetId()
	{
		if (_target != null)
		{
			return _target.ObjectId;
		}
		return 0;
	}

	/**
	 * @return the WorldObject targeted or null.
	 */
	public WorldObject? getTarget()
	{
		return _target;
	}

	// called from AIAccessor only

	/**
	 * Calculate movement data for a move to location action and add the Creature to MOVING_OBJECTS of MovementTaskManager (only called by AI Accessor).<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * At the beginning of the move action, all properties of the movement are stored in the MoveData object called <b>_move</b> of the Creature.<br>
	 * The position of the start point and of the destination permit to estimated in function of the movement speed the time to achieve the destination.<br>
	 * All Creature in movement are identified in <b>MOVING_OBJECTS</b> of MovementTaskManager that will call the updatePosition method of those Creature each 0.1s.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Get current position of the Creature</li>
	 * <li>Calculate distance (dx,dy) between current position and destination including offset</li>
	 * <li>Create and Init a MoveData object</li>
	 * <li>Set the Creature _move object to MoveData object</li>
	 * <li>Add the Creature to MOVING_OBJECTS of the MovementTaskManager</li>
	 * <li>Create a task to notify the AI that Creature arrives at a check point of the movement</li>
	 * </ul>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T send Server=>Client packet MoveToPawn/MoveToLocation.</b></font><br>
	 * <br>
	 * <b><u>Example of use</u>:</b>
	 * <ul>
	 * <li>AI : onIntentionMoveTo(Location), onIntentionPickUp(WorldObject), onIntentionInteract(WorldObject)</li>
	 * <li>FollowTask</li>
	 * </ul>
	 * @param xValue The X position of the destination
	 * @param yValue The Y position of the destination
	 * @param zValue The Y position of the destination
	 * @param offsetValue The size of the interaction area of the Creature targeted
	 */
	public virtual void moveToLocation(Location3D location, int offsetValue)
	{
		// Get the Move Speed of the Creature
		double speed = _stat.getMoveSpeed();
		if (speed <= 0 || isMovementDisabled())
		{
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}

		Location3D loc = location;
		int offset = offsetValue;

		// Get current position of the Creature
		Location3D curLoc = Location.Location3D;

		// Calculate distance (dx,dy) between current position and destination
		// TODO: improve Z axis move/follow support when dx,dy are small compared to dz
		Location3D dLoc = loc - curLoc;
		double distance = dLoc.Length2D;
		bool verticalMovementOnly = _isFlying && distance == 0 && dLoc.Z != 0;
		if (verticalMovementOnly)
			distance = Math.Abs(dLoc.Z);

		// Make water move short and use no geodata checks for swimming chars distance in a click can easily be over 3000.
		bool isInWater = isInsideZone(ZoneId.WATER) && !isInsideZone(ZoneId.CASTLE);
		if (isInWater && distance > 700)
		{
			double divider = 700 / distance;
			loc = curLoc + dLoc.Scale(divider);
			dLoc = loc - curLoc;
			distance = dLoc.Length2D;
		}

		// @formatter:off
		// Define movement angles needed
		// ^
		// |    X (x,y)
		// |   /
		// |  / distance
		// | /
		// |/ angle
		// X ---------=>
		// (curx,cury)
		// @formatter:on

		double cos;
		double sin;

		// Check if a movement offset is defined or no distance to go through
		if (offset > 0 || distance < 1)
		{
			// approximation for moving closer when z coordinates are different
			// TODO: handle Z axis movement better
			offset -= Math.Abs(dLoc.Z);
			if (offset < 5)
				offset = 5;

			// If no distance to go through, the movement is canceled
			if (distance < 1 || distance - offset <= 0)
			{
				// Notify the AI that the Creature is arrived at destination
				getAI().notifyEvent(CtrlEvent.EVT_ARRIVED);
				return;
			}

			// Calculate movement angles needed
			sin = dLoc.Y / distance;
			cos = dLoc.X / distance;
			distance -= offset - 5; // due to rounding error, we have to move a bit closer to be in range

			// Calculate the new destination with offset included
			int newX = curLoc.X + (int) (distance * cos);
			int newY = curLoc.Y + (int) (distance * sin);
			loc = loc with { X = newX, Y = newY };
		}
		else
		{
			// Calculate movement angles needed
			sin = dLoc.Y / distance;
			cos = dLoc.X / distance;
		}

		// Create and Init a MoveData object
		MoveData move = new MoveData();

		// GEODATA MOVEMENT CHECKS AND PATHFINDING
		WorldRegion region = getWorldRegion();
		move.disregardingGeodata = region == null || !region.areNeighborsActive();
		move.onGeodataPathIndex = -1; // Initialize not on geodata path
		if (!move.disregardingGeodata && !_isFlying && !isInWater && !isVehicle() && !_cursorKeyMovement)
		{
			bool isInVehicle = isPlayer() && getActingPlayer().getVehicle() != null;
			if (isInVehicle)
			{
				move.disregardingGeodata = true;
			}

			// Movement checks.
			if (Config.PATHFINDING > 0 && this is not FriendlyNpc)
			{
				Location3D originalLoc = loc;
				double originalDistance = distance;
				int gtx = (originalLoc.X - World.WORLD_X_MIN) >> 4;
				int gty = (originalLoc.Y - World.WORLD_Y_MIN) >> 4;
				if (isOnGeodataPath())
				{
					try
					{
						if (gtx == _move.geoPathGtx && gty == _move.geoPathGty)
						{
							return;
						}
						_move.onGeodataPathIndex = -1; // Set not on geodata path.
					}
					catch (NullReferenceException)
					{
						// nothing
					}
				}

				// Support for player attack with direct movement. Tested at retail on May 11th 2023.
				bool directMove = isPlayer() && hasAI() &&
					getActingPlayer().getAI().getIntention() == CtrlIntention.AI_INTENTION_ATTACK;

				if (directMove //
				    || (!isInVehicle // Not in vehicle.
					    && !(isPlayer() && distance > 3000) // Should be able to click far away and move.
					    && !(isMonster() && Math.Abs(dLoc.Z) > 100) // Monsters can move on ledges.
					    && !(curLoc.Z - loc.Z > 300 && distance < 300))) // Prohibit correcting destination if character wants to fall.
				{
					// location different if destination wasn't reached (or just z coord is different)
					Location3D destiny = GeoEngine.getInstance().getValidLocation(curLoc, loc, getInstanceWorld());
					loc = isPlayer() ? destiny with { Z = loc.Z } : destiny;
					dLoc = loc - curLoc;
					distance = verticalMovementOnly ? Math.Pow(dLoc.Z, 2) : dLoc.Length2D;
				}

				// Pathfinding checks.
				if (!directMove && originalDistance - distance > 30 && !isControlBlocked() && !isInVehicle)
				{
					// Path calculation -- overrides previous movement check
					move.geoPath = PathFinding.getInstance().findPath(curLoc, originalLoc, getInstanceWorld(), isPlayer());

					bool found = move.geoPath != null && move.geoPath.Count > 1;

					// If path not found and this is an Attackable, attempt to find closest path to destination.
					if (!found && isAttackable())
					{
						int xMin = Math.Min(curLoc.X, originalLoc.X);
						int xMax = Math.Max(curLoc.X, originalLoc.X);
						int yMin = Math.Min(curLoc.Y, originalLoc.Y);
						int yMax = Math.Max(curLoc.Y, originalLoc.Y);
						int maxDiff = Math.Min(Math.Max(xMax - xMin, yMax - yMin), 500);
						xMin -= maxDiff;
						xMax += maxDiff;
						yMin -= maxDiff;
						yMax += maxDiff;
						int destinationX = 0;
						int destinationY = 0;
						double shortDistance = double.MaxValue;
						for (int sX = xMin; sX < xMax; sX += 500)
						{
							for (int sY = yMin; sY < yMax; sY += 500)
							{
								double tempDistance = double.Hypot(sX - originalLoc.X, sY - originalLoc.Y);
								if (tempDistance < shortDistance)
								{
									List<AbstractNodeLoc> tempPath = PathFinding.getInstance().findPath(curLoc, new Location3D(sX, sY, originalLoc.Z), getInstanceWorld(), false);

									if (tempPath != null && tempPath.Count > 1)
									{
										shortDistance = tempDistance;
										move.geoPath = tempPath;
										destinationX = sX;
										destinationY = sY;
									}
								}
							}
						}
						found = move.geoPath != null && move.geoPath.Count > 1;
						if (found)
						{
							originalLoc = originalLoc with { X = destinationX, Y = destinationY };
						}
					}

					if (found)
					{
						move.onGeodataPathIndex = 0; // On first segment.
						move.geoPathGtx = gtx;
						move.geoPathGty = gty;
						move.geoPathAccurateTx = originalLoc.X;
						move.geoPathAccurateTy = originalLoc.Y;
						AbstractNodeLoc node = move.geoPath[move.onGeodataPathIndex];
						loc = node.Location;
						dLoc = loc - curLoc;
						distance = verticalMovementOnly ? Math.Pow(dLoc.Z, 2) : dLoc.Length2D;
						sin = dLoc.Y / distance;
						cos = dLoc.X / distance;
					}
					else // No path found.
					{
						if (isPlayer() && !_isFlying && !isInWater)
						{
							sendPacket(ActionFailedPacket.STATIC_PACKET);
							return;
						}

						move.disregardingGeodata = true;

						loc = originalLoc;
						distance = originalDistance;
					}
				}
			}

			// If no distance to go through, the movement is canceled
			if (distance < 1 && (Config.PATHFINDING > 0 || isPlayable()))
			{
				if (isSummon())
				{
					// Do not break following owner.
					if (getAI().getTarget() != getActingPlayer())
					{
						((Summon) this).setFollowStatus(false);
						getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
					}
				}
				else
				{
					getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
					sendPacket(ActionFailedPacket.STATIC_PACKET);
				}
				return;
			}
		}

		// Apply Z distance for flying or swimming for correct timing calculations
		if ((_isFlying || isInWater) && !verticalMovementOnly)
		{
			distance = double.Hypot(distance, dLoc.Z);
		}

		// Calculate the number of ticks between the current position and the destination.
		int ticksToMove = (int) (GameTimeTaskManager.TICKS_PER_SECOND * distance / speed);
		move.xDestination = loc.X;
		move.yDestination = loc.Y;
		move.zDestination = loc.Z; // this is what was requested from client

		// Calculate and set the heading of the Creature
		move.heading = 0; // initial value for coordinate sync
		// Does not broke heading on vertical movements
		if (!verticalMovementOnly)
		{
			setHeading(HeadingUtil.CalculateHeading(cos, sin));
		}

		move.moveStartTime = GameTimeTaskManager.getInstance().getGameTicks();

		// Set the Creature _move object to MoveData object
		_move = move;

		// Add the Creature to moving objects of the MovementTaskManager.
		// The MovementTaskManager manages object movement.
		MovementTaskManager.getInstance().registerMovingObject(this);

		// Create a task to notify the AI that Creature arrives at a check point of the movement
		if (ticksToMove * GameTimeTaskManager.MILLIS_IN_TICK > 3000)
		{
			ThreadPool.schedule(new NotifyAITask(this, CtrlEvent.EVT_ARRIVED_REVALIDATE), 2000);
		}
		// the CtrlEvent.EVT_ARRIVED will be sent when the character will actually arrive to destination by MovementTaskManager
	}

	public virtual bool moveToNextRoutePoint()
	{
		if (!isOnGeodataPath())
		{
			// Cancel the move action
			_move = null;
			return false;
		}

		// Get the Move Speed of the Creature
		double speed = _stat.getMoveSpeed();
		if (speed <= 0 || isMovementDisabled())
		{
			// Cancel the move action
			_move = null;
			return false;
		}

		MoveData md = _move;
		if (md == null)
		{
			return false;
		}

		// Get current position of the Creature
		int curX = getX();
		int curY = getY();

		// Create and Init a MoveData object
		MoveData m = new MoveData();

		// Update MoveData object
		m.onGeodataPathIndex = md.onGeodataPathIndex + 1; // next segment
		m.geoPath = md.geoPath;
		m.geoPathGtx = md.geoPathGtx;
		m.geoPathGty = md.geoPathGty;
		m.geoPathAccurateTx = md.geoPathAccurateTx;
		m.geoPathAccurateTy = md.geoPathAccurateTy;
		Location3D geoNodeLocation = md.geoPath[m.onGeodataPathIndex].Location;
		if (md.onGeodataPathIndex == md.geoPath.Count - 2)
		{
			m.xDestination = md.geoPathAccurateTx;
			m.yDestination = md.geoPathAccurateTy;
			m.zDestination = geoNodeLocation.Z;
		}
		else
		{
			m.xDestination = geoNodeLocation.X;
			m.yDestination = geoNodeLocation.Y;
			m.zDestination = geoNodeLocation.Z;
		}

		// Calculate and set the heading of the Creature.
		double distance = double.Hypot(m.xDestination - curX, m.yDestination - curY);
		if (distance != 0)
		{
			setHeading(new Location2D(curX, curY).HeadingTo(new Location2D(m.xDestination, m.yDestination)));
		}

		// Calculate the number of ticks between the current position and the destination.
		int ticksToMove = (int) (GameTimeTaskManager.TICKS_PER_SECOND * distance / speed);
		m.heading = 0; // initial value for coordinate sync
		m.moveStartTime = GameTimeTaskManager.getInstance().getGameTicks();

		// Set the Creature _move object to MoveData object
		_move = m;

		// Add the Creature to moving objects of the MovementTaskManager.
		// The MovementTaskManager manages object movement.
		MovementTaskManager.getInstance().registerMovingObject(this);

		// Create a task to notify the AI that Creature arrives at a check point of the movement
		if (ticksToMove * GameTimeTaskManager.MILLIS_IN_TICK > 3000)
		{
			ThreadPool.schedule(new NotifyAITask(this, CtrlEvent.EVT_ARRIVED_REVALIDATE), 2000);
		}

		// the CtrlEvent.EVT_ARRIVED will be sent when the character will actually arrive to destination by MovementTaskManager

		// Send a Server=>Client packet MoveToLocation to the actor and all Player in its _knownPlayers
		broadcastMoveToLocation();
		return true;
	}

	public bool validateMovementHeading(int heading)
	{
		MoveData m = _move;
		if (m == null)
		{
			return true;
		}

		bool result = true;
		if (m.heading != heading)
		{
			result = m.heading == 0; // initial value or false
			m.heading = heading;
		}

		return result;
	}

	/**
	 * <b><u>Overridden in</u>:</b>
	 * <li>Player</li>
	 * @return True if arrows are available.
	 * @param type
	 */
	protected virtual bool checkAndEquipAmmunition(EtcItemType type)
	{
		return true;
	}

	/**
	 * Add Exp and Sp to the Creature.<br>
	 * <br>
	 * <b><u>Overridden in</u>:</b>
	 * <li>Player</li>
	 * <li>Pet</li><br>
	 * @param addToExp
	 * @param addToSp
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public virtual void addExpAndSp(double addToExp, double addToSp)
	{
		// Dummy method (overridden by players and pets)
	}

	/**
	 * <b><u>Overridden in</u>:</b>
	 * <li>Player</li>
	 * @return the active weapon instance (always equipped in the right hand).
	 */
	public abstract Item? getActiveWeaponInstance();

	/**
	 * <b><u>Overridden in</u>:</b>
	 * <li>Player</li>
	 * @return the active weapon item (always equipped in the right hand).
	 */
	public abstract Weapon? getActiveWeaponItem();

	/**
	 * <b><u>Overridden in</u>:</b>
	 * <li>Player</li>
	 * @return the secondary weapon instance (always equipped in the left hand).
	 */
	public abstract Item? getSecondaryWeaponInstance();

	/**
	 * <b><u>Overridden in</u>:</b>
	 * <li>Player</li>
	 * @return the secondary {@link ItemTemplate} item (always equipped in the left hand).
	 */
	public abstract ItemTemplate? getSecondaryWeaponItem();

	/**
	 * Manage hit process (called by Hit Task).<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>If the attacker/target is dead or use fake death, notify the AI with EVT_CANCEL and send a Server=>Client packet ActionFailed (if attacker is a Player)</li>
	 * <li>If attack isn't aborted, send a message system (critical hit, missed...) to attacker/target if they are Player</li>
	 * <li>If attack isn't aborted and hit isn't missed, reduce HP of the target and calculate reflection damage to reduce HP of attacker if necessary</li>
	 * <li>if attack isn't aborted and hit isn't missed, manage attack or cast break of the target (calculating rate, sending message...)</li>
	 * </ul>
	 * @param weapon the weapon used for the hit
	 * @param attack the attack data of targets to hit
	 * @param hitTime the time it took for this hit to occur
	 * @param attackTime the time it takes for the whole attack to complete
	 */
	public void onHitTimeNotDual(Weapon weapon, AttackPacket attack, int hitTime, int attackTime)
	{
		if (_isDead)
		{
			getAI().notifyEvent(CtrlEvent.EVT_CANCEL);
			return;
		}

		foreach (Hit hit in attack.getHits())
		{
			Creature target = (Creature) hit.getTarget();
			if (target == null || target.isDead() || !isInSurroundingRegion(target))
			{
				continue;
			}

			if (hit.isMiss())
			{
				notifyAttackAvoid(target, false);
			}
			else
			{
				// Avoid arrows dealing damage when the target hides behind something.
				if (weapon != null && weapon.getItemType().isRanged() && !GeoEngine.getInstance().canSeeTarget(this, target))
				{
					if (target.isPlayer())
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_AVOIDED_C1_S_ATTACK);
						sm.Params.addString(getName());
						target.sendPacket(sm);
					}
					if (isPlayer())
					{
						sendPacket(SystemMessageId.YOU_HAVE_MISSED);
					}
					continue;
				}

				onHitTarget(target, weapon, hit);
			}
		}

		CreatureAttackTaskManager.getInstance().onAttackFinish(this, attack, attackTime - hitTime);
	}

	public void onFirstHitTimeForDual(Weapon weapon, AttackPacket attack, int hitTime, int attackTime, int delayForSecondAttack)
	{
		if (_isDead)
		{
			getAI().notifyEvent(CtrlEvent.EVT_CANCEL);
			return;
		}

		CreatureAttackTaskManager.getInstance().onSecondHitTimeForDual(this, weapon, attack, hitTime, attackTime, delayForSecondAttack);

		// First dual attack is the first hit only.
		Hit hit = attack.getHits()[0];
		Creature target = (Creature) hit.getTarget();
		if (target == null || target.isDead() || !isInSurroundingRegion(target))
		{
			getAI().notifyEvent(CtrlEvent.EVT_CANCEL);
			return;
		}

		if (hit.isMiss())
		{
			notifyAttackAvoid(target, false);
		}
		else
		{
			onHitTarget(target, weapon, hit);
		}
	}

	public void onSecondHitTimeForDual(Weapon weapon, AttackPacket attack, int hitTime1, int hitTime2, int attackTime)
	{
		if (_isDead)
		{
			getAI().notifyEvent(CtrlEvent.EVT_CANCEL);
			return;
		}

		// Second dual attack is the remaining hits (first hit not included)
		for (int i = 1; i < attack.getHits().Count; i++)
		{
			Hit hit = attack.getHits()[i];
			Creature target = (Creature) hit.getTarget();
			if (target == null || target.isDead() || !isInSurroundingRegion(target))
			{
				continue;
			}

			if (hit.isMiss())
			{
				notifyAttackAvoid(target, false);
			}
			else
			{
				onHitTarget(target, weapon, hit);
			}
		}

		CreatureAttackTaskManager.getInstance().onAttackFinish(this, attack, attackTime - (hitTime1 + hitTime2));
	}

	public void onHitTarget(Creature target, Weapon weapon, Hit hit)
	{
		// reduce targets HP
		doAttack(hit.getDamage(), target, null, false, false, hit.isCritical(), false);

		// Notify to scripts when the attack has been done.
		if (_eventContainer.HasSubscribers<OnCreatureAttack>())
		{
			_onCreatureAttack ??= new OnCreatureAttack();
			_onCreatureAttack.setAttacker(this);
			_onCreatureAttack.setTarget(target);
			_onCreatureAttack.setSkill(null);
			_onCreatureAttack.Terminate = false;
			_onCreatureAttack.Abort = false;
			_eventContainer.Notify(_onCreatureAttack);
		}

		if (target.Events.HasSubscribers<OnCreatureAttacked>())
		{
			_onCreatureAttacked ??= new OnCreatureAttacked();
			_onCreatureAttacked.setAttacker(this);
			_onCreatureAttacked.setTarget(target);
			_onCreatureAttacked.setSkill(null);
			_onCreatureAttacked.Terminate = false;
			_onCreatureAttacked.Abort = false;
			target.Events.Notify(_onCreatureAttacked);
		}

		if (_triggerSkills != null)
		{
			foreach (OptionSkillHolder holder in _triggerSkills.Values)
			{
				if (((!hit.isCritical() && holder.getSkillType() == OptionSkillType.ATTACK) || (holder.getSkillType() == OptionSkillType.CRITICAL && hit.isCritical())) && Rnd.get(100) < holder.getChance())
				{
					SkillCaster.triggerCast(this, target, holder.getSkill(), null, false);
				}
			}
		}

		// Launch weapon Special ability effect if available
		if (hit.isCritical() && weapon != null)
		{
			weapon.applyConditionalSkills(this, target, null, ItemSkillType.ON_CRITICAL_SKILL);
		}

		if (isPlayer() && !target.isHpBlocked())
		{
			Player player = getActingPlayer();

			// If hit by a cursed weapon, CP is reduced to 0.
			// If a cursed weapon is hit by a Hero, CP is reduced to 0.
			if (player.isCursedWeaponEquipped() || (player.isHero() && target.isPlayer() && target.getActingPlayer().isCursedWeaponEquipped()))
			{
				target.setCurrentCp(0);
			}

			if (player.isDeathKnight())
			{
				if (target.isAttackable() || target.isPlayable())
				{
					player.setDeathPoints(player.getDeathPoints() + 1);
				}
			}
			else if (player.isVanguard())
			{
				if (target.isAttackable() || target.isPlayable())
				{
					player.setBeastPoints(player.getBeastPoints() + 1);
				}
			}
			else if (player.isAssassin() && CategoryData.getInstance().isInCategory(CategoryType.FOURTH_CLASS_GROUP, player.getBaseTemplate().getClassId()) && target.isDead())
			{
				if (target.isPlayable())
				{
					player.setAssassinationPoints(player.getAssassinationPoints() + 1000);
					player.sendPacket(new UserInfoPacket(player));
				}
				else if (target.isAttackable())
				{
					player.setAssassinationPoints(player.getAssassinationPoints() + 5);
					player.sendPacket(new UserInfoPacket(player));
				}
			}
		}
	}

	public void onAttackFinish(AttackPacket attack)
	{
		// Recharge any active auto-soulshot tasks for current creature after the attack has successfully hit.
		foreach (Hit hit in attack.getHits())
		{
			if (!hit.isMiss())
			{
				rechargeShots(true, false, false);
				break;
			}
		}

		// Notify that this character is ready to act for the next attack
		getAI().notifyEvent(CtrlEvent.EVT_READY_TO_ACT);
	}

	/**
	 * Break an attack and send Server=>Client ActionFailed packet and a System Message to the Creature.
	 */
	public void breakAttack()
	{
		if (isAttackingNow())
		{
			// Abort the attack of the Creature and send Server=>Client ActionFailed packet
			abortAttack();
			if (isPlayer())
			{
				// Send a system message
				sendPacket(SystemMessageId.YOUR_ATTACK_HAS_FAILED);
			}
		}
	}

	/**
	 * Break a cast and send Server=>Client ActionFailed packet and a System Message to the Creature.
	 */
	public void breakCast()
	{
		// Break only one skill at a time while casting.
		SkillCaster skillCaster = getSkillCaster(c => c.isAnyNormalType());
		if (skillCaster != null && skillCaster.getSkill().isMagic())
		{
			// Abort the cast of the Creature and send Server=>Client MagicSkillCanceled/ActionFailed packet.
			skillCaster.stopCasting(true);

			if (isPlayer())
			{
				// Send a system message
				sendPacket(SystemMessageId.YOUR_CASTING_HAS_BEEN_INTERRUPTED);
			}
		}
	}

	/**
	 * Manage Forced attack (shift + select target).<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>If Creature or target is in a town area, send a system message TARGET_IN_PEACEZONE a Server=>Client packet ActionFailed</li>
	 * <li>If target is confused, send a Server=>Client packet ActionFailed</li>
	 * <li>If Creature is a Artefact, send a Server=>Client packet ActionFailed</li>
	 * <li>Send a Server=>Client packet MyTargetSelected to start attack and Notify AI with AI_INTENTION_ATTACK</li>
	 * </ul>
	 * @param player The Player to attack
	 */
	public override void onForcedAttack(Player player)
	{
		if (isInsidePeaceZone(player))
		{
			// If Creature or target is in a peace zone, send a system message TARGET_IN_PEACEZONE a Server=>Client packet ActionFailed
			player.sendPacket(SystemMessageId.YOU_CANNOT_ATTACK_THIS_TARGET_IN_A_PEACEFUL_ZONE);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}
		if (player.isInOlympiadMode() && player.getTarget() != null && player.getTarget().isPlayable())
		{
			Player target = null;
			WorldObject obj = player.getTarget();
			if (obj != null && obj.isPlayable())
			{
				target = obj.getActingPlayer();
			}

			if (target == null || (target.isInOlympiadMode() && (!player.isOlympiadStart() || player.getOlympiadGameId() != target.getOlympiadGameId())))
			{
				// if Player is in Olympia and the match isn't already start, send a Server=>Client packet ActionFailed
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return;
			}
		}
		if (player.getTarget() != null && !player.getTarget().canBeAttacked() && !player.getAccessLevel().allowPeaceAttack())
		{
			// If target is not attackable, send a Server=>Client packet ActionFailed
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}
		if (player.isConfused())
		{
			// If target is confused, send a Server=>Client packet ActionFailed
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}

		// GeoData Los Check or dz > 1000
		// if (!GeoEngine.getInstance().canSeeTarget(player, this))
		// {
		// player.sendPacket(SystemMessageId.CANNOT_SEE_TARGET);
		// player.sendPacket(ActionFailed.STATIC_PACKET);
		// return;
		// }

		// Notify AI with AI_INTENTION_ATTACK
		player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, this);
	}

	/**
	 * @param attacker
	 * @return True if inside peace zone.
	 */
	public bool isInsidePeaceZone(WorldObject attacker)
	{
		return isInsidePeaceZone(attacker, this);
	}

	public bool isInsidePeaceZone(WorldObject attacker, WorldObject target)
	{
		Instance instanceWorld = getInstanceWorld();
		if (target == null || !((target.isPlayable() || target.isFakePlayer()) && attacker.isPlayable()) || (instanceWorld != null && instanceWorld.isPvP()))
		{
			return false;
		}

		if (Config.ALT_GAME_KARMA_PLAYER_CAN_BE_KILLED_IN_PEACEZONE)
		{
			// allows red to be attacked and red to attack flagged players
			if (target.getActingPlayer() != null && target.getActingPlayer().getReputation() < 0)
			{
				return false;
			}
			if (attacker.getActingPlayer() != null && attacker.getActingPlayer().getReputation() < 0 && target.getActingPlayer() != null && target.getActingPlayer().getPvpFlag() != PvpFlagStatus.None)
			{
				return false;
			}
		}

		if (attacker.getActingPlayer() != null && attacker.getActingPlayer().getAccessLevel().allowPeaceAttack())
		{
			return false;
		}

		return target.isInsideZone(ZoneId.PEACE) || attacker.isInsideZone(ZoneId.PEACE) || target.isInsideZone(ZoneId.NO_PVP) || attacker.isInsideZone(ZoneId.NO_PVP);
	}

	/**
	 * @return true if this character is inside an active grid.
	 */
	public bool isInActiveRegion()
	{
		WorldRegion region = getWorldRegion();
		return region != null && region.isActive();
	}

	/**
	 * @return True if the Creature has a Party in progress.
	 */
	public virtual bool isInParty()
	{
		return false;
	}

	/**
	 * @return the Party object of the Creature.
	 */
	public virtual Party getParty()
	{
		return null;
	}

	/**
	 * Add a skill to the Creature _skills and its Func objects to the calculator set of the Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * All skills own by a Creature are identified in <b>_skills</b><br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Replace oldSkill by newSkill or Add the newSkill</li>
	 * <li>If an old skill has been replaced, remove all its Func objects of Creature calculator set</li>
	 * <li>Add Func objects of newSkill to the calculator set of the Creature</li>
	 * </ul>
	 * <br>
	 * <b><u>Overridden in</u>:</b>
	 * <ul>
	 * <li>Player : Save update in the character_skills table of the database</li>
	 * </ul>
	 * @param skill The Skill to add to the Creature
	 * @return The Skill replaced or null if just added a new Skill
	 */
	public virtual Skill addSkill(Skill skill)
	{
		Skill oldSkill = null;
		Skill newSkill = skill;
		if (newSkill != null)
		{
			// Mobius: Keep sublevel on skill level increase.
			Skill existingSkill = _skills.get(newSkill.getId());
			if (existingSkill != null && existingSkill.getSubLevel() > 0 && newSkill.getSubLevel() == 0 && existingSkill.getLevel() < newSkill.getLevel())
			{
				newSkill = SkillData.getInstance().getSkill(newSkill.getId(), newSkill.getLevel(), existingSkill.getSubLevel());
			}

			// Replace oldSkill by newSkill or Add the newSkill
			oldSkill = _skills.put(newSkill.getId(), newSkill);
			// If an old skill has been replaced, remove all its Func objects
			if (oldSkill != null)
			{
				// Stop all effects of that skill
				if (oldSkill.isPassive())
				{
					_effectList.stopSkillEffects(SkillFinishType.REMOVED, oldSkill);
				}

				_stat.recalculateStats(true);
			}

			if (newSkill.isPassive())
			{
				newSkill.applyEffects(this, this, false, true, false, TimeSpan.Zero, null);
			}
		}
		return oldSkill;
	}

	public virtual Skill? removeSkill(Skill skill, bool cancelEffect)
	{
		return skill != null ? removeSkill(skill.getId(), cancelEffect) : null;
	}

	public Skill removeSkill(int skillId)
	{
		return removeSkill(skillId, true);
	}

	public Skill removeSkill(int skillId, bool cancelEffect)
	{
		// Remove the skill from the Creature _skills
		Skill oldSkill = _skills.remove(skillId);
		// Remove all its Func objects from the Creature calculator set
		if (oldSkill != null)
		{
			// Stop casting if this skill is used right now
			abortCast(s => s.getSkill().getId() == skillId);

			// Stop effects.
			if (cancelEffect || oldSkill.isToggle() || oldSkill.isPassive())
			{
				stopSkillEffects(SkillFinishType.REMOVED, oldSkill.getId());
				_stat.recalculateStats(true);
			}
		}
		return oldSkill;
	}

	/**
	 * @return all skills this creature currently has.
	 */
	public ICollection<Skill> getAllSkills()
	{
		return _skills.Values;
	}

	public void removeAllSkills()
	{
		if (isPlayer())
		{
			Player player = getActingPlayer();
			while (_skills.Count != 0)
			{
				player.removeSkill(_skills.First().Value);
			}
		}
		else
		{
			while (_skills.Count != 0)
			{
				removeSkill(_skills.First().Value, true);
			}
		}
	}

	/**
	 * @return the map containing this character skills.
	 */
	public Map<int, Skill> getSkills()
	{
		return _skills;
	}

	/**
	 * Return the level of a skill owned by the Creature.
	 * @param skillId The identifier of the Skill whose level must be returned
	 * @return The level of the Skill identified by skillId
	 */
	public virtual int getSkillLevel(int skillId)
	{
		Skill skill = getKnownSkill(skillId);
		return skill == null ? 0 : skill.getLevel();
	}

	/**
	 * @param skillId The identifier of the Skill to check the knowledge
	 * @return the skill from the known skill.
	 */
	public virtual Skill? getKnownSkill(int skillId)
	{
		return _skills.get(skillId);
	}

	/**
	 * Return the number of buffs affecting this Creature.
	 * @return The number of Buffs affecting this Creature
	 */
	public int getBuffCount()
	{
		return _effectList.getBuffCount();
	}

	public int getDanceCount()
	{
		return _effectList.getDanceCount();
	}

	// Quest event ON_SPELL_FNISHED
	public virtual void notifyQuestEventSkillFinished(Skill skill, WorldObject target)
	{
	}

	/**
	 * @return the Level Modifier ((level + 89) / 100).
	 */
	public double getLevelMod()
	{
		// Untested: (lvl + 89 + unk5,5forSkill4.0Else * odyssey_lvl_mod) / 100; odyssey_lvl_mod = (lvl-99) min 0.
		double defaultLevelMod = (getLevel() + 89) / 100d;
		if (_transform is not null && _transform.isStance())
		{
			return _transform.getLevelMod(this);
		}

		return defaultLevelMod;
	}

	private bool _disabledAI;

	/**
	 * Dummy value that gets overriden in Playable.
	 * @return 0
	 */
	public virtual PvpFlagStatus getPvpFlag()
	{
		return PvpFlagStatus.None;
	}

	public virtual void updatePvPFlag(PvpFlagStatus value)
	{
		// Overridden in Player
	}

	/**
	 * @return a multiplier based on weapon random damage
	 */
	public virtual double getRandomDamageMultiplier()
	{
		int random = (int) _stat.getValue(Stat.RANDOM_DAMAGE);
		return 1 + (double) Rnd.get(-random, random) / 100;
	}

	public DateTime getAttackEndTime()
	{
		return _attackEndTime;
	}

	public DateTime getRangedAttackEndTime()
	{
		return _disableRangedAttackEndTime;
	}

	/**
	 * Not Implemented.
	 * @return
	 */
	public abstract int getLevel();

	public int getAccuracy()
	{
		return _stat.getAccuracy();
	}

	public virtual int getMagicAccuracy()
	{
		return _stat.getMagicAccuracy();
	}

	public int getMagicEvasionRate()
	{
		return _stat.getMagicEvasionRate();
	}

	public double getAttackSpeedMultiplier()
	{
		return _stat.getAttackSpeedMultiplier();
	}

	public double getCriticalDmg(int init)
	{
		return _stat.getCriticalDmg(init);
	}

	public virtual int getCriticalHit()
	{
		return _stat.getCriticalHit();
	}

	public int getEvasionRate()
	{
		return _stat.getEvasionRate();
	}

	public int getMagicalAttackRange(Skill skill)
	{
		return _stat.getMagicalAttackRange(skill);
	}

	public int getMaxCp()
	{
		return _stat.getMaxCp();
	}

	public int getMaxRecoverableCp()
	{
		return _stat.getMaxRecoverableCp();
	}

	public virtual int getMAtk()
	{
		return _stat.getMAtk();
	}

	public int getMAtkSpd()
	{
		return _stat.getMAtkSpd();
	}

	public int getMaxMp()
	{
		return _stat.getMaxMp();
	}

	public int getMaxRecoverableMp()
	{
		return _stat.getMaxRecoverableMp();
	}

	public int getMaxHp()
	{
		return _stat.getMaxHp();
	}

	public int getMaxRecoverableHp()
	{
		return _stat.getMaxRecoverableHp();
	}

	public int getMCriticalHit()
	{
		return _stat.getMCriticalHit();
	}

	public virtual int getMDef()
	{
		return _stat.getMDef();
	}

	public virtual int getPAtk()
	{
		return _stat.getPAtk();
	}

	public int getPAtkSpd()
	{
		return _stat.getPAtkSpd();
	}

	public int getPDef()
	{
		return _stat.getPDef();
	}

	public int getPhysicalAttackRange()
	{
		return _stat.getPhysicalAttackRange();
	}

	public virtual double getMovementSpeedMultiplier()
	{
		return _stat.getMovementSpeedMultiplier();
	}

	public virtual double getRunSpeed()
	{
		return _stat.getRunSpeed();
	}

	public virtual double getWalkSpeed()
	{
		return _stat.getWalkSpeed();
	}

	public double getSwimRunSpeed()
	{
		return _stat.getSwimRunSpeed();
	}

	public double getSwimWalkSpeed()
	{
		return _stat.getSwimWalkSpeed();
	}

	public virtual double getMoveSpeed()
	{
		return _stat.getMoveSpeed();
	}

	public int getShldDef()
	{
		return _stat.getShldDef();
	}

	public int getSTR()
	{
		return _stat.getSTR();
	}

	public int getDEX()
	{
		return _stat.getDEX();
	}

	public int getCON()
	{
		return _stat.getCON();
	}

	public int getINT()
	{
		return _stat.getINT();
	}

	public int getWIT()
	{
		return _stat.getWIT();
	}

	public int getMEN()
	{
		return _stat.getMEN();
	}

	// Status - NEED TO REMOVE ONCE CREATURESTATUS IS COMPLETE
	public void addStatusListener(Creature obj)
	{
		_status.addStatusListener(obj);
	}

	public virtual void doAttack(double damageValue, Creature target, Skill skill, bool isDOT, bool directlyToHp, bool critical, bool reflect)
	{
		// Check if fake players should aggro each other.
		if (isFakePlayer() && !Config.FAKE_PLAYER_AGGRO_FPC && target.isFakePlayer())
		{
			return;
		}

		// Start attack stance and notify being attacked.
		if (target.hasAI())
		{
			target.getAI().clientStartAutoAttack();
			target.getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, this);
		}
		getAI().clientStartAutoAttack();

		// ImmobileDamageBonus and ImmobileDamageResist effect bonuses.
		double damage = damageValue;
		if (target.isImmobilized())
		{
			damage *= _stat.getMul(Stat.IMMOBILE_DAMAGE_BONUS, 1);
			damage *= Math.Max(0.22, target.getStat().getMul(Stat.IMMOBILE_DAMAGE_RESIST, 1));
		}

		if (!reflect && !isDOT)
		{
			// RearDamage effect bonus.
			if (this.IsBehindOf(target))
			{
				damage *= _stat.getMul(Stat.REAR_DAMAGE_RATE, 1);
			}

			// Counterattacks happen before damage received.
			if (!target.isDead() && skill != null)
			{
				Formulas.calcCounterAttack(this, target, skill, true);

				// Shield Deflect Magic: Reflect all damage on caster.
				if (skill.isMagic() && target.getStat().getValue(Stat.VENGEANCE_SKILL_MAGIC_DAMAGE, 0) > Rnd.get(100))
				{
					reduceCurrentHp(damage, target, skill, isDOT, directlyToHp, critical, true);
					return;
				}
			}
		}

		// Absorb HP from the damage inflicted
		bool isPvP = isPlayable() && (target.isPlayable() || target.isFakePlayer());
		if (!isPvP || Config.VAMPIRIC_ATTACK_AFFECTS_PVP)
		{
			if (skill == null || Config.VAMPIRIC_ATTACK_WORKS_WITH_SKILLS)
			{
				double absorbHpPercent = getStat().getValue(Stat.ABSORB_DAMAGE_PERCENT, 0);
				if (absorbHpPercent > 0 && Rnd.nextDouble() < _stat.getValue(Stat.ABSORB_DAMAGE_CHANCE))
				{
					double absorbDamage = Math.Min(absorbHpPercent * damage, _stat.getMaxRecoverableHp() - _status.getCurrentHp());
					absorbDamage = Math.Min(absorbDamage, (int) target.getCurrentHp());
					absorbDamage *= target.getStat().getValue(Stat.ABSORB_DAMAGE_DEFENCE, 1);
					if (absorbDamage > 0)
					{
						setCurrentHp(_status.getCurrentHp() + absorbDamage);
					}
				}
			}
		}

		// Absorb MP from the damage inflicted.
		if (!isPvP || Config.MP_VAMPIRIC_ATTACK_AFFECTS_PVP)
		{
			if (skill != null || Config.MP_VAMPIRIC_ATTACK_WORKS_WITH_MELEE)
			{
				double absorbMpPercent = _stat.getValue(Stat.ABSORB_MANA_DAMAGE_PERCENT, 0);
				if (absorbMpPercent > 0 && Rnd.nextDouble() < _stat.getValue(Stat.ABSORB_MANA_DAMAGE_CHANCE))
				{
					int absorbDamage = (int) Math.Min(absorbMpPercent * damage, _stat.getMaxRecoverableMp() - _status.getCurrentMp());
					absorbDamage = Math.Min(absorbDamage, (int) target.getCurrentMp());
					if (absorbDamage > 0)
					{
						setCurrentMp(_status.getCurrentMp() + absorbDamage);
					}
				}
			}
		}

		// Target receives the damage.
		target.reduceCurrentHp(damage, this, skill, isDOT, directlyToHp, critical, reflect);

		// Check if damage should be reflected or absorbed. When killing blow is made, the target doesn't reflect (vamp too?).
		if (!reflect && !isDOT && !target.isDead())
		{
			int reflectedDamage = 0;

			// Reduce HP of the target and calculate reflection damage to reduce HP of attacker if necessary
			double reflectPercent = Math.Min(target.getStat().getValue(Stat.REFLECT_DAMAGE_PERCENT, 0) - getStat().getValue(Stat.REFLECT_DAMAGE_PERCENT_DEFENSE, 0), target.isPlayer() ? Config.PLAYER_REFLECT_PERCENT_LIMIT : Config.NON_PLAYER_REFLECT_PERCENT_LIMIT);
			if (reflectPercent > 0)
			{
				reflectedDamage = (int) (reflectPercent / 100.0 * damage);
				reflectedDamage = Math.Min(reflectedDamage, target.getMaxHp());

				// Reflected damage is limited by P.Def/M.Def
				if (skill != null && skill.isMagic())
				{
					reflectedDamage = (int) Math.Min(reflectedDamage, target.getStat().getMDef() * 1.5);
				}
				else
				{
					reflectedDamage = Math.Min(reflectedDamage, target.getStat().getPDef());
				}
			}

			if (reflectedDamage > 0)
			{
				target.doAttack(reflectedDamage, this, skill, isDOT, directlyToHp, critical, true);
			}
		}

		// Break casting of target during attack.
		if (!target.isRaid() && Formulas.calcAtkBreak(target, damage))
		{
			target.breakAttack();
			target.breakCast();
		}
	}

	public virtual void reduceCurrentHp(double amount, Creature attacker, Skill? skill)
	{
		reduceCurrentHp(amount, attacker, skill, false, false, false, false);
	}

	public virtual void reduceCurrentHp(double amountValue, Creature attacker, Skill? skill, bool isDOT, bool directlyToHp, bool critical, bool reflect)
	{
		double amount = amountValue;

		// Notify of this attack only if there is an attacking creature.
		if (attacker != null)
		{
			_onCreatureDamageDealt ??= new OnCreatureDamageDealt();
			_onCreatureDamageDealt.setAttacker(attacker);
			_onCreatureDamageDealt.setTarget(this);
			_onCreatureDamageDealt.setDamage(amount);
			_onCreatureDamageDealt.setSkill(skill);
			_onCreatureDamageDealt.setCritical(critical);
			_onCreatureDamageDealt.setDamageOverTime(isDOT);
			_onCreatureDamageDealt.setReflect(reflect);
			_onCreatureDamageDealt.Abort = false;
			attacker._eventContainer.Notify(_onCreatureDamageDealt);
		}

		_onCreatureDamageReceived ??= new OnCreatureDamageReceived();
		_onCreatureDamageReceived.setAttacker(attacker);
		_onCreatureDamageReceived.setTarget(this);
		_onCreatureDamageReceived.setDamage(amount);
		_onCreatureDamageReceived.setSkill(skill);
		_onCreatureDamageReceived.setCritical(critical);
		_onCreatureDamageReceived.setDamageOverTime(isDOT);
		_onCreatureDamageReceived.setReflect(reflect);
		_onCreatureDamageReceived.Abort = false;
		if (_eventContainer.Notify(_onCreatureDamageReceived))
		{
			if (_onCreatureDamageReceived.Terminate)
				return;

			if (_onCreatureDamageReceived.OverrideDamage)
				amount = _onCreatureDamageReceived.OverridenDamage;
		}

		double elementalDamage = 0;
		bool elementalCrit = false;

		// Calculate PvP/PvE damage received. It is a post-attack stat.
		if (attacker != null)
		{
			if (attacker.isPlayable())
			{
				amount *= (100 + Math.Max(_stat.getValue(Stat.PVP_DAMAGE_TAKEN), -80)) / 100;
			}
			else
			{
				amount *= (100 + Math.Max(_stat.getValue(Stat.PVE_DAMAGE_TAKEN), -80)) / 100;
			}

			if (attacker.isRaid() || attacker.isRaidMinion())
			{
				amount *= (100 + Math.Max(_stat.getValue(Stat.PVE_DAMAGE_TAKEN_RAID), -80)) / 100;
			}
			else if (attacker.isMonster())
			{
				amount *= (100 + Math.Max(_stat.getValue(Stat.PVE_DAMAGE_TAKEN_MONSTER), -80)) / 100;
			}

			if (!reflect)
			{
				elementalCrit = Formulas.calcSpiritElementalCrit(attacker, this);
				elementalDamage = Formulas.calcSpiritElementalDamage(attacker, this, amount, elementalCrit);
				amount += elementalDamage;
			}
		}

		double damageCap = _stat.getValue(Stat.DAMAGE_LIMIT);
		if (damageCap > 0)
		{
			amount = Math.Min(amount, damageCap);
		}

		if (Config.CHAMPION_ENABLE && isChampion() && Config.CHAMPION_HP != 0)
		{
			_status.reduceHp(amount / Config.CHAMPION_HP, attacker, skill == null || !skill.isToggle(), isDOT, false);
		}
		else if (isPlayer())
		{
			getActingPlayer().getStatus().reduceHp(amount, attacker, skill, skill == null || !skill.isToggle(), isDOT, false, directlyToHp);
		}
		else
		{
			_status.reduceHp(amount, attacker, skill == null || !skill.isToggle(), isDOT, false);
		}

		if (attacker != null)
		{
			attacker.sendDamageMessage(this, skill, (int) amount, elementalDamage, critical, false, elementalCrit);
		}

		if (isMonster() && attacker is Playable)
		{
			ElementalSpirit[] playerSpirits = attacker.getActingPlayer().getSpirits();
			if (playerSpirits != null)
			{
				ElementalType monsterElementalType = getElementalSpiritType();
				if (monsterElementalType != ElementalType.NONE && attacker.getActingPlayer().getActiveElementalSpiritType() != monsterElementalType)
				{
					attacker.getActingPlayer().changeElementalSpirit(monsterElementalType.superior());
				}
			}
		}
	}

	public void reduceCurrentMp(double amount)
	{
		_status.reduceMp(amount);
	}

	public override void removeStatusListener(Creature @object)
	{
		_status.removeStatusListener(@object);
	}

	protected void stopHpMpRegeneration()
	{
		_status.stopHpMpRegeneration();
	}

	public double getCurrentCp()
	{
		return _status.getCurrentCp();
	}

	public int getCurrentCpPercent()
	{
		return (int) (_status.getCurrentCp() * 100 / _stat.getMaxCp());
	}

	public void setCurrentCp(double newCp)
	{
		_status.setCurrentCp(newCp);
	}

	public void setCurrentCp(double newCp, bool broadcast)
	{
		_status.setCurrentCp(newCp, broadcast);
	}

	public double getCurrentHp()
	{
		return _status.getCurrentHp();
	}

	public int getCurrentHpPercent()
	{
		return (int) (_status.getCurrentHp() * 100 / _stat.getMaxHp());
	}

	public void setCurrentHp(double newHp)
	{
		_status.setCurrentHp(newHp);
	}

	public void setCurrentHp(double newHp, bool broadcast)
	{
		_status.setCurrentHp(newHp, broadcast);
	}

	public void setCurrentHpMp(double newHp, double newMp)
	{
		_status.setCurrentHpMp(newHp, newMp);
	}

	public double getCurrentMp()
	{
		return _status.getCurrentMp();
	}

	public int getCurrentMpPercent()
	{
		return (int) (_status.getCurrentMp() * 100 / _stat.getMaxMp());
	}

	public void setCurrentMp(double newMp)
	{
		_status.setCurrentMp(newMp);
	}

	public void setCurrentMp(double newMp, bool broadcast)
	{
		_status.setCurrentMp(newMp, false);
	}

	/**
	 * @return the max weight that the Creature can load.
	 */
	public int getMaxLoad()
	{
		if (isPlayer() || isPet())
		{
			// Weight Limit = (CON Modifier*69000) * Skills
			// Source http://l2p.bravehost.com/weightlimit.html (May 2007)
			double baseLoad = Math.Floor(BaseStat.CON.calcBonus(this) * 69000 * Config.ALT_WEIGHT_LIMIT);
			return (int) _stat.getValue(Stat.WEIGHT_LIMIT, baseLoad);
		}
		return 0;
	}

	public int getBonusWeightPenalty()
	{
		if (isPlayer() || isPet())
		{
			return (int) _stat.getValue(Stat.WEIGHT_PENALTY, 1);
		}
		return 0;
	}

	/**
	 * @return the current weight of the Creature.
	 */
	public int getCurrentLoad()
	{
		if (isPlayer() || isPet())
		{
			return getInventory().getTotalWeight();
		}
		return 0;
	}

	public virtual bool isChampion()
	{
		return false;
	}

	/**
	 * Send system message about damage.
	 * @param target
	 * @param skill
	 * @param damage
	 * @param elementalDamage
	 * @param crit
	 * @param miss
	 * @param elementalCrit
	 */
	public virtual void sendDamageMessage(Creature target, Skill skill, int damage, double elementalDamage, bool crit, bool miss, bool elementalCrit)
	{
	}

	public virtual AttributeType getAttackElement()
	{
		return _stat.getAttackElement();
	}

	public virtual int getAttackElementValue(AttributeType attackAttribute)
	{
		return _stat.getAttackElementValue(attackAttribute);
	}

	public virtual int getDefenseElementValue(AttributeType defenseAttribute)
	{
		return _stat.getDefenseElementValue(defenseAttribute);
	}

	public void startPhysicalAttackMuted()
	{
		abortAttack();
	}

	public void disableCoreAI(bool value)
	{
		_disabledAI = value;
	}

	public bool isCoreAIDisabled()
	{
		return _disabledAI;
	}

	/**
	 * @return true
	 */
	public virtual bool giveRaidCurse()
	{
		return false;
	}

	/**
	 * Check if target is affected with special buff
	 * @param flag int
	 * @return bool
	 * @see EffectList#isAffected(EffectFlag)
	 */
	public bool isAffected(EffectFlag flag)
	{
		return _effectList.isAffected(flag);
	}

	public virtual Team getTeam()
	{
		return _team;
	}

	public virtual void setTeam(Team team)
	{
		_team = team;
	}

	public virtual void addOverrideCond(params PlayerCondOverride[] excs)
	{
		foreach (PlayerCondOverride exc in excs)
		{
			_exceptions |= exc.getMask();
		}
	}

	public virtual void removeOverridedCond(params PlayerCondOverride[] excs)
	{
		foreach (PlayerCondOverride exc in excs)
		{
			_exceptions &= ~exc.getMask();
		}
	}

	public bool canOverrideCond(PlayerCondOverride excs)
	{
		return (_exceptions & excs.getMask()) == excs.getMask();
	}

	public void setOverrideCond(long masks)
	{
		_exceptions = masks;
	}

	public void setLethalable(bool value)
	{
		_lethalable = value;
	}

	public bool isLethalable()
	{
		return _lethalable;
	}

	public bool hasTriggerSkills()
	{
		return _triggerSkills != null && _triggerSkills.Count != 0;
	}

	public Map<int, OptionSkillHolder> getTriggerSkills()
	{
		if (_triggerSkills == null)
		{
			lock (this)
			{
				if (_triggerSkills == null)
				{
					_triggerSkills = new();
				}
			}
		}
		return _triggerSkills;
	}

	public void addTriggerSkill(OptionSkillHolder holder)
	{
		getTriggerSkills().put(holder.getSkill().getId(), holder);
	}

	public void removeTriggerSkill(OptionSkillHolder holder)
	{
		getTriggerSkills().remove(holder.getSkill().getId());
	}

	/**
	 * Dummy method overriden in {@link Player}
	 * @return {@code true} if current player can revive and shows 'To Village' button upon death, {@code false} otherwise.
	 */
	public virtual bool canRevive()
	{
		return true;
	}

	/**
	 * Dummy method overriden in {@link Player}
	 * @param value
	 */
	public virtual void setCanRevive(bool value)
	{
	}

	/**
	 * Dummy method overriden in {@link Attackable}
	 * @return {@code true} if there is a loot to sweep, {@code false} otherwise.
	 */
	public virtual bool isSweepActive()
	{
		return false;
	}

	/**
	 * Dummy method overriden in {@link Player}
	 * @return the clan id of current character.
	 */
	public virtual int? getClanId()
	{
		return null;
	}

	/**
	 * Dummy method overriden in {@link Player}
	 * @return the clan of current character.
	 */
	public virtual Clan getClan()
	{
		return null;
	}

	/**
	 * Dummy method overriden in {@link Player}
	 * @return {@code true} if player is in academy, {@code false} otherwise.
	 */
	public virtual bool isAcademyMember()
	{
		return false;
	}

	/**
	 * Dummy method overriden in {@link Player}
	 * @return the pledge type of current character.
	 */
	public virtual int getPledgeType()
	{
		return 0;
	}

	/**
	 * Dummy method overriden in {@link Player}
	 * @return the alliance id of current character.
	 */
	public virtual int? getAllyId()
	{
		return null;
	}

	/**
	 * Notifies to listeners that current character avoid attack.
	 * @param target
	 * @param isDot
	 */
	public void notifyAttackAvoid(Creature target, bool isDot)
	{
		if (target.Events.HasSubscribers<OnCreatureAttackAvoid>())
		{
			_onCreatureAttackAvoid ??= new OnCreatureAttackAvoid();
			_onCreatureAttackAvoid.setAttacker(this);
			_onCreatureAttackAvoid.setTarget(target);
			_onCreatureAttackAvoid.setDamageOverTime(isDot);
			_onCreatureAttackAvoid.Abort = false;
			target.Events.Notify(_onCreatureAttackAvoid);
		}
	}

	/**
	 * @return {@link WeaponType} of current character's weapon or basic weapon type.
	 */
	public WeaponType getAttackType()
	{
		Weapon weapon = getActiveWeaponItem();
		if (weapon != null)
		{
			return weapon.getItemType().AsWeaponType();
		}

		WeaponType defaultWeaponType = _template.getBaseAttackType();
		if (_transform is null)
			return defaultWeaponType;

		return _transform.getBaseAttackType(this, defaultWeaponType);
	}

	public bool isInCategory(CategoryType type)
	{
		return CategoryData.getInstance().isInCategory(type, getId());
	}

	public bool isInOneOfCategory(params CategoryType[] types)
	{
		foreach (CategoryType type in types)
		{
			if (CategoryData.getInstance().isInCategory(type, getId()))
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * @return the character that summoned this NPC.
	 */
	public Creature getSummoner()
	{
		return _summoner;
	}

	/**
	 * @param summoner the summoner of this NPC.
	 */
	public void setSummoner(Creature summoner)
	{
		_summoner = summoner;
	}

	/**
	 * Adds a summoned NPC.
	 * @param npc the summoned NPC
	 */
	public void addSummonedNpc(Npc npc)
	{
		if (_summonedNpcs == null)
		{
			lock (this)
			{
				if (_summonedNpcs == null)
				{
					_summonedNpcs = new();
				}
			}
		}

		_summonedNpcs.put(npc.ObjectId, npc);

		npc.setSummoner(this);
	}

	/**
	 * Removes a summoned NPC by object ID.
	 * @param objectId the summoned NPC object ID
	 */
	public void removeSummonedNpc(int objectId)
	{
		if (_summonedNpcs != null)
		{
			_summonedNpcs.remove(objectId);
		}
	}

	/**
	 * Gets the summoned NPCs.
	 * @return the summoned NPCs
	 */
	public ICollection<Npc> getSummonedNpcs()
	{
		return _summonedNpcs != null ? _summonedNpcs.Values : new List<Npc>();
	}

	/**
	 * Gets the summoned NPC by object ID.
	 * @param objectId the summoned NPC object ID
	 * @return the summoned NPC
	 */
	public Npc getSummonedNpc(int objectId)
	{
		if (_summonedNpcs != null)
		{
			return _summonedNpcs.get(objectId);
		}
		return null;
	}

	/**
	 * Gets the summoned NPC count.
	 * @return the summoned NPC count
	 */
	public int getSummonedNpcCount()
	{
		return _summonedNpcs != null ? _summonedNpcs.Count : 0;
	}

	/**
	 * Resets the summoned NPCs list.
	 */
	public void resetSummonedNpcs()
	{
		if (_summonedNpcs != null)
		{
			_summonedNpcs.Clear();
		}
	}

	public override bool isCreature()
	{
		return true;
	}

	public virtual int getMinShopDistance()
	{
		return 0;
	}

	public ICollection<SkillCaster> getSkillCasters()
	{
		return _skillCasters.Values;
	}

	public SkillCaster addSkillCaster(SkillCastingType castingType, SkillCaster skillCaster)
	{
		return _skillCasters.put(castingType, skillCaster);
	}

	public SkillCaster removeSkillCaster(SkillCastingType castingType)
	{
		return _skillCasters.remove(castingType);
	}

	public List<SkillCaster> getSkillCasters(Predicate<SkillCaster> filterValue, params Predicate<SkillCaster>[] filters)
	{
		Predicate<SkillCaster> filter = s => filterValue(s) && filters.All(f => f(s));

		List<SkillCaster> result = new();
		foreach (SkillCaster skillCaster in _skillCasters.Values)
		{
			if (filter(skillCaster))
			{
				result.Add(skillCaster);
			}
		}
		return result;
	}

	public SkillCaster getSkillCaster(Predicate<SkillCaster> filterValue, params Predicate<SkillCaster>[] filters)
	{
		Predicate<SkillCaster> filter = s => filterValue(s) && filters.All(f => f(s));

		foreach (SkillCaster skillCaster in _skillCasters.Values)
		{
			if (filter(skillCaster))
			{
				return skillCaster;
			}
		}
		return null;
	}

	/**
	 * @return {@code true} if current character is casting channeling skill, {@code false} otherwise.
	 */
	public bool isChanneling()
	{
		return _channelizer != null && _channelizer.isChanneling();
	}

	public SkillChannelizer getSkillChannelizer()
	{
		if (_channelizer == null)
		{
			_channelizer = new SkillChannelizer(this);
		}
		return _channelizer;
	}

	/**
	 * @return {@code true} if current character is affected by channeling skill, {@code false} otherwise.
	 */
	public bool isChannelized()
	{
		return _channelized != null && !_channelized.isChannelized();
	}

	public SkillChannelized getSkillChannelized()
	{
		if (_channelized == null)
		{
			_channelized = new SkillChannelized();
		}
		return _channelized;
	}

	public void addIgnoreSkillEffects(SkillHolder holder)
	{
		IgnoreSkillHolder? ignoreSkillHolder = getIgnoreSkillEffects().get(holder.getSkillId());
		if (ignoreSkillHolder != null)
		{
			ignoreSkillHolder.increaseInstances();
			return;
		}
		getIgnoreSkillEffects().put(holder.getSkillId(), new IgnoreSkillHolder(holder));
	}

	public void removeIgnoreSkillEffects(SkillHolder holder)
	{
		IgnoreSkillHolder ignoreSkillHolder = getIgnoreSkillEffects().get(holder.getSkillId());
		if (ignoreSkillHolder != null && ignoreSkillHolder.decreaseInstances() < 1)
		{
			getIgnoreSkillEffects().remove(holder.getSkillId());
		}
	}

	public bool isIgnoringSkillEffects(int skillId, int skillLevel)
	{
		if (_ignoreSkillEffects != null)
		{
			SkillHolder? holder = getIgnoreSkillEffects().get(skillId);
			return holder != null && (holder.getSkillLevel() < 1 || holder.getSkillLevel() == skillLevel);
		}
		return false;
	}

	private Map<int, IgnoreSkillHolder> getIgnoreSkillEffects()
	{
		if (_ignoreSkillEffects == null)
		{
			lock (this)
			{
				if (_ignoreSkillEffects == null)
				{
					_ignoreSkillEffects = new();
				}
			}
		}
		return _ignoreSkillEffects;
	}

	public virtual Race getRace()
	{
		return _template.getRace();
	}

	public override void setXYZ(int newX, int newY, int newZ)
	{
		// 0, 0 is not a valid location.
		if (newX == 0 && newY == 0)
		{
			return;
		}

		ZoneRegion? oldZoneRegion = ZoneManager.getInstance().getRegion(Location.Location2D);
		ZoneRegion? newZoneRegion = ZoneManager.getInstance().getRegion(new Location2D(newX, newY));

		// Mobius: Prevent moving to nonexistent regions.
		if (newZoneRegion == null)
		{
			return;
		}

		if (oldZoneRegion != newZoneRegion)
		{
			oldZoneRegion?.removeFromZones(this);
			newZoneRegion.revalidateZones(this);
		}

		base.setXYZ(newX, newY, newZ);
	}

	public Map<int, RelationCache> getKnownRelations()
	{
		return _knownRelations;
	}

	public override bool isTargetable()
	{
		return base.isTargetable() && !isAffected(EffectFlag.UNTARGETABLE);
	}

	public bool isTargetingDisabled()
	{
		return isAffected(EffectFlag.TARGETING_DISABLED);
	}

	public bool cannotEscape()
	{
		return isAffected(EffectFlag.CANNOT_ESCAPE);
	}

	/**
	 * Sets amount of debuffs that player can avoid
	 * @param times
	 */
	public void setAbnormalShieldBlocks(int times)
	{
		_abnormalShieldBlocks.set(times);
	}

	/**
	 * @return the amount of debuffs that player can avoid
	 */
	public int getAbnormalShieldBlocks()
	{
		return _abnormalShieldBlocks.get();
	}

	/**
	 * @return the amount of debuffs that player can avoid
	 */
	public int decrementAbnormalShieldBlocks()
	{
		return _abnormalShieldBlocks.decrementAndGet();
	}

	public bool hasAbnormalType(AbnormalType abnormalType)
	{
		return _effectList.hasAbnormalType(abnormalType);
	}

	public void addBlockActionsAllowedSkill(int skillId)
	{
		_blockActionsAllowedSkills.computeIfAbsent(skillId, k => new AtomicInteger()).incrementAndGet();
	}

	public void removeBlockActionsAllowedSkill(int skillId)
	{
		_blockActionsAllowedSkills.computeIfPresent(skillId, (k, v) => v.decrementAndGet() != 0 ? v : null);
	}

	public bool isBlockedActionsAllowedSkill(Skill skill)
	{
		return _blockActionsAllowedSkills.ContainsKey(skill.getId());
	}

	protected void initSeenCreatures()
	{
		if (_seenCreatures == null)
		{
			lock (this)
			{
				if (_seenCreatures == null)
				{
					if (isNpc())
					{
						NpcTemplate template = ((Npc) this).getTemplate();
						if (template != null && template.getAggroRange() > 0)
						{
							_seenCreatureRange = template.getAggroRange();
						}
					}

					_seenCreatures = new();
				}
			}
		}

		CreatureSeeTaskManager.getInstance().add(this);
	}

	public void updateSeenCreatures()
	{
		if (_seenCreatures == null || _isDead || !isSpawned())
		{
			return;
		}

		// Check if region and its neighbors are active.
		WorldRegion? region = getWorldRegion();
		if (region == null || !region.areNeighborsActive())
		{
			return;
		}

		World.getInstance().forEachVisibleObjectInRange<Creature>(this, _seenCreatureRange, creature =>
		{
			if (!creature.isInvisible() && _seenCreatures.add(creature))
			{
				_eventContainer.Notify(new OnCreatureSee(this, creature));
			}
		});
	}

	public void removeSeenCreature(WorldObject worldObject)
	{
		if (_seenCreatures == null)
		{
			return;
		}

		if (worldObject is Creature creature)
			_seenCreatures.remove(creature);
	}

	public virtual MoveType getMoveType()
	{
		if (isMoving() && _isRunning)
		{
			return MoveType.RUNNING;
		}
		else if (isMoving() && !_isRunning)
		{
			return MoveType.WALKING;
		}
		return MoveType.STANDING;
	}

	protected void computeStatusUpdate(StatusUpdatePacket su, StatusUpdateType type)
	{
		int newValue = type.getValue(this);
		_statusUpdates.compute(type, (key, oldValue) =>
		{
			if (oldValue == null || oldValue != newValue)
			{
				su.addUpdate(type, newValue);
				if (isPlayer())
				{
					if (type == StatusUpdateType.MAX_DP)
					{
						su.addUpdate(StatusUpdateType.CUR_DP, getActingPlayer().getDeathPoints());
					}
					else if (type == StatusUpdateType.MAX_BP)
					{
						su.addUpdate(StatusUpdateType.CUR_BP, getActingPlayer().getBeastPoints());
					}
					else if (type == StatusUpdateType.MAX_AP)
					{
						getActingPlayer().sendPacket(new ExMaxPacket()); // TODO: Investigate this.
						su.addUpdate(StatusUpdateType.CUR_AP, getActingPlayer().getAssassinationPoints());
					}
				}
				return newValue;
			}
			return oldValue;
		});
	}

	protected void addStatusUpdateValue(StatusUpdateType type)
	{
		_statusUpdates.put(type, type.getValue(this));
	}

	protected virtual void initStatusUpdateCache()
	{
		addStatusUpdateValue(StatusUpdateType.MAX_HP);
		addStatusUpdateValue(StatusUpdateType.MAX_MP);
		addStatusUpdateValue(StatusUpdateType.CUR_HP);
		addStatusUpdateValue(StatusUpdateType.CUR_MP);
	}

	/**
	 * Checks if the creature has basic property resist towards mesmerizing debuffs.
	 * @return {@code true}.
	 */
	public virtual bool hasBasicPropertyResist()
	{
		return true;
	}

	/**
	 * Gets the basic property resist.
	 * @param basicProperty the basic property
	 * @return the basic property resist
	 */
	public BasicPropertyResist getBasicPropertyResist(BasicProperty basicProperty)
	{
		if (_basicPropertyResists == null)
		{
			lock (this)
			{
				if (_basicPropertyResists == null)
				{
					_basicPropertyResists = new();
				}
			}
		}
		return _basicPropertyResists.computeIfAbsent(basicProperty, k => new BasicPropertyResist());
	}

	public virtual int getReputation()
	{
		return _reputation;
	}

	public virtual void setReputation(int reputation)
	{
		_reputation = reputation;
	}

	public bool isChargedShot(ShotType type)
	{
		return _chargedShots.Contains(type);
	}

	/**
	 * @param type of the shot to charge
	 * @return {@code true} if there was no shot of this type charged before, {@code false} otherwise.
	 */
	public bool chargeShot(ShotType type)
	{
		return _chargedShots.add(type);
	}

	/**
	 * @param type of the shot to uncharge
	 * @return {@code true} if there was a charged shot of this type, {@code false} otherwise.
	 */
	public bool unchargeShot(ShotType type)
	{
		return _chargedShots.remove(type);
	}

	public void unchargeAllShots()
	{
		_chargedShots = new();
	}

	public virtual void rechargeShots(bool physical, bool magic, bool fish)
	{
		// Dummy method to be overriden.
	}

	public void setCursorKeyMovement(bool value)
	{
		_cursorKeyMovement = value;
	}

	public bool isMovementSuspended()
	{
		return _suspendedMovement;
	}

	public List<Item> getFakePlayerDrops()
	{
		return _fakePlayerDrops;
	}

	public void addBuffInfoTime(BuffInfo info)
	{
		if (_buffFinishTask == null)
		{
			_buffFinishTask = new BuffFinishTask();
		}
		_buffFinishTask.addBuffInfo(info);
	}

	public void removeBuffInfoTime(BuffInfo info)
	{
		if (_buffFinishTask != null)
		{
			_buffFinishTask.removeBuffInfo(info);
		}
	}

	public void cancelBuffFinishTask()
	{
		if (_buffFinishTask != null)
		{
			ScheduledFuture task = _buffFinishTask.getTask();
			if (task != null && !task.isCancelled() && !task.isDone())
			{
				task.cancel(true);
			}

			_buffFinishTask = null;
		}
	}

	public virtual double getElementalSpiritDefenseOf(ElementalType type)
	{
		return getElementalSpiritType() == type ? 100 : 0;
	}

	public virtual double getElementalSpiritAttackOf(ElementalType type)
	{
		return getElementalSpiritType() == type ? 100 : 0;
	}

	public virtual ElementalType getElementalSpiritType()
	{
		return ElementalType.NONE;
	}
}