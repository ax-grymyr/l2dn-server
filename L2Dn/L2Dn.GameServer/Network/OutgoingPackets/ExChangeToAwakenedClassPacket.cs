using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExChangeToAwakenedClassPacket: IOutgoingPacket
{
    private readonly int _classId;
	
    public ExChangeToAwakenedClassPacket(int classId)
    {
        _classId = classId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_TO_AWAKENED_CLASS);
        
        writer.WriteInt32(_classId);
    }
}