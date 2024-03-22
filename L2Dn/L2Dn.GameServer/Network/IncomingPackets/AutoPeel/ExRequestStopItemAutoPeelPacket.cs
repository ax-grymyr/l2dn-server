using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPeel;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.AutoPeel;

public struct ExRequestStopItemAutoPeelPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
        //reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.removeRequest<AutoPeelRequest>();
        player.sendPacket(new ExStopItemAutoPeelPacket(true));
        player.sendPacket(new ExReadyItemAutoPeelPacket(false, 0));
        
        return ValueTask.CompletedTask;
    }
}