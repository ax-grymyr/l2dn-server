using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

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
					player.sendPacket(new FakePlayerInfoPacket((Npc) creature));
				}
				else if (creature.getRunSpeed() == 0)
				{
					player.sendPacket(new ServerObjectInfoPacket((Npc) creature, player));
				}
				else
				{
					player.sendPacket(new NpcInfoPacket((Npc) creature));
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
			// TODO temporary fix of monsters remaining corpses after respawn
			// if Npc is deleted from the world, then forEachVisibleObject must return no visible object for it
			// Changes in forEachVisibleObject may affect many functionality, so temporary fix here.
			if (((Npc)creature).isDecayed())
				return;

			World.getInstance().forEachVisibleObject<Player>(creature, player =>
			{
				if (creature.isFakePlayer())
				{
					player.sendPacket(new FakePlayerInfoPacket((Npc) creature));
				}
				else if (creature.getRunSpeed() == 0)
				{
					player.sendPacket(new ServerObjectInfoPacket((Npc) creature, player));
				}
				else
				{
					player.sendPacket(new NpcInfoPacket((Npc) creature));
				}
			});
		}
	}

	public int getWaterZ()
	{
		return getZone().getHighZ();
	}
}