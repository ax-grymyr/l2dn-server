using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CannotMoveAnymoreInVehiclePacket: IIncomingPacket<GameSession>
{
    private int _boatId;
    private Location _location;

    public void ReadContent(PacketBitReader reader)
    {
        _boatId = reader.ReadInt32();
        _location = reader.ReadLocation();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.isInBoat() && player.getBoat().ObjectId == _boatId)
        {
            player.setInVehiclePosition(_location.Location3D);
            player.setHeading(_location.Heading);
            player.broadcastPacket(new StopMoveInVehiclePacket(player, _boatId));
        }

        return ValueTask.CompletedTask;
    }
}