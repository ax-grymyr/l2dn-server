using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

public class WaterZone : ZoneType
{
	public WaterZone(int id): base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.WATER, true);
		
		// TODO: update to only send speed status when that packet is known
		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			if (player.checkTransformed(transform => !transform.canSwim()))
			{
				creature.stopTransformation(true);
			}
			else
			{
				player.broadcastUserInfo();
			}
		}
		else if (creature.isNpc())
		{
			World.getInstance().forEachVisibleObject<Player>(creature, player =>
			{
				if (creature.isFakePlayer())
				{
					player.sendPacket(new FakePlayerInfo((Npc) creature));
				}
				else if (creature.getRunSpeed() == 0)
				{
					player.sendPacket(new ServerObjectInfo((Npc) creature, player));
				}
				else
				{
					player.sendPacket(new NpcInfo((Npc) creature));
				}
			});
		}
	}
	
	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.WATER, false);
		
		// TODO: update to only send speed status when that packet is known
		if (creature.isPlayer())
		{
			// Mobius: Attempt to stop water task.
			if (!creature.isInsideZone(ZoneId.WATER))
			{
				((Player) creature).stopWaterTask();
			}
			if (!creature.isTeleporting())
			{
				creature.getActingPlayer().broadcastUserInfo();
			}
		}
		else if (creature.isNpc())
		{
			World.getInstance().forEachVisibleObject<Player>(creature, player =>
			{
				if (creature.isFakePlayer())
				{
					player.sendPacket(new FakePlayerInfo((Npc) creature));
				}
				else if (creature.getRunSpeed() == 0)
				{
					player.sendPacket(new ServerObjectInfo((Npc) creature, player));
				}
				else
				{
					player.sendPacket(new NpcInfo((Npc) creature));
				}
			});
		}
	}
	
	public int getWaterZ()
	{
		return getZone().getHighZ();
	}
}