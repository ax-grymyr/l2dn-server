using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TutorialShowQuestionMarkPacket: IOutgoingPacket
{
    private readonly int _markId;
    private readonly int _markType;
	
    public TutorialShowQuestionMarkPacket(int markId, int markType)
    {
        _markId = markId;
        _markType = markType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TUTORIAL_SHOW_QUESTION_MARK);
        
        writer.WriteByte((byte)_markType);
        writer.WriteInt32(_markId);
    }
}