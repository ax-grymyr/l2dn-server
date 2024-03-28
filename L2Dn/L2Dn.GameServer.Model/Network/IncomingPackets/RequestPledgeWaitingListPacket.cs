using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeWaitingListPacket: IIncomingPacket<GameSession>
{
    private int _clanId;

    public void ReadContent(PacketBitReader reader)
    {
        _clanId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || (player.getClanId() != _clanId))
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPledgeWaitingListPacket(_clanId));

        return ValueTask.CompletedTask;
    }
}