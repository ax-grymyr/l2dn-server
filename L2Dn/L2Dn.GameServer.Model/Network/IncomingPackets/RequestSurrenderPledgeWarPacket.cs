using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSurrenderPledgeWarPacket: IIncomingPacket<GameSession>
{
    private string _pledgeName;

    public void ReadContent(PacketBitReader reader)
    {
        _pledgeName = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        foreach (ClanMember member in clan.getMembers())
        {
            Player? memberPlayer = member.getPlayer();
            if (member != null && member.isOnline() && memberPlayer != null && memberPlayer.isInCombat())
            {
                connection.Send(SystemMessageId.THE_CLAN_WAR_CANNOT_BE_STOPPED_BECAUSE_SOMEONE_FROM_YOUR_CLAN_IS_STILL_ENGAGED_IN_BATTLE);
                connection.Send(ActionFailedPacket.STATIC_PACKET);
                return ValueTask.CompletedTask;
            }
        }

        Clan? targetClan = ClanTable.getInstance().getClanByName(_pledgeName);
        if (targetClan == null)
        {
            connection.Send("No such clan.");
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (!player.hasClanPrivilege(ClanPrivilege.CL_PLEDGE_WAR))
        {
            connection.Send(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        ClanWar? clanWar = clan.getWarWith(targetClan.Id);
        if (clanWar == null)
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_NOT_DECLARED_A_CLAN_WAR_AGAINST_THE_CLAN_S1);
            sm.Params.addString(targetClan.getName());
            connection.Send(sm);
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (clanWar.getState() == ClanWarState.BLOOD_DECLARATION)
        {
            connection.Send(SystemMessageId.YOU_CANNOT_DECLARE_DEFEAT_AS_IT_HAS_NOT_BEEN_7_DAYS_SINCE_STARTING_A_CLAN_WAR_WITH_CLAN_S1);
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        clanWar.cancel(player, clan);
        return ValueTask.CompletedTask;
    }
}