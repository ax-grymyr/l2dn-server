using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.SteadyBoxes;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.SteadyBox;

public struct RequestSteadyBoxLoadPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        player.getAchievementBox().tryFinishBox();
        connection.Send(new ExSteadyAllBoxUpdatePacket(player));
        return ValueTask.CompletedTask;
    }
}