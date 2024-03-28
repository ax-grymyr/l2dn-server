using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.WorldExchange;

public struct ExWorldExchangeTotalListPacket: IIncomingPacket<GameSession>
{
    private List<int>? _itemIds;

    public void ReadContent(PacketBitReader reader)
    {
        int size = reader.ReadInt32();
        if (size > 0 && size < 20000)
        {
            _itemIds = new List<int>(size);
            for (int index = 0; index < size; index++)
                _itemIds.Add(reader.ReadInt32());
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_itemIds != null)
            player.sendPacket(new WorldExchangeTotalListPacket(_itemIds));
        
        return ValueTask.CompletedTask;
    }
}