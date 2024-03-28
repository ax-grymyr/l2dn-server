using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAskModifyPartyLootingPacket: IOutgoingPacket
{
	private readonly String _requestor;
	private readonly PartyDistributionType _partyDistributionType;
	
	public ExAskModifyPartyLootingPacket(String name, PartyDistributionType partyDistributionType)
	{
		_requestor = name;
		_partyDistributionType = partyDistributionType;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_ASK_MODIFY_PARTY_LOOTING);
		
		writer.WriteString(_requestor);
		writer.WriteInt32((int)_partyDistributionType);
	}
}