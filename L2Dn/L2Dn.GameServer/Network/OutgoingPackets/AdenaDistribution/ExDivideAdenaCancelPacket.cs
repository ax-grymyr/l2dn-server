using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AdenaDistribution;

/**
 * @author Sdw
 */
public readonly struct ExDivideAdenaCancelPacket: IOutgoingPacket
{
	public static readonly ExDivideAdenaCancelPacket STATIC_PACKET = new();
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_DIVIDE_ADENA_CANCEL);
		
		writer.WriteByte(0); // TODO: Find me
	}
}