using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExSearchOrcPacket: IOutgoingPacket
{
    public static readonly ExSearchOrcPacket STATIC_PACKET = new();
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SEARCH_ORC);
    }
}