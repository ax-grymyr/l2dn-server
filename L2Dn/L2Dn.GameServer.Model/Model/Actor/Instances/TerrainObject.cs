using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class TerrainObject: Npc
{
    public TerrainObject(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.TerrainObject;
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