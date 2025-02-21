using System.Collections.Immutable;
using L2Dn.Events;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones.Conditions;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.InstanceZones;

/**
 * Template holder for instances.
 * @author malyelfik
 */
public class InstanceTemplate: IIdentifiable, INamable, IEventContainerProvider
{
	// Basic instance parameters
	private readonly EventContainer _eventContainer;
	private readonly int _templateId;
	private string _name;
	private TimeSpan _duration;
	private TimeSpan _emptyDestroyTime;
	private TimeSpan _ejectTime = TimeSpan.FromMinutes(Config.EJECT_DEAD_PLAYER_TIME);
	private readonly int _maxWorldCount;
	private bool _isPvP;
	private bool _allowPlayerSummon;
	private float _expRate = Config.RATE_INSTANCE_XP;
	private float _spRate = Config.RATE_INSTANCE_SP;
	private float _expPartyRate = Config.RATE_INSTANCE_PARTY_XP;
	private float _spPartyRate = Config.RATE_INSTANCE_PARTY_SP;
	private StatSet _parameters = StatSet.EMPTY_STATSET;
	private readonly Map<int, DoorTemplate> _doors = new();
	private readonly Map<int, bool> _doorStates = new();
	private readonly List<SpawnTemplate> _spawns = new();
	// Locations
	private InstanceTeleportType _enterLocationType = InstanceTeleportType.NONE;
	private ImmutableArray<Location> _enterLocations = ImmutableArray<Location>.Empty;
	private InstanceTeleportType _exitLocationType = InstanceTeleportType.NONE;
	private ImmutableArray<Location3D> _exitLocations = ImmutableArray<Location3D>.Empty;

	// Reenter data
	private InstanceReenterType _reenterType = InstanceReenterType.NONE;
	private List<InstanceReenterTimeHolder> _reenterData = new();

	// Buff remove data
	private InstanceRemoveBuffType _removeBuffType = InstanceRemoveBuffType.NONE;
	private List<int> _removeBuffExceptions = new();

	// Conditions
	private List<Condition> _conditions = new();
	private GroupType _groupMask = GroupType.Player;

	/**
	 * @param set
	 */
	public InstanceTemplate(int id, string name, int maxWorldCount)
	{
		_templateId = id;
		_name = name;
		_maxWorldCount = maxWorldCount;
		_eventContainer = new($"Instance template {_templateId}", GlobalEvents.Global);
	}

	public EventContainer Events => _eventContainer;

	// -------------------------------------------------------------
	// Setters
	// -------------------------------------------------------------

	/**
	 * Set name of instance world.
	 * @param name instance name
	 */
	public void setName(string name)
	{
		if (!string.IsNullOrEmpty(name))
			_name = name;
	}

	/**
	 * Set instance world duration.
	 * @param duration time in minutes
	 */
	public void setDuration(TimeSpan duration)
	{
		if (duration > TimeSpan.Zero)
		{
			_duration = duration;
		}
	}

	/**
	 * Set time after empty instance will be destroyed.
	 * @param emptyDestroyTime time in minutes
	 */
	public void setEmptyDestroyTime(TimeSpan emptyDestroyTime)
	{
		if (emptyDestroyTime >= TimeSpan.Zero)
		{
			_emptyDestroyTime = emptyDestroyTime;
		}
	}

	/**
	 * Set time after death player will be ejected from instance world.<br>
	 * Default: {@link Config#EJECT_DEAD_PLAYER_TIME}
	 * @param ejectTime time in minutes
	 */
	public void setEjectTime(TimeSpan ejectTime)
	{
		if (ejectTime >= TimeSpan.Zero)
		{
			_ejectTime = ejectTime;
		}
	}

	/**
	 * Allow summoning players (that are out of instance) to instance world by players inside.
	 * @param value {@code true} means summon is allowed, {@code false} means summon is prohibited
	 */
	public void allowPlayerSummon(bool value)
	{
		_allowPlayerSummon = value;
	}

	/**
	 * Set instance as PvP world.
	 * @param value {@code true} world is PvP zone, {@code false} world use classic zones
	 */
	public void setPvP(bool value)
	{
		_isPvP = value;
	}

	/**
	 * Set parameters shared between instances with same template id.
	 * @param set map containing parameters
	 */
	public void setParameters(Map<string, object> set)
	{
		if (set.Count != 0)
		{
			_parameters = new StatSet(set);
		}
	}

	/**
	 * Add door into instance world.
	 * @param templateId template id of door
	 * @param template door template
	 */
	public void addDoor(int templateId, DoorTemplate template)
	{
		_doors.put(templateId, template);
	}

	public void addDoorState(int templateId, bool isDefaultOpen)
	{
		_doorStates.put(templateId, isDefaultOpen);
	}

	/**
	 * Add new group of NPC spawns into instance world.<br>
	 * Group with name "general" will be spawned on instance world create.
	 * @param spawns list of NPC spawn data
	 */
	public void addSpawns(List<SpawnTemplate> spawns)
	{
		_spawns.AddRange(spawns);
	}

	/**
	 * Set enter locations for instance world.
	 * @param type type of teleport ({@link InstanceTeleportType#FIXED} or {@link InstanceTeleportType#RANDOM} are supported)
	 * @param locations list of locations used for determining enter location
	 */
	public void setEnterLocation(InstanceTeleportType type, ImmutableArray<Location> locations)
	{
		_enterLocationType = type;
		_enterLocations = locations;
	}

	/**
	 * Set exit locations for instance world.
	 * @param type type of teleport (see {@link InstanceTeleportType} for all possible types)
	 * @param locations list of locations used for determining exit location
	 */
	public void setExitLocation(InstanceTeleportType type, ImmutableArray<Location3D> locations)
	{
		_exitLocationType = type;
		_exitLocations = locations;
	}

	/**
	 * Set re-enter data for instance world.<br>
	 * This method also enable re-enter condition for instance world.
	 * @param type reenter type means when reenter restriction should be applied (see {@link InstanceReenterType} for more info)
	 * @param holder data which are used to calculate reenter time
	 */
	public void setReenterData(InstanceReenterType type, List<InstanceReenterTimeHolder> holder)
	{
		_reenterType = type;
		_reenterData = holder;
	}

	/**
	 * Set remove buff list for instance world.<br>
	 * These data are used to restrict player buffs when he enters into instance.
	 * @param type type of list like blacklist, whitelist, ... (see {@link InstanceRemoveBuffType} for more info)
	 * @param exceptionList
	 */
	public void setRemoveBuff(InstanceRemoveBuffType type, List<int> exceptionList)
	{
		_removeBuffType = type;
		_removeBuffExceptions = exceptionList;
	}

	/**
	 * Register conditions to instance world.<br>
	 * This method also set new enter group mask according to given conditions.
	 * @param conditions list of conditions
	 */
	public void setConditions(List<Condition> conditions)
	{
		// Set conditions
		_conditions = conditions;

		// Now iterate over conditions and determine enter group data
		bool onlyCC = false;
		int min = 1;
		int max = 1;
		foreach (Condition cond in _conditions)
		{
			if (cond is ConditionCommandChannel)
			{
				onlyCC = true;
			}
			else if (cond is ConditionGroupMin)
			{
				min = ((ConditionGroupMin) cond).getLimit();
			}
			else if (cond is ConditionGroupMax)
			{
				max = ((ConditionGroupMax) cond).getLimit();
			}
		}

		// Reset group mask before setting new group
		_groupMask = 0;
		// Check if player can enter in other group then Command channel
		if (!onlyCC)
		{
			// Player
			if (min == 1)
			{
				_groupMask |= GroupType.Player;
			}
			// Party
			int partySize = Config.ALT_PARTY_MAX_MEMBERS;
			if ((max > 1 && max <= partySize) || (min <= partySize && max > partySize))
			{
				_groupMask |= GroupType.PARTY;
			}
		}
		// Command channel
		if (onlyCC || max > 7)
		{
			_groupMask |= GroupType.COMMAND_CHANNEL;
		}
	}

	// -------------------------------------------------------------
	// Getters
	// -------------------------------------------------------------
	public int getId()
	{
		return _templateId;
	}

	public string getName()
	{
		return _name;
	}

	/**
	 * Get all enter locations defined in XML template.
	 * @return list of enter locations
	 */
	public ImmutableArray<Location> getEnterLocations()
	{
		return _enterLocations;
	}

	/**
	 * Get enter location to instance world.
	 * @return enter location if instance has any, otherwise {@code null}
	 */
	public Location? getEnterLocation()
	{
		Location? loc = null;
		if (_enterLocations.Length != 0)
		{
			switch (_enterLocationType)
			{
				case InstanceTeleportType.RANDOM:
				{
					loc = _enterLocations[Rnd.get(_enterLocations.Length)];
					break;
				}
				case InstanceTeleportType.FIXED:
				{
					loc = _enterLocations[0];
					break;
				}
			}
		}

		return loc;
	}

	/**
	 * Get type of exit location.
	 * @return exit location type (see {@link InstanceTeleportType} for possible values)
	 */
	public InstanceTeleportType getExitLocationType()
	{
		return _exitLocationType;
	}

	/**
	 * Get exit location from instance world.
	 * @param player player who wants to leave instance
	 * @return exit location if instance has any, otherwise {@code null}
	 */
	public Location3D? getExitLocation(Player player)
	{
		Location3D? location = null;
		switch (_exitLocationType)
		{
			case InstanceTeleportType.RANDOM:
			{
				if (_exitLocations.Length != 0)
					location = _exitLocations[Rnd.get(_exitLocations.Length)];

				break;
			}
			case InstanceTeleportType.FIXED:
			{
				if (_exitLocations.Length != 0)
					location = _exitLocations[0];

				break;
			}
			case InstanceTeleportType.ORIGIN:
			{
				PlayerVariables vars = player.getVariables();
				if (vars.Contains(PlayerVariables.INSTANCE_ORIGIN))
				{
					int[] loc = vars.getIntArray(PlayerVariables.INSTANCE_ORIGIN, ";");
					if (loc != null && loc.Length == 3)
					{
						location = new Location3D(loc[0], loc[1], loc[2]);
					}
					vars.remove(PlayerVariables.INSTANCE_ORIGIN);
				}
				break;
			}
			case InstanceTeleportType.TOWN:
			{
				if (player.getReputation() < 0)
				{
					location = MapRegionManager.getInstance().getNearestKarmaRespawn(player);
				}
				else
				{
					location = MapRegionManager.getInstance().getNearestTownRespawn(player);
				}
				break;
			}
		}
		return location;
	}

	/**
	 * Get time after empty instance is destroyed.
	 * @return time in milliseconds
	 */
	public TimeSpan getEmptyDestroyTime()
	{
		return _emptyDestroyTime;
	}

	/**
	 * Get instance duration time.
	 * @return time in minutes
	 */
	public TimeSpan getDuration()
	{
		return _duration;
	}

	/**
	 * Get time after dead player is ejected from instance world.
	 * @return time in minutes
	 */
	public TimeSpan getEjectTime()
	{
		return _ejectTime;
	}

	/**
	 * Check if summoning player into instance is allowed.
	 * @return {@code true} if summon is allowed, otherwise {@code false}
	 */
	public bool isPlayerSummonAllowed()
	{
		return _allowPlayerSummon;
	}

	/**
	 * Check if instance is PvP zone.
	 * @return {@code true} if instance is PvP, otherwise {@code false}
	 */
	public bool isPvP()
	{
		return _isPvP;
	}

	/**
	 * Get doors data for instance world.
	 * @return map in form <i>doorId, door template</i>
	 */
	public Map<int, DoorTemplate> getDoors()
	{
		return _doors;
	}

	public Map<int, bool> getDoorStates()
	{
		return _doorStates;
	}

	/**
	 * @return list of all spawn templates
	 */
	public List<SpawnTemplate> getSpawns()
	{
		return _spawns;
	}

	/**
	 * Get count of instance worlds which can run concurrently with same template ID.
	 * @return count of worlds
	 */
	public int getMaxWorlds()
	{
		return _maxWorldCount;
	}

	/**
	 * Get instance template parameters.
	 * @return parameters of template
	 */
	public StatSet getParameters()
	{
		return _parameters;
	}

	/**
	 * Check if buffs are removed upon instance enter.
	 * @return {@code true} if any buffs should be removed, otherwise {@code false}
	 */
	public bool isRemoveBuffEnabled()
	{
		return _removeBuffType != InstanceRemoveBuffType.NONE;
	}

	/**
	 * Remove buffs from player according to remove buff data
	 * @param player player which loose buffs
	 */
	public void removePlayerBuff(Player player)
	{
		// Make list of affected playable objects
		List<Playable> affected = new();
		affected.Add(player);
		player.getServitors().Values.ForEach(x => affected.Add(x));
        Pet? pet = player.getPet();
		if (player.hasPet() && pet != null)
		{
			affected.Add(pet);
		}

		// Now remove buffs by type
		if (_removeBuffType == InstanceRemoveBuffType.ALL)
		{
			foreach (Playable playable in affected)
			{
				playable.stopAllEffectsExceptThoseThatLastThroughDeath();
			}
		}
		else
		{
			foreach (Playable playable in affected)
			{
				// Stop all buffs.
				playable.getEffectList().stopEffects(info =>
					!info.getSkill().isIrreplacableBuff() && info.getSkill().getBuffType() == SkillBuffType.BUFF &&
					hasRemoveBuffException(info.getSkill()), true, true);
			}
		}
	}

	/**
	 * Check if given buff {@code skill} should be removed.
	 * @param skill buff which should be removed
	 * @return {@code true} if buff will be removed, otherwise {@code false}
	 */
	private bool hasRemoveBuffException(Skill skill)
	{
		bool containsSkill = _removeBuffExceptions.Contains(skill.getId());
		return _removeBuffType == InstanceRemoveBuffType.BLACKLIST ? containsSkill : !containsSkill;
	}

	/**
	 * Get type of re-enter data.
	 * @return type of re-enter (see {@link InstanceReenterType} for possible values)
	 */
	public InstanceReenterType getReenterType()
	{
		return _reenterType;
	}

	/**
	 * Calculate re-enter time for instance world.
	 * @return re-enter time in milliseconds
	 */
	public DateTime calculateReenterTime()
	{
		DateTime? time = null;
		foreach (InstanceReenterTimeHolder data in _reenterData)
		{
			if (data.getTime() > TimeSpan.Zero)
			{
				time = DateTime.UtcNow + data.getTime().Value;
				break;
			}

			DateTime calendar = DateTime.Now;
			calendar = new DateTime(calendar.Year, calendar.Month, calendar.Day, data.getHour().Value,
				data.getMinute().Value, 0);

			// If calendar time is lower than current, add one more day
			if (calendar <= DateTime.Now)
			{
				calendar = calendar.AddDays(1);
			}

			// Modify calendar day
			if (data.getDay() != null)
			{
				// DayOfWeek starts with Monday(1) but Calendar starts with Sunday(1)
				DayOfWeek day = data.getDay().Value + 1;
				if (day > DayOfWeek.Saturday)
				{
					day = DayOfWeek.Sunday;
				}

				// Set exact day. If modified date is before current, add one more week.
				while (calendar.DayOfWeek != day || calendar < DateTime.Now)
					calendar = calendar.AddDays(1);
			}

			if (time is null || calendar < time)
			{
				time = calendar;
			}
		}

		if (time is not null)
			time = time.Value.ToUniversalTime();

		return time ?? DateTime.UtcNow;
	}

	/**
	 * Check if enter group mask contains given group type {@code type}.
	 * @param type type of group
	 * @return {@code true} if mask contains given group, otherwise {@code false}
	 */
	private bool groupMaskContains(GroupType type)
	{
		return (_groupMask & type) == type;
	}

	/**
	 * Get enter group which can enter into instance world based on player's group.
	 * @param player player who wants to enter
	 * @return group type which can enter if any can enter, otherwise {@code null}
	 */
	private GroupType getEnterGroupType(Player player)
	{
		// If mask doesn't contain any group
		if (_groupMask == 0)
		{
			return GroupType.None;
		}

		// If player can override instance conditions then he can enter alone
		if (player.canOverrideCond(PlayerCondOverride.INSTANCE_CONDITIONS))
		{
			return GroupType.Player;
		}

		// Check if mask contains player's group
		GroupType playerGroup = player.getGroupType();
		if (groupMaskContains(playerGroup))
		{
			return playerGroup;
		}

		// Check if mask contains only one group
		GroupType type = _groupMask;
		if (type != GroupType.None)
		{
			return type;
		}

		// When mask contains more group types but without player's group, choose nearest one
		// player < party < command channel
		foreach (GroupType t in EnumUtil.GetValues<GroupType>())
		{
			if (t != playerGroup && groupMaskContains(t))
			{
				return t;
			}
		}

		// nothing found? then player cannot enter
		return GroupType.None;
	}

	/**
	 * Get player's group based on result of {@link InstanceTemplate#getEnterGroupType(Player)}.
	 * @param player player who wants to enter into instance
	 * @return list of players (first player in list is player who make enter request)
	 */
	public List<Player> getEnterGroup(Player player)
	{
		GroupType type = getEnterGroupType(player);
		if (type == GroupType.None)
		{
			return [];
		}

		// Make list of players which can enter into instance world
		List<Player> group = new();
		group.Add(player); // Put player who made request at first position inside list

		// Check if player has group in which he can enter
		AbstractPlayerGroup? pGroup = null;
		if (type == GroupType.PARTY)
		{
			pGroup = player.getParty();
		}
		else if (type == GroupType.COMMAND_CHANNEL)
		{
			pGroup = player.getCommandChannel();
		}

		// If any group found then put them into enter group list
		if (pGroup != null)
		{
			foreach (Player member in pGroup.getMembers())
			{
				if (member != player)
				{
					group.Add(member);
				}
			}
		}
		return group;
	}

	/**
	 * Validate instance conditions for given group.
	 * @param group group of players which want to enter instance world
	 * @param npc instance of NPC used to enter to instance
	 * @param htmlCallback callback function used to display fail HTML when condition validate failed
	 * @return {@code true} when all condition are met, otherwise {@code false}
	 */
	public bool validateConditions(List<Player> group, Npc npc, Action<Player, string> htmlCallback)
	{
		foreach (Condition cond in _conditions)
		{
			if (!cond.validate(npc, group, htmlCallback))
			{
				return false;
			}
		}
		return true;
	}

	/**
	 * Apply condition effects for each player from enter group.
	 * @param group players from enter group
	 */
	public void applyConditionEffects(List<Player> group)
	{
		_conditions.ForEach(c => c.applyEffect(group));
	}

	/**
	 * @return the exp rate of the instance
	 **/
	public float getExpRate()
	{
		return _expRate;
	}

	/**
	 * Sets the exp rate of the instance
	 * @param expRate
	 **/
	public void setExpRate(float expRate)
	{
		_expRate = expRate;
	}

	/**
	 * @return the sp rate of the instance
	 */
	public float getSPRate()
	{
		return _spRate;
	}

	/**
	 * Sets the sp rate of the instance
	 * @param spRate
	 **/
	public void setSPRate(float spRate)
	{
		_spRate = spRate;
	}

	/**
	 * @return the party exp rate of the instance
	 */
	public float getExpPartyRate()
	{
		return _expPartyRate;
	}

	/**
	 * Sets the party exp rate of the instance
	 * @param expRate
	 **/
	public void setExpPartyRate(float expRate)
	{
		_expPartyRate = expRate;
	}

	/**
	 * @return the party sp rate of the instance
	 */
	public float getSPPartyRate()
	{
		return _spPartyRate;
	}

	/**
	 * Sets the party sp rate of the instance
	 * @param spRate
	 **/
	public void setSPPartyRate(float spRate)
	{
		_spPartyRate = spRate;
	}

	/**
	 * Get count of created instance worlds.
	 * @return count of created instances
	 */
	public long getWorldCount()
	{
		return InstanceManager.getInstance().getWorldCount(getId());
	}

	public override string ToString()
	{
		return "ID: " + _templateId + " Name: " + _name;
	}
}