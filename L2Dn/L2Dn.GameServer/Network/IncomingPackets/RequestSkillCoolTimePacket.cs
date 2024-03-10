using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSkillCoolTimePacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        
        // ignore packet when player entering game, SkillCoolTime will be sent later
        if (player != null && session.State == GameSessionState.InGame)
            connection.Send(new SkillCoolTimePacket(player));

        return ValueTask.CompletedTask;
    }
}