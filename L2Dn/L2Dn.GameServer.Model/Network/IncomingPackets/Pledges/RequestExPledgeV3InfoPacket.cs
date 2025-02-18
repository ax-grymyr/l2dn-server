using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestExPledgeV3InfoPacket: IIncomingPacket<GameSession>
{
    private int _page;

    public void ReadContent(PacketBitReader reader)
    {
        _page = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPledgeV3InfoPacket(clan.getExp(), clan.getRank(),
            clan.getNotice(), clan.isNoticeEnabled()));

        player.sendPacket(new PledgeReceiveWarListPacket(clan, _page));
        player.sendPacket(new ExPledgeClassicRaidInfoPacket(player));

        return ValueTask.CompletedTask;
    }
}