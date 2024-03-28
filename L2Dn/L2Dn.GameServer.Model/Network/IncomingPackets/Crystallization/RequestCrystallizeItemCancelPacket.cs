using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Crystallization;

public struct RequestCrystallizeItemCancelPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // if (!client.getFloodProtectors().canPerformTransaction())
        // {
        // player.sendMessage("You are crystallizing too fast.");
        // return;
        // }

        if (player.isInCrystallize())
        {
            player.setInCrystallize(false);
        }
        
        return ValueTask.CompletedTask;
    }
}