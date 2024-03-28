using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRegenMaxPacket: IOutgoingPacket
{
    private readonly int _time;
    private readonly int _tickInterval;
    private readonly double _amountPerTick;
	
    public ExRegenMaxPacket(int time, int tickInterval, double amountPerTick)
    {
        _time = time;
        _tickInterval = tickInterval;
        _amountPerTick = amountPerTick;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_REGEN_MAX);
        
        writer.WriteInt32(1);
        writer.WriteInt32(_time);
        writer.WriteInt32(_tickInterval);
        writer.WriteDouble(_amountPerTick);
    }
}