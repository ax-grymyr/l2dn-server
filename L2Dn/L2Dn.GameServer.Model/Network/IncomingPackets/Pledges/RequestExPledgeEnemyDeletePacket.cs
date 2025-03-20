using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestExPledgeEnemyDeletePacket: IIncomingPacket<GameSession>
{
    private int _clanId;

    public void ReadContent(PacketBitReader reader)
    {
        _clanId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? playerClan = player.getClan();
        if (playerClan == null)
            return ValueTask.CompletedTask;

        Clan? enemyClan = ClanTable.getInstance().getClan(_clanId);
        if (enemyClan == null)
        {
            player.sendPacket(SystemMessageId.THERE_IS_NO_SUCH_CLAN);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (!playerClan.isAtWarWith(enemyClan.Id))
        {
            player.sendPacket(SystemMessageId.ENTER_THE_NAME_OF_THE_CLAN_YOU_WISH_TO_END_THE_WAR_WITH);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (!player.hasClanPrivilege(ClanPrivilege.CL_PLEDGE_WAR))
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
            return ValueTask.CompletedTask;
        }

        foreach (ClanMember member in playerClan.getMembers())
        {
            if (member == null || member.getPlayer() == null)
            {
                continue;
            }

            if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(member.getPlayer()))
            {
                player.sendPacket(SystemMessageId.THE_CLAN_WAR_CANNOT_BE_STOPPED_BECAUSE_SOMEONE_FROM_YOUR_CLAN_IS_STILL_ENGAGED_IN_BATTLE);
                return ValueTask.CompletedTask;
            }
        }

        // Reduce reputation.
        playerClan.takeReputationScore(500);
        ClanTable.getInstance().deleteClanWars(playerClan.Id, enemyClan.Id);

        broadcastClanInfo(playerClan, enemyClan);

        return ValueTask.CompletedTask;
    }

    private void broadcastClanInfo(Clan playerClan, Clan enemyClan)
    {
        foreach (ClanMember member in playerClan.getMembers())
        {
            Player? player = member.getPlayer();
            if (member != null && member.isOnline() && player != null)
            {
                player.sendPacket(new ExPledgeEnemyInfoListPacket(playerClan));
                player.broadcastUserInfo();
            }
        }

        foreach (ClanMember member in enemyClan.getMembers())
        {
            Player? player = member.getPlayer();
            if (member != null && member.isOnline() && player != null)
            {
                player.sendPacket(new ExPledgeEnemyInfoListPacket(enemyClan));
                player.broadcastUserInfo();
            }
        }
    }
}