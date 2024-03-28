using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct OnEventTriggerPacket: IOutgoingPacket
{
    private readonly int _emitterId;
    private readonly bool _enabled;
	
    public OnEventTriggerPacket(int emitterId, bool enabled)
    {
        _emitterId = emitterId;
        _enabled = enabled;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EVENT_TRIGGER);
        writer.WriteInt32(_emitterId);
        writer.WriteByte(_enabled);
    }
}