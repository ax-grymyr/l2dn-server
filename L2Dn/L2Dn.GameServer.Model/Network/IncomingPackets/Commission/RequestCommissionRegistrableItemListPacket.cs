using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Commission;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Commission;

public struct RequestCommissionRegistrableItemListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (!ItemCommissionManager.isPlayerAllowedToInteract(player))
        {
            player.sendPacket(ExCloseCommissionPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(new ExResponseCommissionItemListPacket(1,
            player.getInventory().getAvailableItems(false, false, false)));
        
        player.sendPacket(new ExResponseCommissionItemListPacket(2,
            player.getInventory().getAvailableItems(false, false, false)));

        return ValueTask.CompletedTask;
    }
}