using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExTutorialShowIdPacket: IOutgoingPacket
{
    private readonly int _id;
	
    public ExTutorialShowIdPacket(int id)
    {
        _id = id;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TUTORIAL_SHOW_ID);
        
        writer.WriteInt32(_id);
    }
}