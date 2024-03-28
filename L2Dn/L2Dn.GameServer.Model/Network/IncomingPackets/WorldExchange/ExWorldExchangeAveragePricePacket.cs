using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.WorldExchange;

public struct ExWorldExchangeAveragePricePacket: IIncomingPacket<GameSession>
{
    private int _itemId;

    public void ReadContent(PacketBitReader reader)
    {
        _itemId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new WorldExchangeAveragePricePacket(_itemId));

        return ValueTask.CompletedTask;
    }
}