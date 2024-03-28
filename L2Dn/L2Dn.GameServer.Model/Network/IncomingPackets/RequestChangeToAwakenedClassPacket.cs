using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestChangeToAwakenedClassPacket: IIncomingPacket<GameSession>
{
    private bool _change;

    public void ReadContent(PacketBitReader reader)
    {
        _change = reader.ReadInt32() == 1;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_change)
        {
            if (player.Events.HasSubscribers<OnPlayerChangeToAwakenedClass>())
            {
                player.Events.NotifyAsync(new OnPlayerChangeToAwakenedClass(player));
            }
        }
        else
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
        }
        
        return ValueTask.CompletedTask;
    }
}