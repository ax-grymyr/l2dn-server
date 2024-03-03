using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExSetPartyLootingPacket: IOutgoingPacket
{
    private readonly int _result;
    private readonly PartyDistributionType _partyDistributionType;
	
    public ExSetPartyLootingPacket(int result, PartyDistributionType partyDistributionType)
    {
        _result = result;
        _partyDistributionType = partyDistributionType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SET_PARTY_LOOTING);
        
        writer.WriteInt32(_result);
        writer.WriteInt32((int)_partyDistributionType);
    }
}