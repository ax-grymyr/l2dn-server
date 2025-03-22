using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;
using L2Dn.GameServer.StaticData;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author Mobius
 */
public class TimedHuntingZone(int id, ZoneForm form): Zone(id, form)
{
    protected override void onEnter(Creature creature)
	{
		if (!creature.isPlayer())
		{
			return;
		}

		Player? player = creature.getActingPlayer();
		if (player != null)
		{
			player.setInsideZone(ZoneId.TIMED_HUNTING, true);

			foreach (TimedHuntingZoneHolder holder in TimedHuntingZoneData.Instance.HuntingZones)
			{
				if (!player.isInTimedHuntingZone(holder.ZoneId))
				{
					continue;
				}

				TimeSpan remainingTime = player.getTimedHuntingZoneRemainingTime(holder.ZoneId);
				if (remainingTime > TimeSpan.Zero)
				{
					player.startTimedHuntingZone(holder.ZoneId, DateTime.UtcNow + remainingTime);
					player.getVariables().Set(PlayerVariables.LAST_HUNTING_ZONE_ID, holder.ZoneId);
					if (holder.IsPvpZone)
					{
						if (!player.isInsideZone(ZoneId.PVP))
						{
							player.sendPacket(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
						}

						player.setInsideZone(ZoneId.PVP, true);
						if (player.hasServitors())
						{
							player.getServitors().Values.ForEach(s => s.setInsideZone(ZoneId.PVP, true));
						}

                        Pet? pet = player.getPet();
						if (player.hasPet() && pet != null)
						{
							pet.setInsideZone(ZoneId.PVP, true);
						}
					}
					else if (holder.IsNoPvpZone)
					{
						player.setInsideZone(ZoneId.NO_PVP, true);
						if (player.hasServitors())
						{
							player.getServitors().Values.ForEach(s => s.setInsideZone(ZoneId.NO_PVP, true));
						}

                        Pet? pet = player.getPet();
						if (player.hasPet() && pet != null)
						{
							pet.setInsideZone(ZoneId.NO_PVP, true);
						}
					}

					// Send player info to nearby players.
					if (!player.isTeleporting())
					{
						player.broadcastInfo();
					}
					return;
				}
				break;
			}

			if (!player.isGM())
			{
				player.teleToLocation(MapRegionManager.GetTeleToLocation(player, TeleportWhereType.TOWN));
			}
		}
	}

	protected override void onExit(Creature creature)
	{
		if (!creature.isPlayer())
		{
			return;
		}

		Player? player = creature.getActingPlayer();
		if (player != null)
		{
			player.setInsideZone(ZoneId.TIMED_HUNTING, false);

			int lastHuntingZoneId = player.getVariables().Get(PlayerVariables.LAST_HUNTING_ZONE_ID, 0);
			TimedHuntingZoneHolder? holder = TimedHuntingZoneData.Instance.GetHuntingZone(lastHuntingZoneId);
			if (holder != null)
			{
				if (holder.IsPvpZone)
				{
					player.setInsideZone(ZoneId.PVP, false);
					if (player.hasServitors())
					{
						player.getServitors().Values.ForEach(s => s.setInsideZone(ZoneId.PVP, false));
					}

                    Pet? pet = player.getPet();
					if (player.hasPet() && pet != null)
					{
						pet.setInsideZone(ZoneId.PVP, false);
					}

					if (!player.isInsideZone(ZoneId.PVP))
					{
						creature.sendPacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
					}
				}
				else if (holder.IsNoPvpZone)
				{
					player.setInsideZone(ZoneId.NO_PVP, false);
					if (player.hasServitors())
					{
						player.getServitors().Values.ForEach(s => s.setInsideZone(ZoneId.NO_PVP, false));
					}

                    Pet? pet = player.getPet();
                    if (player.hasPet() && pet != null)
					{
						pet.setInsideZone(ZoneId.NO_PVP, false);
					}
				}

				// Send player info to nearby players.
				if (!player.isTeleporting())
				{
					player.broadcastInfo();
				}
			}

			ThreadPool.schedule(() =>
			{
				if (!player.isInTimedHuntingZone())
				{
					player.sendPacket(new TimedHuntingZoneExitPacket(lastHuntingZoneId));
				}
			}, 1000);
		}
	}
}