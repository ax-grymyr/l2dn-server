using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Teleports;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Teleports;

public struct ExRequestTeleportFavoritesUiTogglePacket: IIncomingPacket<GameSession>
{
    private bool _enable;

    public void ReadContent(PacketBitReader reader)
    {
        _enable = reader.ReadByte() == 1;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        connection.Send(new ExTeleportFavoritesListPacket(player, _enable));
       
        return ValueTask.CompletedTask;
    }
}