using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeWaitingListPacket: IOutgoingPacket
{
    private readonly Map<int, PledgeApplicantInfo> _pledgePlayerRecruitInfos;
	
    public ExPledgeWaitingListPacket(int clanId)
    {
        _pledgePlayerRecruitInfos = ClanEntryManager.getInstance().getApplicantListForClan(clanId);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_WAITING_LIST);
        
        writer.WriteInt32(_pledgePlayerRecruitInfos.size());
        foreach (PledgeApplicantInfo recruitInfo in _pledgePlayerRecruitInfos.Values)
        {
            writer.WriteInt32(recruitInfo.getPlayerId());
            writer.WriteString(recruitInfo.getPlayerName());
            writer.WriteInt32((int)recruitInfo.getClassId());
            writer.WriteInt32(recruitInfo.getPlayerLvl());
        }
    }
}