using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeWaitingListAppliedPacket: IOutgoingPacket
{
    private readonly PledgeApplicantInfo _pledgePlayerRecruitInfo;
    private readonly PledgeRecruitInfo _pledgeRecruitInfo;

    public ExPledgeWaitingListAppliedPacket(int clanId, int playerId)
    {
        _pledgePlayerRecruitInfo = ClanEntryManager.getInstance().getPlayerApplication(clanId, playerId) ??
            throw new ArgumentException("Clan entry not found", nameof(clanId));

        _pledgeRecruitInfo = ClanEntryManager.getInstance().getClanById(clanId) ??
            throw new ArgumentException("Clan entry not found", nameof(clanId));
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_WAITING_LIST_APPLIED);

        writer.WriteInt32(_pledgeRecruitInfo.getClan().getId());
        writer.WriteString(_pledgeRecruitInfo.getClan().getName());
        writer.WriteString(_pledgeRecruitInfo.getClan().getLeaderName());
        writer.WriteInt32(_pledgeRecruitInfo.getClan().getLevel());
        writer.WriteInt32(_pledgeRecruitInfo.getClan().getMembersCount());
        writer.WriteInt32(_pledgeRecruitInfo.getKarma());
        writer.WriteString(_pledgeRecruitInfo.getInformation());
        writer.WriteString(_pledgePlayerRecruitInfo.getMessage());
    }
}