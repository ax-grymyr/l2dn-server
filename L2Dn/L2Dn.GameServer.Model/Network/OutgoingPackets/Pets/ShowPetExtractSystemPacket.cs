using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

/**
 * @author Geremy
 */
public readonly struct ShowPetExtractSystemPacket: IOutgoingPacket
{
	public static readonly ShowPetExtractSystemPacket STATIC_PACKET = new();
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_PET_EXTRACT_SYSTEM);
		writer.WriteInt32(0);
	}
}