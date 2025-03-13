using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;
using L2Dn.GameServer.StaticData;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.WorldExchange;

public struct ExWorldExchangeSettleListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
        //reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.ENABLE_WORLD_EXCHANGE)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(WorldExchangeItemListPacket.EMPTY_LIST);
        player.sendPacket(new WorldExchangeSettleListPacket(player));

        return ValueTask.CompletedTask;
    }
}