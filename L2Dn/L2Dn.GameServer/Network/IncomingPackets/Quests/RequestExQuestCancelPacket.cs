using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Network.OutgoingPackets.Quests;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Quests;

public struct RequestExQuestCancelPacket: IIncomingPacket<GameSession>
{
    private int _questId;

    public void ReadContent(PacketBitReader reader)
    {
        _questId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Quest quest = QuestManager.getInstance().getQuest(_questId);
        if (quest is null)
            return ValueTask.CompletedTask;
        
        QuestState qs = quest.getQuestState(player, false);
        if ((qs != null) && !qs.isCompleted())
        {
            qs.setSimulated(false);
            qs.exitQuest(QuestType.REPEATABLE);
            player.sendPacket(new ExQuestUiPacket(player));
            player.sendPacket(new ExQuestNotificationAllPacket(player));
			
            if (player.Events.HasSubscribers<OnPlayerQuestAbort>())
            {
                player.Events.NotifyAsync(new OnPlayerQuestAbort(player, _questId));
            }
			
            quest.onQuestAborted(player);
        }
        
        return ValueTask.CompletedTask;
    }
}