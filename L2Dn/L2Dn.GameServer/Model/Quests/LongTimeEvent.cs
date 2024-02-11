using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.InstanceManagers.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Announcements;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Scripts;
using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Quests;

/**
 * Parent class for long time events.<br>
 * Maintains config reading, spawn of NPCs, adding of event's drop.
 * @author GKR
 */
public class LongTimeEvent: Quest
{
	protected String _eventName;
	protected DateRange _eventPeriod = null;
	protected bool _active = false;
	protected bool _enableShrines = false;
	
	// Messages
	protected String _onEnterMsg = "";
	protected String _endMsg = "";
	protected int _enterAnnounceId = -1;
	
	// NPCs to spawn and their spawn points
	protected readonly List<NpcSpawn> _spawnList = new();
	
	// Drop data for event
	protected readonly List<EventDropHolder> _dropList = new();
	
	// Items to destroy when event ends
	protected readonly List<int> _destroyItemsOnEnd = new();
	
	protected class NpcSpawn
	{
		protected readonly  int npcId;
		protected readonly  Location loc;
		protected readonly  TimeSpan respawnTime;
		
		protected NpcSpawn(int spawnNpcId, Location spawnLoc, TimeSpan spawnRespawnTime)
		{
			npcId = spawnNpcId;
			loc = spawnLoc;
			respawnTime = spawnRespawnTime;
		}
	}
	
	public LongTimeEvent(): base(-1)
	{
		loadConfig();
		
		if (_eventPeriod != null)
		{
			if (_eventPeriod.isWithinRange(new Date()))
			{
				startEvent();
				LOGGER.Info("Event " + _eventName + " active till " + _eventPeriod.getEndDate());
			}
			else if (_eventPeriod.getStartDate().after(new Date()))
			{
				long delay = _eventPeriod.getStartDate().getTime() - System.currentTimeMillis();
				ThreadPool.schedule(new ScheduleStart(), delay);
				LOGGER.Info("Event " + _eventName + " will be started at " + _eventPeriod.getStartDate());
			}
			else
			{
				// Destroy items that must exist only on event period.
				destroyItemsOnEnd();
				LOGGER.Info("Event " + _eventName + " has passed... Ignored ");
			}
		}
	}
	
	/**
	 * Load event configuration file
	 */
	private void loadConfig()
	{
		new IXmlReader()
		{
			public void load()
			{
				parseDatapackFile("data/scripts/events/" + getScriptName() + "/config.xml");
			}
			
			public void parseDocument(Document doc, File f)
			{
				if (!doc.getDocumentElement().getNodeName().equalsIgnoreCase("event"))
				{
					throw new NullPointerException("WARNING!!! " + getScriptName() + " event: bad config file!");
				}
				
				_eventName = doc.getDocumentElement().getAttributes().getNamedItem("name").getNodeValue();
				String currentYear = String.valueOf(Calendar.getInstance().get(Calendar.YEAR));
				String period = doc.getDocumentElement().getAttributes().getNamedItem("active").getNodeValue();
				if ((doc.getDocumentElement().getAttributes().getNamedItem("enableShrines") != null) && doc.getDocumentElement().getAttributes().getNamedItem("enableShrines").getNodeValue().equalsIgnoreCase("true"))
				{
					_enableShrines = true;
				}
				if (period.length() == 21)
				{
					// dd MM yyyy-dd MM yyyy
					_eventPeriod = DateRange.parse(period, new SimpleDateFormat("dd MM yyyy", Locale.US));
				}
				else if (period.length() == 11)
				{
					// dd MM-dd MM
					String start = period.split("-")[0].concat(" ").concat(currentYear);
					String end = period.split("-")[1].concat(" ").concat(currentYear);
					String activePeriod = start.concat("-").concat(end);
					_eventPeriod = DateRange.parse(activePeriod, new SimpleDateFormat("dd MM yyyy", Locale.US));
				}
				
				if (_eventPeriod == null)
				{
					throw new NullPointerException("WARNING!!! " + getName() + " event: illegal event period");
				}
				
				Date today = new Date();
				
				if (_eventPeriod.getStartDate().after(today) || _eventPeriod.isWithinRange(today))
				{
					for (Node n = doc.getDocumentElement().getFirstChild(); n != null; n = n.getNextSibling())
					{
						// Loading droplist
						if (n.getNodeName().equalsIgnoreCase("droplist"))
						{
							for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
							{
								if (d.getNodeName().equalsIgnoreCase("add"))
								{
									try
									{
										int itemId = int.parseInt(d.getAttributes().getNamedItem("item").getNodeValue());
										int minCount = int.parseInt(d.getAttributes().getNamedItem("min").getNodeValue());
										int maxCount = int.parseInt(d.getAttributes().getNamedItem("max").getNodeValue());
										String chance = d.getAttributes().getNamedItem("chance").getNodeValue();
										double finalChance = !chance.isEmpty() && chance.endsWith("%") ? Double.parseDouble(chance.substring(0, chance.length() - 1)) : 0;
										Node minLevelNode = d.getAttributes().getNamedItem("minLevel");
										int minLevel = minLevelNode == null ? 1 : int.parseInt(minLevelNode.getNodeValue());
										Node maxLevelNode = d.getAttributes().getNamedItem("maxLevel");
										int maxLevel = maxLevelNode == null ? int.MAX_VALUE : int.parseInt(maxLevelNode.getNodeValue());
										Node monsterIdsNode = d.getAttributes().getNamedItem("monsterIds");
										Set<int> monsterIds = new HashSet<>();
										if (monsterIdsNode != null)
										{
											for (String id : monsterIdsNode.getNodeValue().split(","))
											{
												monsterIds.add(int.parseInt(id));
											}
										}
										
										if (ItemData.getInstance().getTemplate(itemId) == null)
										{
											LOGGER.warning(getName() + " event: " + itemId + " is wrong item id, item was not added in droplist");
											continue;
										}
										
										if (minCount > maxCount)
										{
											LOGGER.warning(getName() + " event: item " + itemId + " - min greater than max, item was not added in droplist");
											continue;
										}
										
										if ((finalChance < 0) || (finalChance > 100))
										{
											LOGGER.warning(getName() + " event: item " + itemId + " - incorrect drop chance, item was not added in droplist");
											continue;
										}
										
										_dropList.add(new EventDropHolder(itemId, minCount, maxCount, finalChance, minLevel, maxLevel, monsterIds));
									}
									catch (NumberFormatException nfe)
									{
										LOGGER.warning("Wrong number format in config.xml droplist block for " + getName() + " event");
									}
								}
							}
						}
						else if (n.getNodeName().equalsIgnoreCase("spawnlist"))
						{
							// Loading spawnlist
							for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
							{
								if (d.getNodeName().equalsIgnoreCase("add"))
								{
									try
									{
										int npcId = int.parseInt(d.getAttributes().getNamedItem("npc").getNodeValue());
										int xPos = int.parseInt(d.getAttributes().getNamedItem("x").getNodeValue());
										int yPos = int.parseInt(d.getAttributes().getNamedItem("y").getNodeValue());
										int zPos = int.parseInt(d.getAttributes().getNamedItem("z").getNodeValue());
										Node headingNode = d.getAttributes().getNamedItem("heading");
										String headingValue = headingNode == null ? null : headingNode.getNodeValue();
										int heading = headingValue != null ? int.parseInt(headingValue) : 0;
										Node respawnTimeNode = d.getAttributes().getNamedItem("respawnTime");
										String respawnTimeValue = respawnTimeNode == null ? null : respawnTimeNode.getNodeValue();
										Duration respawnTime = TimeUtil.parseDuration(respawnTimeValue != null ? respawnTimeValue : "0sec");
										
										if (NpcData.getInstance().getTemplate(npcId) == null)
										{
											LOGGER.warning(getName() + " event: " + npcId + " is wrong NPC id, NPC was not added in spawnlist");
											continue;
										}
										
										_spawnList.add(new NpcSpawn(npcId, new Location(xPos, yPos, zPos, heading), respawnTime));
									}
									catch (NumberFormatException nfe)
									{
										LOGGER.warning("Wrong number format in config.xml spawnlist block for " + getName() + " event");
									}
								}
							}
						}
						else if (n.getNodeName().equalsIgnoreCase("messages"))
						{
							// Loading Messages
							for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
							{
								if (d.getNodeName().equalsIgnoreCase("add"))
								{
									String msgType = d.getAttributes().getNamedItem("type").getNodeValue();
									String msgText = d.getAttributes().getNamedItem("text").getNodeValue();
									if ((msgType != null) && (msgText != null))
									{
										if (msgType.equalsIgnoreCase("onEnd"))
										{
											_endMsg = msgText;
										}
										else if (msgType.equalsIgnoreCase("onEnter"))
										{
											_onEnterMsg = msgText;
										}
									}
								}
							}
						}
					}
				}
				
				// Load destroy item list at all times.
				for (Node n = doc.getDocumentElement().getFirstChild(); n != null; n = n.getNextSibling())
				{
					if (n.getNodeName().equalsIgnoreCase("destroyItemsOnEnd"))
					{
						long endtime = _eventPeriod.getEndDate().getTime();
						for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
						{
							if (d.getNodeName().equalsIgnoreCase("item"))
							{
								try
								{
									int itemId = int.parseInt(d.getAttributes().getNamedItem("id").getNodeValue());
									if (ItemData.getInstance().getTemplate(itemId) == null)
									{
										LOGGER.warning(getScriptName() + " event: Item " + itemId + " does not exist.");
										continue;
									}
									_destroyItemsOnEnd.add(itemId);
									
									// Add item deletion info to manager.
									if (endtime > System.currentTimeMillis())
									{
										ItemDeletionInfoManager.getInstance().addItemDate(itemId, (int) (endtime / 1000));
									}
								}
								catch (NumberFormatException nfe)
								{
									LOGGER.warning("Wrong number format in config.xml destroyItemsOnEnd block for " + getScriptName() + " event");
								}
							}
						}
					}
				}
			}
		}.load();
	}
	
	protected class ScheduleStart: Runnable
	{
		public void run()
		{
			startEvent();
		}
	}
	
	protected void startEvent()
	{
		// Set Active.
		_active = true;
		
		// Add event drops.
		EventDropManager.getInstance().addDrops(this, _dropList);
		
		// Add spawns on server start.
		if (!_spawnList.isEmpty())
		{
			Containers.Global().addListener(new ConsumerEventListener(Containers.Global(), EventType.ON_SERVER_START, _spawnNpcs, this));
		}
		
		// Enable town shrines.
		if (_enableShrines)
		{
			EventShrineManager.getInstance().setEnabled(true);
		}
		
		// Event enter announcement.
		if (!_onEnterMsg.isEmpty())
		{
			// Send message on begin.
			Broadcast.toAllOnlinePlayers(_onEnterMsg);
			
			// Add announce for entering players.
			EventAnnouncement announce = new EventAnnouncement(_eventPeriod, _onEnterMsg);
			AnnouncementsTable.getInstance().addAnnouncement(announce);
			_enterAnnounceId = announce.getId();
		}
		
		// Schedule event end.
		Long millisToEventEnd = _eventPeriod.getEndDate().getTime() - System.currentTimeMillis();
		ThreadPool.schedule(new ScheduleEnd(), millisToEventEnd);
	}
	
	/**
	 * Event spawns must initialize after server loads scripts.
	 */
	private Action<OnServerStart> _spawnNpcs = ev =>
	{
		Long millisToEventEnd = _eventPeriod.getEndDate().getTime() - System.currentTimeMillis();
		foreach (NpcSpawn npcSpawn in _spawnList)
		{
			Npc npc = addSpawn(npcSpawn.npcId, npcSpawn.loc.getX(), npcSpawn.loc.getY(), npcSpawn.loc.getZ(), npcSpawn.loc.getHeading(), false, millisToEventEnd, false);
			int respawnDelay = (int) npcSpawn.respawnTime.toMillis();
			if (respawnDelay > 0)
			{
				Spawn spawn = npc.getSpawn();
				spawn.setRespawnDelay(respawnDelay);
				spawn.startRespawn();
				ThreadPool.schedule(spawn::stopRespawn, millisToEventEnd - respawnDelay);
			}
		}
		
		Containers.Global().removeListenerIf(EventType.ON_SERVER_START, listener => listener.getOwner() == this);
	};
	
	protected class ScheduleEnd: Runnable
	{
		public void run()
		{
			stopEvent();
		}
	}
	
	protected void stopEvent()
	{
		// Set Active.
		_active = false;
		
		// Stop event drops.
		EventDropManager.getInstance().removeDrops(this);
		
		// Disable town shrines.
		if (_enableShrines)
		{
			EventShrineManager.getInstance().setEnabled(false);
		}
		
		// Destroy items that must exist only on event period.
		destroyItemsOnEnd();
		
		// Send message on end.
		if (!_endMsg.isEmpty())
		{
			Broadcast.toAllOnlinePlayers(_endMsg);
		}
		
		// Remove announce for entering players.
		if (_enterAnnounceId != -1)
		{
			AnnouncementsTable.getInstance().deleteAnnouncement(_enterAnnounceId);
		}
	}
	
	protected void destroyItemsOnEnd()
	{
		if (!_destroyItemsOnEnd.isEmpty())
		{
			foreach (int itemId in _destroyItemsOnEnd)
			{
				// Remove item from online players.
				foreach (Player player in World.getInstance().getPlayers())
				{
					if (player != null)
					{
						player.destroyItemByItemId(_eventName, itemId, -1, player, true);
					}
				}
				// Update database.
				try
				{
					using GameServerDbContext ctx = new();
					PreparedStatement statement = con.prepareStatement("DELETE FROM items WHERE item_id=?");
					statement.setInt(1, itemId);
					statement.execute();
				}
				catch (Exception e)
				{
					LOGGER.Warn(e);
				}
			}
		}
	}
	
	public DateRange getEventPeriod()
	{
		return _eventPeriod;
	}
	
	/**
	 * @return {@code true} if now is event period
	 */
	public bool isEventPeriod()
	{
		return _active;
	}
}