using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExDualInventorySwapPacket: IIncomingPacket<GameSession>
{
    private int _slot;

    public void ReadContent(PacketBitReader reader)
    {
        _slot = reader.ReadByte() == 0 ? 0 : 1; // 0 = A, 1 = B
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.setDualInventorySlot(_slot);
        
        return ValueTask.CompletedTask;
    }
}