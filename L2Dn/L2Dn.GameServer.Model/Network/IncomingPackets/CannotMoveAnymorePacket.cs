using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CannotMoveAnymorePacket: IIncomingPacket<GameSession>
{
    private Location _location;

    public void ReadContent(PacketBitReader reader)
    {
        _location = reader.ReadLocationWithHeading();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.getAI() != null)
        {
            player.getAI().notifyEvent(CtrlEvent.EVT_ARRIVED_BLOCKED, _location);
        }

        return ValueTask.CompletedTask;
    }
}