using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AllyDismissPacket: IIncomingPacket<GameSession>
{
    private string _clanName;

    public void ReadContent(PacketBitReader reader)
    {
        _clanName = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || string.IsNullOrEmpty(_clanName))
            return ValueTask.CompletedTask;

        Clan? leaderClan = player.getClan();
        if (leaderClan == null)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_A_CLAN_MEMBER_2);
            return ValueTask.CompletedTask;
        }

        if (leaderClan.getAllyId() == 0)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_IN_AN_ALLIANCE);
            return ValueTask.CompletedTask;
        }

        if (!player.isClanLeader() || leaderClan.getId() != leaderClan.getAllyId())
        {
            player.sendPacket(SystemMessageId.ACCESS_ONLY_FOR_THE_CHANNEL_FOUNDER);
            return ValueTask.CompletedTask;
        }

        Clan? clan = ClanTable.getInstance().getClanByName(_clanName);
        if (clan == null)
        {
            player.sendPacket(SystemMessageId.THAT_CLAN_DOES_NOT_EXIST);
            return ValueTask.CompletedTask;
        }

        if (clan.getId() == leaderClan.getId())
        {
            player.sendPacket(SystemMessageId.ALLIANCE_LEADERS_CANNOT_WITHDRAW);
            return ValueTask.CompletedTask;
        }

        if (clan.getAllyId() != leaderClan.getAllyId())
        {
            player.sendPacket(SystemMessageId.DIFFERENT_ALLIANCE);
            return ValueTask.CompletedTask;
        }

        DateTime currentTime = DateTime.UtcNow;
        leaderClan.setAllyPenaltyExpiryTime(currentTime.AddDays(Config.Character.ALT_ACCEPT_CLAN_DAYS_WHEN_DISMISSED), Clan.PENALTY_TYPE_DISMISS_CLAN);
        leaderClan.updateClanInDB();

        clan.setAllyId(0);
        clan.setAllyName(null);
        clan.changeAllyCrest(0, true);
        clan.setAllyPenaltyExpiryTime(currentTime.AddDays(Config.Character.ALT_ALLY_JOIN_DAYS_WHEN_DISMISSED), Clan.PENALTY_TYPE_CLAN_DISMISSED);
        clan.updateClanInDB();

        player.sendPacket(SystemMessageId.THE_CLAN_IS_DISMISSED_FROM_THE_ALLIANCE);

        return ValueTask.CompletedTask;
    }
}