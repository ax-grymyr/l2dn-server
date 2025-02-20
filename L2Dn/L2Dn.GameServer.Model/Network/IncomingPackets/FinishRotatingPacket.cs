using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct FinishRotatingPacket: IIncomingPacket<GameSession>
{
    private int _degree;
    private int _unknown;

    public void ReadContent(PacketBitReader reader)
    {
        _degree = reader.ReadInt32();
        _unknown = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.ENABLE_KEYBOARD_MOVEMENT)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        StopRotationPacket sr;
        AirShip? airShip = player.getAirShip();
        if (player.isInAirShip() && airShip != null && airShip.isCaptain(player))
        {
            airShip.setHeading(_degree);
            sr = new StopRotationPacket(airShip.ObjectId, _degree, 0);
            airShip.broadcastPacket(sr);
        }
        else
        {
            player.setHeading(_degree);
            sr = new StopRotationPacket(player.ObjectId, _degree, 0);
            player.broadcastPacket(sr);
        }

        return ValueTask.CompletedTask;
    }
}