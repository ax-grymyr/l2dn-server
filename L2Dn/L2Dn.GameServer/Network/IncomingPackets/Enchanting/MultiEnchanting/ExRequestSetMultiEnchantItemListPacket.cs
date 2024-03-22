using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.MultiEnchanting;

public struct ExRequestSetMultiEnchantItemListPacket: IIncomingPacket<GameSession>
{
    private int _slotId;
    private List<int> _itemObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _slotId = reader.ReadInt32();
        _itemObjectId = new List<int>();
        while (reader.Length != 0)
            _itemObjectId.Add(reader.ReadInt32());
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        EnchantItemRequest request = player.getRequest<EnchantItemRequest>();
        if (request == null)
        {
            player.sendPacket(new ExResultSetMultiEnchantItemListPacket(player, 1));
            return ValueTask.CompletedTask;
        }
		
        if (request.getMultiEnchantingItemsBySlot(_slotId) != -1)
        {
            request.clearMultiEnchantingItemsBySlot();
            for (int i = 0; i <= _slotId - 1; i++)
            {
                request.addMultiEnchantingItems(i, _itemObjectId[i]);
            }
        }
        else
        {
            request.addMultiEnchantingItems(_slotId, _itemObjectId[_slotId]);
        }
		
        player.sendPacket(new ExResultSetMultiEnchantItemListPacket(player, 0));
        player.sendPacket(new ChangedEnchantTargetItemProbabilityListPacket(player, true));
        
        return ValueTask.CompletedTask;
    }
}