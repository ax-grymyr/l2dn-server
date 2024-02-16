using System.Runtime.CompilerServices;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers.Tasks;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.WalkerTasks;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * This class manages walking monsters.
 * @author GKR
 */
public class WalkingManager: IXmlReader
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(WalkingManager));
	
	// Repeat style:
	// -1 - no repeat
	// 0 - go back
	// 1 - go to first point (circle style)
	// 2 - teleport to first point (conveyor style)
	// 3 - random walking between points.
	public const byte NO_REPEAT = 255;
	public const byte REPEAT_GO_BACK = 0;
	public const byte REPEAT_GO_FIRST = 1;
	public const byte REPEAT_TELE_FIRST = 2;
	public const byte REPEAT_RANDOM = 3;
	
	private readonly Set<int> _targetedNpcIds = new();
	private readonly Map<String, WalkRoute> _routes = new(); // all available routes
	private readonly Map<int, WalkInfo> _activeRoutes = new(); // each record represents NPC, moving by predefined route from _routes, and moving progress
	private readonly Map<int, NpcRoutesHolder> _routesToAttach = new(); // each record represents NPC and all available routes for it
	private readonly Map<Npc, ScheduledFuture> _startMoveTasks = new();
	private readonly Map<Npc, ScheduledFuture> _repeatMoveTasks = new();
	private readonly Map<Npc, ScheduledFuture> _arriveTasks = new();
	
	protected WalkingManager()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackFile("data/Routes.xml");
		LOGGER.Info(GetType().Name +": Loaded " + _routes.size() + " walking routes.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node d = doc.getFirstChild().getFirstChild(); d != null; d = d.getNextSibling())
		{
			if (d.getNodeName().equals("route"))
			{
				String routeName = parseString(d.getAttributes(), "name");
				bool repeat = parseBoolean(d.getAttributes(), "repeat");
				String repeatStyle = d.getAttributes().getNamedItem("repeatStyle").getNodeValue().toLowerCase();
				byte repeatType;
				switch (repeatStyle)
				{
					case "back":
					{
						repeatType = REPEAT_GO_BACK;
						break;
					}
					case "cycle":
					{
						repeatType = REPEAT_GO_FIRST;
						break;
					}
					case "conveyor":
					{
						repeatType = REPEAT_TELE_FIRST;
						break;
					}
					case "random":
					{
						repeatType = REPEAT_RANDOM;
						break;
					}
					default:
					{
						repeatType = NO_REPEAT;
						break;
					}
				}
				
				List<NpcWalkerNode> list = new();
				for (Node r = d.getFirstChild(); r != null; r = r.getNextSibling())
				{
					if (r.getNodeName().equals("point"))
					{
						NamedNodeMap attrs = r.getAttributes();
						int x = parseInteger(attrs, "X");
						int y = parseInteger(attrs, "Y");
						int z = parseInteger(attrs, "Z");
						int delay = parseInteger(attrs, "delay");
						bool run = parseBoolean(attrs, "run");
						NpcStringId npcString = null;
						String chatString = null;
						Node node = attrs.getNamedItem("string");
						if (node != null)
						{
							chatString = node.getNodeValue();
						}
						else
						{
							node = attrs.getNamedItem("npcString");
							if (node != null)
							{
								npcString = NpcStringId.getNpcStringId(node.getNodeValue());
								if (npcString == null)
								{
									LOGGER.Warn(GetType().Name + ": Unknown npcString '" + node.getNodeValue() + "' for route '" + routeName + "'");
									continue;
								}
							}
							else
							{
								node = attrs.getNamedItem("npcStringId");
								if (node != null)
								{
									npcString = NpcStringId.getNpcStringId(int.Parse(node.getNodeValue()));
									if (npcString == null)
									{
										LOGGER.Warn(GetType().Name + ": Unknown npcString '" + node.getNodeValue() + "' for route '" + routeName + "'");
										continue;
									}
								}
							}
						}
						list.add(new NpcWalkerNode(x, y, z, delay, run, npcString, chatString));
					}
					
					else if (r.getNodeName().equals("target"))
					{
						NamedNodeMap attrs = r.getAttributes();
						try
						{
							int npcId = int.Parse(attrs.getNamedItem("id").getNodeValue());
							int x = int.Parse(attrs.getNamedItem("spawnX").getNodeValue());
							int y = int.Parse(attrs.getNamedItem("spawnY").getNodeValue());
							int z = int.Parse(attrs.getNamedItem("spawnZ").getNodeValue());
							if (NpcData.getInstance().getTemplate(npcId) != null)
							{
								NpcRoutesHolder holder = _routesToAttach.containsKey(npcId) ? _routesToAttach.get(npcId) : new NpcRoutesHolder();
								holder.addRoute(routeName, new Location(x, y, z));
								_routesToAttach.put(npcId, holder);
								
								if (!_targetedNpcIds.Contains(npcId))
								{
									_targetedNpcIds.add(npcId);
								}
							}
							else
							{
								LOGGER.Warn(GetType().Name + ": NPC with id " + npcId + " for route '" + routeName + "' does not exist.");
							}
						}
						catch (Exception e)
						{
							LOGGER.Warn(GetType().Name + ": Error in target definition for route '" + routeName + "'");
						}
					}
				}
				_routes.put(routeName, new WalkRoute(routeName, list, repeat, repeatType));
			}
		}
	}
	
	/**
	 * @param npc NPC to check
	 * @return {@code true} if given NPC, or its leader is controlled by Walking Manager and moves currently.
	 */
	public bool isOnWalk(Npc npc)
	{
		Monster monster = npc.isMonster() ? ((Monster) npc).getLeader() == null ? (Monster) npc : ((Monster) npc).getLeader() : null;
		if (((monster != null) && !isRegistered(monster)) || !isRegistered(npc))
		{
			return false;
		}
		
		WalkInfo walk = monster != null ? _activeRoutes.get(monster.getObjectId()) : _activeRoutes.get(npc.getObjectId());
		return !walk.isStoppedByAttack() && !walk.isSuspended();
	}
	
	public WalkRoute getRoute(String route)
	{
		return _routes.get(route);
	}
	
	/**
	 * @param npc NPC to check
	 * @return {@code true} if given NPC id is registered as a route target.
	 */
	public bool isTargeted(Npc npc)
	{
		return _targetedNpcIds.Contains(npc.getId());
	}
	
	/**
	 * @param npc NPC to check
	 * @return {@code true} if given NPC controlled by Walking Manager.
	 */
	private bool isRegistered(Npc npc)
	{
		return _activeRoutes.containsKey(npc.getObjectId());
	}
	
	/**
	 * @param npc
	 * @return name of route
	 */
	public String getRouteName(Npc npc)
	{
		return _activeRoutes.containsKey(npc.getObjectId()) ? _activeRoutes.get(npc.getObjectId()).getRoute().getName() : "";
	}
	
	/**
	 * Start to move given NPC by given route
	 * @param npc NPC to move
	 * @param routeName name of route to move by
	 */
	public void startMoving(Npc npc, String routeName)
	{
		if (_routes.containsKey(routeName) && (npc != null) && !npc.isDead()) // check, if these route and NPC present
		{
			if (!_activeRoutes.containsKey(npc.getObjectId())) // new walk task
			{
				// only if not already moved / not engaged in battle... should not happens if called on spawn
				if ((npc.getAI().getIntention() == CtrlIntention.AI_INTENTION_ACTIVE) || (npc.getAI().getIntention() == CtrlIntention.AI_INTENTION_IDLE))
				{
					WalkInfo walk = new WalkInfo(routeName);
					NpcWalkerNode node = walk.getCurrentNode();
					
					// adjust next waypoint, if NPC spawns at first waypoint
					if ((npc.getX() == node.getX()) && (npc.getY() == node.getY()))
					{
						walk.calculateNextNode(npc);
						node = walk.getCurrentNode();
					}
					
					if (!npc.isInsideRadius3D(node, 3000))
					{
						LOGGER.Warn(GetType().Name + ": " + "Route '" + routeName + "': NPC (id=" + npc.getId() + ", x=" + npc.getX() + ", y=" + npc.getY() + ", z=" + npc.getZ() + ") is too far from starting point (node x=" + node.getX() + ", y=" + node.getY() + ", z=" + node.getZ() + ", range=" + npc.calculateDistance3D(node) + "). Teleporting to proper location.");
						npc.teleToLocation(node);
					}
					
					if (node.runToLocation())
					{
						npc.setRunning();
					}
					else
					{
						npc.setWalking();
					}
					npc.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, node);
					
					ScheduledFuture task = _repeatMoveTasks.get(npc);
					if ((task == null) || task.isCancelled() || task.isDone())
					{
						ScheduledFuture newTask = ThreadPool.scheduleAtFixedRate(new StartMovingTask(npc, routeName), 10000, 10000);
						_repeatMoveTasks.put(npc, newTask);
						walk.setWalkCheckTask(newTask); // start walk check task, for resuming walk after fight
					}
					
					npc.setWalker();
					_activeRoutes.put(npc.getObjectId(), walk); // register route
				}
				else
				{
					ScheduledFuture task = _startMoveTasks.get(npc);
					if ((task == null) || task.isCancelled() || task.isDone())
					{
						_startMoveTasks.put(npc, ThreadPool.schedule(new StartMovingTask(npc, routeName), 10000));
					}
				}
			}
			else // walk was stopped due to some reason (arrived to node, script action, fight or something else), resume it
			{
				if (_activeRoutes.containsKey(npc.getObjectId()) && ((npc.getAI().getIntention() == CtrlIntention.AI_INTENTION_ACTIVE) || (npc.getAI().getIntention() == CtrlIntention.AI_INTENTION_IDLE)))
				{
					WalkInfo walk = _activeRoutes.get(npc.getObjectId());
					if (walk == null)
					{
						return;
					}
					
					// Prevent call simultaneously from scheduled task and onArrived() or temporarily stop walking for resuming in future
					if (walk.isBlocked() || walk.isSuspended())
					{
						return;
					}
					
					walk.setBlocked(true);
					NpcWalkerNode node = walk.getCurrentNode();
					if (node.runToLocation())
					{
						npc.setRunning();
					}
					else
					{
						npc.setWalking();
					}
					npc.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, node);
					walk.setBlocked(false);
					walk.setStoppedByAttack(false);
				}
			}
		}
	}
	
	/**
	 * Cancel NPC moving permanently
	 * @param npc NPC to cancel
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void cancelMoving(Npc npc)
	{
		WalkInfo walk = _activeRoutes.remove(npc.getObjectId());
		if (walk != null)
		{
			ScheduledFuture task = walk.getWalkCheckTask();
			if (task != null)
			{
				task.cancel(true);
			}
		}
	}
	
	/**
	 * Resumes previously stopped moving
	 * @param npc NPC to resume
	 */
	public void resumeMoving(Npc npc)
	{
		WalkInfo walk = _activeRoutes.get(npc.getObjectId());
		if (walk != null)
		{
			walk.setSuspended(false);
			walk.setStoppedByAttack(false);
			startMoving(npc, walk.getRoute().getName());
		}
	}
	
	/**
	 * Pause NPC moving until it will be resumed
	 * @param npc NPC to pause moving
	 * @param suspend {@code true} if moving was temporarily suspended for some reasons of AI-controlling script
	 * @param stoppedByAttack {@code true} if moving was suspended because of NPC was attacked or desired to attack
	 */
	public void stopMoving(Npc npc, bool suspend, bool stoppedByAttack)
	{
		Monster monster = npc.isMonster() ? ((Monster) npc).getLeader() == null ? (Monster) npc : ((Monster) npc).getLeader() : null;
		if (((monster != null) && !isRegistered(monster)) || !isRegistered(npc))
		{
			return;
		}
		
		WalkInfo walk = monster != null ? _activeRoutes.get(monster.getObjectId()) : _activeRoutes.get(npc.getObjectId());
		walk.setSuspended(suspend);
		walk.setStoppedByAttack(stoppedByAttack);
		
		if (monster != null)
		{
			monster.stopMove(null);
			monster.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}
		else
		{
			npc.stopMove(null);
			npc.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}
	}
	
	/**
	 * Manage "node arriving"-related tasks: schedule move to next node; send ON_NODE_ARRIVED event to Quest script
	 * @param npc NPC to manage
	 */
	public void onArrived(Npc npc)
	{
		if (!_activeRoutes.containsKey(npc.getObjectId()))
		{
			return;
		}
		
		// Notify quest
		if (EventDispatcher.getInstance().hasListener(EventType.ON_NPC_MOVE_NODE_ARRIVED, npc))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnNpcMoveNodeArrived(npc), npc);
		}
		
		WalkInfo walk = _activeRoutes.get(npc.getObjectId());
		// Opposite should not happen... but happens sometime
		if ((walk.getCurrentNodeId() < 0) || (walk.getCurrentNodeId() >= walk.getRoute().getNodesCount()))
		{
			return;
		}
		
		List<NpcWalkerNode> nodelist = walk.getRoute().getNodeList();
		NpcWalkerNode node = nodelist.get(Math.Min(walk.getCurrentNodeId(), nodelist.size() - 1));
		if (!npc.isInsideRadius2D(node, 10))
		{
			return;
		}
		
		walk.calculateNextNode(npc);
		walk.setBlocked(true); // prevents to be ran from walk check task, if there is delay in this node.
		if (node.getNpcString() != null)
		{
			npc.broadcastSay(ChatType.NPC_GENERAL, node.getNpcString());
		}
		else if (!node.getChatText().isEmpty())
		{
			npc.broadcastSay(ChatType.NPC_GENERAL, node.getChatText());
		}
		
		ScheduledFuture task = _arriveTasks.get(npc);
		if ((task == null) || task.isCancelled() || task.isDone())
		{
			_arriveTasks.put(npc, ThreadPool.schedule(new ArrivedTask(npc, walk), 100 + (node.getDelay() * 1000)));
		}
	}
	
	/**
	 * Manage "on death"-related tasks: permanently cancel moving of died NPC
	 * @param npc NPC to manage
	 */
	public void onDeath(Npc npc)
	{
		cancelMoving(npc);
	}
	
	/**
	 * Manage "on spawn"-related tasks: start NPC moving, if there is route attached to its spawn point
	 * @param npc NPC to manage
	 */
	public void onSpawn(Npc npc)
	{
		if (_routesToAttach.containsKey(npc.getId()))
		{
			String routeName = _routesToAttach.get(npc.getId()).getRouteName(npc);
			if (!routeName.isEmpty())
			{
				startMoving(npc, routeName);
			}
		}
	}
	
	public static WalkingManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly WalkingManager INSTANCE = new WalkingManager();
	}
}