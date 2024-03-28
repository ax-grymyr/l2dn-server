using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Friends;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Friends;

public struct RequestUpdateFriendMemoPacket: IIncomingPacket<GameSession>
{
    private string _name;
    private string _memo;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
        _memo = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.updateFriendMemo(_name, _memo);
        player.sendPacket(new ExFriendDetailInfoPacket(player, _name));

        return ValueTask.CompletedTask;
    }
}