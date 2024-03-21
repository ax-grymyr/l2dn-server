using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestInZoneWaitingTimePacket: IIncomingPacket<GameSession>
{
    private bool _hide;

    public void ReadContent(PacketBitReader reader)
    {
        _hide = reader.ReadByte() == 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExInzoneWaitingPacket(player, _hide));

        return ValueTask.CompletedTask;
    }
}