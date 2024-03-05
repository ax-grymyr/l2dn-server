using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ensoul;

public readonly struct ExShowEnsoulWindowPacket: IOutgoingPacket
{
    public static readonly ExShowEnsoulWindowPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_ENSOUL_WINDOW);
    }
}