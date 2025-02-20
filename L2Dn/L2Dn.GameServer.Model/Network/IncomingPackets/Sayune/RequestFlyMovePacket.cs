using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Sayune;

public struct RequestFlyMovePacket: IIncomingPacket<GameSession>
{
    private int _locationId;

    public void ReadContent(PacketBitReader reader)
    {
        _locationId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        SayuneRequest? request = player.getRequest<SayuneRequest>();
        if (request == null)
            return ValueTask.CompletedTask;

        request.move(player, _locationId);

        return ValueTask.CompletedTask;
    }
}