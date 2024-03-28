using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMPCCPartyInfoUpdatePacket: IOutgoingPacket
{
    private readonly int _mode;
    private readonly int _LeaderOID;
    private readonly int _memberCount;
    private readonly string _name;
	
    /**
     * @param party
     * @param mode 0 = Remove, 1 = Add
     */
    public ExMPCCPartyInfoUpdatePacket(Party party, int mode)
    {
        _name = party.getLeader().getName();
        _LeaderOID = party.getLeaderObjectId();
        _memberCount = party.getMemberCount();
        _mode = mode;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MPCCPARTY_INFO_UPDATE);
        
        writer.WriteString(_name);
        writer.WriteInt32(_LeaderOID);
        writer.WriteInt32(_memberCount);
        writer.WriteInt32(_mode); // mode 0 = Remove Party, 1 = AddParty, maybe more...
    }
}