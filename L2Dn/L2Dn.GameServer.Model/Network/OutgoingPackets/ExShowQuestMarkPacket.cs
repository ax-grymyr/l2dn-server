using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowQuestMarkPacket: IOutgoingPacket
{
    private readonly int _questId;
    private readonly int _questState;
	
    public ExShowQuestMarkPacket(int questId, int questState)
    {
        _questId = questId;
        _questState = questState;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_QUEST_MARK);
        
        writer.WriteInt32(_questId);
        writer.WriteInt32(_questState);
    }
}