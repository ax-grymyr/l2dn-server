using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author Mobius
 */
public class TimedHuntingZone : ZoneType
{
	public TimedHuntingZone(int id): base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
		if (!creature.isPlayer())
		{
			return;
		}
		
		Player player = creature.getActingPlayer();
		if (player != null)
		{
			player.setInsideZone(ZoneId.TIMED_HUNTING, true);
			
			foreach (TimedHuntingZoneHolder holder in TimedHuntingZoneData.getInstance().getAllHuntingZones())
			{
				if (!player.isInTimedHuntingZone(holder.getZoneId()))
				{
					continue;
				}
				
				int remainingTime = player.getTimedHuntingZoneRemainingTime(holder.getZoneId());
				if (remainingTime > 0)
				{
					player.startTimedHuntingZone(holder.getZoneId(), DateTime.UtcNow.AddMilliseconds(remainingTime));
					player.getVariables().set(PlayerVariables.LAST_HUNTING_ZONE_ID, holder.getZoneId());
					if (holder.isPvpZone())
					{
						if (!player.isInsideZone(ZoneId.PVP))
						{
							player.sendPacket(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
						}
						
						player.setInsideZone(ZoneId.PVP, true);
						if (player.hasServitors())
						{
							player.getServitors().values().ForEach(s => s.setInsideZone(ZoneId.PVP, true));
						}
						if (player.hasPet())
						{
							player.getPet().setInsideZone(ZoneId.PVP, true);
						}
					}
					else if (holder.isNoPvpZone())
					{
						player.setInsideZone(ZoneId.NO_PVP, true);
						if (player.hasServitors())
						{
							player.getServitors().values().ForEach(s => s.setInsideZone(ZoneId.NO_PVP, true));
						}
						if (player.hasPet())
						{
							player.getPet().setInsideZone(ZoneId.NO_PVP, true);
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
				player.teleToLocation(MapRegionManager.getInstance().getTeleToLocation(player, TeleportWhereType.TOWN));
			}
		}
	}
	
	protected override void onExit(Creature creature)
	{
		if (!creature.isPlayer())
		{
			return;
		}
		
		Player player = creature.getActingPlayer();
		if (player != null)
		{
			player.setInsideZone(ZoneId.TIMED_HUNTING, false);
			
			int lastHuntingZoneId = player.getVariables().getInt(PlayerVariables.LAST_HUNTING_ZONE_ID, 0);
			TimedHuntingZoneHolder holder = TimedHuntingZoneData.getInstance().getHuntingZone(lastHuntingZoneId);
			if (holder != null)
			{
				if (holder.isPvpZone())
				{
					player.setInsideZone(ZoneId.PVP, false);
					if (player.hasServitors())
					{
						player.getServitors().values().ForEach(s => s.setInsideZone(ZoneId.PVP, false));
					}
					if (player.hasPet())
					{
						player.getPet().setInsideZone(ZoneId.PVP, false);
					}
					
					if (!player.isInsideZone(ZoneId.PVP))
					{
						creature.sendPacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
					}
				}
				else if (holder.isNoPvpZone())
				{
					player.setInsideZone(ZoneId.NO_PVP, false);
					if (player.hasServitors())
					{
						player.getServitors().values().ForEach(s => s.setInsideZone(ZoneId.NO_PVP, false));
					}
					if (player.hasPet())
					{
						player.getPet().setInsideZone(ZoneId.NO_PVP, false);
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