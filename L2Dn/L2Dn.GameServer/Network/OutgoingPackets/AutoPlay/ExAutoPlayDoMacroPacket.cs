using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AutoPlay;

public readonly struct ExAutoPlayDoMacroPacket: IOutgoingPacket
{
    public static readonly ExAutoPlayDoMacroPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_AUTOPLAY_DO_MACRO);
        
        writer.WriteInt32(276);
    }
}