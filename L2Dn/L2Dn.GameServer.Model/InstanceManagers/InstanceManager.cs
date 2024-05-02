using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.InstanceZones.Conditions;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.DataPack;
using L2Dn.Model.Enums;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Instance manager.
 * @author evill33t, GodKratos, malyelfik
 */
public class InstanceManager: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(InstanceManager));

	private delegate Condition InstanceConditionFactory(InstanceTemplate instanceTemplate, StatSet parameters,
		bool onlyLeader, bool showMessageAndHtml);

	private readonly FrozenDictionary<XmlInstanceConditionType, InstanceConditionFactory> _conditionFactories =
		new (XmlInstanceConditionType Type, InstanceConditionFactory Factory)[]
		{
			(XmlInstanceConditionType.CommandChannel,
				(template, parameters, leader, html)
					=> new ConditionCommandChannel(template, parameters, leader, html)),
			(XmlInstanceConditionType.CommandChannelLeader,
				(template, parameters, leader, html)
					=> new ConditionCommandChannelLeader(template, parameters, leader, html)),
			(XmlInstanceConditionType.Party,
				(template, parameters, leader, html) => new ConditionParty(template, parameters, leader, html)),
			(XmlInstanceConditionType.PartyLeader,
				(template, parameters, leader, html) => new ConditionPartyLeader(template, parameters, leader, html)),
			(XmlInstanceConditionType.NoParty,
				(template, parameters, leader, html) => new ConditionNoParty(template, parameters, leader, html)),
			(XmlInstanceConditionType.Distance,
				(template, parameters, leader, html) => new ConditionDistance(template, parameters, leader, html)),
			(XmlInstanceConditionType.GroupMin,
				(template, parameters, leader, html) => new ConditionGroupMin(template, parameters, leader, html)),
			(XmlInstanceConditionType.GroupMax,
				(template, parameters, leader, html) => new ConditionGroupMax(template, parameters, leader, html)),
			(XmlInstanceConditionType.Item,
				(template, parameters, leader, html) => new ConditionItem(template, parameters, leader, html)),
			(XmlInstanceConditionType.Level,
				(template, parameters, leader, html) => new ConditionLevel(template, parameters, leader, html)),
			(XmlInstanceConditionType.Quest,
				(template, parameters, leader, html) => new ConditionQuest(template, parameters, leader, html)),
			(XmlInstanceConditionType.Reenter,
				(template, parameters, leader, html) => new ConditionReenter(template, parameters, leader, html)),
			(XmlInstanceConditionType.HasResidence,
				(template, parameters, leader, html) => new ConditionHasResidence(template, parameters, leader, html)),
		}.ToFrozenDictionary(t => t.Type, t => t.Factory);
	
	// Client instance names
	private readonly Map<int, string> _instanceNames = new();
	
	// Instance templates holder
	private readonly Map<int, InstanceTemplate> _instanceTemplates = new();
	
	// Created instance worlds
	private int _currentInstanceId;
	private readonly Map<int, Instance> _instanceWorlds = new();
	
	// Player reenter times
	private readonly Map<int, Map<int, DateTime>> _playerTimes = new();
	
	private InstanceManager()
	{
		load();
	}
	
	// --------------------------------------------------------------------
	// Instance data loader
	// --------------------------------------------------------------------
	public void load()
	{
		// Load instance names
		_instanceNames.clear();

		LoadXmlDocument<XmlInstanceNameList>(DataFileLocation.Data, "InstanceNames.xml")
			.Instances
			.ForEach(instance => _instanceNames.put(instance.Id, instance.Name));
		
		_logger.Info(GetType().Name +": Loaded " + _instanceNames.size() + " instance names.");
		
		// Load instance templates
		_instanceTemplates.clear();

		LoadXmlDocuments<XmlInstance>(DataFileLocation.Data, "instances", true)
			.ForEach(t => parseInstanceTemplate(t.FilePath, t.Document));
		
		_logger.Info(GetType().Name +": Loaded " + _instanceTemplates.Count + " instance templates.");
		// Load player's reenter data
		_playerTimes.clear();
		restoreInstanceTimes();
		_logger.Info(GetType().Name +": Loaded instance reenter times for " + _playerTimes.Count + " players.");
	}
	
	/**
	 * Parse instance template from XML file.
	 * @param instanceNode start XML tag
	 * @param file currently parsed file
	 */
	private void parseInstanceTemplate(string filePath, XmlInstance xmlInstance)
	{
		int id = xmlInstance.Id;
		if (_instanceTemplates.ContainsKey(id))
		{
			_logger.Warn(GetType().Name + ": Instance template with ID " + id + " already exists");
			return;
		}

		string name = xmlInstance.Name;
		if (string.IsNullOrEmpty(name))
			name = _instanceNames[id];
		
		InstanceTemplate template = new(id, name, xmlInstance.MaxWorlds);

		XmlInstanceTime? xmlInstanceTime = xmlInstance.Time;
		if (xmlInstanceTime != null)
		{
			template.setDuration(TimeSpan.FromMinutes(xmlInstanceTime.DurationMinutes));
			template.setEmptyDestroyTime(TimeSpan.FromMinutes(xmlInstanceTime.EmptyCloseMinutes));
			template.setEjectTime(TimeSpan.FromMinutes(xmlInstanceTime.EjectMinutes));
		}

		XmlInstanceMisc? xmlInstanceMisc = xmlInstance.Misc;
		if (xmlInstanceMisc != null)
		{
			template.allowPlayerSummon(xmlInstanceMisc.AllowPlayerSummon);
			template.setPvP(xmlInstanceMisc.IsPvP);
		}

		XmlInstanceRates? xmlInstanceRates = xmlInstance.Rates;
		if (xmlInstanceRates != null)
		{
			template.setExpRate(xmlInstanceRates.ExpSpecified ? xmlInstanceRates.Exp : Config.RATE_INSTANCE_XP);
			template.setSPRate(xmlInstanceRates.SpSpecified ? xmlInstanceRates.Sp : Config.RATE_INSTANCE_SP);
			
			template.setExpPartyRate(xmlInstanceRates.PartyExpSpecified
				? xmlInstanceRates.PartyExp
				: Config.RATE_INSTANCE_PARTY_XP);
			
			template.setSPPartyRate(xmlInstanceRates.PartySpSpecified
				? xmlInstanceRates.PartySp
				: Config.RATE_INSTANCE_PARTY_SP);
		}

		XmlInstanceLocations? xmlInstanceLocations = xmlInstance.Locations;
		if (xmlInstanceLocations != null)
		{
			XmlInstanceEnterLocations? xmlInstanceEnterLocations = xmlInstanceLocations.EnterLocations;
			XmlInstanceExitLocations? xmlInstanceExitLocations = xmlInstanceLocations.ExitLocations;
			if (xmlInstanceEnterLocations != null)
			{
				ImmutableArray<LocationHeading> locations = xmlInstanceEnterLocations.Locations
					.Select(loc => new LocationHeading(loc.X, loc.Y, loc.Z, loc.Heading))
					.ToImmutableArray();
				
				template.setEnterLocation(xmlInstanceEnterLocations.Type, locations);
			}

			if (xmlInstanceExitLocations != null)
			{
				InstanceTeleportType type = xmlInstanceExitLocations.Type;
				switch (type)
				{
					case InstanceTeleportType.ORIGIN:
					case InstanceTeleportType.TOWN:
						break;
					
					default:
					{
						ImmutableArray<Location3D> locations = xmlInstanceExitLocations.Locations
							.Select(loc => new Location3D(loc.X, loc.Y, loc.Z))
							.ToImmutableArray();

						if (locations.isEmpty())
						{
							_logger.Warn(GetType().Name + ": Missing exit location data for instance " +
							             template.getName() + " (" + template.getId() + ")!");
						}
						else
							template.setExitLocation(type, locations);
						
						break;
					}
				}
			}
		}

		XmlSpawn? xmlSpawn = xmlInstance.Spawns;
		if (xmlSpawn != null)
		{
			List<SpawnTemplate> spawns = [];
			SpawnData.getInstance().parseSpawn(xmlSpawn, filePath, spawns);
			template.addSpawns(spawns);
		}

		foreach (XmlDoor xmlDoor in xmlInstance.Doors)
		{
			int doorId = xmlDoor.Id;
			DoorTemplate? doorTemplate = DoorData.getInstance().getDoorTemplate(doorId);
			if (doorTemplate == null)
			{
				_logger.Warn(GetType().Name + ": Cannot find template for door: " + doorId + ", instance: " +
				             template.getName() + " (" + template.getId() + ")");
				
				continue;
			}

			template.addDoor(doorId, doorTemplate);
			
			bool defaultOpen = xmlDoor.OpenStatus?.Default == XmlDoorDefaultOpenStatus.open;
			if (defaultOpen != doorTemplate.isOpenByDefault())
			{
				// Override instance default open state.
				template.addDoorState(doorId, defaultOpen);
			}
		}

		XmlInstanceRemoveBuffs? xmlInstanceRemoveBuffs = xmlInstance.RemoveBuffs;
		if (xmlInstanceRemoveBuffs != null)
		{
			InstanceRemoveBuffType removeBuffType = xmlInstanceRemoveBuffs.Type;
			List<int> exceptionBuffList = [];
			foreach (XmlInstanceRemoveBuffsSkill xmlInstanceRemoveBuffsSkill in xmlInstanceRemoveBuffs.Skills)
				exceptionBuffList.Add(xmlInstanceRemoveBuffsSkill.Id);
					
			template.setRemoveBuff(removeBuffType, exceptionBuffList);
		}

		XmlInstanceReenter? xmlInstanceReenter = xmlInstance.Reenter;
		if (xmlInstanceReenter != null)
		{
			InstanceReenterType type = xmlInstanceReenter.Apply;
			List<InstanceReenterTimeHolder> data = [];
			foreach (XmlInstanceReenterReset xmlInstanceReenterReset in xmlInstanceReenter.Resets)
			{
				if (xmlInstanceReenterReset is { TimeMinutesSpecified: true, TimeMinutes: > 0 })
					data.Add(new InstanceReenterTimeHolder(TimeSpan.FromMinutes(xmlInstanceReenterReset.TimeMinutes)));
				else
				{
					DayOfWeek? day = xmlInstanceReenterReset.DayOfWeekSpecified
						? (DayOfWeek)xmlInstanceReenterReset.DayOfWeek
						: null;
					
					int hour = xmlInstanceReenterReset.Hour;
					int minute = xmlInstanceReenterReset.Minute;
					data.Add(new InstanceReenterTimeHolder(day, hour, minute));
				}
			}

			template.setReenterData(type, data);
		}

		if (xmlInstance.Parameters.Count != 0)
		{
			Map<string, object> statSet = new();
			foreach (XmlParameter xmlParameter in xmlInstance.Parameters)
			{
				object? value = xmlParameter switch
				{
					XmlParameterString xmlParameterString => xmlParameterString.Value,
					XmlParameterSkill xmlParameterSkill => new SkillHolder(xmlParameterSkill.Id,
						xmlParameterSkill.Level),

					XmlParameterLocation xmlParameterLocation => new LocationHeading(xmlParameterLocation.X,
						xmlParameterLocation.Y, xmlParameterLocation.Z, xmlParameterLocation.Heading),

					_ => null
				};

				if (value == null)
					_logger.Error(nameof(InstanceManager) + ": Invalid instance parameter");
				else
					statSet[xmlParameter.Name] = value;
			}
			
			template.setParameters(statSet);
		}

		if (xmlInstance.Conditions.Count != 0)
		{
			List<Condition> conditions = [];
			foreach (XmlInstanceCondition xmlInstanceCondition in xmlInstance.Conditions)
			{
				XmlInstanceConditionType type = xmlInstanceCondition.Type;
				bool onlyLeader = xmlInstanceCondition.OnlyLeader;
				bool showMessageAndHtml = xmlInstanceCondition.ShowMessageAndHtml;

				// Load parameters
				StatSet parameters = new();
				foreach (XmlParameterString xmlParameter in xmlInstanceCondition.Parameters)
					parameters.set(xmlParameter.Name, xmlParameter.Value);

				// Now when everything is loaded register condition to template.
				if (!_conditionFactories.TryGetValue(type, out InstanceConditionFactory? factory))
				{
					_logger.Error(GetType().Name + ": Unknown condition type " + type + " for instance " +
					              template.getName() + " (" + id + ")!");
				}
				else
				{
					Condition condition = factory(template, parameters, onlyLeader, showMessageAndHtml);
					conditions.add(condition);
				}
			}

			template.setConditions(conditions);
		}
		
		// Save template
		_instanceTemplates.put(id, template);
	}
	
	// --------------------------------------------------------------------
	// Instance data loader - END
	// --------------------------------------------------------------------
	
	/**
	 * Create new instance from given template.
	 * @param template template used for instance creation
	 * @param player player who create instance.
	 * @return newly created instance if success, otherwise {@code null}
	 */
	public Instance createInstance(InstanceTemplate template, Player player)
	{
		return template != null ? new Instance(getNewInstanceId(), template, player) : null;
	}
	
	/**
	 * Create new instance with template defined in datapack.
	 * @param id template id of instance
	 * @param player player who create instance
	 * @return newly created instance if template was found, otherwise {@code null}
	 */
	public Instance? createInstance(int id, Player player)
	{
		if (!_instanceTemplates.TryGetValue(id, out InstanceTemplate? template))
		{
			_logger.Warn(GetType().Name + ": Missing template for instance with id " + id + "!");
			return null;
		}
		
		return new Instance(getNewInstanceId(), template, player);
	}
	
	/**
	 * Get instance world with given ID.
	 * @param instanceId ID of instance
	 * @return instance itself if found, otherwise {@code null}
	 */
	public Instance? getInstance(int instanceId)
	{
		return _instanceWorlds.GetValueOrDefault(instanceId);
	}
	
	/**
	 * Get all active instances.
	 * @return Collection of all instances
	 */
	public ICollection<Instance> getInstances()
	{
		return _instanceWorlds.Values;
	}
	
	/**
	 * Get instance world for player.
	 * @param player player who wants to get instance world
	 * @param isInside when {@code true} find world where player is currently located, otherwise find world where player can enter
	 * @return instance if found, otherwise {@code null}
	 */
	public Instance? getPlayerInstance(Player player, bool isInside)
	{
		foreach (Instance instance in _instanceWorlds.Values)
		{
			if (isInside)
			{
				if (instance.containsPlayer(player))
					return instance;
			}
			else
			{
				if (instance.isAllowed(player))
					return instance;
			}
		}
		
		return null;
	}
	
	/**
	 * Get ID for newly created instance.
	 * @return instance id
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	private int getNewInstanceId()
	{
		do
		{
			if (_currentInstanceId == int.MaxValue)
				_currentInstanceId = 0;

			_currentInstanceId++;
		}
		while (_instanceWorlds.ContainsKey(_currentInstanceId));
		return _currentInstanceId;
	}
	
	/**
	 * Register instance world.
	 * @param instance instance which should be registered
	 */
	public void register(Instance instance)
	{
		_instanceWorlds.TryAdd(instance.getId(), instance);
	}
	
	/**
	 * Unregister instance world.<br>
	 * <b><font color=red>To remove instance world properly use {@link Instance#destroy()}.</font></b>
	 * @param instanceId ID of instance to unregister
	 */
	public void unregister(int instanceId)
	{
		_instanceWorlds.Remove(instanceId, out _);
	}
	
	/**
	 * Get instance name from file "InstanceNames.xml"
	 * @param templateId template ID of instance
	 * @return name of instance if found, otherwise {@code null}
	 */
	public string? getInstanceName(int templateId)
	{
		return _instanceNames.GetValueOrDefault(templateId);
	}
	
	/**
	 * Restore instance reenter data for all players.
	 */
	private void restoreInstanceTimes()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (CharacterInstance record in ctx.CharacterInstances.OrderBy(r => r.CharacterId))
			{
				// Check if instance penalty passed
				DateTime time = record.Time;
				if (time > DateTime.UtcNow)
				{
					// Load params
					int charId = record.CharacterId;
					int instanceId = record.InstanceId;
					
					// Set penalty
					setReenterPenalty(charId, instanceId, time);
				}
			}
		}
		catch (Exception e)
		{
			_logger.Warn(GetType().Name + ": Cannot restore players instance reenter data: " + e);
		}
	}
	
	/**
	 * Get all instance re-enter times for specified player.<br>
	 * This method also removes the penalties that have already expired.
	 * @param player instance of player who wants to get re-enter data
	 * @return map in form templateId, penaltyEndTime
	 */
	public Map<int, DateTime> getAllInstanceTimes(Player player)
	{
		// When player don't have any instance penalty
		Map<int, DateTime>? instanceTimes = _playerTimes.GetValueOrDefault(player.getObjectId());
		if (instanceTimes == null || instanceTimes.isEmpty())
		{
			return new();
		}
		
		// Find passed penalty
		List<int> invalidPenalty = new();
		foreach (var entry in instanceTimes)
		{
			if (entry.Value <= DateTime.UtcNow)
			{
				invalidPenalty.add(entry.Key);
			}
		}
		
		// Remove them
		if (!invalidPenalty.isEmpty())
		{
			try 
			{
				int playerId = player.getObjectId();
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				foreach (int id in invalidPenalty)
					ctx.CharacterInstances.Where(r => r.CharacterId == playerId && r.InstanceId == id).ExecuteDelete();

				invalidPenalty.forEach(x => instanceTimes.remove(x));
			}
			catch (Exception e)
			{
				_logger.Warn(GetType().Name + ": Cannot delete instance character reenter data: " + e);
			}
		}
		return instanceTimes;
	}
	
	/**
	 * Set re-enter penalty for specified player.<br>
	 * <font color=red><b>This method store penalty into memory only. Use {@link Instance#setReenterTime} to set instance penalty properly.</b></font>
	 * @param objectId object ID of player
	 * @param id instance template id
	 * @param time penalty time
	 */
	public void setReenterPenalty(int objectId, int id, DateTime time)
	{
		_playerTimes.computeIfAbsent(objectId, k => new()).put(id, time);
	}
	
	/**
	 * Get re-enter time to instance (by template ID) for player.<br>
	 * This method also removes penalty if expired.
	 * @param player player who wants to get re-enter time
	 * @param id template ID of instance
	 * @return penalty end time if penalty is found, otherwise -1
	 */
	public DateTime getInstanceTime(Player player, int id)
	{
		// Check if exists reenter data for player.
		Map<int, DateTime>? playerData = _playerTimes.GetValueOrDefault(player.getObjectId());
		if (playerData == null || !playerData.containsKey(id))
		{
			return DateTime.MinValue;
		}
		
		// If reenter time is higher than current, delete it.
		DateTime time = playerData.get(id);
		if (time <= DateTime.UtcNow)
		{
			deleteInstanceTime(player, id);
			return DateTime.MinValue;
		}
		return time;
	}
	
	/**
	 * Remove re-enter penalty for specified instance from player.
	 * @param player player who wants to delete penalty
	 * @param id template id of instance world
	 */
	public void deleteInstanceTime(Player player, int id)
	{
		try 
		{
			int playerId = player.getObjectId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterInstances.Where(r => r.CharacterId == playerId && r.InstanceId == id).ExecuteDelete();
			if (_playerTimes.get(player.getObjectId()) != null)
			{
				_playerTimes.get(player.getObjectId()).remove(id);
			}
		}
		catch (Exception e)
		{
			_logger.Warn(GetType().Name + ": Could not delete character instance reenter data: " + e);
		}
	}
	
	/**
	 * Get instance template by template ID.
	 * @param id template id of instance
	 * @return instance template if found, otherwise {@code null}
	 */
	public InstanceTemplate? getInstanceTemplate(int id)
	{
		return _instanceTemplates.GetValueOrDefault(id);
	}
	
	/**
	 * Get all instances template.
	 * @return Collection of all instance templates
	 */
	public ICollection<InstanceTemplate> getInstanceTemplates()
	{
		return _instanceTemplates.Values;
	}
	
	/**
	 * Get count of created instance worlds with same template ID.
	 * @param templateId template id of instance
	 * @return count of created instances
	 */
	public int getWorldCount(int templateId)
	{
		return _instanceWorlds.Count(p => p.Value.getTemplateId() == templateId);
	}
	
	/**
	 * Gets the single instance of {@code InstanceManager}.
	 * @return single instance of {@code InstanceManager}
	 */
	public static InstanceManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly InstanceManager INSTANCE = new();
	}
}