using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Commission;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Commission;

public struct RequestCommissionInfoPacket: IIncomingPacket<GameSession>
{
    private int _itemObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjectId = reader.ReadInt32();
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
		
        Item itemInstance = player.getInventory().getItemByObjectId(_itemObjectId);
        if (itemInstance != null)
        {
            player.sendPacket(player.getLastCommissionInfos().getOrDefault(itemInstance.getId(), ExResponseCommissionInfoPacket.EMPTY));
        }
        else
        {
            player.sendPacket(ExResponseCommissionInfoPacket.EMPTY);
        }
        
        return ValueTask.CompletedTask;
    }
}