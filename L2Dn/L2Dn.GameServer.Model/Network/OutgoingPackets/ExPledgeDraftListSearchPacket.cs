using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeDraftListSearchPacket: IOutgoingPacket
{
    private readonly List<PledgeWaitingInfo> _pledgeRecruitList;
	
    public ExPledgeDraftListSearchPacket(List<PledgeWaitingInfo> pledgeRecruitList)
    {
        _pledgeRecruitList = pledgeRecruitList;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_DRAFT_LIST_SEARCH);
        
        writer.WriteInt32(_pledgeRecruitList.Count);
        foreach (PledgeWaitingInfo prl in _pledgeRecruitList)
        {
            writer.WriteInt32(prl.getPlayerId());
            writer.WriteString(prl.getPlayerName());
            writer.WriteInt32(prl.getKarma());
            writer.WriteInt32((int)prl.getPlayerClassId());
            writer.WriteInt32(prl.getPlayerLvl());
        }
    }
}