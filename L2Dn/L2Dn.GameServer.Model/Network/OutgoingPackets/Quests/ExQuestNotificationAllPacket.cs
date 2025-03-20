using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Quests;

public readonly struct ExQuestNotificationAllPacket: IOutgoingPacket
{
    private readonly Player _activeChar;

    public ExQuestNotificationAllPacket(Player activeChar)
    {
        _activeChar = activeChar;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_QUEST_NOTIFICATION_ALL);

        ICollection<Quest> quests = _activeChar.getAllActiveQuests();

        writer.WriteInt32(quests.Count);
        foreach (Quest quest in quests)
        {
            QuestState? questState = quest.getQuestState(_activeChar, false);
            writer.WriteInt32(quest.Id);
            writer.WriteInt32(questState?.getCount() ?? 0);
        }
    }
}