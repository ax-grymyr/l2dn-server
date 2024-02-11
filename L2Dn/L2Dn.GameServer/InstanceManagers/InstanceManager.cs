using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Instance manager.
 * @author evill33t, GodKratos, malyelfik
 */
public class InstanceManager: IXmlReader
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(InstanceManager));
	// Database query
	private const string DELETE_INSTANCE_TIME = "DELETE FROM character_instance_time WHERE charId=? AND instanceId=?";
	
	// Client instance names
	private readonly Map<int, String> _instanceNames = new();
	// Instance templates holder
	private readonly Map<int, InstanceTemplate> _instanceTemplates = new();
	// Created instance worlds
	private int _currentInstanceId = 0;
	private readonly Map<int, Instance> _instanceWorlds = new();
	// Player reenter times
	private readonly Map<int, Map<int, long>> _playerTimes = new();
	
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
		parseDatapackFile("data/InstanceNames.xml");
		LOGGER.Info(GetType().Name +": Loaded " + _instanceNames.size() + " instance names.");
		// Load instance templates
		_instanceTemplates.clear();
		parseDatapackDirectory("data/instances", true);
		LOGGER.Info(GetType().Name +": Loaded " + _instanceTemplates.size() + " instance templates.");
		// Load player's reenter data
		_playerTimes.clear();
		restoreInstanceTimes();
		LOGGER.Info(GetType().Name +": Loaded instance reenter times for " + _playerTimes.size() + " players.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, IXmlReader::isNode, listNode =>
		{
			switch (listNode.getNodeName())
			{
				case "list":
				{
					parseInstanceName(listNode);
					break;
				}
				case "instance":
				{
					parseInstanceTemplate(listNode, f);
					break;
				}
			}
		});
	}
	
	/**
	 * Read instance names from XML file.
	 * @param n starting XML tag
	 */
	private void parseInstanceName(Node n)
	{
		forEach(n, "instance", instanceNode =>
		{
			NamedNodeMap attrs = instanceNode.getAttributes();
			_instanceNames.put(parseInteger(attrs, "id"), parseString(attrs, "name"));
		});
	}
	
	/**
	 * Parse instance template from XML file.
	 * @param instanceNode start XML tag
	 * @param file currently parsed file
	 */
	private void parseInstanceTemplate(Node instanceNode, File file)
	{
		// Parse "instance" node
		int id = parseInteger(instanceNode.getAttributes(), "id");
		if (_instanceTemplates.containsKey(id))
		{
			LOGGER.Warn(GetType().Name + ": Instance template with ID " + id + " already exists");
			return;
		}
		
		InstanceTemplate template = new InstanceTemplate(new StatSet(parseAttributes(instanceNode)));
		
		// Update name if wasn't provided
		if (template.getName() == null)
		{
			template.setName(_instanceNames.get(id));
		}
		
		// Parse "instance" node children
		forEach(instanceNode, IXmlReader::isNode, innerNode =>
		{
			switch (innerNode.getNodeName())
			{
				case "time":
				{
					NamedNodeMap attrs = innerNode.getAttributes();
					template.setDuration(parseInteger(attrs, "duration", -1));
					template.setEmptyDestroyTime(parseInteger(attrs, "empty", -1));
					template.setEjectTime(parseInteger(attrs, "eject", -1));
					break;
				}
				case "misc":
				{
					NamedNodeMap attrs = innerNode.getAttributes();
					template.allowPlayerSummon(parseBoolean(attrs, "allowPlayerSummon", false));
					template.setPvP(parseBoolean(attrs, "isPvP", false));
					break;
				}
				case "rates":
				{
					NamedNodeMap attrs = innerNode.getAttributes();
					template.setExpRate(parseFloat(attrs, "exp", Config.RATE_INSTANCE_XP));
					template.setSPRate(parseFloat(attrs, "sp", Config.RATE_INSTANCE_SP));
					template.setExpPartyRate(parseFloat(attrs, "partyExp", Config.RATE_INSTANCE_PARTY_XP));
					template.setSPPartyRate(parseFloat(attrs, "partySp", Config.RATE_INSTANCE_PARTY_SP));
					break;
				}
				case "locations":
				{
					forEach(innerNode, IXmlReader::isNode, locationsNode =>
					{
						switch (locationsNode.getNodeName())
						{
							case "enter":
							{
								InstanceTeleportType type = parseEnum(locationsNode.getAttributes(), InstanceTeleportType.class, "type");
								List<Location> locations = new();
								forEach(locationsNode, "location", locationNode => locations.add(parseLocation(locationNode)));
								template.setEnterLocation(type, locations);
								break;
							}
							case "exit":
							{
								InstanceTeleportType type = parseEnum(locationsNode.getAttributes(), InstanceTeleportType.class, "type");
								if (type.equals(InstanceTeleportType.ORIGIN))
								{
									template.setExitLocation(type, null);
								}
								else if (type.equals(InstanceTeleportType.TOWN))
								{
									template.setExitLocation(type, null);
								}
								else
								{
									List<Location> locations = new();
									forEach(locationsNode, "location", locationNode => locations.add(parseLocation(locationNode)));
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
					});
					break;
				}
				case "spawnlist":
				{
					List<SpawnTemplate> spawns = new();
					SpawnData.getInstance().parseSpawn(innerNode, file, spawns);
					template.addSpawns(spawns);
					break;
				}
				case "doorlist":
				{
					for (Node doorNode = innerNode.getFirstChild(); doorNode != null; doorNode = doorNode.getNextSibling())
					{
						if (doorNode.getNodeName().equals("door"))
						{
							StatSet parsedSet = DoorData.getInstance().parseDoor(doorNode);
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
					}
					break;
				}
				case "removeBuffs":
				{
					InstanceRemoveBuffType removeBuffType = parseEnum(innerNode.getAttributes(), InstanceRemoveBuffType.class, "type");
					List<int> exceptionBuffList = new();
					for (Node e = innerNode.getFirstChild(); e != null; e = e.getNextSibling())
					{
						if (e.getNodeName().equals("skill"))
						{
							exceptionBuffList.add(parseInteger(e.getAttributes(), "id"));
						}
					}
					template.setRemoveBuff(removeBuffType, exceptionBuffList);
					break;
				}
				case "reenter":
				{
					InstanceReenterType type = parseEnum(innerNode.getAttributes(), InstanceReenterType.class, "apply", InstanceReenterType.NONE);
					List<InstanceReenterTimeHolder> data = new();
					for (Node e = innerNode.getFirstChild(); e != null; e = e.getNextSibling())
					{
						if (e.getNodeName().equals("reset"))
						{
							NamedNodeMap attrs = e.getAttributes();
							int time = parseInteger(attrs, "time", -1);
							if (time > 0)
							{
								data.add(new InstanceReenterTimeHolder(time));
							}
							else
							{
								DayOfWeek day = parseEnum(attrs, DayOfWeek.class, "day");
								int hour = parseInteger(attrs, "hour", -1);
								int minute = parseInteger(attrs, "minute", -1);
								data.add(new InstanceReenterTimeHolder(day, hour, minute));
							}
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
					for (Node conditionNode = innerNode.getFirstChild(); conditionNode != null; conditionNode = conditionNode.getNextSibling())
					{
						if (conditionNode.getNodeName().equals("condition"))
						{
							NamedNodeMap attrs = conditionNode.getAttributes();
							String type = parseString(attrs, "type");
							bool onlyLeader = parseBoolean(attrs, "onlyLeader", false);
							bool showMessageAndHtml = parseBoolean(attrs, "showMessageAndHtml", false);
							// Load parameters
							StatSet params = null;
							for (Node f = conditionNode.getFirstChild(); f != null; f = f.getNextSibling())
							{
								if (f.getNodeName().equals("param"))
								{
									if (params == null)
									{
										params = new StatSet();
									}
									
									params.set(parseString(f.getAttributes(), "name"), parseString(f.getAttributes(), "value"));
								}
							}
							
							// If none parameters found then set empty StatSet
							if (params == null)
							{
								params = StatSet.EMPTY_STATSET;
							}
							
							// Now when everything is loaded register condition to template
							try
							{
								Class<?> clazz = Class.forName("org.l2jmobius.gameserver.model.instancezone.conditions.Condition" + type);
								Constructor<?> constructor = clazz.getConstructor(InstanceTemplate.class, StatSet.class, bool.class, bool.class);
								conditions.add((Condition) constructor.newInstance(template, params, onlyLeader, showMessageAndHtml));
							}
							catch (Exception ex)
							{
								LOGGER.Warn(GetType().Name + ": Unknown condition type " + type + " for instance " + template.getName() + " (" + id + ")!");
							}
						}
					}
					template.setConditions(conditions);
					break;
				}
			}
		});
		
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
			using GameServerDbContext ctx = new();
			Statement ps = con.createStatement();
			ResultSet rs = ps.executeQuery("SELECT * FROM character_instance_time ORDER BY charId");
			while (rs.next())
			{
				// Check if instance penalty passed
				long time = rs.getLong("time");
				if (time > System.currentTimeMillis())
				{
					// Load params
					int charId = rs.getInt("charId");
					int instanceId = rs.getInt("instanceId");
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
	public Map<int, long> getAllInstanceTimes(Player player)
	{
		// When player don't have any instance penalty
		Map<int, long> instanceTimes = _playerTimes.get(player.getObjectId());
		if ((instanceTimes == null) || instanceTimes.isEmpty())
		{
			return new();
		}
		
		// Find passed penalty
		List<int> invalidPenalty = new();
		foreach (var entry in instanceTimes)
		{
			if (entry.Value <= System.currentTimeMillis())
			{
				invalidPenalty.add(entry.Key);
			}
		}
		
		// Remove them
		if (!invalidPenalty.isEmpty())
		{
			try 
			{
				using GameServerDbContext ctx = new();
				PreparedStatement ps = con.prepareStatement(DELETE_INSTANCE_TIME);
				foreach (int id in invalidPenalty)
				{
					ps.setInt(1, player.getObjectId());
					ps.setInt(2, id);
					ps.addBatch();
				}
				ps.executeBatch();
				invalidPenalty.forEach(instanceTimes::remove);
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
	public void setReenterPenalty(int objectId, int id, long time)
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
	public long getInstanceTime(Player player, int id)
	{
		// Check if exists reenter data for player
		Map<int, long> playerData = _playerTimes.get(player.getObjectId());
		if ((playerData == null) || !playerData.containsKey(id))
		{
			return -1;
		}
		
		// If reenter time is higher then current, delete it
		long time = playerData.get(id);
		if (time <= System.currentTimeMillis())
		{
			deleteInstanceTime(player, id);
			return -1;
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
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(DELETE_INSTANCE_TIME);
			ps.setInt(1, player.getObjectId());
			ps.setInt(2, id);
			ps.execute();
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