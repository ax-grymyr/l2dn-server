using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestMoveToLocationInVehiclePacket: IIncomingPacket<GameSession>
{
    private int _boatId;
    private Location3D _targetLocation;
    private Location3D _originLocation;

    public void ReadContent(PacketBitReader reader)
    {
        _boatId = reader.ReadInt32(); // objectId of boat
        _targetLocation = reader.ReadLocation3D();
        _originLocation = reader.ReadLocation3D();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (Config.PLAYER_MOVEMENT_BLOCK_TIME > 0 && !player.isGM() && player.getNotMoveUntil() > DateTime.UtcNow)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_MOVE_WHILE_SPEAKING_TO_AN_NPC_ONE_MOMENT_PLEASE);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (_targetLocation == _originLocation)
        {
            player.sendPacket(new StopMoveInVehiclePacket(player, _boatId));
            return ValueTask.CompletedTask;
        }

        if (player.isAttackingNow() && player.getActiveWeaponItem() != null && player.getActiveWeaponItem().getItemType() == WeaponType.BOW)
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (player.isSitting() || player.isMovementDisabled())
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (player.hasSummon())
        {
            player.sendPacket(SystemMessageId.YOU_SHOULD_RELEASE_YOUR_SERVITOR_SO_THAT_IT_DOES_NOT_FALL_OFF_OF_THE_BOAT_AND_DROWN);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (player.isTransformed())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_POLYMORPH_WHILE_RIDING_A_BOAT);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        Boat? boat;
        if (player.isInBoat())
        {
            boat = player.getBoat();
            if (boat != null && boat.ObjectId != _boatId)
            {
                boat = BoatManager.getInstance().getBoat(_boatId);
                player.setVehicle(boat);
            }
        }
        else
        {
            boat = BoatManager.getInstance().getBoat(_boatId);
            player.setVehicle(boat);
        }

        player.setInVehiclePosition(_targetLocation);
        player.broadcastPacket(new MoveToLocationInVehiclePacket(player, _targetLocation, _originLocation));
        return ValueTask.CompletedTask;
    }
}