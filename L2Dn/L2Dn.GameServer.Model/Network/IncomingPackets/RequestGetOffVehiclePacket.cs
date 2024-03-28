using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestGetOffVehiclePacket: IIncomingPacket<GameSession>
{
    private int _boatId;
    private int _x;
    private int _y;
    private int _z;

    public void ReadContent(PacketBitReader reader)
    {
        _boatId = reader.ReadInt32();
        _x = reader.ReadInt32();
        _y = reader.ReadInt32();
        _z = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (!player.isInBoat() || (player.getBoat().getObjectId() != _boatId) || player.getBoat().isMoving() || !player.isInsideRadius3D(_x, _y, _z, 1000))
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        player.broadcastPacket(new StopMoveInVehiclePacket(player, _boatId));
        player.setVehicle(null);
        player.setInVehiclePosition(null);
        player.sendPacket(ActionFailedPacket.STATIC_PACKET);
        player.broadcastPacket(new GetOffVehiclePacket(player.getObjectId(), _boatId, _x, _y, _z));
        player.setXYZ(_x, _y, _z);
        player.setInsideZone(ZoneId.PEACE, false);
        player.revalidateZone(true);
        return ValueTask.CompletedTask;
    }
}