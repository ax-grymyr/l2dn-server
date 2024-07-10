using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers.Tasks;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.WalkerTasks;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * This class manages walking monsters.
 * @author GKR
 */
public class WalkingManager: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(WalkingManager));
	
	// Repeat style:
	// -1 - no repeat
	// 0 - go back
	// 1 - go to first point (circle style)
	// 2 - teleport to first point (conveyor style)
	// 3 - random walking between points.
	public const byte NO_REPEAT = 255; // TODO: enum
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
		LoadXmlDocument(DataFileLocation.Data, "Routes.xml").Elements("routes").Elements("route").ForEach(parseRoute);
		LOGGER.Info(GetType().Name +": Loaded " + _routes.size() + " walking routes.");
	}

	private void parseRoute(XElement element)
	{
		string routeName = element.GetAttributeValueAsString("name");
		bool repeat = element.GetAttributeValueAsBoolean("repeat");
		string repeatStyle = element.GetAttributeValueAsString("repeatStyle").ToLower();
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
		foreach (XElement r in element.Elements())
		{
			if (r.Name.LocalName.equals("point"))
			{
				int x = r.GetAttributeValueAsInt32("X");
				int y = r.GetAttributeValueAsInt32("Y");
				int z = r.GetAttributeValueAsInt32("Z");
				int delay = r.GetAttributeValueAsInt32("delay");
				bool run = r.GetAttributeValueAsBoolean("run");
				string? chatString = r.Attribute("string")?.GetString();
				NpcStringId? npcString = (NpcStringId?)r.Attribute("npcString")?.GetInt32();
				if (npcString is null)
					npcString = (NpcStringId?)r.Attribute("npcStringId")?.GetInt32();

				if (npcString is not null && !Enum.IsDefined(npcString.Value))
				{
					LOGGER.Error(GetType().Name + ": Unknown npcString '" + (int)npcString.Value + "' for route '" +
					             routeName + "'");
					continue;
				}

				list.Add(new NpcWalkerNode(new Location3D(x, y, z), delay, run, npcString ?? 0, chatString));
			}
			else if (r.Name.LocalName.equals("target"))
			{
				try
				{
					int npcId = r.GetAttributeValueAsInt32("id");
					int x = r.GetAttributeValueAsInt32("spawnX");
					int y = r.GetAttributeValueAsInt32("spawnY");
					int z = r.GetAttributeValueAsInt32("spawnZ");
					if (NpcData.getInstance().getTemplate(npcId) != null)
					{
						NpcRoutesHolder holder = _routesToAttach.GetValueOrDefault(npcId) ?? new NpcRoutesHolder();
						holder.addRoute(routeName, new Location3D(x, y, z));
						_routesToAttach[npcId] = holder;

						if (!_targetedNpcIds.Contains(npcId))
							_targetedNpcIds.Add(npcId);
					}
					else
					{
						LOGGER.Warn(GetType().Name + ": NPC with id " + npcId + " for route '" + routeName +
						            "' does not exist.");
					}
				}
				catch (Exception e)
				{
					LOGGER.Error(GetType().Name + ": Error in target definition for route '" + routeName + "': " + e);
				}
			}
		}

		_routes.put(routeName, new WalkRoute(routeName, list, repeat, repeatType));
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
					if ((npc.getX() == node.Location.X) && (npc.getY() == node.Location.Y))
					{
						walk.calculateNextNode(npc);
						node = walk.getCurrentNode();
					}
					
					if (!npc.IsInsideRadius3D(node.Location, 3000))
					{
						LOGGER.Warn(GetType().Name + ": " + "Route '" + routeName + "': NPC (id=" + npc.getId() +
							", x=" + npc.getX() + ", y=" + npc.getY() + ", z=" + npc.getZ() +
							") is too far from starting point (node x=" + node.Location.X + ", y=" + node.Location.Y +
							", z=" + node.Location.Z + ", range=" + npc.Distance3D(node.Location) +
							"). Teleporting to proper location.");

						Location teleLoc = new(node.Location, npc.getHeading());
						npc.teleToLocation(teleLoc);
					}
					
					if (node.runToLocation())
					{
						npc.setRunning();
					}
					else
					{
						npc.setWalking();
					}
					npc.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, node.Location);
					
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
					npc.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, node.Location);
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
		if (npc.Events.HasSubscribers<OnNpcMoveNodeArrived>())
		{
			npc.Events.NotifyAsync(new OnNpcMoveNodeArrived(npc));
		}
		
		WalkInfo walk = _activeRoutes.get(npc.getObjectId());
		// Opposite should not happen... but happens sometime
		if ((walk.getCurrentNodeId() < 0) || (walk.getCurrentNodeId() >= walk.getRoute().getNodesCount()))
		{
			return;
		}
		
		List<NpcWalkerNode> nodelist = walk.getRoute().getNodeList();
		NpcWalkerNode node = nodelist[Math.Min(walk.getCurrentNodeId(), nodelist.Count - 1)];
		if (!npc.IsInsideRadius2D(node.Location, 10))
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