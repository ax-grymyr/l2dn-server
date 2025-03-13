using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Clans;

public class ClanWar
{
	public static readonly TimeSpan TIME_TO_CANCEL_NON_MUTUAL_CLAN_WAR = TimeSpan.FromDays(7);
	public static readonly TimeSpan TIME_TO_DELETION_AFTER_CANCELLATION = TimeSpan.FromDays(5);
	public static readonly TimeSpan TIME_TO_DELETION_AFTER_DEFEAT = TimeSpan.FromDays(21);
	private readonly  int _attackerClanId;
	private readonly int _attackedClanId;
	private int? _winnerClanId;
	private ClanWarState _state;
	private ScheduledFuture? _cancelTask;
	private readonly DateTime _startTime;
	private DateTime? _endTime;

	private int _attackerKillCount;
	private int _attackedKillCount;

	public ClanWar(Clan attacker, Clan attacked)
	{
		_attackerClanId = attacker.getId();
		_attackedClanId = attacked.getId();
		_startTime = DateTime.UtcNow;
		_state = ClanWarState.BLOOD_DECLARATION;
		_cancelTask = ThreadPool.schedule(clanWarTimeout, TIME_TO_CANCEL_NON_MUTUAL_CLAN_WAR);
		attacker.addWar(attacked.getId(), this);
		attacked.addWar(attacker.getId(), this);

		if (GlobalEvents.Global.HasSubscribers<OnClanWarStart>())
		{
			GlobalEvents.Global.NotifyAsync(new OnClanWarStart(attacker, attacked));
		}

		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_DECLARED_A_CLAN_WAR_WITH_S1);
		sm.Params.addString(attacked.getName());
		attacker.broadcastToOnlineMembers(sm);

		sm = new SystemMessagePacket(SystemMessageId.S1_HAS_DECLARED_A_CLAN_WAR_THE_WAR_WILL_AUTOMATICALLY_START_IF_YOU_KILL_MORE_THAN_5_S1_CLAN_MEMBERS_IN_A_WEEK);
		sm.Params.addString(attacker.getName());
		attacked.broadcastToOnlineMembers(sm);
	}

	public ClanWar(Clan attacker, Clan attacked, int attackerKillCount, int attackedKillCount, int? winnerClan, DateTime startTime, DateTime? endTime, ClanWarState state)
	{
		_attackerClanId = attacker.getId();
		_attackedClanId = attacked.getId();
		_startTime = startTime;
		_endTime = endTime;
		_state = state;
		_attackerKillCount = attackerKillCount;
		_attackedKillCount = attackedKillCount;
		_winnerClanId = winnerClan;
		if (_startTime + TIME_TO_CANCEL_NON_MUTUAL_CLAN_WAR > DateTime.UtcNow)
		{
			_cancelTask = ThreadPool.schedule(clanWarTimeout, _startTime + TIME_TO_CANCEL_NON_MUTUAL_CLAN_WAR - DateTime.UtcNow);
		}

		if (_endTime is not null)
		{
			DateTime endTimePeriod = _endTime.Value + (_state == ClanWarState.TIE ? TIME_TO_DELETION_AFTER_CANCELLATION : TIME_TO_DELETION_AFTER_DEFEAT);
			TimeSpan delay = endTimePeriod - DateTime.UtcNow;
			if (delay > TimeSpan.Zero)
			{
				ThreadPool.schedule(() => ClanTable.getInstance().deleteClanWars(_attackerClanId, _attackedClanId), delay);
			}
			else
			{
				ThreadPool.schedule(() => ClanTable.getInstance().deleteClanWars(_attackerClanId, _attackedClanId), TimeSpan.FromSeconds(10));
			}
		}
	}

	public void onKill(Player killer, Player victim)
	{
		Clan? victimClan = victim.getClan();
		Clan? killerClan = killer.getClan();
        if (victimClan == null || killerClan == null)
            return;

		// Reputation increase by killing an enemy (over level 4) in a clan war under the condition of mutual war declaration
		if (victim.getLevel() > 4 && _state == ClanWarState.MUTUAL)
		{
			// however, when the other side reputation score is 0 or below, your clan cannot acquire any reputation points from them.
			if (victimClan.getReputationScore() > 0)
			{
				victimClan.takeReputationScore(Config.REPUTATION_SCORE_PER_KILL);
				killerClan.addReputationScore(Config.REPUTATION_SCORE_PER_KILL);
			}

			// System Message notification to clan members
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_KILLED_BY_A_MEMBER_OF_THE_S2_CLAN_CLAN_REPUTATION_POINTS_1);
			sm.Params.addPcName(victim);
			sm.Params.addString(killerClan.getName());
			victimClan.broadcastToOnlineMembers(sm);
			sm = new SystemMessagePacket(SystemMessageId.A_MEMBER_OF_THE_S1_CLAN_IS_KILLED_BY_C2_CLAN_REPUTATION_POINTS_1);
			sm.Params.addString(victimClan.getName());
			sm.Params.addPcName(killer);
			killerClan.broadcastToOnlineMembers(sm);
			if (killerClan.getId() == _attackerClanId)
			{
				Interlocked.Increment(ref _attackerKillCount);
			}
			else
			{
				Interlocked.Increment(ref _attackedKillCount);
			}
		}
		else if (_state == ClanWarState.BLOOD_DECLARATION && victimClan.getId() == _attackerClanId && victim.getReputation() >= 0)
		{
			int killCount = Interlocked.Increment(ref _attackedKillCount);
			if (killCount >= 5)
			{
				_state = ClanWarState.MUTUAL;
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.A_CLAN_WAR_WITH_CLAN_S1_HAS_STARTED_THE_CLAN_THAT_CANCELS_THE_WAR_FIRST_WILL_LOSE_500_CLAN_REPUTATION_POINTS_IF_YOUR_CLAN_MEMBER_GETS_KILLED_BY_THE_OTHER_CLAN_XP_DECREASES_BY_1_4_OF_THE_AMOUNT_THAT_DECREASES_IN_HUNTING_ZONES);
				sm.Params.addString(victimClan.getName());
				killerClan.broadcastToOnlineMembers(sm);

				sm = new SystemMessagePacket(SystemMessageId.A_CLAN_WAR_WITH_CLAN_S1_HAS_STARTED_THE_CLAN_THAT_CANCELS_THE_WAR_FIRST_WILL_LOSE_500_CLAN_REPUTATION_POINTS_IF_YOUR_CLAN_MEMBER_GETS_KILLED_BY_THE_OTHER_CLAN_XP_DECREASES_BY_1_4_OF_THE_AMOUNT_THAT_DECREASES_IN_HUNTING_ZONES);
				sm.Params.addString(killerClan.getName());
				victimClan.broadcastToOnlineMembers(sm);

				if (_cancelTask != null)
				{
					_cancelTask.cancel(true);
					_cancelTask = null;
				}
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.A_CLAN_MEMBER_OF_S1_WAS_KILLED_BY_YOUR_CLAN_MEMBER_IF_YOUR_CLAN_KILLS_S2_MEMBERS_OF_CLAN_S1_A_CLAN_WAR_WITH_CLAN_S1_WILL_START);
				sm.Params.addString(victimClan.getName());
				sm.Params.addInt(5 - killCount);
				killerClan.broadcastToOnlineMembers(sm);
			}
		}
	}

	public void cancel(Player player, Clan cancelor)
    {
        Clan? winnerClan = cancelor.getId() == _attackerClanId
            ? ClanTable.getInstance().getClan(_attackedClanId)
            : ClanTable.getInstance().getClan(_attackerClanId);
        if (winnerClan == null)
            return;

		// Reduce reputation.
		cancelor.takeReputationScore(500);
		player.sendPacket(new SurrenderPledgeWarPacket(cancelor.getName(), player.getName()));
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_WAR_IS_OVER_AS_YOU_VE_ADMITTED_DEFEAT_FROM_THE_CLAN_S1_YOU_VE_LOST);
		sm.Params.addString(winnerClan.getName());
		cancelor.broadcastToOnlineMembers(sm);

		sm = new SystemMessagePacket(SystemMessageId.THE_WAR_ENDED_BY_THE_S1_CLAN_S_DEFEAT_DECLARATION_YOU_HAVE_WON_THE_CLAN_WAR_OVER_THE_S1_CLAN);
		sm.Params.addString(cancelor.getName());
		winnerClan.broadcastToOnlineMembers(sm);

		_winnerClanId = winnerClan.getId();
		_endTime = DateTime.UtcNow;
		ThreadPool.schedule(() => ClanTable.getInstance().deleteClanWars(cancelor.getId(), winnerClan.getId()), 5000 /* (_endTime + TIME_TO_DELETION_AFTER_DEFEAT) - System.currentTimeMillis() */);
	}

	public void clanWarTimeout()
	{
		Clan? attackerClan = ClanTable.getInstance().getClan(_attackerClanId);
		Clan? attackedClan = ClanTable.getInstance().getClan(_attackedClanId);
		if (attackerClan != null && attackedClan != null)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_WAR_DECLARED_BY_THE_S1_CLAN_HAS_ENDED);
			sm.Params.addString(attackerClan.getName());
			attackedClan.broadcastToOnlineMembers(sm);

			sm = new SystemMessagePacket(SystemMessageId.BECAUSE_CLAN_S1_DID_NOT_FIGHT_BACK_FOR_1_WEEK_THE_CLAN_WAR_WAS_CANCELLED);
			sm.Params.addString(attackedClan.getName());
			attackerClan.broadcastToOnlineMembers(sm);

			_state = ClanWarState.TIE;
			_endTime = DateTime.UtcNow;
			ThreadPool.schedule(() => ClanTable.getInstance().deleteClanWars(attackerClan.getId(), attackedClan.getId()), 5000 /* (_endTime + TIME_TO_DELETION_AFTER_CANCELLATION) - System.currentTimeMillis() */);
		}
	}

	public void mutualClanWarAccepted(Clan attacker, Clan attacked)
	{
		_state = ClanWarState.MUTUAL;
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.A_CLAN_WAR_WITH_CLAN_S1_HAS_STARTED_THE_CLAN_THAT_CANCELS_THE_WAR_FIRST_WILL_LOSE_500_CLAN_REPUTATION_POINTS_IF_YOUR_CLAN_MEMBER_GETS_KILLED_BY_THE_OTHER_CLAN_XP_DECREASES_BY_1_4_OF_THE_AMOUNT_THAT_DECREASES_IN_HUNTING_ZONES);
		sm.Params.addString(attacker.getName());
		attacked.broadcastToOnlineMembers(sm);

		sm = new SystemMessagePacket(SystemMessageId.A_CLAN_WAR_WITH_CLAN_S1_HAS_STARTED_THE_CLAN_THAT_CANCELS_THE_WAR_FIRST_WILL_LOSE_500_CLAN_REPUTATION_POINTS_IF_YOUR_CLAN_MEMBER_GETS_KILLED_BY_THE_OTHER_CLAN_XP_DECREASES_BY_1_4_OF_THE_AMOUNT_THAT_DECREASES_IN_HUNTING_ZONES);
		sm.Params.addString(attacked.getName());
		attacker.broadcastToOnlineMembers(sm);

		if (_cancelTask != null)
		{
			_cancelTask.cancel(true);
			_cancelTask = null;
		}
	}

	public int getKillDifference(Clan clan)
	{
		return _attackerClanId == clan.getId() ? _attackerKillCount - _attackedKillCount : _attackedKillCount - _attackerKillCount;
	}

	public ClanWarState getClanWarState(Clan clan)
	{
		if (_winnerClanId > 0)
		{
			return _winnerClanId == clan.getId() ? ClanWarState.WIN : ClanWarState.LOSS;
		}
		return _state;
	}

	public int getAttackerClanId()
	{
		return _attackerClanId;
	}

	public int getAttackedClanId()
	{
		return _attackedClanId;
	}

	public int getAttackerKillCount()
	{
		return _attackerKillCount;
	}

	public int getAttackedKillCount()
	{
		return _attackedKillCount;
	}

	public int? getWinnerClanId()
	{
		return _winnerClanId;
	}

	public DateTime getStartTime()
	{
		return _startTime;
	}

	public DateTime? getEndTime()
	{
		return _endTime;
	}

	public ClanWarState getState()
	{
		return _state;
	}

	public int getKillToStart()
	{
		return _state == ClanWarState.BLOOD_DECLARATION ? 5 - _attackedKillCount : 0;
	}

	public TimeSpan getRemainingTime()
	{
		return _startTime + TIME_TO_CANCEL_NON_MUTUAL_CLAN_WAR - DateTime.UtcNow;
	}

	public Clan? getOpposingClan(Clan clan)
    {
        return _attackerClanId == clan.getId()
            ? ClanTable.getInstance().getClan(_attackedClanId)
            : ClanTable.getInstance().getClan(_attackerClanId);
    }
}