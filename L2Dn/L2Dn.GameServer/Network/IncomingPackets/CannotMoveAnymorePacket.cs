using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CannotMoveAnymorePacket: IIncomingPacket<GameSession>
{
    private int _x;
    private int _y;
    private int _z;
    private int _heading;

    public void ReadContent(PacketBitReader reader)
    {
        _x = reader.ReadInt32();
        _y = reader.ReadInt32();
        _z = reader.ReadInt32();
        _heading = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.getAI() != null)
        {
            player.getAI().notifyEvent(CtrlEvent.EVT_ARRIVED_BLOCKED, new Location(_x, _y, _z, _heading));
        }

        return ValueTask.CompletedTask;
    }
}