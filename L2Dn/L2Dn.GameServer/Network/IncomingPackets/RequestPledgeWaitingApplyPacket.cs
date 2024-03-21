using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeWaitingApplyPacket: IIncomingPacket<GameSession>
{
    private int _karma;
    private int _clanId;
    private String _message;

    public void ReadContent(PacketBitReader reader)
    {
        _karma = reader.ReadInt32();
        _clanId = reader.ReadInt32();
        _message = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || (player.getClan() != null))
            return ValueTask.CompletedTask;
        
        Clan clan = ClanTable.getInstance().getClan(_clanId);
        if (clan == null)
            return ValueTask.CompletedTask;
		
        PledgeApplicantInfo info = new PledgeApplicantInfo(player.getObjectId(), player.getName(), player.getLevel(), _karma, _clanId, _message);
        if (ClanEntryManager.getInstance().addPlayerApplicationToClan(_clanId, info))
        {
            player.sendPacket(new ExPledgeRecruitApplyInfoPacket(ClanEntryStatus.WAITING));
			
            Player clanLeader = World.getInstance().getPlayer(clan.getLeaderId());
            if (clanLeader != null)
            {
                clanLeader.sendPacket(ExPledgeWaitingListAlarmPacket.STATIC_PACKET);
            }
        }
        else
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_MAY_APPLY_FOR_ENTRY_IN_S1_MIN_AFTER_CANCELLING_YOUR_APPLICATION);
            sm.Params.addLong((long)ClanEntryManager.getInstance().getPlayerLockTime(player.getObjectId()).TotalMinutes);
            player.sendPacket(sm);
        }

        return ValueTask.CompletedTask;
    }
}