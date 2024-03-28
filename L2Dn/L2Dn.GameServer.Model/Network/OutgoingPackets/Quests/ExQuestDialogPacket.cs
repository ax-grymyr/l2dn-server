using L2Dn.GameServer.Model.Quests;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Quests;

public readonly struct ExQuestDialogPacket: IOutgoingPacket
{
    private readonly int _questId;
    private readonly QuestDialogType _dialogType;
	
    public ExQuestDialogPacket(int questId, QuestDialogType dialogType)
    {
        _questId = questId;
        _dialogType = dialogType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_QUEST_DIALOG);
        
        writer.WriteInt32(_questId);
        writer.WriteInt32((int)_dialogType);
    }
}