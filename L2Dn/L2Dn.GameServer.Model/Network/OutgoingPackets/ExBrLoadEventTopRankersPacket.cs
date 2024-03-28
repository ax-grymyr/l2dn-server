using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBrLoadEventTopRankersPacket: IOutgoingPacket
{
    private readonly int _eventId;
    private readonly int _day;
    private readonly int _count;
    private readonly int _bestScore;
    private readonly int _myScore;
	
    public ExBrLoadEventTopRankersPacket(int eventId, int day, int count, int bestScore, int myScore)
    {
        _eventId = eventId;
        _day = day;
        _count = count;
        _bestScore = bestScore;
        _myScore = myScore;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_LOAD_EVENT_TOP_RANKERS);
        
        writer.WriteInt32(_eventId);
        writer.WriteInt32(_day);
        writer.WriteInt32(_count);
        writer.WriteInt32(_bestScore);
        writer.WriteInt32(_myScore);
    }
}