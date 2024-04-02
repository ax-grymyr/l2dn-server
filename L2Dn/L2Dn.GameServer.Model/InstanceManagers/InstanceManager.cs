using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.InstanceZones.Conditions;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Instance manager.
 * @author evill33t, GodKratos, malyelfik
 */
public class InstanceManager: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(InstanceManager));
	
	// Client instance names
	private readonly Map<int, String> _instanceNames = new();
	// Instance templates holder
	private readonly Map<int, InstanceTemplate> _instanceTemplates = new();
	// Created instance worlds
	private int _currentInstanceId = 0;
	private readonly Map<int, Instance> _instanceWorlds = new();
	// Player reenter times
	private readonly Map<int, Map<int, DateTime>> _playerTimes = new();
	
	protected InstanceManager()
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
		
		LoadXmlDocument(DataFileLocation.Data, "InstanceNames.xml").Elements("list").Elements("instance").ForEach(el =>
		{
			int id = el.GetAttributeValueAsInt32("id");
			string name = el.GetAttributeValueAsString("name");
			_instanceNames.put(id, name);
		});
		
		LOGGER.Info(GetType().Name +": Loaded " + _instanceNames.size() + " instance names.");
		
		// Load instance templates
		_instanceTemplates.clear();
		
		LoadXmlDocuments(DataFileLocation.Data, "instances", true).ForEach(t =>
		{
			t.Document.Elements("instance").ForEach(el => parseInstanceTemplate(t.FilePath, el));
		});
		
		LOGGER.Info(GetType().Name +": Loaded " + _instanceTemplates.size() + " instance templates.");
		// Load player's reenter data
		_playerTimes.clear();
		restoreInstanceTimes();
		LOGGER.Info(GetType().Name +": Loaded instance reenter times for " + _playerTimes.size() + " players.");
	}
	
	/**
	 * Parse instance template from XML file.
	 * @param instanceNode start XML tag
	 * @param file currently parsed file
	 */
	private void parseInstanceTemplate(string filePath, XElement element)
	{
		// Parse "instance" node
		int id = element.GetAttributeValueAsInt32("id");
		if (_instanceTemplates.containsKey(id))
		{
			LOGGER.Warn(GetType().Name + ": Instance template with ID " + id + " already exists");
			return;
		}
		
		InstanceTemplate template = new InstanceTemplate(new StatSet(element));
		
		// Update name if wasn't provided
		if (template.getName() == null)
		{
			template.setName(_instanceNames.get(id));
		}
		
		// Parse "instance" node children
		foreach (XElement innerNode in element.Elements())
		{
			string nodeName = innerNode.Name.LocalName;
			switch (nodeName)
			{
				case "time":
				{
					template.setDuration(TimeSpan.FromMinutes(innerNode.Attribute("duration").GetInt32(-1)));
					template.setEmptyDestroyTime(TimeSpan.FromMinutes(innerNode.Attribute("empty").GetInt32(-1)));
					template.setEjectTime(TimeSpan.FromMinutes(innerNode.Attribute("eject").GetInt32(-1)));
					break;
				}
				case "misc":
				{
					template.allowPlayerSummon(innerNode.Attribute("allowPlayerSummon").GetBoolean(false));
					template.setPvP(innerNode.Attribute("isPvP").GetBoolean(false));
					break;
				}
				case "rates":
				{
					template.setExpRate(innerNode.Attribute("exp").GetFloat(Config.RATE_INSTANCE_XP));
					template.setSPRate(innerNode.Attribute("sp").GetFloat(Config.RATE_INSTANCE_SP));
					template.setExpPartyRate(innerNode.Attribute("partyExp").GetFloat(Config.RATE_INSTANCE_PARTY_XP));
					template.setSPPartyRate(innerNode.Attribute("partySp").GetFloat(Config.RATE_INSTANCE_PARTY_SP));
					break;
				}
				case "locations":
				{
					foreach (XElement locationsNode in innerNode.Elements())
					{
						switch (locationsNode.Name.LocalName)
						{
							case "enter":
							{
								InstanceTeleportType type = locationsNode.Attribute("type").GetEnum<InstanceTeleportType>();
								List<Location> locations = new();
								locationsNode.Elements("location").ForEach(locationNode => locations.add(parseLocation(locationNode)));
								template.setEnterLocation(type, locations);
								break;
							}
							case "exit":
							{
								InstanceTeleportType type = locationsNode.Attribute("type").GetEnum<InstanceTeleportType>();
								if (type == InstanceTeleportType.ORIGIN)
								{
									template.setExitLocation(type, null);
								}
								else if (type == InstanceTeleportType.TOWN)
								{
									template.setExitLocation(type, null);
								}
								else
								{
									List<Location> locations = new();
									locationsNode.Elements("location").ForEach(locationNode => locations.add(parseLocation(locationNode)));
									if (locations.isEmpty())
									{
										LOGGER.Warn(GetType().Name + ": Missing exit location data for instance " + template.getName() + " (" + template.getId() + ")!");
									}
									else
									{
										template.setExitLocation(type, locations);
									}
								}
								break;
							}
						}
					}
					
					break;
				}
				case "spawnlist":
				{
					List<SpawnTemplate> spawns = new();
					SpawnData.getInstance().parseSpawn(innerNode, filePath, spawns);
					template.addSpawns(spawns);
					break;
				}
				case "doorlist":
				{
					foreach (XElement doorNode in innerNode.Elements("door"))
					{
						StatSet parsedSet = DoorData.parseDoor(doorNode);
						StatSet mergedSet = new StatSet();
						int doorId = parsedSet.getInt("id");
						StatSet templateSet = DoorData.getInstance().getDoorTemplate(doorId);
						if (templateSet != null)
						{
							mergedSet.merge(templateSet);
						}
						else
						{
							LOGGER.Warn(GetType().Name + ": Cannot find template for door: " + doorId + ", instance: " + template.getName() + " (" + template.getId() + ")");
						}
						mergedSet.merge(parsedSet);
						
						try
						{
							template.addDoor(doorId, new DoorTemplate(mergedSet));
						}
						catch (Exception e)
						{
							LOGGER.Warn(GetType().Name + ": Cannot initialize template for door: " + doorId + ", instance: " + template.getName() + " (" + template.getId() + ")" + e);
						}
					}
					break;
				}
				case "removeBuffs":
				{
					InstanceRemoveBuffType removeBuffType = innerNode.Attribute("type").GetEnum<InstanceRemoveBuffType>();
					List<int> exceptionBuffList = new();
					foreach (XElement e in innerNode.Elements("skill"))
						exceptionBuffList.add(e.GetAttributeValueAsInt32("id"));
					
					template.setRemoveBuff(removeBuffType, exceptionBuffList);
					break;
				}
				case "reenter":
				{
					InstanceReenterType type = innerNode.Attribute("apply").GetEnum(InstanceReenterType.NONE);
					List<InstanceReenterTimeHolder> data = new();
					foreach (XElement e in innerNode.Elements("reset"))
					{
						int time = e.Attribute("time").GetInt32(-1);
						if (time > 0)
						{
							data.add(new InstanceReenterTimeHolder(TimeSpan.FromMinutes(time)));
						}
						else
						{
							DayOfWeek day = e.GetAttributeValueAsEnum<DayOfWeek>("day", true);
							int hour = e.Attribute("hour").GetInt32(-1);
							int minute = e.Attribute("minute").GetInt32(-1);
							data.add(new InstanceReenterTimeHolder(day, hour, minute));
						}
					}

					template.setReenterData(type, data);
					break;
				}
				case "parameters":
				{
					template.setParameters(parseParameters(innerNode));
					break;
				}
				case "conditions":
				{
					List<Condition> conditions = new();
					foreach (XElement conditionNode in innerNode.Elements("condition"))
					{
						String type = conditionNode.GetAttributeValueAsString("type");
						bool onlyLeader = conditionNode.Attribute("onlyLeader").GetBoolean(false);
						bool showMessageAndHtml = conditionNode.Attribute("showMessageAndHtml").GetBoolean(false);
						// Load parameters
						StatSet parameters = new();
						foreach (XElement f in conditionNode.Elements("param"))
						{
							parameters.set(f.GetAttributeValueAsString("name"), f.GetAttributeValueAsString("value"));
						}
						
						// Now when everything is loaded register condition to template
						try
						{
							// TODO create factory
							string typeFullName = typeof(Condition).Namespace + ".Condition" + type;
							Type classType = Assembly.GetExecutingAssembly().GetType(typeFullName);
							Condition condition = (Condition)Activator.CreateInstance(classType, template, parameters,
								onlyLeader, showMessageAndHtml);
							
							conditions.add(condition);
						}
						catch (Exception ex)
						{
							LOGGER.Warn(GetType().Name + ": Unknown condition type " + type + " for instance " +
							            template.getName() + " (" + id + ")!");
						}
					}
					
					template.setConditions(conditions);
					break;
				}
			}
		}
		
		// Save template
		_instanceTemplates.put(id, template);
	}
	
	// --------------------------------------------------------------------
	// Instance data loader - END
	// --------------------------------------------------------------------
	
	/**
	 * Create new instance with default template.
	 * @return newly created default instance.
	 */
	public Instance createInstance()
	{
		return new Instance(getNewInstanceId(), new InstanceTemplate(StatSet.EMPTY_STATSET), null);
	}
	
	/**
	 * Create new instance from given template.
	 * @param template template used for instance creation
	 * @param player player who create instance.
	 * @return newly created instance if success, otherwise {@code null}
	 */
	public Instance createInstance(InstanceTemplate template, Player player)
	{
		return (template != null) ? new Instance(getNewInstanceId(), template, player) : null;
	}
	
	/**
	 * Create new instance with template defined in datapack.
	 * @param id template id of instance
	 * @param player player who create instance
	 * @return newly created instance if template was found, otherwise {@code null}
	 */
	public Instance createInstance(int id, Player player)
	{
		if (!_instanceTemplates.containsKey(id))
		{
			LOGGER.Warn(GetType().Name + ": Missing template for instance with id " + id + "!");
			return null;
		}
		return new Instance(getNewInstanceId(), _instanceTemplates.get(id), player);
	}
	
	/**
	 * Get instance world with given ID.
	 * @param instanceId ID of instance
	 * @return instance itself if found, otherwise {@code null}
	 */
	public Instance getInstance(int instanceId)
	{
		return _instanceWorlds.get(instanceId);
	}
	
	/**
	 * Get all active instances.
	 * @return Collection of all instances
	 */
	public ICollection<Instance> getInstances()
	{
		return _instanceWorlds.values();
	}
	
	/**
	 * Get instance world for player.
	 * @param player player who wants to get instance world
	 * @param isInside when {@code true} find world where player is currently located, otherwise find world where player can enter
	 * @return instance if found, otherwise {@code null}
	 */
	public Instance getPlayerInstance(Player player, bool isInside)
	{
		foreach (Instance instance in _instanceWorlds.values())
		{
			if (isInside)
			{
				if (instance.containsPlayer(player))
				{
					return instance;
				}
			}
			else
			{
				if (instance.isAllowed(player))
				{
					return instance;
				}
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
			{
				_currentInstanceId = 0;
			}
			_currentInstanceId++;
		}
		while (_instanceWorlds.containsKey(_currentInstanceId));
		return _currentInstanceId;
	}
	
	/**
	 * Register instance world.
	 * @param instance instance which should be registered
	 */
	public void register(Instance instance)
	{
		int instanceId = instance.getId();
		if (!_instanceWorlds.containsKey(instanceId))
		{
			_instanceWorlds.put(instanceId, instance);
		}
	}
	
	/**
	 * Unregister instance world.<br>
	 * <b><font color=red>To remove instance world properly use {@link Instance#destroy()}.</font></b>
	 * @param instanceId ID of instance to unregister
	 */
	public void unregister(int instanceId)
	{
		if (_instanceWorlds.containsKey(instanceId))
		{
			_instanceWorlds.remove(instanceId);
		}
	}
	
	/**
	 * Get instance name from file "InstanceNames.xml"
	 * @param templateId template ID of instance
	 * @return name of instance if found, otherwise {@code null}
	 */
	public String getInstanceName(int templateId)
	{
		return _instanceNames.get(templateId);
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
			LOGGER.Warn(GetType().Name + ": Cannot restore players instance reenter data: " + e);
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
		Map<int, DateTime> instanceTimes = _playerTimes.get(player.getObjectId());
		if ((instanceTimes == null) || instanceTimes.isEmpty())
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
				LOGGER.Warn(GetType().Name + ": Cannot delete instance character reenter data: " + e);
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
		// Check if exists reenter data for player
		Map<int, DateTime> playerData = _playerTimes.get(player.getObjectId());
		if ((playerData == null) || !playerData.containsKey(id))
		{
			return DateTime.MinValue;
		}
		
		// If reenter time is higher then current, delete it
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
			LOGGER.Warn(GetType().Name + ": Could not delete character instance reenter data: " + e);
		}
	}
	
	/**
	 * Get instance template by template ID.
	 * @param id template id of instance
	 * @return instance template if found, otherwise {@code null}
	 */
	public InstanceTemplate getInstanceTemplate(int id)
	{
		return _instanceTemplates.get(id);
	}
	
	/**
	 * Get all instances template.
	 * @return Collection of all instance templates
	 */
	public ICollection<InstanceTemplate> getInstanceTemplates()
	{
		return _instanceTemplates.values();
	}
	
	/**
	 * Get count of created instance worlds with same template ID.
	 * @param templateId template id of instance
	 * @return count of created instances
	 */
	public long getWorldCount(int templateId)
	{
		long count = 0;
		foreach (Instance i in _instanceWorlds.values())
		{
			if (i.getTemplateId() == templateId)
			{
				count++;
			}
		}
		return count;
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
		public static readonly InstanceManager INSTANCE = new InstanceManager();
	}
}