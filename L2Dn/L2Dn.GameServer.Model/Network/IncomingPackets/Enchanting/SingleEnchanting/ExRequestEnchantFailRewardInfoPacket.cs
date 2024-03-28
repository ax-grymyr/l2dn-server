using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.SingleEnchanting;

public struct ExRequestEnchantFailRewardInfoPacket: IIncomingPacket<GameSession>
{
    private int _itemobjectid;

    public void ReadContent(PacketBitReader reader)
    {
        _itemobjectid = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item addedItem = player.getInventory().getItemByObjectId(_itemobjectid);
        if (addedItem != null)
        {
            player.sendPacket(new ResetEnchantItemFailRewardInfoPacket(player));
        }
        
        return ValueTask.CompletedTask;
    }
}