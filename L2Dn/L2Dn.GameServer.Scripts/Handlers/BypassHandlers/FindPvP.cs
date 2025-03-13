using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

/**
 * @author Mobius (based on Tenkai pvpzone)
 */
public class FindPvP: IBypassHandler
{
	private static readonly string[] COMMANDS =
    [
        "FindPvP",
    ];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (!Config.ENABLE_FIND_PVP || target is null || !target.isNpc())
		{
			return false;
		}

		Player? mostPvP = null;
		int max = -1;
		foreach (Player plr in World.getInstance().getPlayers())
		{
			if (plr == null //
				|| plr.getPvpFlag() == PvpFlagStatus.None //
				|| plr.getInstanceId() != 0 //
				|| plr.isGM() //
				|| plr.isInsideZone(ZoneId.PEACE) //
				|| plr.isInsideZone(ZoneId.SIEGE) //
				|| plr.isInsideZone(ZoneId.NO_SUMMON_FRIEND))
			{
				continue;
			}

			int count = 0;
			foreach (Player pl in World.getInstance().getVisibleObjects<Player>(plr))
			{
				if (pl.getPvpFlag() != PvpFlagStatus.None && !pl.isInsideZone(ZoneId.PEACE))
				{
					count++;
				}
			}

			if (count > max)
			{
				max = count;
				mostPvP = plr;
			}
		}

		if (mostPvP != null)
		{
			// Check if the player's clan is already outnumbering the PvP
            Clan? clan = player.getClan();
			if (clan != null)
			{
				Map<int, int> clanNumbers = new();
				int? allyId = player.getAllyId();
				if (allyId is null)
				{
					allyId = clan.getId();
				}

				clanNumbers.put(allyId.Value, 1);
				foreach (Player known in World.getInstance().getVisibleObjects<Player>(mostPvP))
				{
					int? knownAllyId = known.getAllyId();
					if (knownAllyId is null)
					{
						knownAllyId = known.getClanId();
					}

					if (knownAllyId != null)
					{
						if (clanNumbers.ContainsKey(knownAllyId.Value))
						{
							clanNumbers.put(knownAllyId.Value, clanNumbers.get(knownAllyId.Value) + 1);
						}
						else
						{
							clanNumbers.put(knownAllyId.Value, 1);
						}
					}
				}

				int biggestAllyId = 0;
				int biggestAmount = 2;
				foreach (var clanNumber in clanNumbers)
				{
					if (clanNumber.Value > biggestAmount)
					{
						biggestAllyId = clanNumber.Key;
						biggestAmount = clanNumber.Value;
					}
				}

				if (biggestAllyId == allyId)
				{
					player.sendPacket(new CreatureSayPacket(null, ChatType.WHISPER, target.getName(),
						"Sorry, your clan/ally is outnumbering the place already so you can't move there."));

					return true;
				}
			}

			player.teleToLocation(new Location3D(mostPvP.getX() + Rnd.get(300) - 150,
				mostPvP.getY() + Rnd.get(300) - 150, mostPvP.getZ()));

			player.setSpawnProtection(true);
			if (!player.isGM())
			{
				player.setPvpFlagLasts(DateTime.UtcNow + Config.PVP_PVP_TIME);
				player.startPvPFlag();
			}
		}
		else
		{
			player.sendPacket(new CreatureSayPacket(null, ChatType.WHISPER, target.getName(),
				"Sorry, I can't find anyone in flag status right now."));
		}
		return false;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}