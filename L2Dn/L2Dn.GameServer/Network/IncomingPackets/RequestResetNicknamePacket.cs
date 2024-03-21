using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestResetNicknamePacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.getAppearance().setTitleColor(new Color(0xFFFF77));
        player.setTitle(string.Empty);
        player.broadcastTitleInfo();
        
        return ValueTask.CompletedTask;
    }
}