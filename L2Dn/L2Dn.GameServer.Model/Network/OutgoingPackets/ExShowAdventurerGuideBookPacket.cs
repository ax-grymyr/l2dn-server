using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowAdventurerGuideBookPacket: IOutgoingPacket
{
    public static readonly ExShowAdventurerGuideBookPacket STATIC_PACKET = new();
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_ADVENTURER_GUIDE_BOOK);
    }
}