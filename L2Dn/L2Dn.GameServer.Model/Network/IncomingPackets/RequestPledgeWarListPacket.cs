using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeWarListPacket: IIncomingPacket<GameSession>
{
    private int _page;
    private int _tab;

    public void ReadContent(PacketBitReader reader)
    {
        _page = reader.ReadInt32();
        _tab = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new PledgeReceiveWarListPacket(clan, _tab));

        return ValueTask.CompletedTask;
    }
}