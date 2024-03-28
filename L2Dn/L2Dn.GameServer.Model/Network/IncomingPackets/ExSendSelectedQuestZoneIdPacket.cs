using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ExSendSelectedQuestZoneIdPacket: IIncomingPacket<GameSession>
{
    private int _questZoneId;

    public void ReadContent(PacketBitReader reader)
    {
        _questZoneId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.setQuestZoneId(_questZoneId);

        return ValueTask.CompletedTask;
    }
}