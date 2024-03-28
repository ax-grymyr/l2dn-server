using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Quests;

public struct RequestExQuestTeleportPacket: IIncomingPacket<GameSession>
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
        if (quest == null)
            return ValueTask.CompletedTask;
		
        quest.notifyEvent("TELEPORT", null, player);
        
        return ValueTask.CompletedTask;
    }
}