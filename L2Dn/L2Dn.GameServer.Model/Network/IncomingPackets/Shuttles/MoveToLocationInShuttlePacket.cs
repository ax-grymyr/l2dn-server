using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Shuttle;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Shuttles;

public struct MoveToLocationInShuttlePacket: IIncomingPacket<GameSession>
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

        if (_targetX == _originX && _targetY == _originY && _targetZ == _originZ)
        {
            player.sendPacket(new ExStopMoveInShuttlePacket(player, _boatId));
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
		
        player.setInVehiclePosition(new Location(_targetX, _targetY, _targetZ));
        player.broadcastPacket(new ExMoveToLocationInShuttlePacket(player, _originX, _originY, _originZ));
        
        return ValueTask.CompletedTask;
    }
}