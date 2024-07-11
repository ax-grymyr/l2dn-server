using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMPCCPartyMasterListPacket: IOutgoingPacket
{
    private readonly Set<string> _leadersName;
	
    public ExMPCCPartyMasterListPacket(Set<string> leadersName)
    {
        _leadersName = leadersName;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MPCC_PARTYMASTER_LIST);
        
        writer.WriteInt32(_leadersName.size());
        foreach (string leaderName in _leadersName)
        {
            writer.WriteString(leaderName);
        }
    }
}