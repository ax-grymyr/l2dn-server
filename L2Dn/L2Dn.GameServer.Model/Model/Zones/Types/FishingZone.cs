using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Fishings;
using L2Dn.GameServer.Network.OutgoingPackets.Fishing;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A fishing zone
 * @author durgus
 */
public class FishingZone : ZoneType
{
	public FishingZone(int id): base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
		if (creature.isPlayer())
		{
			if ((Config.ALLOW_FISHING || creature.canOverrideCond(PlayerCondOverride.ZONE_CONDITIONS)) && !creature.isInsideZone(ZoneId.FISHING))
			{
				WeakReference<Player> weakPlayer = new WeakReference<Player>(creature.getActingPlayer());
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
		return getZone().getHighZ();
	}

	protected class FishingAvailableTask: Runnable
	{
		private readonly WeakReference<Player> _weakPlayer;

		public FishingAvailableTask(WeakReference<Player> weakPlayer)
		{
			_weakPlayer = weakPlayer;
		}

		public void run()
		{
			Player player = _weakPlayer.get();
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