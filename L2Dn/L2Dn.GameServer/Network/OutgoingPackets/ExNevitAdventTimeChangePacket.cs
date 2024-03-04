using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExNevitAdventTimeChangePacket: IOutgoingPacket
{
    private readonly bool _paused;
    private readonly int _time;
	
    public ExNevitAdventTimeChangePacket(int time)
    {
        _time = time > 240000 ? 240000 : time;
        _paused = _time < 1;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_CRYSTALIZING_ESTIMATION); // TODO: packet code

        // state 0 - pause 1 - started
        writer.WriteByte(!_paused);
        
        // left time in ms max is 16000 its 4m and state is automatically changed to quit
        writer.WriteInt32(_time);
    }
}