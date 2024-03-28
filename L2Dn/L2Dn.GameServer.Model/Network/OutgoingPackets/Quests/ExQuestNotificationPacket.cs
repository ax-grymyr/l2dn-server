using L2Dn.GameServer.Model.Quests;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Quests;

public readonly struct ExQuestNotificationPacket: IOutgoingPacket
{
    private readonly QuestState _questState;
	
    public ExQuestNotificationPacket(QuestState questState)
    {
        _questState = questState;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_QUEST_NOTIFICATION);
        
        writer.WriteInt32(_questState.getQuest().getId());
        writer.WriteInt32(_questState.getCount());
        writer.WriteByte(_questState.getState());
    }
}