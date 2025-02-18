using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeRecruitApplyInfoPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();

        ClanEntryStatus status;
        if (clan != null && player.isClanLeader() && ClanEntryManager.getInstance().isClanRegistred(clan.getId()))
        {
            status = ClanEntryStatus.ORDERED;
        }
        else if (clan == null && ClanEntryManager.getInstance().isPlayerRegistred(player.ObjectId))
        {
            status = ClanEntryStatus.WAITING;
        }
        else
        {
            status = ClanEntryStatus.DEFAULT;
        }

        player.sendPacket(new ExPledgeRecruitApplyInfoPacket(status));
        return ValueTask.CompletedTask;
    }
}