using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SunSetPacket: IOutgoingPacket
{
    public static readonly SunSetPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SUN_SET);
    }
}