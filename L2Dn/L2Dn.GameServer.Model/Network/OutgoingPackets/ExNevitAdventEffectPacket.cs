using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExNevitAdventEffectPacket: IOutgoingPacket
{
    private readonly int _timeLeft;
	
    public ExNevitAdventEffectPacket(int timeLeft)
    {
        _timeLeft = timeLeft;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_CHANNELING_EFFECT);
        
        writer.WriteInt32(_timeLeft);
    }
}