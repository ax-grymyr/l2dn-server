using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestGetBossRecordPacket: IIncomingPacket<GameSession>
{
    private int _bossId;

    public void ReadContent(PacketBitReader reader)
    {
        _bossId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        PacketLogger.Instance.Warn(player + " (boss ID: " + _bossId + ") used unsuded packet " +
                                   nameof(RequestGetBossRecordPacket));
        
        return ValueTask.CompletedTask;
    }
}