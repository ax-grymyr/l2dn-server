using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestQuestAbortPacket: IIncomingPacket<GameSession>
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

        Quest qe = QuestManager.getInstance().getQuest(_questId);
        if (qe != null)
        {
            QuestState? qs = player.getQuestState(qe.Name);
            if (qs != null)
            {
                qs.setSimulated(false);
                qs.exitQuest(QuestType.REPEATABLE);
                player.sendPacket(new QuestListPacket(player));

                if (player.Events.HasSubscribers<OnPlayerQuestAbort>())
                {
                    player.Events.NotifyAsync(new OnPlayerQuestAbort(player, _questId));
                }

                qe.onQuestAborted(player);
            }
        }

        return ValueTask.CompletedTask;
    }
}