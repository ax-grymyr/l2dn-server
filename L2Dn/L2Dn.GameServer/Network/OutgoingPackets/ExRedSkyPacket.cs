using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRedSkyPacket: IOutgoingPacket
{
    private readonly int _duration;
	
    public ExRedSkyPacket(int duration)
    {
        _duration = duration;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RED_SKY);
        
        writer.WriteInt32(_duration);
    }
}