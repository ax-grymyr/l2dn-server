using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author DS
 */
public class OlympiadGameTask: Runnable
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(OlympiadGameTask));

	private static readonly int[] TELEPORT_TO_ARENA_TIMES = [120, 60, 30, 15, 10, 5, 4, 3, 2, 1, 0];
	private static readonly int[] BATTLE_START_TIME_FIRST = [60, 55, 50, 40, 30, 20, 10, 0];
	private static readonly int[] BATTLE_START_TIME_SECOND = [10, 5, 4, 3, 2, 1, 0];
	private static readonly int[] BATTLE_END_TIME_SECOND = [120, 60, 30, 10, 5];
	private static readonly int[] TELEPORT_TO_TOWN_TIMES = [40, 30, 20, 10, 5, 4, 3, 2, 1, 0];

	private readonly OlympiadStadium _stadium;
	private AbstractOlympiadGame _game;
	private OlympiadGameState _state = OlympiadGameState.IDLE;
	private bool _needAnnounce = false;
	private int _countDown = 0;

	public OlympiadGameTask(OlympiadStadium stadium)
	{
		_stadium = stadium;
		_stadium.registerTask(this);
	}

	public bool isRunning()
	{
		return _state != OlympiadGameState.IDLE;
	}

	public bool isGameStarted()
	{
		return _state >= OlympiadGameState.GAME_STARTED && _state <= OlympiadGameState.CLEANUP;
	}

	public bool isBattleStarted()
	{
		return _state == OlympiadGameState.BATTLE_IN_PROGRESS;
	}

	public bool isBattleFinished()
	{
		return _state == OlympiadGameState.TELEPORT_TO_TOWN;
	}

	public bool needAnnounce()
	{
		if (_needAnnounce)
		{
			_needAnnounce = false;
			return true;
		}

		return false;
	}

	public OlympiadStadium getStadium()
	{
		return _stadium;
	}

	public AbstractOlympiadGame getGame()
	{
		return _game;
	}

	public void attachGame(AbstractOlympiadGame game)
	{
		if (game != null && _state != OlympiadGameState.IDLE)
		{
			LOGGER.Warn("Attempt to overwrite non-finished game in state " + _state);
			return;
		}

		_game = game;
		_state = OlympiadGameState.BEGIN;
		_needAnnounce = false;
		ThreadPool.execute(this);
	}

	public void run()
	{
		try
		{
			int delay = 1; // schedule next call after 1s
			switch (_state)
			{
				// Game created
				case OlympiadGameState.BEGIN:
				{
					_state = OlympiadGameState.TELEPORT_TO_ARENA;
					_countDown = Config.ALT_OLY_WAIT_TIME;
					break;
				}
				// Teleport to arena countdown
				case OlympiadGameState.TELEPORT_TO_ARENA:
				{
					if (_countDown > 0)
					{
						SystemMessagePacket sm =
							new SystemMessagePacket(SystemMessageId.YOU_WILL_BE_TAKEN_TO_THE_OLYMPIC_STADIUM_IN_S1_SEC);
						sm.Params.addInt(_countDown);
						_game.broadcastPacket(sm);
					}

					if (_countDown == 1)
					{
						_game.untransformPlayers();
					}

					delay = getDelay(TELEPORT_TO_ARENA_TIMES);
					if (_countDown <= 0)
					{
						_state = OlympiadGameState.GAME_STARTED;
					}

					break;
				}
				// Game start, port players to arena
				case OlympiadGameState.GAME_STARTED:
				{
					if (!startGame())
					{
						_state = OlympiadGameState.GAME_STOPPED;
						break;
					}

					_state = OlympiadGameState.BATTLE_COUNTDOWN_FIRST;
					_countDown = BATTLE_START_TIME_FIRST[0];
					_stadium
						.updateZoneInfoForObservers(); // TODO lion temp hack for remove old info from client about prevoius match
					delay = 5;
					break;
				}
				// Battle start countdown, first part (60-10)
				case OlympiadGameState.BATTLE_COUNTDOWN_FIRST:
				{
					if (_countDown > 0)
					{
						if (_countDown == 55) // 55sec
						{
							_game.healPlayers();
						}
						else
						{
							SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_MATCH_BEGINS_IN_S1_SEC);
							sm.Params.addInt(_countDown);
							_stadium.broadcastPacket(sm);
						}
					}

					delay = getDelay(BATTLE_START_TIME_FIRST);
					if (_countDown <= 0)
					{
						_game.makePlayersInvul();
						_game.resetDamage();
						_stadium.openDoors();

						_state = OlympiadGameState.BATTLE_COUNTDOWN_SECOND;
						_countDown = BATTLE_START_TIME_SECOND[0];
						delay = getDelay(BATTLE_START_TIME_SECOND);
					}

					break;
				}
				// Battle start countdown, second part (10-0)
				case OlympiadGameState.BATTLE_COUNTDOWN_SECOND:
				{
					if (_countDown > 0)
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_MATCH_BEGINS_IN_S1_SEC);
						sm.Params.addInt(_countDown);
						_stadium.broadcastPacket(sm);
					}

					delay = getDelay(BATTLE_START_TIME_SECOND);
					if (_countDown <= 0)
					{
						_state = OlympiadGameState.BATTLE_STARTED;
						_game.removePlayersInvul();
						_stadium.broadcastPacket(new SystemMessagePacket(SystemMessageId.HIDDEN_MSG_START_OLYMPIAD));
					}

					break;
				}
				// Beginning of the battle
				case OlympiadGameState.BATTLE_STARTED:
				{
					_countDown = 0;
					_state = OlympiadGameState.BATTLE_IN_PROGRESS; // set state first, used in zone update
					if (!startBattle())
					{
						_state = OlympiadGameState.GAME_STOPPED;
					}

					break;
				}
				// Checks during battle
				case OlympiadGameState.BATTLE_IN_PROGRESS:
				{
					_countDown += 1000;
					int remaining = (int)((Config.ALT_OLY_BATTLE - _countDown) / 1000);
					foreach (int announceTime in BATTLE_END_TIME_SECOND)
					{
						if (announceTime == remaining)
						{
							SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_GAME_ENDS_IN_S1_SEC);
							sm.Params.addInt(announceTime);
							_stadium.broadcastPacket(sm);
							break;
						}
					}

					if (checkBattle() || _countDown > Config.ALT_OLY_BATTLE)
					{
						_state = OlympiadGameState.GAME_STOPPED;
					}

					break;
				}
				// End of the battle
				case OlympiadGameState.GAME_STOPPED:
				{
					_state = OlympiadGameState.TELEPORT_TO_TOWN;
					_countDown = TELEPORT_TO_TOWN_TIMES[0];
					stopGame();
					delay = getDelay(TELEPORT_TO_TOWN_TIMES);
					break;
				}
				// Teleport to town countdown
				case OlympiadGameState.TELEPORT_TO_TOWN:
				{
					if (_countDown > 0)
					{
						SystemMessagePacket sm =
							new SystemMessagePacket(SystemMessageId.YOU_WILL_BE_MOVED_BACK_TO_TOWN_IN_S1_SECOND_S);
						sm.Params.addInt(_countDown);
						_game.broadcastPacket(sm);
					}

					delay = getDelay(TELEPORT_TO_TOWN_TIMES);
					if (_countDown <= 0)
					{
						_state = OlympiadGameState.CLEANUP;
					}

					break;
				}
				// Removals
				case OlympiadGameState.CLEANUP:
				{
					cleanupGame();
					_state = OlympiadGameState.IDLE;
					_game = null;
					return;
				}
			}

			ThreadPool.schedule(this, delay * 1000);
		}
		catch (Exception e)
		{
			switch (_state)
			{
				case OlympiadGameState.GAME_STOPPED:
				case OlympiadGameState.TELEPORT_TO_TOWN:
				case OlympiadGameState.CLEANUP:
				case OlympiadGameState.IDLE:
				{
					LOGGER.Warn("Unable to return players back in town, exception: " + e);
					_state = OlympiadGameState.IDLE;
					_game = null;
					return;
				}
			}

			LOGGER.Warn("Exception in " + _state + ", trying to port players back: " + e);
			_state = OlympiadGameState.GAME_STOPPED;
			ThreadPool.schedule(this, 1000);
		}
	}

	private int getDelay(int[] times)
	{
		int time;
		for (int i = 0; i < times.Length - 1; i++)
		{
			time = times[i];
			if (time >= _countDown)
			{
				continue;
			}

			int delay = _countDown - time;
			_countDown = time;
			return delay;
		}

		// should not happens
		_countDown = -1;
		return 1;
	}

	/**
	 * Second stage: check for defaulted, port players to arena, announce game.
	 * @return true if no participants defaulted.
	 */
	private bool startGame()
	{
		try
		{
			// Checking for opponents and teleporting to arena
			if (_game.checkDefaulted())
			{
				return false;
			}

			_stadium.closeDoors();
			if (_game.needBuffers())
			{
				_stadium.spawnBuffers();
			}

			if (!_game.portPlayersToArena(_stadium.getZone().getSpawns(), _stadium.getInstance()))
			{
				return false;
			}

			_game.removals();
			_needAnnounce = true;
			OlympiadGameManager.getInstance().startBattle(); // inform manager
			return true;
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		return false;
	}

	/**
	 * Fourth stage: last checks, remove buffers, start competition itself.
	 * @return true if all participants online and ready on the stadium.
	 */
	private bool startBattle()
	{
		try
		{
			if (_game.needBuffers())
			{
				_stadium.deleteBuffers();
			}

			if (_game.checkBattleStatus() && _game.makeCompetitionStart())
			{
				// game successfully started
				_game.broadcastOlympiadInfo(_stadium);
				_stadium.broadcastPacket(new SystemMessagePacket(SystemMessageId.THE_MATCH_HAS_BEGUN_FIGHT));
				_stadium.updateZoneStatusForCharactersInside();
				return true;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		return false;
	}

	/**
	 * Fifth stage: battle is running, returns true if winner found.
	 * @return
	 */
	private bool checkBattle()
	{
		try
		{
			return _game.haveWinner();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		return true;
	}

	/**
	 * Sixth stage: winner's validations
	 */
	private void stopGame()
	{
		try
		{
			_game.validateWinner(_stadium);
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		try
		{
			_game.cleanEffects();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		try
		{
			_game.makePlayersInvul();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		try
		{
			_stadium.updateZoneStatusForCharactersInside();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}

	/**
	 * Seventh stage: game cleanup (port players back, closing doors, etc)
	 */
	private void cleanupGame()
	{
		try
		{
			_game.removePlayersInvul();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		try
		{
			_game.playersStatusBack();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		try
		{
			_game.portPlayersBack();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		try
		{
			_game.clearPlayers();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}

		try
		{
			_stadium.closeDoors();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}
}