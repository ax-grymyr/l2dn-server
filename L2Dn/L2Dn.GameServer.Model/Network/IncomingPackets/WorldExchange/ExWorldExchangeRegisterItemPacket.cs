using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.WorldExchange;

public struct ExWorldExchangeRegisterItemPacket: IIncomingPacket<GameSession>
{
    private long _price;
    private int _itemId;
    private long _amount;

    public void ReadContent(PacketBitReader reader)
    {
        _price = reader.ReadInt64();
        _itemId = reader.ReadInt32();
        _amount = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.WorldExchange.ENABLE_WORLD_EXCHANGE)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        WorldExchangeManager.getInstance().registerItemBid(player, _itemId, _amount, _price);

        return ValueTask.CompletedTask;
    }
}