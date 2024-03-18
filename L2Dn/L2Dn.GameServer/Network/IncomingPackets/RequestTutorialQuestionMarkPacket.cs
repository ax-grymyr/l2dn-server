using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestTutorialQuestionMarkPacket: IIncomingPacket<GameSession>
{
    private int _number;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadByte(); // index ?
        _number = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        // Notify scripts
        if (player.Events.HasSubscribers<OnPlayerPressTutorialMark>())
        {
            player.Events.NotifyAsync(new OnPlayerPressTutorialMark(player, _number));
        }

        return ValueTask.CompletedTask;
    }
}