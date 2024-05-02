using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct MoveToLocationInAirShipPacket: IIncomingPacket<GameSession>
{
    private int _shipId;
    private Location3D _targetLocation;
    private Location3D _originLocation;

    public void ReadContent(PacketBitReader reader)
    {
        _shipId = reader.ReadInt32();
        _targetLocation = reader.ReadLocation3D();
        _originLocation = reader.ReadLocation3D();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_targetLocation == _originLocation)
        {
            player.sendPacket(new StopMoveInVehiclePacket(player, _shipId));
            return ValueTask.CompletedTask;
        }

        if (player.isAttackingNow() && player.getActiveWeaponItem() != null &&
            player.getActiveWeaponItem().getItemType() == WeaponType.BOW)
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (player.isSitting() || player.isMovementDisabled())
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (!player.isInAirShip())
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        AirShip airShip = player.getAirShip();
        if (airShip.getObjectId() != _shipId)
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.setInVehiclePosition(_targetLocation);
        player.broadcastPacket(new ExMoveToLocationInAirShipPacket(player));

        return ValueTask.CompletedTask;
    }
}