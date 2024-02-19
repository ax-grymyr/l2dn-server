using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

/**
 * @author Geremy
 */
public readonly struct ResultPetExtractSystemPacket: IOutgoingPacket
{
	private readonly bool _success;
	
	public ResultPetExtractSystemPacket(bool success)
	{
		_success = success;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_RESULT_PET_EXTRACT_SYSTEM);
		writer.WriteInt32(_success);
	}
}