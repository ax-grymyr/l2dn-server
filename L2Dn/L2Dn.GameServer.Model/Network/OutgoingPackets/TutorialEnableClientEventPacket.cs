using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TutorialEnableClientEventPacket: IOutgoingPacket
{
    private readonly int _eventId;
	
    public TutorialEnableClientEventPacket(int eventId)
    {
        _eventId = eventId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TUTORIAL_ENABLE_CLIENT_EVENT);
        
        writer.WriteInt32(_eventId);
    }
}