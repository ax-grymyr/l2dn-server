using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author GodKratos, DS
 */
public class OlympiadGameManager: Runnable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(OlympiadGameManager));
	private const int STADIUM_COUNT = 80; // TODO dynamic
	
	private volatile bool _battleStarted = false;
	private readonly List<OlympiadStadium> _tasks;
	private int _delay = 0;
	
	protected OlympiadGameManager()
	{
		ICollection<OlympiadStadiumZone> zones = ZoneManager.getInstance().getAllZones<OlympiadStadiumZone>();
		if ((zones == null) || zones.isEmpty())
		{
			throw new InvalidOperationException("No olympiad stadium zones defined !");
		}
		
		OlympiadStadiumZone[] array = zones.ToArray();
		_tasks = new(STADIUM_COUNT);
		
		int zonesCount = array.Length;
		for (int i = 0; i < STADIUM_COUNT; i++)
		{
			OlympiadStadium stadium = new OlympiadStadium(array[i % zonesCount], i);
			stadium.registerTask(new OlympiadGameTask(stadium));
			_tasks.add(stadium);
		}
		
		LOGGER.Info("Olympiad System: Loaded " + _tasks.Count + " stadiums.");
	}
	
	public bool isBattleStarted()
	{
		return _battleStarted;
	}
	
	public void startBattle()
	{
		_battleStarted = true;
	}
	
	public void run()
	{
		if (Olympiad.getInstance().isOlympiadEnd())
		{
			return;
		}
		
		if (Olympiad.getInstance().inCompPeriod())
		{
			AbstractOlympiadGame newGame;
			List<Set<int>> readyClassed = OlympiadManager.getInstance().hasEnoughRegisteredClassed();
			bool readyNonClassed = OlympiadManager.getInstance().hasEnoughRegisteredNonClassed();
			if ((readyClassed != null) || readyNonClassed)
			{
				// reset delay broadcast
				_delay = 0;
				
				// set up the games queue
				for (int i = 0; i < _tasks.Count; i++)
				{
					OlympiadGameTask task = _tasks[i].getTask();
					lock (task)
					{
						if (!task.isRunning())
						{
							// Fair arena distribution
							// 0,2,4,6,8.. arenas checked for classed or teams first
							if (readyClassed != null)
							{
								newGame = OlympiadGameClassed.createGame(i, readyClassed);
								if (newGame != null)
								{
									task.attachGame(newGame);
									continue;
								}
								readyClassed = null;
							}
							// 1,3,5,7,9.. arenas used for non-classed
							// also other arenas will be used for non-classed if no classed or teams available
							if (readyNonClassed)
							{
								newGame = OlympiadGameNonClassed.createGame(i, OlympiadManager.getInstance().getRegisteredNonClassBased());
								if (newGame != null)
								{
									task.attachGame(newGame);
									continue;
								}
								readyNonClassed = false;
							}
						}
					}
					
					// stop generating games if no more participants
					if ((readyClassed == null) && !readyNonClassed)
					{
						break;
					}
				}
			}
			// olympiad is delayed
			else
			{
				_delay++;
				if (_delay >= 10) // 5min
				{
					foreach (int id in OlympiadManager.getInstance().getRegisteredNonClassBased())
					{
						if (id == null)
						{
							continue;
						}
						
						Player noble = World.getInstance().getPlayer(id);
						if (noble != null)
						{
							noble.sendPacket(new SystemMessagePacket(SystemMessageId.THE_GAMES_MAY_BE_DELAYED_DUE_TO_AN_INSUFFICIENT_NUMBER_OF_PLAYERS_WAITING));
						}
					}
					
					foreach (Set<int> list in OlympiadManager.getInstance().getRegisteredClassBased().values())
					{
						foreach (int id in list)
						{
							if (id == null)
							{
								continue;
							}
							
							Player noble = World.getInstance().getPlayer(id);
							if (noble != null)
							{
								noble.sendPacket(new SystemMessagePacket(SystemMessageId.THE_GAMES_MAY_BE_DELAYED_DUE_TO_AN_INSUFFICIENT_NUMBER_OF_PLAYERS_WAITING));
							}
						}
					}
					
					_delay = 0;
				}
			}
		}
		else
		{
			// not in competition period
			if (isAllTasksFinished())
			{
				OlympiadManager.getInstance().clearRegistered();
				_battleStarted = false;
				// LOGGER.info("Olympiad System: All current games finished.");
			}
		}
	}
	
	public bool isAllTasksFinished()
	{
		foreach (OlympiadStadium stadium in _tasks)
		{
			OlympiadGameTask task = stadium.getTask();
			if (task.isRunning())
			{
				return false;
			}
		}
		return true;
	}
	
	public OlympiadGameTask getOlympiadTask(int id)
	{
		if ((id < 0) || (id >= _tasks.Count))
		{
			return null;
		}
		return _tasks[id].getTask();
	}
	
	public int getNumberOfStadiums()
	{
		return _tasks.Count;
	}
	
	public void notifyCompetitorDamage(Player attacker, int damage)
	{
		if (attacker == null)
		{
			return;
		}
		
		int id = attacker.getOlympiadGameId();
		if ((id < 0) || (id >= _tasks.Count))
		{
			return;
		}
		
		AbstractOlympiadGame game = _tasks[id].getTask().getGame();
		if (game != null)
		{
			game.addDamage(attacker, damage);
		}
	}
	
	public List<OlympiadStadium> getTasks()
	{
		return _tasks;
	}
	
	public static OlympiadGameManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static OlympiadGameManager INSTANCE = new OlympiadGameManager();
	}
}