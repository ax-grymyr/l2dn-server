using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.InstanceManagers;

public class DuelManager
{
	private static readonly int[] ARENAS =
	{
		147, // OlympiadGrassyArena.xml
		148, // OlympiadThreeBridgesArena.xml
		149, // OlympiadHerossVestigesArena.xml
		150, // OlympiadOrbisArena.xml
	};
	private readonly Map<int, Duel> _duels = new();
	private readonly AtomicInteger _currentDuelId = new AtomicInteger();

	protected DuelManager()
	{
	}

	public Duel? getDuel(int duelId)
	{
		return _duels.get(duelId);
	}

	public void addDuel(Player playerA, Player playerB, bool partyDuel)
	{
		if (playerA == null || playerB == null)
			return;

        Party? partyA = playerA.getParty();
        Party? partyB = playerB.getParty();
        if (partyA == null || partyB == null)
            return;

		// return if a player has PvPFlag
		string engagedInPvP = "The duel was canceled because a duelist engaged in PvP combat.";
		if (partyDuel)
		{
			bool playerInPvP = false;
			foreach (Player temp in partyA.getMembers())
			{
				if (temp.getPvpFlag() != PvpFlagStatus.None)
				{
					playerInPvP = true;
					break;
				}
			}
			if (!playerInPvP)
			{
				foreach (Player temp in partyB.getMembers())
				{
					if (temp.getPvpFlag() != PvpFlagStatus.None)
					{
						playerInPvP = true;
						break;
					}
				}
			}
			// A player has PvP flag
			if (playerInPvP)
			{
				foreach (Player temp in partyA.getMembers())
				{
					temp.sendMessage(engagedInPvP);
				}
				foreach (Player temp in partyB.getMembers())
				{
					temp.sendMessage(engagedInPvP);
				}
				return;
			}
		}
		else if (playerA.getPvpFlag() != PvpFlagStatus.None || playerB.getPvpFlag() != PvpFlagStatus.None)
		{
			playerA.sendMessage(engagedInPvP);
			playerB.sendMessage(engagedInPvP);
			return;
		}
		int duelId = _currentDuelId.incrementAndGet();
		_duels.put(duelId, new Duel(playerA, playerB, partyDuel, duelId));
	}

	public void removeDuel(Duel duel)
	{
		_duels.remove(duel.getId());
	}

	public void doSurrender(Player player)
	{
		if (player == null || !player.isInDuel())
		{
			return;
		}

        Duel? duel = getDuel(player.getDuelId());
		duel?.doSurrender(player);
	}

	/**
	 * Updates player states.
	 * @param player - the dying player
	 */
	public void onPlayerDefeat(Player player)
	{
		if (player == null || !player.isInDuel())
		{
			return;
		}

		Duel? duel = getDuel(player.getDuelId());
		if (duel != null)
		{
			duel.onPlayerDefeat(player);
		}
	}

	/**
	 * Registers a buff which will be removed if the duel ends
	 * @param player
	 * @param buff
	 */
	public void onBuff(Player player, Skill buff)
	{
		if (player == null || !player.isInDuel() || buff == null)
		{
			return;
		}
		Duel? duel = getDuel(player.getDuelId());
		if (duel != null)
		{
			duel.onBuff(player, buff);
		}
	}

	/**
	 * Removes player from duel.
	 * @param player - the removed player
	 */
	public void onRemoveFromParty(Player player)
	{
		if (player == null || !player.isInDuel())
		{
			return;
		}
		Duel? duel = getDuel(player.getDuelId());
		if (duel != null)
		{
			duel.onRemoveFromParty(player);
		}
	}

	/**
	 * Broadcasts a packet to the team opposing the given player.
	 * @param player
	 * @param packet
	 */
	public void broadcastToOppositTeam<TPacket>(Player player, TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		if (player == null || !player.isInDuel())
		{
			return;
		}
		Duel? duel = getDuel(player.getDuelId());
		if (duel == null)
		{
			return;
		}

        Player playerA = duel.getPlayerA();
        Player playerB = duel.getPlayerB();
		if (playerA == null || playerB == null)
			return;

		if (playerA == player)
		{
			duel.broadcastToTeam2(packet);
		}
		else if (playerB == player)
		{
			duel.broadcastToTeam1(packet);
		}
		else if (duel.isPartyDuel())
        {
            Party? partyA = playerA.getParty();
            Party? partyB = playerB.getParty();
			if (partyA != null && partyA.getMembers().Contains(player))
			{
				duel.broadcastToTeam2(packet);
			}
			else if (partyB != null && partyB.getMembers().Contains(player))
			{
				duel.broadcastToTeam1(packet);
			}
		}
	}

	/**
	 * Gets new a random Olympiad Stadium instance name.
	 * @return an instance name
	 */
	public int getDuelArena()
	{
		return ARENAS[Rnd.get(ARENAS.Length)];
	}

	public static DuelManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly DuelManager INSTANCE = new DuelManager();
	}
}