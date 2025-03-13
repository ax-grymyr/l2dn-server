using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.WorldExchange;

public struct ExWorldExchangeSettleRecvResultPacket: IIncomingPacket<GameSession>
{
    private long _worldExchangeIndex;

    public void ReadContent(PacketBitReader reader)
    {
        _worldExchangeIndex = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.ENABLE_WORLD_EXCHANGE)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        WorldExchangeManager.getInstance().getItemStatusAndMakeAction(player, _worldExchangeIndex);

        return ValueTask.CompletedTask;
    }
}