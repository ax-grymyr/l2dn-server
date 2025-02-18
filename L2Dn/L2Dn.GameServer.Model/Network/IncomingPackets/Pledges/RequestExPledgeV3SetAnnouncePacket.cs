using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestExPledgeV3SetAnnouncePacket: IIncomingPacket<GameSession>
{
    private string _announce;
    private bool _enterWorldShow;

    public void ReadContent(PacketBitReader reader)
    {
        _announce = reader.ReadSizedString();
        _enterWorldShow = reader.ReadByte() == 1;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        clan.setNotice(_announce);
        clan.setNoticeEnabled(_enterWorldShow);

        clan.broadcastToOnlineMembers(new ExPledgeV3InfoPacket(clan.getExp(), clan.getRank(), clan.getNotice(),
            clan.isNoticeEnabled()));

        return ValueTask.CompletedTask;
    }
}