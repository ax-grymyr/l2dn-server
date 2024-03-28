using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Fishing;

public readonly struct ExAutoFishAvailablePacket: IOutgoingPacket
{
    public static readonly ExAutoFishAvailablePacket YES = new(true);
    public static readonly ExAutoFishAvailablePacket NO = new(false);
	
    private readonly bool _available;
	
    private ExAutoFishAvailablePacket(bool available)
    {
        _available = available;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_AUTO_FISH_AVAILABLE);
        writer.WriteByte(_available);
    }
}