using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Shuttle;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Shuttles;

public struct MoveToLocationInShuttlePacket: IIncomingPacket<GameSession>
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

        if (_targetLocation == _originLocation)
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
		
        player.setInVehiclePosition(_targetLocation);
        player.broadcastPacket(new ExMoveToLocationInShuttlePacket(player, _originLocation));
        
        return ValueTask.CompletedTask;
    }
}