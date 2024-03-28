using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SnoopQuitPacket: IIncomingPacket<GameSession>
{
    private int _snoopId;

    public void ReadContent(PacketBitReader reader)
    {
        _snoopId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player target = World.getInstance().getPlayer(_snoopId);
        if (target == null)
            return ValueTask.CompletedTask;
		
        target.removeSnooper(player);
        player.removeSnooped(target);

        return ValueTask.CompletedTask;
    }
}