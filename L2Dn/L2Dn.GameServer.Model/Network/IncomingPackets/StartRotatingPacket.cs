using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct StartRotatingPacket: IIncomingPacket<GameSession>
{
    private int _degree;
    private int _side;

    public void ReadContent(PacketBitReader reader)
    {
        _degree = reader.ReadInt32();
        _side = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.ENABLE_KEYBOARD_MOVEMENT)
            return ValueTask.CompletedTask;
		
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.isInAirShip() && player.getAirShip().isCaptain(player))
        {
            player.getAirShip().broadcastPacket(new StartRotationPacket(player.getAirShip().ObjectId, _degree, _side, 0));
        }
        else
        {
            player.broadcastPacket(new StartRotationPacket(player.ObjectId, _degree, _side, 0));
        }

        return ValueTask.CompletedTask;
    }
}