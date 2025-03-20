using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AllyLeavePacket: IIncomingPacket<GameSession>
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
        if (clan == null)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_A_CLAN_MEMBER_2);
            return ValueTask.CompletedTask;
        }

        if (!player.isClanLeader())
        {
            player.sendPacket(SystemMessageId.ONLY_THE_CLAN_LEADER_MAY_APPLY_FOR_WITHDRAWAL_FROM_THE_ALLIANCE);
            return ValueTask.CompletedTask;
        }

        if (clan.getAllyId() == 0)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_IN_AN_ALLIANCE);
            return ValueTask.CompletedTask;
        }

        if (clan.Id == clan.getAllyId())
        {
            player.sendPacket(SystemMessageId.ALLIANCE_LEADERS_CANNOT_WITHDRAW);
            return ValueTask.CompletedTask;
        }

        DateTime currentTime = DateTime.UtcNow;
        clan.setAllyId(0);
        clan.setAllyName(null);
        clan.changeAllyCrest(0, true);
        clan.setAllyPenaltyExpiryTime(currentTime.AddDays(Config.Character.ALT_ALLY_JOIN_DAYS_WHEN_LEAVED * 86400000), Clan.PENALTY_TYPE_CLAN_LEAVED);
        clan.updateClanInDB();

        player.sendPacket(SystemMessageId.YOU_HAVE_LEFT_THE_ALLIANCE);

        return ValueTask.CompletedTask;
    }
}