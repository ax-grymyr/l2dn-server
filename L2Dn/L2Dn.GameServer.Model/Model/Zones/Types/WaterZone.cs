using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Zones.Types;

public class WaterZone(int id, ZoneForm form): ZoneType(id, form)
{
    protected override void onEnter(Creature creature)
    {
        creature.setInsideZone(ZoneId.WATER, true);

        // TODO: update to only send speed status when that packet is known
        Player? player = creature.getActingPlayer();
        if (creature.isPlayer() && player != null)
        {
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
                    player.sendPacket(new FakePlayerInfoPacket((Npc)creature));
                }
                else if (creature.getRunSpeed() == 0)
                {
                    player.sendPacket(new ServerObjectInfoPacket((Npc)creature, player));
                }
                else
                {
                    player.sendPacket(new NpcInfoPacket((Npc)creature));
                }
            });
        }
    }

    protected override void onExit(Creature creature)
    {
        creature.setInsideZone(ZoneId.WATER, false);

        // TODO: update to only send speed status when that packet is known
        Player? player = creature.getActingPlayer();
        if (creature.isPlayer() && player != null)
        {
            // Mobius: Attempt to stop water task.
            if (!creature.isInsideZone(ZoneId.WATER))
            {
                player.stopWaterTask();
            }

            if (!creature.isTeleporting())
            {
                player.broadcastUserInfo();
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
                    player.sendPacket(new FakePlayerInfoPacket((Npc)creature));
                }
                else if (creature.getRunSpeed() == 0)
                {
                    player.sendPacket(new ServerObjectInfoPacket((Npc)creature, player));
                }
                else
                {
                    player.sendPacket(new NpcInfoPacket((Npc)creature));
                }
            });
        }
    }

    public int getWaterZ()
    {
        return getZone().HighZ;
    }
}