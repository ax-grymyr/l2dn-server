using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model;

public class Duel
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Duel));

	public const int DUELSTATE_NODUEL = 0;
	public const int DUELSTATE_DUELLING = 1;
	public const int DUELSTATE_DEAD = 2;
	public const int DUELSTATE_WINNER = 3;
	public const int DUELSTATE_INTERRUPTED = 4;

	private static readonly PlaySoundPacket B04_S01 = new(1, "B04_S01", 0, 0, 0, 0, 0);

	private const int PARTY_DUEL_DURATION = 300;
	private const int PLAYER_DUEL_DURATION = 120;

	private readonly int _duelId;
	private readonly Player _playerA;
	private readonly Player _playerB;
    private readonly Parties? _parties;
	private readonly DateTime _duelEndTime;
	private int _surrenderRequest;
	private int _countdown = 5;
	private bool _finished;

	private readonly Map<int, PlayerCondition> _playerConditions = new();
	private Instance? _duelInstance;

	public Duel(Player playerA, Player playerB, bool partyDuel, int duelId)
	{
		_duelId = duelId;
		_playerA = playerA;
		_playerB = playerB;

        Party? partyA = playerA.getParty();
        Party? partyB = playerB.getParty();

        if (partyDuel)
        {
            if (partyA == null || partyB == null)
                return;

            _parties = new Parties(partyA, partyB);
            foreach (Player member in _parties.PartyA.getMembers())
                member.setStartingDuel();

            foreach (Player member in _parties.PartyB.getMembers())
                member.setStartingDuel();
        }
		else
		{
			_playerA.setStartingDuel();
			_playerB.setStartingDuel();
		}

		_duelEndTime = DateTime.UtcNow.AddSeconds(IsPartyDuel ? PARTY_DUEL_DURATION : PLAYER_DUEL_DURATION);
		setFinished(false);

		if (IsPartyDuel)
		{
			// inform players that they will be ported shortly
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.IN_A_MOMENT_YOU_WILL_BE_TRANSPORTED_TO_THE_SITE_WHERE_THE_DUEL_WILL_TAKE_PLACE);
			broadcastToTeam1(sm);
			broadcastToTeam2(sm);
		}

		// Schedule duel start
		ThreadPool.schedule(new ScheduleStartDuelTask(this), 3000);
	}

    public bool IsPartyDuel => _parties != null;

    private sealed class Parties(Party partyA, Party partyB)
    {
        public Party PartyA { get; } = partyA;
        public Party PartyB { get; } = partyB;
    }

	public sealed class PlayerCondition
	{
		private readonly Player _player;
		private readonly double _hp;
		private readonly double _mp;
		private readonly double _cp;
		private readonly bool _isPartyDuel;
		private readonly int _x;
		private readonly int _y;
		private readonly int _z;
		private readonly Set<Skill> _debuffs = [];

		public PlayerCondition(Player player, bool partyDuel)
		{
			_player = player;
			_hp = _player.getCurrentHp();
			_mp = _player.getCurrentMp();
			_cp = _player.getCurrentCp();
			_isPartyDuel = partyDuel;
			_x = _player.getX();
			_y = _player.getY();
			_z = _player.getZ();
		}

        public void restoreCondition()
        {
            _player.setCurrentHp(_hp);
            _player.setCurrentMp(_mp);
            _player.setCurrentCp(_cp);

            if (_isPartyDuel)
                teleportBack();

            // Debuff removal
            foreach (Skill skill in _debuffs)
                _player.stopSkillEffects(SkillFinishType.REMOVED, skill.getId());
        }

        public void registerDebuff(Skill debuff)
        {
            _debuffs.add(debuff);
        }

		public void teleportBack()
		{
			if (_isPartyDuel)
				_player.teleToLocation(new Location3D(_x, _y, _z));
		}

		public Player getPlayer() => _player;
    }

	public sealed class ScheduleDuelTask(Duel duel): Runnable
    {
        public void run()
		{
			try
			{
				switch (duel.checkEndDuelCondition())
				{
					case DuelResult.CANCELED:
					{
						// do not schedule duel end if it was interrupted
						duel.setFinished(true);
						duel.endDuel(DuelResult.CANCELED);
						break;
					}
					case DuelResult.CONTINUE:
					{
						ThreadPool.schedule(this, 1000);
						break;
					}
					default:
					{
						duel.setFinished(true);
						duel.playKneelAnimation();
						ThreadPool.schedule(new ScheduleEndDuelTask(duel, duel.checkEndDuelCondition()), 5000);
						if (duel._duelInstance != null)
						{
							duel._duelInstance.destroy();
						}
						break;
					}
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("There has been a problem while runing a duel task!" + e);
			}
		}
	}

	public sealed class ScheduleStartDuelTask(Duel duel): Runnable
    {
        public void run()
		{
			try
			{
				// start/continue countdown
				int count = duel.countdown();
				if (count == 4)
				{
					// Save player conditions before teleporting players
					duel.savePlayerConditions();

					duel.teleportPlayers();

					// give players 20 seconds to complete teleport and get ready (its ought to be 30 on offical..)
					ThreadPool.schedule(this, 20000);
				}
				else if (count > 0) // duel not started yet - continue countdown
				{
					ThreadPool.schedule(this, 1000);
				}
				else
				{
					duel.startDuel();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("There has been a problem while runing a duel start task! " + e);
			}
		}
	}

	public sealed class ScheduleEndDuelTask(Duel duel, DuelResult result): Runnable
    {
        public void run()
		{
			try
			{
				duel.endDuel(result);
			}
			catch (Exception e)
			{
				LOGGER.Error("There has been a problem while runing a duel end task!" + e);
			}
		}
	}

	public Instance? getDueldInstance()
	{
		return _duelInstance;
	}

	/**
	 * Stops all players from attacking. Used for duel timeout / interrupt.
	 */
	private void stopFighting()
	{
		ActionFailedPacket af = ActionFailedPacket.STATIC_PACKET;
		if (_parties != null)
        {
            foreach (Player temp in _parties.PartyA.getMembers())
            {
                temp.abortCast();
                temp.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
                temp.setTarget(null);
                temp.sendPacket(af);
                temp.getServitorsAndPets().ForEach(s =>
                {
                    s.abortCast();
                    s.abortAttack();
                    s.setTarget(null);
                    s.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
                });
            }

            foreach (Player temp in _parties.PartyB.getMembers())
            {
                temp.abortCast();
                temp.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
                temp.setTarget(null);
                temp.sendPacket(af);
                temp.getServitorsAndPets().ForEach(s =>
                {
                    s.abortCast();
                    s.abortAttack();
                    s.setTarget(null);
                    s.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
                });
            }
        }
		else
		{
			_playerA.abortCast();
			_playerB.abortCast();
			_playerA.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			_playerA.setTarget(null);
			_playerB.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			_playerB.setTarget(null);
			_playerA.sendPacket(af);
			_playerB.sendPacket(af);
			_playerA.getServitorsAndPets().ForEach(s =>
			{
				s.abortCast();
				s.abortAttack();
				s.setTarget(null);
				s.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			});
			_playerB.getServitorsAndPets().ForEach(s =>
			{
				s.abortCast();
				s.abortAttack();
				s.setTarget(null);
				s.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			});
		}
	}

	/**
	 * Check if a player engaged in pvp combat (only for 1on1 duels)
	 * @param sendMessage
	 * @return returns true if a duelist is engaged in Pvp combat
	 */
	public bool isDuelistInPvp(bool sendMessage)
	{
		if (IsPartyDuel)
		{
			// Party duels take place in arenas - should be no other players there
			return false;
		}

        if (_playerA.getPvpFlag() != PvpFlagStatus.None || _playerB.getPvpFlag() != PvpFlagStatus.None)
		{
			if (sendMessage)
			{
				string engagedInPvP = "The duel was canceled because a duelist engaged in PvP combat.";
				_playerA.sendMessage(engagedInPvP);
				_playerB.sendMessage(engagedInPvP);
			}
			return true;
		}
		return false;
	}

	/**
	 * Starts the duel
	 */
	public void startDuel()
	{
		if (_playerA.isInDuel() || _playerB.isInDuel())
		{
			_playerConditions.Clear();
			DuelManager.getInstance().removeDuel(this);
			return;
		}

		if (_parties != null)
		{
            // Set duel state and team
			foreach (Player temp in _parties.PartyA.getMembers())
			{
				temp.cancelActiveTrade();
				temp.setInDuel(_duelId);
				temp.setTeam(Team.BLUE);
				temp.broadcastUserInfo();
				broadcastToTeam2(new ExDuelUpdateUserInfoPacket(temp));
			}

			foreach (Player temp in _parties.PartyB.getMembers())
			{
				temp.cancelActiveTrade();
				temp.setInDuel(_duelId);
				temp.setTeam(Team.RED);
				temp.broadcastUserInfo();
				broadcastToTeam1(new ExDuelUpdateUserInfoPacket(temp));
			}

			// Send duel packets
			broadcastToTeam1(ExDuelReadyPacket.PARTY_DUEL);
			broadcastToTeam2(ExDuelReadyPacket.PARTY_DUEL);
			broadcastToTeam1(ExDuelStartPacket.PARTY_DUEL);
			broadcastToTeam2(ExDuelStartPacket.PARTY_DUEL);

            if (_duelInstance != null)
            {
                foreach (Door door in _duelInstance.getDoors())
                {
                    if (!door.isOpen())
                        door.openMe();
                }
            }
        }
		else
		{
			// set isInDuel() state
			_playerA.setInDuel(_duelId);
			_playerA.setTeam(Team.BLUE);
			_playerB.setInDuel(_duelId);
			_playerB.setTeam(Team.RED);

			// Send duel packets
			broadcastToTeam1(ExDuelReadyPacket.PLAYER_DUEL);
			broadcastToTeam2(ExDuelReadyPacket.PLAYER_DUEL);
			broadcastToTeam1(ExDuelStartPacket.PLAYER_DUEL);
			broadcastToTeam2(ExDuelStartPacket.PLAYER_DUEL);

			broadcastToTeam1(new ExDuelUpdateUserInfoPacket(_playerB));
			broadcastToTeam2(new ExDuelUpdateUserInfoPacket(_playerA));
			_playerA.broadcastUserInfo();
			_playerB.broadcastUserInfo();
		}

		// play sound
		broadcastToTeam1(B04_S01);
		broadcastToTeam2(B04_S01);

		// start duelling task
		ThreadPool.schedule(new ScheduleDuelTask(this), 1000);
	}

	/**
	 * Save the current player condition: hp, mp, cp, location
	 */
	public void savePlayerConditions()
	{
		if (_parties != null)
		{
            foreach (Player player in _parties.PartyA.getMembers())
                _playerConditions.put(player.ObjectId, new PlayerCondition(player, true));

            foreach (Player player in _parties.PartyB.getMembers())
                _playerConditions.put(player.ObjectId, new PlayerCondition(player, true));
        }
		else
		{
			_playerConditions.put(_playerA.ObjectId, new PlayerCondition(_playerA, false));
			_playerConditions.put(_playerB.ObjectId, new PlayerCondition(_playerB, false));
		}
	}

	/**
	 * Restore player conditions
	 * @param abnormalDuelEnd true if the duel was the duel canceled
	 */
	public void restorePlayerConditions(bool abnormalDuelEnd)
	{
		// update isInDuel() state for all players
        if (_parties != null)
		{
            foreach (Player temp in _parties.PartyA.getMembers())
			{
				temp.setInDuel(0);
				temp.setTeam(Team.NONE);
				temp.broadcastUserInfo();
			}

            foreach (Player temp in _parties.PartyB.getMembers())
			{
				temp.setInDuel(0);
				temp.setTeam(Team.NONE);
				temp.broadcastUserInfo();
			}
		}
		else
		{
			_playerA.setInDuel(0);
			_playerA.setTeam(Team.NONE);
			_playerA.broadcastUserInfo();
			_playerB.setInDuel(0);
			_playerB.setTeam(Team.NONE);
			_playerB.broadcastUserInfo();
		}

		// if it is an abnormal DuelEnd do not restore hp, mp, cp
		if (abnormalDuelEnd)
		{
			return;
		}

		// restore player conditions
		_playerConditions.Values.ForEach(x => x.restoreCondition());
	}

	/**
	 * Get the duel id
	 * @return id
	 */
	public int getId()
	{
		return _duelId;
	}

	/**
	 * Returns the remaining time
	 * @return remaining time
	 */
	public TimeSpan getRemainingTime()
	{
		return _duelEndTime - DateTime.UtcNow;
	}

	/**
	 * Get the player that requested the duel
	 * @return duel requester
	 */
	public Player getPlayerA()
	{
		return _playerA;
	}

	/**
	 * Get the player that was challenged
	 * @return challenged player
	 */
	public Player getPlayerB()
	{
		return _playerB;
	}

	/**
	 * Returns whether this is a party duel or not
	 * @return is party duel
	 */
	public bool isPartyDuel()
	{
		return IsPartyDuel;
	}

	public void setFinished(bool mode)
	{
		_finished = mode;
	}

	public bool getFinished()
	{
		return _finished;
	}

	/**
	 * Teleports all players to a free arena.
	 */
	public void teleportPlayers()
	{
		if (_parties == null)
			return;

		int instanceId = DuelManager.getInstance().getDuelArena();
		OlympiadStadiumZone? zone = null;
		foreach (OlympiadStadiumZone z in ZoneManager.getInstance().getAllZones<OlympiadStadiumZone>())
		{
			if (z.getInstanceTemplateId() == instanceId)
			{
				zone = z;
				break;
			}
		}
		if (zone == null)
		{
			throw new InvalidOperationException("Unable to find a party duel arena!");
		}

		ImmutableArray<Location3D> spawns = zone.getSpawns();

        InstanceTemplate duelInstance = InstanceManager.getInstance().getInstanceTemplate(instanceId) ??
            throw new InvalidOperationException($"Instance template id={instanceId} not found");

		_duelInstance = InstanceManager.getInstance().createInstance(duelInstance, null);

		Location3D spawn1 = spawns[Rnd.get(spawns.Length / 2)];
		foreach (Player temp in _parties.PartyA.getMembers())
			temp.teleToLocation(new Location(spawn1, 0), _duelInstance);

		Location3D spawn2 = spawns[Rnd.get(spawns.Length / 2, spawns.Length)];
		foreach (Player temp in _parties.PartyB.getMembers())
			temp.teleToLocation(new Location(spawn2, 0), _duelInstance);
	}

	/**
	 * Broadcast a packet to the challenger team
	 * @param packet
	 */
	public void broadcastToTeam1<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		if (_parties != null)
		{
			foreach (Player temp in _parties.PartyA.getMembers())
				temp.sendPacket(packet);
		}
		else
			_playerA.sendPacket(packet);
	}

	/**
	 * Broadcast a packet to the challenged team
	 * @param packet
	 */
	public void broadcastToTeam2<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		if (_parties != null)
		{
			foreach (Player temp in _parties.PartyB.getMembers())
				temp.sendPacket(packet);
		}
		else
			_playerB.sendPacket(packet);
	}

	/**
	 * Get the duel winner
	 * @return winner
	 */
	public Player? getWinner()
	{
		if (!_finished || _playerA == null || _playerB == null)
			return null;

        if (_playerA.getDuelState() == DUELSTATE_WINNER)
			return _playerA;

        if (_playerB.getDuelState() == DUELSTATE_WINNER)
			return _playerB;

		return null;
	}

	/**
	 * Get the duel looser
	 * @return looser
	 */
	public Player? getLooser()
	{
		if (!_finished || _playerA == null || _playerB == null)
			return null;

        if (_playerA.getDuelState() == DUELSTATE_WINNER)
			return _playerB;

        if (_playerB.getDuelState() == DUELSTATE_WINNER)
			return _playerA;

        return null;
	}

	/**
	 * Playback the bow animation for all loosers
	 */
	public void playKneelAnimation()
	{
		Player? looser = getLooser();
		if (looser == null)
			return;

        Party? party = looser.getParty();
		if (IsPartyDuel && party != null)
		{
			foreach (Player temp in party.getMembers())
				temp.broadcastPacket(new SocialActionPacket(temp.ObjectId, 7));
		}
		else
			looser.broadcastPacket(new SocialActionPacket(looser.ObjectId, 7));
	}

	/**
	 * Do the countdown and send message to players if necessary
	 * @return current count
	 */
	public int countdown()
	{
		_countdown--;

		if (_countdown > 3)
		{
			return _countdown;
		}

		// Broadcast countdown to duelists
		SystemMessagePacket sm;
		if (_countdown > 0)
		{
			sm = new SystemMessagePacket(SystemMessageId.THE_DUEL_STARTS_IN_S1_SEC);
			sm.Params.addInt(_countdown);
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.LET_THE_DUEL_BEGIN);
		}

		broadcastToTeam1(sm);
		broadcastToTeam2(sm);

		return _countdown;
	}

	/**
	 * The duel has reached a state in which it can no longer continue
	 * @param result the duel result.
	 */
	public void endDuel(DuelResult result)
	{
		if (_playerA == null || _playerB == null)
		{
			// clean up
			_playerConditions.Clear();
			DuelManager.getInstance().removeDuel(this);
			return;
		}

		// inform players of the result
		SystemMessagePacket sm;
		switch (result)
		{
			case DuelResult.TEAM_1_WIN:
			case DuelResult.TEAM_2_SURRENDER:
			{
				restorePlayerConditions(false);
				sm = IsPartyDuel ? new SystemMessagePacket(SystemMessageId.C1_S_PARTY_HAS_WON_THE_DUEL) : new SystemMessagePacket(SystemMessageId.C1_HAS_WON_THE_DUEL);
				sm.Params.addString(_playerA.getName());

				broadcastToTeam1(sm);
				broadcastToTeam2(sm);
				break;
			}
			case DuelResult.TEAM_1_SURRENDER:
			case DuelResult.TEAM_2_WIN:
			{
				restorePlayerConditions(false);
				sm = IsPartyDuel ? new SystemMessagePacket(SystemMessageId.C1_S_PARTY_HAS_WON_THE_DUEL) : new SystemMessagePacket(SystemMessageId.C1_HAS_WON_THE_DUEL);
				sm.Params.addString(_playerB.getName());

				broadcastToTeam1(sm);
				broadcastToTeam2(sm);
				break;
			}
			case DuelResult.CANCELED:
			{
				stopFighting();
				// Don't restore hp, mp, cp
				restorePlayerConditions(true);
				// TODO: is there no other message for a canceled duel?
				// send SystemMessage
				sm = new SystemMessagePacket(SystemMessageId.THE_DUEL_HAS_ENDED_IN_A_TIE);
				broadcastToTeam1(sm);
				broadcastToTeam2(sm);
				break;
			}
			case DuelResult.TIMEOUT:
			{
				stopFighting();
				// hp,mp,cp seem to be restored in a timeout too...
				restorePlayerConditions(false);
				// send SystemMessage
				sm = new SystemMessagePacket(SystemMessageId.THE_DUEL_HAS_ENDED_IN_A_TIE);
				broadcastToTeam1(sm);
				broadcastToTeam2(sm);
				break;
			}
		}

		// Send end duel packet
		ExDuelEndPacket duelEnd = IsPartyDuel ? ExDuelEndPacket.PARTY_DUEL : ExDuelEndPacket.PLAYER_DUEL;
		broadcastToTeam1(duelEnd);
		broadcastToTeam2(duelEnd);

		// clean up
		_playerConditions.Clear();
		DuelManager.getInstance().removeDuel(this);
	}

	/**
	 * Did a situation occur in which the duel has to be ended?
	 * @return DuelResult duel status
	 */
	public DuelResult checkEndDuelCondition()
	{
		// one of the players might leave during duel
		if (_playerA == null || _playerB == null)
		{
			return DuelResult.CANCELED;
		}

		// got a duel surrender request?
		if (_surrenderRequest != 0)
		{
			return _surrenderRequest == 1 ? DuelResult.TEAM_1_SURRENDER : DuelResult.TEAM_2_SURRENDER;
		}
		// duel timed out

        if (getRemainingTime() <= TimeSpan.Zero)
        {
            return DuelResult.TIMEOUT;
        }
        // Has a player been declared winner yet?

        if (_playerA.getDuelState() == DUELSTATE_WINNER)
        {
            // If there is a Winner already there should be no more fighting going on
            stopFighting();
            return DuelResult.TEAM_1_WIN;
        }

        if (_playerB.getDuelState() == DUELSTATE_WINNER)
        {
            // If there is a Winner already there should be no more fighting going on
            stopFighting();
            return DuelResult.TEAM_2_WIN;
        }

        // More end duel conditions for 1on1 duels

        if (!IsPartyDuel)
        {
            // Duel was interrupted e.g.: player was attacked by mobs / other players
            if (_playerA.getDuelState() == DUELSTATE_INTERRUPTED || _playerB.getDuelState() == DUELSTATE_INTERRUPTED)
            {
                return DuelResult.CANCELED;
            }

            // Are the players too far apart?
            if (!_playerA.IsInsideRadius2D(_playerB, 1600))
            {
                return DuelResult.CANCELED;
            }

            // Did one of the players engage in PvP combat?
            if (isDuelistInPvp(true))
            {
                return DuelResult.CANCELED;
            }

            // is one of the players in a Siege, Peace or PvP zone?
            if (_playerA.isInsideZone(ZoneId.PEACE) || _playerB.isInsideZone(ZoneId.PEACE) || _playerA.isInsideZone(ZoneId.NO_PVP) || _playerB.isInsideZone(ZoneId.NO_PVP) || _playerA.isInsideZone(ZoneId.SIEGE) || _playerB.isInsideZone(ZoneId.SIEGE) || _playerA.isInsideZone(ZoneId.PVP) || _playerB.isInsideZone(ZoneId.PVP))
            {
                return DuelResult.CANCELED;
            }
        }

        return DuelResult.CONTINUE;
	}

	/**
	 * Register a surrender request
	 * @param player the player that surrenders.
	 */
	public void doSurrender(Player player)
	{
		// already received a surrender request
		if (_surrenderRequest != 0)
		{
			return;
		}

		// stop the fight
		stopFighting();

		// TODO: Can every party member cancel a party duel? or only the party leaders?
		if (_parties != null)
		{
			if (_parties.PartyA.getMembers().Contains(player))
			{
				_surrenderRequest = 1;
				foreach (Player temp in _parties.PartyA.getMembers())
					temp.setDuelState(DUELSTATE_DEAD);

                foreach (Player temp in _parties.PartyB.getMembers())
					temp.setDuelState(DUELSTATE_WINNER);
			}
			else if (_parties.PartyB.getMembers().Contains(player))
			{
				_surrenderRequest = 2;
				foreach (Player temp in _parties.PartyB.getMembers())
					temp.setDuelState(DUELSTATE_DEAD);

                foreach (Player temp in _parties.PartyA.getMembers())
					temp.setDuelState(DUELSTATE_WINNER);
			}
		}
		else if (player == _playerA)
		{
			_surrenderRequest = 1;
			_playerA.setDuelState(DUELSTATE_DEAD);
			_playerB.setDuelState(DUELSTATE_WINNER);
		}
		else if (player == _playerB)
		{
			_surrenderRequest = 2;
			_playerB.setDuelState(DUELSTATE_DEAD);
			_playerA.setDuelState(DUELSTATE_WINNER);
		}
	}

	/**
	 * This function is called whenever a player was defeated in a duel
	 * @param player the player defeated.
	 */
	public void onPlayerDefeat(Player player)
	{
		// Set player as defeated
		player.setDuelState(DUELSTATE_DEAD);

        Party? playerParty = player.getParty();
		if (_parties != null && playerParty != null)
		{
			bool teamDefeated = playerParty.getMembers().Any(member => member.getDuelState() == DUELSTATE_DUELLING);
			if (teamDefeated)
			{
				Party winnerParty = _parties.PartyA.getMembers().Contains(player) ? _parties.PartyB : _parties.PartyA;
				foreach (Player temp in winnerParty.getMembers())
					temp.setDuelState(DUELSTATE_WINNER);
			}
		}
		else
		{
			if (player != _playerA && player != _playerB)
				LOGGER.Warn("Error in onPlayerDefeat(): player is not part of this 1vs1 duel");

			if (_playerA == player)
				_playerB.setDuelState(DUELSTATE_WINNER);
			else
				_playerA.setDuelState(DUELSTATE_WINNER);
		}
	}

	/**
	 * This function is called whenever a player leaves a party
	 * @param player the player quitting.
	 */
	public void onRemoveFromParty(Player player)
	{
		// if it isn't a party duel ignore this
		if (!IsPartyDuel)
			return;

		// this player is leaving his party during party duel
		// if he's either playerA or playerB cancel the duel and port the players back
		if (player == _playerA || player == _playerB)
		{
			foreach (PlayerCondition cond in _playerConditions.Values)
			{
				cond.teleportBack();
				cond.getPlayer().setInDuel(0);
			}
		}
		else
		// teleport the player back & delete his PlayerCondition record
		{
			PlayerCondition? cond = _playerConditions.remove(player.ObjectId);
			if (cond != null)
			{
				cond.teleportBack();
			}
			player.setInDuel(0);
		}
	}

	public void onBuff(Player player, Skill debuff)
	{
		PlayerCondition? cond = _playerConditions.get(player.ObjectId);
		if (cond != null)
			cond.registerDebuff(debuff);
	}
}