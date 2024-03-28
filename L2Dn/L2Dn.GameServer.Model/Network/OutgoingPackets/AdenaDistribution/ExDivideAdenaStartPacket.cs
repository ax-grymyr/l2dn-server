using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AdenaDistribution;

/**
 * @author Sdw
 */
public readonly struct ExDivideAdenaStartPacket: IOutgoingPacket
{
	public static readonly ExDivideAdenaStartPacket STATIC_PACKET = default;
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_DIVIDE_ADENA_START);
	}
}