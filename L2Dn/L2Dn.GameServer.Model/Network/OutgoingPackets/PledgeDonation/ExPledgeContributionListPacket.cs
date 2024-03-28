using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeDonation;

public readonly struct ExPledgeContributionListPacket: IOutgoingPacket
{
    private readonly ICollection<ClanMember> _contributors;
	
    public ExPledgeContributionListPacket(ICollection<ClanMember> contributors)
    {
        _contributors = contributors;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_CONTRIBUTION_LIST);
        
        writer.WriteInt32(_contributors.Count);
        foreach (ClanMember contributor in _contributors)
        {
            writer.WriteSizedString(contributor.getName());
            writer.WriteInt32(contributor.getClan().getClanContributionWeekly(contributor.getObjectId()));
            writer.WriteInt32(contributor.getClan().getClanContribution(contributor.getObjectId()));
        }
    }
}