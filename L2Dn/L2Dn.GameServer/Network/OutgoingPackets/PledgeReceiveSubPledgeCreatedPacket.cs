using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeReceiveSubPledgeCreatedPacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PledgeReceiveSubPledgeCreatedPacket));
    private readonly Clan.SubPledge _subPledge;
    private readonly Clan _clan;
	
    public PledgeReceiveSubPledgeCreatedPacket(Clan.SubPledge subPledge, Clan clan)
    {
        _subPledge = subPledge;
        _clan = clan;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_RECEIVE_SUB_PLEDGE_CREATED);
        
        writer.WriteInt32(1);
        writer.WriteInt32(_subPledge.getId());
        writer.WriteString(_subPledge.getName());
        writer.WriteString(getLeaderName());
    }
	
    private string getLeaderName()
    {
        int leaderId = _subPledge.getLeaderId();
        if ((_subPledge.getId() == Clan.SUBUNIT_ACADEMY) || (leaderId == 0))
        {
            return "";
        }
        
        if (_clan.getClanMember(leaderId) == null)
        {
            _logger.Error("SubPledgeLeader: " + leaderId + " is missing from clan: " + _clan.getName() + "[" + _clan.getId() + "]");
            return "";
        }

        return _clan.getClanMember(leaderId).getName();
    }
}