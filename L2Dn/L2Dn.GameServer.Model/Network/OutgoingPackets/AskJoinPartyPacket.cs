using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AskJoinPartyPacket: IOutgoingPacket
{
    private readonly string _requestorName;
    private readonly PartyDistributionType _partyDistributionType;
	
    /**
     * @param requestorName
     * @param partyDistributionType
     */
    public AskJoinPartyPacket(string requestorName, PartyDistributionType partyDistributionType)
    {
        _requestorName = requestorName;
        _partyDistributionType = partyDistributionType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ASK_JOIN_PARTY);
        
        writer.WriteString(_requestorName);
        writer.WriteInt32((int)_partyDistributionType);
    }
}