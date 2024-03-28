using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.CastleWar;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.CastleWar;

public struct ExMercenaryCastleWarCastleSiegeDefenderListPacket: IIncomingPacket<GameSession>
{
    private int _castleId;

    public void ReadContent(PacketBitReader reader)
    {
        _castleId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new MercenaryCastleWarCastleSiegeDefenderListPacket(_castleId));
        
        return ValueTask.CompletedTask;
    }
}