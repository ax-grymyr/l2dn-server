using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMPCCShowPartyMemberInfoPacket: IOutgoingPacket
{
    private readonly Party _party;
	
    public ExMPCCShowPartyMemberInfoPacket(Party party)
    {
        _party = party;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MPCCSHOW_PARTY_MEMBER_INFO);
        
        writer.WriteInt32(_party.getMemberCount());
        foreach (Player pc in _party.getMembers())
        {
            writer.WriteString(pc.getName());
            writer.WriteInt32(pc.getObjectId());
            writer.WriteInt32((int)pc.getClassId());
        }
    }
}