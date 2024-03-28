using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Friends;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Friends;

public struct RequestBlockDetailInfoPacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExBlockDetailInfoPacket(player, _name));

        return ValueTask.CompletedTask;
    }
}