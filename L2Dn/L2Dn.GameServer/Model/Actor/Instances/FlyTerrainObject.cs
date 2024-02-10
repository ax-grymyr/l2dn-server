using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class FlyTerrainObject: Npc
{
    public FlyTerrainObject(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.FlyTerrainObject);
        setFlying(true);
    }

    public override void onAction(Player player, bool interact)
    {
        player.sendPacket(ActionFailedPacket.STATIC_PACKET);
    }

    public override void onActionShift(Player player)
    {
        if (player.isGM())
        {
            base.onActionShift(player);
        }
        else
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
        }
    }
}