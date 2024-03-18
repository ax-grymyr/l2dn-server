using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestMoveToLocationInVehiclePacket: IIncomingPacket<GameSession>
{
    private int _boatId;
    private int _targetX;
    private int _targetY;
    private int _targetZ;
    private int _originX;
    private int _originY;
    private int _originZ;

    public void ReadContent(PacketBitReader reader)
    {
        _boatId = reader.ReadInt32(); // objectId of boat
        _targetX = reader.ReadInt32();
        _targetY = reader.ReadInt32();
        _targetZ = reader.ReadInt32();
        _originX = reader.ReadInt32();
        _originY = reader.ReadInt32();
        _originZ = reader.ReadInt32();
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
		
        if (_targetX == _originX && _targetY == _originY && _targetZ == _originZ)
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
		
        Boat boat;
        if (player.isInBoat())
        {
            boat = player.getBoat();
            if (boat.getObjectId() != _boatId)
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
		
        Location pos = new Location(_targetX, _targetY, _targetZ);
        Location originPos = new Location(_originX, _originY, _originZ);
        player.setInVehiclePosition(pos);
        player.broadcastPacket(new MoveToLocationInVehiclePacket(player, pos, originPos));
        return ValueTask.CompletedTask;
    }
}