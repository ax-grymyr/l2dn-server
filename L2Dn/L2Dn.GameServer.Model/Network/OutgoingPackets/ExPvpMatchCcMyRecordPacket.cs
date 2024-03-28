using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPvpMatchCcMyRecordPacket: IOutgoingPacket
{
    private readonly int _points;
	
    public ExPvpMatchCcMyRecordPacket(int points)
    {
        _points = points;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PVP_MATCH_CCMY_RECORD);
        
        writer.WriteInt32(_points);
    }
}