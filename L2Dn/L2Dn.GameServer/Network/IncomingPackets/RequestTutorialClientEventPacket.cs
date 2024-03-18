using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestTutorialClientEventPacket: IIncomingPacket<GameSession>
{
    private int _eventId;

    public void ReadContent(PacketBitReader reader)
    {
        _eventId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        // TODO: UNHARDCODE ME!
        QuestState qs = player.getQuestState("255_Tutorial");
        if (qs != null)
        {
            qs.getQuest().notifyEvent("CE" + _eventId + "", null, player);
        }

        return ValueTask.CompletedTask;
    }
}