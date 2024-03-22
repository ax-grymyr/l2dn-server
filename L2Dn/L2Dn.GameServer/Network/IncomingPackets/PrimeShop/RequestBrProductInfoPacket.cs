using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.PrimeShop;

public struct RequestBrProductInfoPacket: IIncomingPacket<GameSession>
{
    private int _brId;

    public void ReadContent(PacketBitReader reader)
    {
        _brId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        PrimeShopData.getInstance().showProductInfo(player, _brId);
        
        return ValueTask.CompletedTask;
    }
}