using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Fishings;
using L2Dn.GameServer.Network.OutgoingPackets.Fishing;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A fishing zone
 * @author durgus
 */
public class FishingZone(int id, ZoneForm form): Zone(id, form)
{
    protected override void onEnter(Creature creature)
    {
        Player? player = creature.getActingPlayer();
		if (creature.isPlayer() && player != null)
		{
			if ((Config.General.ALLOW_FISHING || creature.canOverrideCond(PlayerCondOverride.ZONE_CONDITIONS)) && !creature.isInsideZone(ZoneId.FISHING))
			{
				WeakReference<Player> weakPlayer = new WeakReference<Player>(player);
				ThreadPool.execute(new FishingAvailableTask(weakPlayer));
			}
			creature.setInsideZone(ZoneId.FISHING, true);
		}
	}

	protected override void onExit(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.FISHING, false);
			creature.sendPacket(ExAutoFishAvailablePacket.NO);
		}
	}

	/*
	 * getWaterZ() this added function returns the Z value for the water surface. In effect this simply returns the upper Z value of the zone. This required some modification of ZoneForm, and zone form extensions.
	 */
	public int getWaterZ()
	{
		return getZone().HighZ;
	}

	protected class FishingAvailableTask(WeakReference<Player> weakPlayer): Runnable
    {
        public void run()
		{
			Player? player = weakPlayer.get();
			if (player != null)
			{
				Fishing fishing = player.getFishing();
				if (player.isInsideZone(ZoneId.FISHING))
				{
					if (fishing.canFish() && !fishing.isFishing())
					{
						if (fishing.isAtValidLocation())
						{
							player.sendPacket(ExAutoFishAvailablePacket.YES);
						}
						else
						{
							player.sendPacket(ExAutoFishAvailablePacket.NO);
						}
					}
					ThreadPool.schedule(this, 1500);
				}
				else
				{
					player.sendPacket(ExAutoFishAvailablePacket.NO);
				}
			}
		}
	}
}